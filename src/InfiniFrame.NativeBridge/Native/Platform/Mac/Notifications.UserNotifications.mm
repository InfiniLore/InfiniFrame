#ifdef __APPLE__

#include "Window.Cocoa.Internal.h"

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
    [center addNotificationRequest: request withCompletionHandler: ^(NSError * _Nullable error) {
        (void)error;
    }];
}

#endif
