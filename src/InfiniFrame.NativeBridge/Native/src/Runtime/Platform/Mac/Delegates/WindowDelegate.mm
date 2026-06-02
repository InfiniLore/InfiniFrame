// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#import "WindowDelegate.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

@implementation WindowDelegate : NSObject
- (void)windowDidResize:(NSNotification *)notification {
    int width, height;
    infiniFrame->GetSize(&width, &height);
    infiniFrame->InvokeResize(width, height);
}

- (void)windowDidMove:(NSNotification *)notification {
    int x, y;
    infiniFrame->GetPosition(&x, &y);
    infiniFrame->InvokeMove(x, y);
}

- (void)windowDidBecomeKey:(NSNotification *)notification {
    infiniFrame->InvokeFocusIn();
}

- (void)windowDidResignKey:(NSNotification *)notification {
    infiniFrame->InvokeFocusOut();
}

- (void)windowDidMiniaturize:(NSNotification *)notification {
    infiniFrame->InvokeMinimized();
}

- (void)windowDidDeminiaturize:(NSNotification *)notification {
    infiniFrame->InvokeRestored();
}

- (BOOL)windowShouldClose:(id)sender
{
    return !infiniFrame->InvokeClose();
}

- (void)windowWillClose:(NSNotification*)notification
{
    infiniFrame->InvokeClosed();
}
@end
