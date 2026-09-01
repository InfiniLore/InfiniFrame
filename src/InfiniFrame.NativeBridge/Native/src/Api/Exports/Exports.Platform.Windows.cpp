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
/// @brief Registers the Win32 window class.
/// @param hInstance The application instance handle.
/// @return InteropStatus
/// @deprecated Use InfiniFrameNative_Application_register_win32() with InfiniFrameApplication instead.
EXPORTED InteropStatus InfiniFrameNative_register_win32(const HINSTANCE hInstance) {
    return RunExportStatus(
        [&] {
            if (hInstance == nullptr)
                throw std::invalid_argument("Argument 'hInstance' is null.");
            InfiniFrameWindow::Register(hInstance);
        });
}

/// @brief Gets the Win32 HWND handle for the window.
/// @param instance The window handle.
/// @param[out] value Receives the HWND handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_getHwnd_win32(InfiniFrameWindow* instance, HWND* value) {
    ResetOut(value, static_cast<HWND>(nullptr));
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(value, "value"))
                return;
            *value = window->getHwnd();
        });
}

/// @brief Sets the WebView2 runtime path for the window.
/// @param instance The window handle.
/// @param webView2RuntimePath Null-terminated path to the WebView2 runtime.
/// @return InteropStatus
EXPORTED InteropStatus
InfiniFrameNative_setWebView2RuntimePath_win32(InfiniFrameWindow* instance, const char* webView2RuntimePath) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(webView2RuntimePath, "webView2RuntimePath"))
                return;
            window->SetWebView2RuntimePath(webView2RuntimePath);
        });
}

/// @brief Queries whether notifications are enabled.
/// @param instance The window handle.
/// @param[out] enabled Receives true if notifications are enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetNotificationsEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetNotificationsEnabled(enabled);
        });
}

/// @brief Gets the installed WebView2 runtime version.
/// @param[out] value Owned string containing the version, caller must free with InfiniFrameNative_FreeString.
/// @return InteropStatus
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