#ifdef __APPLE__

#include "Platform/Mac/WindowImpl.Cocoa.h"

#include <cmath>

std::vector<Monitor> InfiniFrameWindow::Impl::GetMonitors() const
{
    std::vector<Monitor> monitors;

    for (NSScreen *screen : [NSScreen screens])
    {
        NSRect monitorFrame = [screen frame];
        Monitor::MonitorRect monitorArea;
        monitorArea.x = static_cast<int>(roundf(monitorFrame.origin.x));
        monitorArea.y = static_cast<int>(roundf(monitorFrame.origin.y));
        monitorArea.width = static_cast<int>(roundf(monitorFrame.size.width));
        monitorArea.height = static_cast<int>(roundf(monitorFrame.size.height));

        NSRect workFrame = [screen visibleFrame];
        Monitor::MonitorRect workArea;
        workArea.x = static_cast<int>(roundf(workFrame.origin.x));
        workArea.y = static_cast<int>(roundf(workFrame.origin.y));
        workArea.width = static_cast<int>(roundf(workFrame.size.width));
        workArea.height = static_cast<int>(roundf(workFrame.size.height));

        CGFloat scaleFactor = [screen backingScaleFactor];
        monitors.push_back({monitorArea, workArea, static_cast<double>(scaleFactor)});
    }

    return monitors;
}

#endif
