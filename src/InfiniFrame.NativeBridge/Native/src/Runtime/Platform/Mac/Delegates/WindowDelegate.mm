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
    if (infiniFrame == nullptr) return YES;
    infiniframe::macos::NativeCallbackScope callbackScope;
    return !infiniFrame->InvokeClose();
}

- (void)windowWillClose:(NSNotification*)notification
{
    if (infiniFrame == nullptr) return;
    infiniFrame->CloseWebView();
    {
        infiniframe::macos::NativeCallbackScope callbackScope;
        infiniFrame->InvokeClosed();
    }
    // This must be the final access through the raw C++ back-pointer. It wakes
    // non-main waiters only after the reverse P/Invoke has fully returned.
    if (infiniFrame != nullptr)
        infiniFrame->SignalWindowClosed();
}
@end
