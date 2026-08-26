#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
/**
 * @file AppDelegate.h
 * @brief NSApplication delegate that bootstraps the Cocoa run loop and handles app-level window events
 */
#include <Cocoa/Cocoa.h>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/**
 * @brief Application-level delegate conforming to NSApplicationDelegate, NSWindowDelegate,
 * and NSUserNotificationCenterDelegate.
 *
 * Responsible for initialising the Cocoa application, forwarding notification events,
 * and acting as a fallback window delegate when a per-window delegate is not set
 */
@
interface AppDelegate :
    NSObject<NSApplicationDelegate, NSUserNotificationCenterDelegate>
@
end
