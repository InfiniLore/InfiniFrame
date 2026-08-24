// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
#ifdef _WIN32
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
#endif
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
#ifdef _WIN32
EXPORTED InteropStatus InfiniFrameNative_register_win32(const HINSTANCE hInstance) {
    return RunExportStatus(
        [&] {
            if (hInstance == nullptr)
                throw std::invalid_argument("Argument 'hInstance' is null.");
            InfiniFrameWindow::Register(hInstance);
        });
}

EXPORTED InteropStatus InfiniFrameNative_getHwnd_win32(InfiniFrameWindow* instance, HWND* value) {
    ResetOut(value, static_cast<HWND>(nullptr));
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(value, "value"))
                return;
            *value = window->getHwnd();
        });
}

EXPORTED InteropStatus
InfiniFrameNative_setWebView2RuntimePath_win32(InfiniFrameWindow* instance, const char* webView2RuntimePath) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(webView2RuntimePath, "webView2RuntimePath"))
                return;
            window->SetWebView2RuntimePath(webView2RuntimePath);
        });
}

EXPORTED InteropStatus InfiniFrameNative_GetNotificationsEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetNotificationsEnabled(enabled);
        });
}

EXPORTED InteropStatus InfiniFrameNative_getWebView2RuntimeVersion_win32(const char** value) {
    ResetOut(value, static_cast<const char*>(nullptr));
    return RunExportStatus(
        [&] {
            if (!EnsureOutNotNull(value, "value"))
                return;

            LPWSTR versionInfo = nullptr;
            const HRESULT hr = GetAvailableCoreWebView2BrowserVersionString(nullptr, &versionInfo);
            if (FAILED(hr) || versionInfo == nullptr)
                return;

            auto versionUtf8 = WideToUtf8(versionInfo);
            *value = DuplicateString(versionUtf8.c_str());
            CoTaskMemFree(versionInfo);
        });
}
#endif
}