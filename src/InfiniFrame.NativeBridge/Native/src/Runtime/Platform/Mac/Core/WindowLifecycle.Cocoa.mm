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
#if defined(__aarch64__) || defined(__arm64__)
static constexpr NSTimeInterval WebKitPostDetachSettleInterval = 0.25;
#else
static constexpr NSTimeInterval WebKitPostDetachSettleInterval = 0.10;
#endif

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
    // Capture raw pointers so the copied MRC block does not add an implicit retain. The
    // alloc/init references are deliberately transferred to the coordinator and must be released
    // before we report the native close as complete.
    void* webviewPointer = m_impl->_webview;
    m_impl->_webview = nil;
    void* configurationPointer = m_impl->_webviewConfiguration;
    m_impl->_webviewConfiguration = nil;

    // The coordinator uses NSTimer so it is serviced by the default run-loop mode pumped by
    // WaitForExit, while ensuring that only one WebKit view is detached at a time.
    infiniframe::macos::EnqueueWebKitTeardown(^{
            @autoreleasepool {
                auto* webview = static_cast<WKWebView*>(webviewPointer);
                auto* configuration = static_cast<WKWebViewConfiguration*>(configurationPointer);
                if (webview != nil) {
                    [webview removeFromSuperview];
                    [webview release];
                }
                [configuration release];

                // WebKit can still be delivering the display refresh which observed this view
                // after removeFromSuperview returns. Do not make the managed close observable
                // (and therefore allow another view to be created) until that callback has had
                // several display intervals to leave WebKit.
                [NSTimer scheduledTimerWithTimeInterval:WebKitPostDetachSettleInterval
                                                repeats:NO
                                                  block:^(NSTimer* timer) {
                        (void)timer;
                        this->CompleteCloseAfterWebKitTeardown();
                    }
                ];
            }
        }
    );
}

void InfiniFrameWindow::CompleteCloseAfterWebKitTeardown()
{
    infiniframe::macos::LogLifecycle("window-webkit-teardown-complete", this);
    {
        infiniframe::macos::NativeCallbackScope callbackScope;
        InvokeClosed();
    }
    SignalWindowClosed();

    // SafeHandle disposal can have happened while the WKWebView timer was pending. Do not
    // delete from this callback: AppKit/WebKit may still unwind through the window delegate.
    // A subsequent main-queue turn is the native destruction boundary.
    if (m_impl->_nativeDestructionScheduled) {
        dispatch_async(dispatch_get_main_queue(), ^{
            delete this;
        });
    }
}

void InfiniFrameWindow::ScheduleDeferredDestruction()
{
    void (^requestDestruction)() = ^{
        if (this->m_impl->_nativeDestructionScheduled)
            return;

        infiniframe::macos::LogLifecycle("window-destruction-request", this);
        this->m_impl->_nativeDestructionScheduled = true;
        this->PrepareForDeferredDestruction();

        // CloseWebView owns an NSTimer whose callback references this instance. Let that
        // callback publish the close boundary and enqueue the deletion once it has unwound.
        if (this->m_impl->_webKitTeardownScheduled &&
            !this->m_impl->_windowClosed.load(std::memory_order_acquire))
            return;

        // No close callback is outstanding. Still defer one main-queue turn so disposal from
        // an AppKit delegate cannot delete the instance while that delegate is executing.
        dispatch_async(dispatch_get_main_queue(), ^{
            delete this;
        });
    };

    if ([NSThread isMainThread]) {
        requestDestruction();
        return;
    }

    // Do not dispatch_sync from a worker thread. WaitForExit can be pumping the default
    // AppKit run-loop mode, which does not necessarily service main-queue synchronous work.
    // The native instance is now self-owned, so it is safe for SafeHandle disposal to return
    // before this request reaches AppKit.
    dispatch_async(dispatch_get_main_queue(), requestDestruction);
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
