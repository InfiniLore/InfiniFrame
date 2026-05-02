#include "Interop/ExportApi.h"

using namespace InfiniFrame::Native::Interop;

extern "C" {
#ifdef _WIN32
    /**
     * @brief Register InfiniFrame window class (Windows)
     * @param hInstance Application instance handle
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_register_win32(const HINSTANCE hInstance) {
        return RunExportStatus([&] {
            InfiniFrameWindow::Register(hInstance);
        });
    }

    /**
     * @brief Get native window handle (Windows)
     * @param instance InfiniFrame instance
     * @return HWND window handle
     */
    INFINIFRAME_NATIVE_EXPORT HWND InfiniFrame_getHwnd_win32(InfiniFrameWindow* instance) {
        return RunWindowReturnExport(instance, static_cast<HWND>(nullptr), [](InfiniFrameWindow& window) {
            return window.getHwnd();
        });
    }

    /**
     * @brief Set WebView2 runtime path (Windows)
     * @param instance InfiniFrame instance
     * @param webView2RuntimePath Path to WebView2 runtime
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_setWebView2RuntimePath_win32(
        InfiniFrameWindow*,
        const AutoString webView2RuntimePath
        ) {
        return RunExportStatus([&] {
            InfiniFrameWindow::SetWebView2RuntimePath(webView2RuntimePath);
        });
    }

    /**
     * @brief Get notifications enabled status (Windows)
     * @param instance InfiniFrame instance
     * @param disabled Output: notifications disabled status
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_GetNotificationsEnabled(InfiniFrameWindow* instance, bool* disabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetNotificationsEnabled(disabled);
        }, disabled);
    }
#elif __APPLE__
    /**
     * @brief Register InfiniFrame application (macOS)
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_register_mac() {
        return RunExportStatus([] {
            InfiniFrameWindow::Register();
        });
    }
#endif
}
