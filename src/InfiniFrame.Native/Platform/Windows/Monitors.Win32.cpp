#include "WindowImpl.Win32.h"

#include <Shellscalingapi.h>

namespace {
    BOOL CALLBACK MonitorEnum(const HMONITOR monitor, HDC, LPRECT, const LPARAM arg) {
        auto callback = reinterpret_cast<GetAllMonitorsCallback>(arg);
        UINT dpiX = 96;
        UINT dpiY = 96;
        MONITORINFO info = {};
        info.cbSize = sizeof(MONITORINFO);
        GetMonitorInfo(monitor, &info);
        GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, &dpiX, &dpiY);

        Monitor props = {};
        props.monitor.x = info.rcMonitor.left;
        props.monitor.y = info.rcMonitor.top;
        props.monitor.width = info.rcMonitor.right - info.rcMonitor.left;
        props.monitor.height = info.rcMonitor.bottom - info.rcMonitor.top;
        props.work.x = info.rcWork.left;
        props.work.y = info.rcWork.top;
        props.work.width = info.rcWork.right - info.rcWork.left;
        props.work.height = info.rcWork.bottom - info.rcWork.top;
        props.scale = dpiY / 96.0;

        return callback(&props) ? TRUE : FALSE;
    }
}

unsigned int InfiniFrameWindow::GetScreenDpi() const {
    return GetDpiForWindow(m_impl->_hWnd);
}

void InfiniFrameWindow::GetAllMonitors(GetAllMonitorsCallback callback) const {
    if (callback == nullptr)
        return;

    EnumDisplayMonitors(
        nullptr,
        nullptr,
        MonitorEnum,
        reinterpret_cast<LPARAM>(callback)
        );
}
