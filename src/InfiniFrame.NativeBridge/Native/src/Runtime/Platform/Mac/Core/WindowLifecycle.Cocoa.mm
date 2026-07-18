// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#include <stdexcept>

#include "../Window.Cocoa.Internal.h"

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
    // Do not call [NSApp run] here. Test hosts and embedding applications can already
    // own the main CFRunLoop; starting a nested application run loop may terminate the
    // process when the last test window closes. Pump only until this window closes.
    __block bool windowClosed = m_impl->_windowClosed;
    id observer = [[NSNotificationCenter defaultCenter]
        addObserverForName: NSWindowWillCloseNotification
        object: m_impl->_window
        queue: nil
        usingBlock: ^(NSNotification*) {
            windowClosed = true;
        }];

    while (!windowClosed) {
        [[NSRunLoop currentRunLoop] runMode: NSDefaultRunLoopMode
                                 beforeDate: [NSDate dateWithTimeIntervalSinceNow: 0.05]];
    }

    [[NSNotificationCenter defaultCenter] removeObserver: observer];
}

void InfiniFrameWindow::CloseWebView()
{
    m_impl->_isClosingOrClosed = true;
    m_impl->_windowClosed = true;
    m_impl->_webviewReady = false;
    m_impl->_pendingWebMessages.clear();

    if (m_impl->_webviewConfiguration != nil) {
        [m_impl->_webviewConfiguration.userContentController removeScriptMessageHandlerForName:@"infiniFrameInterop"];
    }

    if (m_impl->_webview != nil) {
        m_impl->_webview.UIDelegate = nil;
        m_impl->_webview.navigationDelegate = nil;
    }
}
