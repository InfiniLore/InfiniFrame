// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Window.Cocoa.Internal.h"
#include "CocoaCoordinates.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::GetAllMonitors(GetAllMonitorsCallback callback) const
{
    if (callback)
    {
        for (NSScreen* screen in [NSScreen screens])
        {
            Monitor props = {};

            NSRect frame = infiniframe::macos::ToInfiniFrameRect([screen frame]);
            props.monitor.x = static_cast<int>(roundf(frame.origin.x));
            props.monitor.y = static_cast<int>(roundf(frame.origin.y));
            props.monitor.width = static_cast<int>(roundf(frame.size.width));
            props.monitor.height = static_cast<int>(roundf(frame.size.height));

            NSRect vframe = infiniframe::macos::ToInfiniFrameRect([screen visibleFrame]);
            props.work.x = static_cast<int>(roundf(vframe.origin.x));
            props.work.y = static_cast<int>(roundf(vframe.origin.y));
            props.work.width = static_cast<int>(roundf(vframe.size.width));
            props.work.height = static_cast<int>(roundf(vframe.size.height));

            props.scale = [screen backingScaleFactor];

            callback(&props);
        }
    }
}
