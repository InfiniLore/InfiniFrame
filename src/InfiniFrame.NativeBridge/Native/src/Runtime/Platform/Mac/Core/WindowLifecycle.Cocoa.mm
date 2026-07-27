// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <stdexcept>

#include "../Window.Cocoa.Internal.h"
#include "../MacDiagnostics.h"
#include "../Delegates/UrlSchemeHandler.h"
#include "../Delegates/UiDelegate.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// Safely runs a block on the main GCD queue.
/// If already on the main thread, runs synchronously; otherwise dispatches synchronously.
static void DispatchToMainSync(void (^block)()) {
    if ([NSThread isMainThread]) {
        block();
    } else {
        dispatch_sync(dispatch_get_main_queue(), block);
    }
}

void InfiniFrameWindow::Center()
{
    [m_impl->_window center];
    [m_impl->_window makeKeyAndOrderFront: m_impl->_window];
}

void InfiniFrameWindow::ClearBrowserAutoFill()
{
    // TODO
}

void InfiniFrameWindow::Close()
{
    infiniframe::macos::LogLifecycle("window-close-request", this);
    DispatchToMainSync(^{
        this->m_impl->_isClosingOrClosed = true;

        if (this->m_impl->_parentWillCloseObserver != nil) {
            [[NSNotificationCenter defaultCenter] removeObserver:this->m_impl->_parentWillCloseObserver];
            this->m_impl->_parentWillCloseObserver = nil;
        }

        if (this->m_impl->_nativeParentWindow != nil && this->m_impl->_window != nil) {
            [this->m_impl->_nativeParentWindow removeChildWindow:this->m_impl->_window];
            this->m_impl->_nativeParentWindow = nil;
        }

        if (this->m_impl->_chromeless)
            [this->m_impl->_window close];
        else
            [this->m_impl->_window performClose: this->m_impl->_window];
    });
}

void InfiniFrameWindow::WaitForExit()
{
    infiniframe::macos::LogLifecycle("window-wait-begin", this);
    // Do not call [NSApp run] here. Test hosts and embedding applications can already
    // own the main CFRunLoop; starting a nested application run loop may terminate the
    // process when the last test window closes. Pump only until this window closes.
    if (![NSThread isMainThread]) {
        std::unique_lock lock(m_impl->_windowClosedMutex);
        m_impl->_windowClosedCondition.wait(lock, [this] {
            return m_impl->_windowClosed.load(std::memory_order_acquire);
        });
        infiniframe::macos::LogLifecycle("window-wait-complete", this);
        return;
    }

    while (!m_impl->_windowClosed.load(std::memory_order_acquire)) {
        @autoreleasepool {
            [[NSRunLoop mainRunLoop] runMode:NSDefaultRunLoopMode
                                   beforeDate:[NSDate dateWithTimeIntervalSinceNow:0.05]];
        }
    }
    infiniframe::macos::LogLifecycle("window-wait-complete", this);
}

void InfiniFrameWindow::CloseWebView()
{
    infiniframe::macos::LogLifecycle("window-native-closed", this);
    if (m_impl->_webKitTeardownScheduled)
        return;

    m_impl->_webKitTeardownScheduled = true;
    m_impl->_isClosingOrClosed = true;
    m_impl->_webviewReady = false;
    m_impl->_pendingWebMessages.clear();

    if (m_impl->_webviewConfiguration != nil) {
        [m_impl->_webviewConfiguration.userContentController removeScriptMessageHandlerForName:@"infiniFrameInterop"];
    }

    for (UrlSchemeHandler* handler : m_impl->_urlSchemeHandlers)
        [handler invalidate];

    if (m_impl->_webview != nil) {
        [m_impl->_webview stopLoading];
        m_impl->_webview.UIDelegate = nil;
        m_impl->_webview.navigationDelegate = nil;
    }

    // Removing a WKWebView from its superview unregisters a WebKit display-link observer. On
    // recent Apple Silicon runners, WebKit can otherwise remove that observer while its refresh
    // callback is enumerating it. Keep the view attached until that callback has had time to
    // quiesce, and do not publish the managed closed event until the complete teardown is done.
    WKWebView* webview = m_impl->_webview;
    m_impl->_webview = nil;
    WKWebViewConfiguration* configuration = m_impl->_webviewConfiguration;
    m_impl->_webviewConfiguration = nil;

    // WaitForExit pumps NSRunLoop in default mode when it is called from the AppKit thread.
    // A main-queue dispatch_after block would not run while that nested loop is active, so the
    // deferred teardown must be scheduled as an NSTimer instead.
    [NSTimer scheduledTimerWithTimeInterval:0.05
                                    repeats:NO
                                      block:^(NSTimer* timer) {
            (void)timer;
            @autoreleasepool {
                if (webview != nil) {
                    [webview removeFromSuperview];
                    [webview release];
                }
                [configuration release];
                this->CompleteCloseAfterWebKitTeardown();
            }
        }
    ];
}

void InfiniFrameWindow::CompleteCloseAfterWebKitTeardown()
{
    infiniframe::macos::LogLifecycle("window-webkit-teardown-complete", this);
    {
        infiniframe::macos::NativeCallbackScope callbackScope;
        InvokeClosed();
    }
    SignalWindowClosed();
}

void InfiniFrameWindow::SignalWindowClosed()
{
    m_impl->_windowClosed.store(true, std::memory_order_release);
    m_impl->_windowClosedCondition.notify_all();
}

void InfiniFrameWindow::PrepareForDeferredDestruction()
{
    m_impl->_isClosingOrClosed = true;
    m_impl->_webviewReady = false;
    m_impl->_pendingWebMessages.clear();

    m_impl->_closingCallback = nullptr;
    m_impl->_closedCallback = nullptr;
    m_impl->_focusInCallback = nullptr;
    m_impl->_focusOutCallback = nullptr;
    m_impl->_movedCallback = nullptr;
    m_impl->_resizedCallback = nullptr;
    m_impl->_maximizedCallback = nullptr;
    m_impl->_restoredCallback = nullptr;
    m_impl->_minimizedCallback = nullptr;
    m_impl->_debugEventCallback = nullptr;
    m_impl->_customSchemeCallback = nullptr;
    m_impl->_webMessageReceivedCallback = nullptr;

    if (m_impl->_uiDelegate != nil)
        m_impl->_uiDelegate->webMessageReceivedCallback = nullptr;
    for (UrlSchemeHandler* handler : m_impl->_urlSchemeHandlers)
        [handler invalidate];
}
