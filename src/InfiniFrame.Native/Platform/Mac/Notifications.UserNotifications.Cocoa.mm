#ifdef __APPLE__

#include "Platform/Mac/WindowImpl.Cocoa.h"

void InfiniFrameWindow::ShowNotification(AutoString title, AutoString body)
{
    UNMutableNotificationContent* notificationContent = [[[UNMutableNotificationContent alloc] init] autorelease];
    notificationContent.title = [NSString stringWithUTF8String: title == nullptr ? "" : title];
    notificationContent.body = [NSString stringWithUTF8String: body == nullptr ? "" : body];
    notificationContent.sound = [UNNotificationSound defaultSound];

    UNTimeIntervalNotificationTrigger* trigger = [UNTimeIntervalNotificationTrigger
        triggerWithTimeInterval: 0.3
        repeats: NO];
    UNNotificationRequest* request = [UNNotificationRequest
        requestWithIdentifier: @"three"
        content: notificationContent
        trigger: trigger];

    UNUserNotificationCenter* center = [UNUserNotificationCenter currentNotificationCenter];
    [center addNotificationRequest: request withCompletionHandler: ^(NSError* _Nullable) {}];
}

#endif
