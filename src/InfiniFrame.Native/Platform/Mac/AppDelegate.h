#ifdef __APPLE__
#pragma once
/**
 * @file AppDelegate.h
 * @brief NSApplication delegate that bootstraps the Cocoa run loop and handles app-level window events
 */
#include <Cocoa/Cocoa.h>

/**
 * @brief Application-level delegate conforming to NSApplicationDelegate, NSWindowDelegate,
 * and NSUserNotificationCenterDelegate.
 *
 * Responsible for initialising the Cocoa application, forwarding notification events,
 * and acting as a fallback window delegate when a per-window delegate is not set
 */
@interface AppDelegate : NSObject <NSApplicationDelegate, NSWindowDelegate, NSUserNotificationCenterDelegate> {
    NSWindow * window; /// Reference to the primary application window
}
@end
#endif
