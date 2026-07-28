// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#import "WindowDelegate.h"

#include "../MacDiagnostics.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
@implementation WindowDelegate : NSObject
- (void)windowDidResize:(NSNotification *)notification {
    if (infiniFrame == nullptr) return;
    infiniframe::macos::NativeCallbackScope callbackScope;
    int width, height;
    infiniFrame->GetSize(&width, &height);
    infiniFrame->InvokeResize(width, height);
}

- (void)windowDidMove:(NSNotification *)notification {
    if (infiniFrame == nullptr) return;
    infiniframe::macos::NativeCallbackScope callbackScope;
    int x, y;
    infiniFrame->GetPosition(&x, &y);
    infiniFrame->InvokeMove(x, y);
}

- (void)windowDidBecomeKey:(NSNotification *)notification {
    if (infiniFrame != nullptr) {
        infiniframe::macos::NativeCallbackScope callbackScope;
        infiniFrame->InvokeFocusIn();
    }
}

- (void)windowDidResignKey:(NSNotification *)notification {
    if (infiniFrame != nullptr) {
        infiniframe::macos::NativeCallbackScope callbackScope;
        infiniFrame->InvokeFocusOut();
    }
}

- (void)windowDidMiniaturize:(NSNotification *)notification {
    if (infiniFrame != nullptr) {
        infiniframe::macos::NativeCallbackScope callbackScope;
        infiniFrame->InvokeMinimized();
    }
}

- (void)windowDidDeminiaturize:(NSNotification *)notification {
    if (infiniFrame != nullptr) {
        infiniframe::macos::NativeCallbackScope callbackScope;
        infiniFrame->InvokeRestored();
    }
}

- (BOOL)windowShouldClose:(id)sender
{
    (void)sender;
    if (infiniFrame == nullptr) return NO;
    infiniframe::macos::NativeCallbackScope callbackScope;
    if (infiniFrame->InvokeClose()) return NO;
    // A logical close deliberately does not close NSWindow.  Keeping the complete host alive is
    // what prevents WKWebView display-link teardown during ordinary managed lifetimes.
    infiniFrame->CloseWebView();
    return NO;
}

- (void)windowWillClose:(NSNotification*)notification
{
    if (infiniFrame == nullptr) return;
    infiniFrame->CloseWebView();
}
@end
