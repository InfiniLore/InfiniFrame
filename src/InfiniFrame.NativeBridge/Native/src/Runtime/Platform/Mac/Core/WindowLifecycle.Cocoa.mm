// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <stdexcept>

#include "../Window.Cocoa.Internal.h"
#include "../MacDiagnostics.h"
#include "../Delegates/UrlSchemeHandler.h"
#include "../Delegates/UiDelegate.h"
#include "../Delegates/NavigationDelegate.h"
#include "../Delegates/WindowDelegate.h"
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
        if (this->m_impl->_isClosingOrClosed || this->m_impl->_window == nil) return;
        infiniframe::macos::NativeCallbackScope callbackScope;
        if (this->InvokeClose()) return;
        this->CloseWebView();
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
    if (m_impl->_isClosingOrClosed)
        return;
    m_impl->_isClosingOrClosed = true;
    m_impl->_webviewReady = false;
    m_impl->_pendingWebMessages.clear();

    if (m_impl->_webviewConfiguration != nil) {
        [m_impl->_webviewConfiguration.userContentController removeScriptMessageHandlerForName:@"infiniFrameInterop"];
        [m_impl->_webviewConfiguration.userContentController removeAllUserScripts];
    }

    for (UrlSchemeHandler* handler : m_impl->_urlSchemeHandlers)
        [handler invalidate];

    if (m_impl->_webview != nil) {
        [m_impl->_webview stopLoading];
        m_impl->_webview.UIDelegate = nil;
        m_impl->_webview.navigationDelegate = nil;
        // Replace the old document before the host is leased again.  The shared process pool is
        // intentionally process-scoped; document JS and pending navigation are not.
        [m_impl->_webview loadHTMLString:@"" baseURL:nil];
    }
    if (m_impl->_uiDelegate != nil) {
        m_impl->_uiDelegate->infiniFrame = nullptr;
        m_impl->_uiDelegate->window = nil;
        m_impl->_uiDelegate->webMessageReceivedCallback = nullptr;
    }
    if (m_impl->_navigationDelegate != nil) { m_impl->_navigationDelegate->infiniFrame = nullptr; m_impl->_navigationDelegate->window = nil; }
    if (m_impl->_windowDelegate != nil) m_impl->_windowDelegate->infiniFrame = nullptr;
    if (m_impl->_parentWillCloseObserver != nil) {
        [[NSNotificationCenter defaultCenter] removeObserver:m_impl->_parentWillCloseObserver];
        m_impl->_parentWillCloseObserver = nil;
    }
    if (m_impl->_nativeParentWindow != nil) [m_impl->_nativeParentWindow removeChildWindow:m_impl->_window];
    m_impl->_nativeParentWindow = nil;
    if ([m_impl->_window isMiniaturized]) [m_impl->_window deminiaturize:nil];
    m_impl->_preMaximizedWidth = m_impl->_preMaximizedHeight = 0;
    m_impl->_preMaximizedXPosition = m_impl->_preMaximizedYPosition = 0;
    [m_impl->_window setLevel:NSNormalWindowLevel];
    [m_impl->_window orderOut:nil];
    // Hosts use WKWebsiteDataStore.nonPersistentDataStore.  Do not make close depend on
    // removeDataOfTypes: its completion can be indefinitely delayed by WebKit while a view is
    // hidden, which would deadlock WaitForExit.  The old document, scripts, handlers, and all
    // native callback routes have already been synchronously detached above.
    m_impl->ReturnPooledMacHost();
    CompleteCloseAfterWebKitTeardown();
}

void InfiniFrameWindow::CompleteCloseAfterWebKitTeardown()
{
    infiniframe::macos::LogLifecycle("window-webkit-teardown-complete", this);

    SignalWindowClosed();
    CompleteOperationsForClose();
    CompleteNavigationForClose();
    CompleteDialogsForClose();

    {
        infiniframe::macos::NativeCallbackScope callbackScope;
        InvokeClosed();
    }

    // InvokeClosed() fires the managed ClosedCallback which may synchronously
    // trigger Dispose() → ScheduleDeferredDestruction() → dispatch_async(delete
    // this).  That dispatch is deferred to the NEXT main-queue iteration, so
    // m_impl is still alive right now.  Call SignalTeardown() synchronously to
    // lock _milestoneMutex while m_impl is guaranteed to be valid.
    //
    // Previous approaches failed because:
    // - CFRunLoopPerformBlock: GCD dispatch sources (used by delete this)
    //   fire BEFORE CFRunLoopPerformBlock blocks → use-after-free.
    // - dispatch_async(SignalTeardown): FIFO ordering between dispatch_async
    //   calls from DIFFERENT threads is not guaranteed → may still race.
    //
    // This approach is safe because SignalTeardown() runs inline before the
    // function returns, and delete this is always deferred via dispatch_async.
    SignalTeardown();
}

void InfiniFrameWindow::ScheduleTeardownCompletion()
{
    CompleteOperationsForClose();
    CompleteNavigationForClose();
    CompleteDialogsForClose();
    SignalTeardown();
}

void InfiniFrameWindow::ScheduleDeferredDestruction()
{
    void (^requestDestruction)() = ^{
        bool expected = false;
        if (!this->m_impl->_nativeDestructionScheduled.compare_exchange_strong(expected, true,
                std::memory_order_acq_rel, std::memory_order_relaxed))
            return;

        infiniframe::macos::LogLifecycle("window-destruction-request", this);
        if (!this->m_impl->_isClosingOrClosed) {
            this->CloseWebView();
            this->PrepareForDeferredDestruction();
            // Completion queues our one deletion turn after the asynchronous store reset.
            return;
        }
        this->PrepareForDeferredDestruction();

        // Still defer one main-queue turn so disposal from
        // an AppKit delegate cannot delete the instance while that delegate is executing.
        bool deletionExpected = false;
        if (this->m_impl->_deletionQueued.compare_exchange_strong(deletionExpected, true,
                std::memory_order_acq_rel, std::memory_order_relaxed)) {
            dispatch_async(dispatch_get_main_queue(), ^{
                delete this;
            });
        }
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
