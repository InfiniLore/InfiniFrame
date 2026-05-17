#include "Core/InfiniFrame.h"
#include "Exports/ExportGuards.h"

#ifdef __linux__
#include <glib.h>
#endif

#ifdef _WIN32
#define EXPORTED __declspec(dllexport)
#else
#define EXPORTED
#endif

using infiniframe::exports::EnsureNotNull;
using infiniframe::exports::GetLastErrorMessageCopy;
using infiniframe::exports::ResetOut;
using infiniframe::exports::ResetOut2;
using infiniframe::exports::RunExportStatus;
using infiniframe::exports::RunReturnExport;
using infiniframe::exports::RunWindowExportStatus;
using infiniframe::exports::RunWindowReturnExport;

extern "C" {
#ifdef _WIN32
    EXPORTED InteropStatus InfiniFrame_register_win32(const HINSTANCE hInstance) {
        return RunExportStatus([&] {
            if (hInstance == nullptr) throw std::invalid_argument("Argument 'hInstance' is null.");
            InfiniFrameWindow::Register(hInstance);
        });
    }

    EXPORTED InteropStatus InfiniFrame_getHwnd_win32(InfiniFrameWindow* instance, HWND* value) {
        ResetOut(value, static_cast<HWND>(nullptr));
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
            *value = window->getHwnd();
        });
    }

    EXPORTED InteropStatus InfiniFrame_setWebView2RuntimePath_win32(InfiniFrameWindow*, const AutoString webView2RuntimePath) {
        return RunExportStatus([&] {
            if (!EnsureNotNull(webView2RuntimePath, "webView2RuntimePath")) throw std::invalid_argument("Argument 'webView2RuntimePath' is null.");
            InfiniFrameWindow::SetWebView2RuntimePath(webView2RuntimePath);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetNotificationsEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetNotificationsEnabled(enabled);
        });
    }
#elif __APPLE__
    EXPORTED InteropStatus InfiniFrame_register_mac() {
        return RunExportStatus([] { InfiniFrameWindow::Register(); });
    }
#endif

    EXPORTED InteropStatus InfiniFrame_ctor(InfiniFrameInitParams* initParams, InfiniFrameWindow** value) {
        ResetOut(value, static_cast<InfiniFrameWindow*>(nullptr));
        return RunExportStatus([&] {
            if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
            if (initParams == nullptr) throw std::invalid_argument("Argument 'initParams' is null.");
            if (initParams->Size != static_cast<int>(sizeof(InfiniFrameInitParams))) {
                throw std::invalid_argument("InfiniFrameInitParams.Size does not match native struct size.");
            }
            auto instance = std::make_unique<InfiniFrameWindow>(initParams);
            *value = instance.release();
        });
    }

    EXPORTED InteropStatus InfiniFrame_dtor(InfiniFrameWindow* instance) {
        return RunExportStatus([&] {
            if (!EnsureNotNull(instance, "instance")) throw std::invalid_argument("Argument 'instance' is null.");
            std::unique_ptr<InfiniFrameWindow> guard{instance};
        });
    }

    EXPORTED InteropStatus InfiniFrame_Center(InfiniFrameWindow* instance) { return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Center(); }); }
    EXPORTED InteropStatus InfiniFrame_ClearBrowserAutoFill(InfiniFrameWindow* instance) { return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->ClearBrowserAutoFill(); }); }
    EXPORTED InteropStatus InfiniFrame_Close(InfiniFrameWindow* instance) { return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Close(); }); }

    EXPORTED InteropStatus InfiniFrame_GetTransparentEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetTransparentEnabled(enabled);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetContextMenuEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetContextMenuEnabled(enabled);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetZoomEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetZoomEnabled(enabled);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetDevToolsEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetDevToolsEnabled(enabled);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetFullScreen(InfiniFrameWindow* instance, bool* fullScreen) {
        ResetOut(fullScreen, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(fullScreen, "fullScreen")) throw std::invalid_argument("Argument 'fullScreen' is null.");
            window->GetFullScreen(fullScreen);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetGrantBrowserPermissions(InfiniFrameWindow* instance, bool* grant) {
        ResetOut(grant, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(grant, "grant")) throw std::invalid_argument("Argument 'grant' is null.");
            window->GetGrantBrowserPermissions(grant);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetUserAgent(InfiniFrameWindow* instance, AutoString* value) {
        ResetOut(value, static_cast<AutoString>(nullptr));
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
            *value = window->GetUserAgent();
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetMediaAutoplayEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetMediaAutoplayEnabled(enabled);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetFileSystemAccessEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetFileSystemAccessEnabled(enabled);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetWebSecurityEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetWebSecurityEnabled(enabled);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetJavascriptClipboardAccessEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetJavascriptClipboardAccessEnabled(enabled);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetMediaStreamEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetMediaStreamEnabled(enabled);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetSmoothScrollingEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetSmoothScrollingEnabled(enabled);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetMaximized(InfiniFrameWindow* instance, bool* isMaximized) {
        ResetOut(isMaximized, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(isMaximized, "isMaximized")) throw std::invalid_argument("Argument 'isMaximized' is null.");
            window->GetMaximized(isMaximized);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetMinimized(InfiniFrameWindow* instance, bool* isMinimized) {
        ResetOut(isMinimized, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(isMinimized, "isMinimized")) throw std::invalid_argument("Argument 'isMinimized' is null.");
            window->GetMinimized(isMinimized);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetIgnoreCertificateErrorsEnabled(InfiniFrameWindow* instance, bool* enabled) {
        ResetOut(enabled, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(enabled, "enabled")) throw std::invalid_argument("Argument 'enabled' is null.");
            window->GetIgnoreCertificateErrorsEnabled(enabled);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetPosition(InfiniFrameWindow* instance, int* x, int* y) {
        ResetOut2(x, y, 0);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(x, "x") || !EnsureNotNull(y, "y")) throw std::invalid_argument("GetPosition out argument is null.");
            window->GetPosition(x, y);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetResizable(InfiniFrameWindow* instance, bool* resizable) {
        ResetOut(resizable, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(resizable, "resizable")) throw std::invalid_argument("Argument 'resizable' is null.");
            window->GetResizable(resizable);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetScreenDpi(InfiniFrameWindow* instance, unsigned int* value) {
        ResetOut(value, static_cast<unsigned int>(0));
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
            *value = window->GetScreenDpi();
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetSize(InfiniFrameWindow* instance, int* width, int* height) {
        ResetOut2(width, height, 0);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(width, "width") || !EnsureNotNull(height, "height")) throw std::invalid_argument("GetSize out argument is null.");
            window->GetSize(width, height);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetMaxSize(InfiniFrameWindow* instance, int* width, int* height) {
        ResetOut2(width, height, 0);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(width, "width") || !EnsureNotNull(height, "height")) throw std::invalid_argument("GetMaxSize out argument is null.");
            window->GetMaxSize(width, height);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetMinSize(InfiniFrameWindow* instance, int* width, int* height) {
        ResetOut2(width, height, 0);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(width, "width") || !EnsureNotNull(height, "height")) throw std::invalid_argument("GetMinSize out argument is null.");
            window->GetMinSize(width, height);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetTitle(InfiniFrameWindow* instance, AutoString* value) {
        ResetOut(value, static_cast<AutoString>(nullptr));
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
            *value = window->GetTitle();
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetTopmost(InfiniFrameWindow* instance, bool* topmost) {
        ResetOut(topmost, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(topmost, "topmost")) throw std::invalid_argument("Argument 'topmost' is null.");
            window->GetTopmost(topmost);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetZoom(InfiniFrameWindow* instance, int* zoom) {
        ResetOut(zoom, 0);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(zoom, "zoom")) throw std::invalid_argument("Argument 'zoom' is null.");
            window->GetZoom(zoom);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetFocused(InfiniFrameWindow* instance, bool* isFocused) {
        ResetOut(isFocused, false);
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(isFocused, "isFocused")) throw std::invalid_argument("Argument 'isFocused' is null.");
            window->GetFocused(isFocused);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetIconFileName(InfiniFrameWindow* instance, AutoString* value) {
        ResetOut(value, static_cast<AutoString>(nullptr));
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
            *value = window->GetIconFileName();
        });
    }

    EXPORTED InteropStatus InfiniFrame_NavigateToString(InfiniFrameWindow* instance, const AutoString content) {
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(content, "content")) throw std::invalid_argument("Argument 'content' is null.");
            window->NavigateToString(content);
        });
    }

    EXPORTED InteropStatus InfiniFrame_NavigateToUrl(InfiniFrameWindow* instance, const AutoString url) {
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(url, "url")) throw std::invalid_argument("Argument 'url' is null.");
            window->NavigateToUrl(url);
        });
    }

    EXPORTED InteropStatus InfiniFrame_Restore(InfiniFrameWindow* instance) { return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Restore(); }); }

    EXPORTED InteropStatus InfiniFrame_SendWebMessage(InfiniFrameWindow* instance, const AutoString message) {
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(message, "message")) throw std::invalid_argument("Argument 'message' is null.");
            window->SendWebMessage(message);
        });
    }

    EXPORTED InteropStatus InfiniFrame_SetTransparentEnabled(InfiniFrameWindow* instance, const bool enabled) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTransparentEnabled(enabled); }); }
    EXPORTED InteropStatus InfiniFrame_SetContextMenuEnabled(InfiniFrameWindow* instance, const bool enabled) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetContextMenuEnabled(enabled); }); }
    EXPORTED InteropStatus InfiniFrame_SetZoomEnabled(InfiniFrameWindow* instance, const bool enabled) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetZoomEnabled(enabled); }); }
    EXPORTED InteropStatus InfiniFrame_SetDevToolsEnabled(InfiniFrameWindow* instance, const bool enabled) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetDevToolsEnabled(enabled); }); }
    EXPORTED InteropStatus InfiniFrame_SetFullScreen(InfiniFrameWindow* instance, const bool fullScreen) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetFullScreen(fullScreen); }); }

    EXPORTED InteropStatus InfiniFrame_SetIconFile(InfiniFrameWindow* instance, const AutoString filename) {
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(filename, "filename")) throw std::invalid_argument("Argument 'filename' is null.");
            window->SetIconFile(filename);
        });
    }

    EXPORTED InteropStatus InfiniFrame_SetMaximized(InfiniFrameWindow* instance, const bool maximized) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMaximized(maximized); }); }
    EXPORTED InteropStatus InfiniFrame_SetMaxSize(InfiniFrameWindow* instance, const int width, const int height) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMaxSize(width, height); }); }
    EXPORTED InteropStatus InfiniFrame_SetMinimized(InfiniFrameWindow* instance, const bool minimized) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMinimized(minimized); }); }
    EXPORTED InteropStatus InfiniFrame_SetMinSize(InfiniFrameWindow* instance, const int width, const int height) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMinSize(width, height); }); }
    EXPORTED InteropStatus InfiniFrame_SetPosition(InfiniFrameWindow* instance, const int x, const int y) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetPosition(x, y); }); }
    EXPORTED InteropStatus InfiniFrame_SetResizable(InfiniFrameWindow* instance, const bool resizable) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetResizable(resizable); }); }
    EXPORTED InteropStatus InfiniFrame_SetSize(InfiniFrameWindow* instance, const int width, const int height) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetSize(width, height); }); }

    EXPORTED InteropStatus InfiniFrame_SetTitle(InfiniFrameWindow* instance, const AutoString title) {
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(title, "title")) throw std::invalid_argument("Argument 'title' is null.");
            window->SetTitle(title);
        });
    }

    EXPORTED InteropStatus InfiniFrame_SetTopmost(InfiniFrameWindow* instance, const bool topmost) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTopmost(topmost); }); }
    EXPORTED InteropStatus InfiniFrame_SetZoom(InfiniFrameWindow* instance, const int zoom) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetZoom(zoom); }); }

    EXPORTED InteropStatus InfiniFrame_ShowNotification(InfiniFrameWindow* instance, const AutoString title, const AutoString body) {
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(title, "title") || !EnsureNotNull(body, "body")) throw std::invalid_argument("ShowNotification argument is null.");
            window->ShowNotification(title, body);
        });
    }

    EXPORTED InteropStatus InfiniFrame_WaitForExit(InfiniFrameWindow* instance) { return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->WaitForExit(); }); }

    EXPORTED InteropStatus InfiniFrame_FreeString(AutoString value) {
        return RunExportStatus([&] {
            if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
#ifdef _WIN32
            delete[] value;
#elif __linux__
            g_free(value);
#else
            free(value);
#endif
        });
    }

    EXPORTED InteropStatus InfiniFrame_FreeStringArray(AutoString* values, const int count) {
        return RunExportStatus([&] {
            if (!EnsureNotNull(values, "values")) throw std::invalid_argument("Argument 'values' is null.");
            if (count < 0) throw std::invalid_argument("Argument 'count' must be >= 0.");
            for (int i = 0; i < count; ++i) {
                if (values[i] != nullptr) {
                    InfiniFrame_FreeString(values[i]);
                }
            }
#ifdef _WIN32
            delete[] values;
#elif __linux__
            delete[] values;
#else
            free(values);
#endif
        });
    }

    EXPORTED InteropStatus InfiniFrame_ShowOpenFile(InfiniFrameWindow* inst, const AutoString title, const AutoString defaultPath, const bool multiSelect, AutoString* filters, const int filterCount, int* resultCount, AutoString** values) {
        ResetOut(resultCount, 0);
        ResetOut(values, static_cast<AutoString*>(nullptr));
        return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(resultCount, "resultCount")) throw std::invalid_argument("Argument 'resultCount' is null.");
            if (!EnsureNotNull(values, "values")) throw std::invalid_argument("Argument 'values' is null.");
            if (filterCount < 0) throw std::invalid_argument("Argument 'filterCount' must be >= 0.");
            *values = window->GetDialog()->ShowOpenFile(title, defaultPath, multiSelect, filters, filterCount, resultCount);
        });
    }

    EXPORTED InteropStatus InfiniFrame_ShowOpenFolder(InfiniFrameWindow* inst, const AutoString title, const AutoString defaultPath, const bool multiSelect, int* resultCount, AutoString** values) {
        ResetOut(resultCount, 0);
        ResetOut(values, static_cast<AutoString*>(nullptr));
        return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(resultCount, "resultCount")) throw std::invalid_argument("Argument 'resultCount' is null.");
            if (!EnsureNotNull(values, "values")) throw std::invalid_argument("Argument 'values' is null.");
            *values = window->GetDialog()->ShowOpenFolder(title, defaultPath, multiSelect, resultCount);
        });
    }

    EXPORTED InteropStatus InfiniFrame_ShowSaveFile(InfiniFrameWindow* inst, const AutoString title, const AutoString defaultPath, AutoString* filters, const int filterCount, const AutoString defaultFileName, AutoString* value) {
        ResetOut(value, static_cast<AutoString>(nullptr));
        return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
            if (filterCount < 0) throw std::invalid_argument("Argument 'filterCount' must be >= 0.");
            *value = window->GetDialog()->ShowSaveFile(title, defaultPath, filters, filterCount, defaultFileName);
        });
    }

    EXPORTED InteropStatus InfiniFrame_ShowMessage(InfiniFrameWindow* inst, const AutoString title, const AutoString text, const DialogButtons buttons, const DialogIcon icon, DialogResult* value) {
        ResetOut(value, DialogResult::Cancel);
        return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
            *value = window->GetDialog()->ShowMessage(title, text, buttons, icon);
        });
    }

    EXPORTED InteropStatus InfiniFrame_AddCustomSchemeName(InfiniFrameWindow* instance, const AutoString scheme) {
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(scheme, "scheme")) throw std::invalid_argument("Argument 'scheme' is null.");
            window->AddCustomSchemeName(scheme);
        });
    }

    EXPORTED InteropStatus InfiniFrame_GetAllMonitors(InfiniFrameWindow* instance, const GetAllMonitorsCallback callback) {
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (callback == nullptr) throw std::invalid_argument("Argument 'callback' is null.");
            window->GetAllMonitors(callback);
        });
    }

    EXPORTED InteropStatus InfiniFrame_SetClosingCallback(InfiniFrameWindow* instance, const ClosingCallback callback) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetClosingCallback(callback); }); }
    EXPORTED InteropStatus InfiniFrame_setClosedClosedCallback(InfiniFrameWindow* instance, const ClosedCallback callback) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetClosedCallback(callback); }); }
    EXPORTED InteropStatus InfiniFrame_SetFocusInCallback(InfiniFrameWindow* instance, const FocusInCallback callback) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetFocusInCallback(callback); }); }
    EXPORTED InteropStatus InfiniFrame_SetFocusOutCallback(InfiniFrameWindow* instance, const FocusOutCallback callback) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetFocusOutCallback(callback); }); }
    EXPORTED InteropStatus InfiniFrame_SetMovedCallback(InfiniFrameWindow* instance, const MovedCallback callback) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMovedCallback(callback); }); }
    EXPORTED InteropStatus InfiniFrame_SetResizedCallback(InfiniFrameWindow* instance, const ResizedCallback callback) { return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetResizedCallback(callback); }); }

    EXPORTED InteropStatus InfiniFrame_Invoke(InfiniFrameWindow* instance, const ACTION callback) {
        return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
            if (callback == nullptr) throw std::invalid_argument("Argument 'callback' is null.");
            window->Invoke(callback);
        });
    }

    EXPORTED InteropStatus InfiniFrame_SetFocused(InfiniFrameWindow* instance) { return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->SetFocused(); }); }

    EXPORTED InteropStatus InfiniFrame_GetLastErrorMessage(AutoString* value) {
        ResetOut(value, static_cast<AutoString>(nullptr));
        return RunExportStatus([&] {
            if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
            *value = GetLastErrorMessageCopy();
        });
    }
}
