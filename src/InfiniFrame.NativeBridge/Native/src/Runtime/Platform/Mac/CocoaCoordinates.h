#pragma once

#include <Cocoa/Cocoa.h>

namespace infiniframe::macos {
    inline CGFloat GlobalDesktopTop() {
        NSArray<NSScreen*>* screens = [NSScreen screens];
        if ([screens count] == 0)
            return 0;

        // Cocoa screen coordinates are bottom-left based. InfiniFrame exposes the
        // Windows-compatible global desktop coordinate space whose origin is the
        // top-left of the primary display. NSScreen's first entry is the primary display.
        return NSMaxY([[screens objectAtIndex:0] frame]);
    }

    inline NSRect ToInfiniFrameRect(NSRect cocoaRect) {
        cocoaRect.origin.y = GlobalDesktopTop() - NSMaxY(cocoaRect);
        return cocoaRect;
    }

    inline CGFloat ToCocoaWindowOriginY(CGFloat top, CGFloat height) {
        return GlobalDesktopTop() - top - height;
    }
}
