// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#include "../Window.Cocoa.Internal.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

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
    m_impl->_isClosingOrClosed = true;

    if (m_impl->_parentWillCloseObserver != nil) {
        [[NSNotificationCenter defaultCenter] removeObserver:m_impl->_parentWillCloseObserver];
        m_impl->_parentWillCloseObserver = nil;
    }

    if (m_impl->_nativeParentWindow != nil && m_impl->_window != nil) {
        [m_impl->_nativeParentWindow removeChildWindow:m_impl->_window];
        m_impl->_nativeParentWindow = nil;
    }

    if (m_impl->_chromeless)
        [m_impl->_window close];
    else
        [m_impl->_window performClose: m_impl->_window];
}

void InfiniFrameWindow::WaitForExit()
{
    if (![NSApp isRunning]) {
        [NSApp run];
        return;
    }

    __block bool windowClosed = false;
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
    m_impl->_webviewReady = false;
    m_impl->_pendingWebMessages.clear();
}
