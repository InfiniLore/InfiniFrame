// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#import "AppDelegate.h"
#include "../Window.Cocoa.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
@implementation AppDelegate

- (void)applicationDidFinishLaunching:(NSNotification *)notification {
    [NSApp activateIgnoringOtherApps:YES];
}

- (BOOL)applicationShouldTerminateAfterLastWindowClosed:(NSApplication *)sender {
    (void)sender;
    // InfiniFrame is an embeddable library. Closing a test or application window must
    // never terminate its host process; the managed host owns process lifetime.
    return NO;
}

- (void)applicationWillTerminate:(NSNotification *)notification {
    (void)notification;
    // The pool normally survives for the process lifetime.  AppKit gives us a main-thread
    // shutdown boundary at which its retained hosts can be released deterministically.
    DrainPooledMacHosts();
}

@end
