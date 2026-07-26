// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
#ifdef _WIN32
EXPORTED InteropStatus InfiniFrameNative_register_win32(const HINSTANCE hInstance) {
    return RunExportStatus([&] {
        if (hInstance == nullptr)
            throw std::invalid_argument("Argument 'hInstance' is null.");
        InfiniFrameWindow::Register(hInstance);
    });
}

EXPORTED InteropStatus InfiniFrameNative_getHwnd_win32(InfiniFrameWindow* instance, HWND* value) {
    ResetOut(value, static_cast<HWND>(nullptr));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->getHwnd();
    });
}

EXPORTED InteropStatus
InfiniFrameNative_setWebView2RuntimePath_win32(InfiniFrameWindow* instance, const AutoString webView2RuntimePath) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(webView2RuntimePath, "webView2RuntimePath"))
            return;
        window->SetWebView2RuntimePath(webView2RuntimePath);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetNotificationsEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetNotificationsEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_getWebView2RuntimeVersion_win32(AutoString* value) {
    ResetOut(value, static_cast<AutoString>(nullptr));
    return RunExportStatus([&] {
        if (!EnsureOutNotNull(value, "value"))
            return;

        LPWSTR versionInfo = nullptr;
        const HRESULT hr = GetAvailableCoreWebView2BrowserVersionString(nullptr, &versionInfo);
        if (FAILED(hr) || versionInfo == nullptr)
            return;

        *value = DuplicateString(versionInfo);
        CoTaskMemFree(versionInfo);
    });
}
#endif
}
