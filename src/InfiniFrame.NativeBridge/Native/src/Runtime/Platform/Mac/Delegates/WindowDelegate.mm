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

- (NSDragOperation)draggingEntered:(id<NSDraggingInfo>)sender {
    return NSDragOperationCopy;
}

- (BOOL)performDragOperation:(id<NSDraggingInfo>)sender {
    if (infiniFrame == nullptr) return NO;
    infiniframe::macos::NativeCallbackScope callbackScope;

    NSPasteboard* pasteboard = [sender draggingPasteboard];
    NSArray<NSURL*>* urls = [pasteboard readObjectsForClasses:@[[NSURL class]] options:nil];

    if (urls.count > 0) {
        std::vector<std::string> paths;
        for (NSURL* url in urls) {
            if (url.isFileURL) {
                paths.push_back(url.path.UTF8String);
            }
        }

        if (!paths.empty()) {
            NSPoint dropPoint = [sender draggingLocation];
            std::vector<const char*> autoStrings;
            autoStrings.reserve(paths.size());
            for (const auto& p : paths) {
                autoStrings.push_back(p.c_str());
            }

            infiniFrame->InvokeFileDropped(
                autoStrings.data(), static_cast<int>(autoStrings.size()),
                static_cast<int>(dropPoint.x), static_cast<int>(dropPoint.y));
            return YES;
        }
    }
    return NO;
}
@end
