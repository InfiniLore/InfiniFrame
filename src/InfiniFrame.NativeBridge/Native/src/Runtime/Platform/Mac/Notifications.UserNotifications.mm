// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Window.Cocoa.Internal.h"
#include "Api/Utilities/ExportStringHelpers.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using namespace infiniframe::exports;

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
    [objNotificationContent release];
}

void InfiniFrameWindow::ShowNotificationWithOptions(
    AutoString title, AutoString body, AutoString iconPath, int urgency, AutoString tag
) {
    UNMutableNotificationContent *objNotificationContent = [[UNMutableNotificationContent alloc] init];
    objNotificationContent.title = [NSString stringWithUTF8String: title];
    objNotificationContent.body = [NSString stringWithUTF8String: body];
    objNotificationContent.sound = [UNNotificationSound defaultSound];

    const char* iconStr = NullToEmpty(iconPath);
    if (iconStr[0] != '\0') {
        NSString *iconPathStr = [NSString stringWithUTF8String: iconStr];
        NSURL *iconURL = [NSURL fileURLWithPath: iconPathStr];
        UNNotificationAttachment *attachment = [UNNotificationAttachment attachmentWithIdentifier: @"icon"
                                                                                              URL: iconURL
                                                                                          options: nil
                                                                                            error: nil];
        if (attachment) {
            objNotificationContent.attachments = @[attachment];
        }
    }

    if (@available(macOS 12.0, *)) {
        switch (urgency) {
            case 1: objNotificationContent.interruptionLevel = UNNotificationInterruptionLevelPassive; break;
            case 2: objNotificationContent.interruptionLevel = UNNotificationInterruptionLevelTimeSensitive; break;
            case 3: objNotificationContent.interruptionLevel = UNNotificationInterruptionLevelCritical; break;
            default: objNotificationContent.interruptionLevel = UNNotificationInterruptionLevelActive; break;
        }
    }

    const char* tagStr = NullToEmpty(tag);
    NSString *identifier = tagStr[0] == '\0'
        ? [[NSUUID UUID] UUIDString]
        : [NSString stringWithUTF8String: tagStr];

    UNTimeIntervalNotificationTrigger *trigger = [UNTimeIntervalNotificationTrigger triggerWithTimeInterval: 0.3 repeats: NO];
    UNNotificationRequest *request = [UNNotificationRequest requestWithIdentifier: identifier
                                                                          content: objNotificationContent
                                                                          trigger: trigger];
    UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
    [center addNotificationRequest: request withCompletionHandler: ^(NSError * _Nullable error) {
        (void)error;
    }];
    [objNotificationContent release];
}

void InfiniFrameWindow::BeginShowNotification(
    uint64_t operationId,
    AutoString title, AutoString body, AutoString iconPath, int urgency, AutoString tag,
    OperationCompletedCallback completion, void* completionContext
) {
    ShowNotificationWithOptions(title, body, iconPath, urgency, tag);

    if (completion) {
        completion(completionContext, operationId, 0, 0, nullptr);
    }
}

void InfiniFrameWindow::CancelNotification(uint64_t operationId, bool* canceled) {
    if (canceled) *canceled = false;
}
