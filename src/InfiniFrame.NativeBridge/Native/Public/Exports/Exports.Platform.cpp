// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Public/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
#ifdef _WIN32
EXPORTED InteropStatus InfiniFrame_register_win32(const HINSTANCE hInstance) {
    return RunExportStatus([&] {
        if (hInstance == nullptr)
            throw std::invalid_argument("Argument 'hInstance' is null.");
        InfiniFrameWindow::Register(hInstance);
    });
}

EXPORTED InteropStatus InfiniFrame_getHwnd_win32(InfiniFrameWindow* instance, HWND* value) {
    ResetOut(value, static_cast<HWND>(nullptr));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->getHwnd();
    });
}

EXPORTED InteropStatus
InfiniFrame_setWebView2RuntimePath_win32(InfiniFrameWindow*, const AutoString webView2RuntimePath) {
    return RunExportStatus([&] {
        if (!EnsureNotNull(webView2RuntimePath, "webView2RuntimePath"))
            return;
        InfiniFrameWindow::SetWebView2RuntimePath(webView2RuntimePath);
    });
}

EXPORTED InteropStatus InfiniFrame_GetNotificationsEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetNotificationsEnabled(enabled);
    });
}
#elif __APPLE__
EXPORTED InteropStatus InfiniFrame_register_mac() {
    return RunExportStatus([] { InfiniFrameWindow::Register(); });
}
#endif
}
