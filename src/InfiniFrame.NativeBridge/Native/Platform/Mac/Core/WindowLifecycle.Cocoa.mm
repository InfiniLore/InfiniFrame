#ifdef __APPLE__

#include "../Window.Cocoa.Internal.h"

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

void InfiniFrameWindow::ShowNotification(AutoString title, AutoString body)
{
    UNMutableNotificationContent *objNotificationContent = [[UNMutableNotificationContent alloc] init];
    objNotificationContent.title = [NSString stringWithUTF8String: title];
    objNotificationContent.body = [NSString stringWithUTF8String: body];
    objNotificationContent.sound = [UNNotificationSound defaultSound];
    UNTimeIntervalNotificationTrigger *trigger = [UNTimeIntervalNotificationTrigger triggerWithTimeInterval: 0.3 repeats: NO];
    UNNotificationRequest *request = [UNNotificationRequest requestWithIdentifier: @"three"
                                                                          content: objNotificationContent
                                                                          trigger: trigger];
    UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
    [center addNotificationRequest: request withCompletionHandler: ^(NSError * _Nullable error) {}];
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
    // Not implemented on macOS
}

#endif
