// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrame_Center(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Center(); });
}

EXPORTED InteropStatus InfiniFrame_ClearBrowserAutoFill(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->ClearBrowserAutoFill(); });
}

EXPORTED InteropStatus InfiniFrame_NavigateToString(InfiniFrameWindow* instance, const AutoString content) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(content, "content"))
            return;
        window->NavigateToString(content);
    });
}

EXPORTED InteropStatus InfiniFrame_NavigateToUrl(InfiniFrameWindow* instance, const AutoString url) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(url, "url"))
            return;
        window->NavigateToUrl(url);
    });
}

EXPORTED InteropStatus InfiniFrame_Restore(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Restore(); });
}

EXPORTED InteropStatus InfiniFrame_SendWebMessage(InfiniFrameWindow* instance, const AutoString message) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->SendWebMessage(NullToEmpty(message));
    });
}

EXPORTED InteropStatus InfiniFrame_SetTransparentEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTransparentEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrame_SetContextMenuEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetContextMenuEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrame_SetZoomEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetZoomEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrame_SetDevToolsEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetDevToolsEnabled(enabled); });
}

EXPORTED InteropStatus InfiniFrame_SetFullScreen(InfiniFrameWindow* instance, const bool fullScreen) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetFullScreen(fullScreen); });
}

EXPORTED InteropStatus InfiniFrame_SetIconFile(InfiniFrameWindow* instance, const AutoString filename) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->SetIconFile(NullToEmpty(filename));
    });
}

EXPORTED InteropStatus InfiniFrame_SetMaximized(InfiniFrameWindow* instance, const bool maximized) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMaximized(maximized); });
}

EXPORTED InteropStatus InfiniFrame_SetMaxSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMaxSize(width, height); });
}

EXPORTED InteropStatus InfiniFrame_SetMinimized(InfiniFrameWindow* instance, const bool minimized) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMinimized(minimized); });
}

EXPORTED InteropStatus InfiniFrame_SetMinSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMinSize(width, height); });
}

EXPORTED InteropStatus InfiniFrame_SetPosition(InfiniFrameWindow* instance, const int x, const int y) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetPosition(x, y); });
}

EXPORTED InteropStatus InfiniFrame_SetResizable(InfiniFrameWindow* instance, const bool resizable) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetResizable(resizable); });
}

EXPORTED InteropStatus InfiniFrame_SetSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetSize(width, height); });
}

EXPORTED InteropStatus InfiniFrame_SetTitle(InfiniFrameWindow* instance, const AutoString title) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTitle(NullToEmpty(title)); });
}

EXPORTED InteropStatus InfiniFrame_SetTopmost(InfiniFrameWindow* instance, const bool topmost) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetTopmost(topmost); });
}

EXPORTED InteropStatus InfiniFrame_SetZoom(InfiniFrameWindow* instance, const int zoom) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetZoom(zoom); });
}

EXPORTED InteropStatus InfiniFrame_ShowNotification(InfiniFrameWindow* instance, const AutoString title, const AutoString body) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->ShowNotification(NullToEmpty(title), NullToEmpty(body));
    });
}

EXPORTED InteropStatus InfiniFrame_SetFocused(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->SetFocused(); });
}

EXPORTED InteropStatus InfiniFrame_GetTransparentEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetTransparentEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrame_GetContextMenuEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetContextMenuEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrame_GetZoomEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetZoomEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrame_GetDevToolsEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetDevToolsEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrame_GetFullScreen(InfiniFrameWindow* instance, bool* fullScreen) {
    ResetOut(fullScreen, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(fullScreen, "fullScreen"))
            return;
        window->GetFullScreen(fullScreen);
    });
}

EXPORTED InteropStatus InfiniFrame_GetGrantBrowserPermissions(InfiniFrameWindow* instance, bool* grant) {
    ResetOut(grant, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(grant, "grant"))
            return;
        window->GetGrantBrowserPermissions(grant);
    });
}

/// @param[out] value Owned string, caller must free with InfiniFrame_FreeString.
EXPORTED InteropStatus InfiniFrame_GetUserAgent(InfiniFrameWindow* instance, AutoString* value) {
    ResetOut(value, static_cast<AutoString>(nullptr));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->GetUserAgent();
    });
}

EXPORTED InteropStatus InfiniFrame_GetMediaAutoplayEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetMediaAutoplayEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrame_GetFileSystemAccessEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetFileSystemAccessEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrame_GetWebSecurityEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetWebSecurityEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrame_GetJavascriptClipboardAccessEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetJavascriptClipboardAccessEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrame_GetMediaStreamEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetMediaStreamEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrame_GetSmoothScrollingEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetSmoothScrollingEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrame_GetMaximized(InfiniFrameWindow* instance, bool* isMaximized) {
    ResetOut(isMaximized, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(isMaximized, "isMaximized"))
            return;
        window->GetMaximized(isMaximized);
    });
}

EXPORTED InteropStatus InfiniFrame_GetMinimized(InfiniFrameWindow* instance, bool* isMinimized) {
    ResetOut(isMinimized, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(isMinimized, "isMinimized"))
            return;
        window->GetMinimized(isMinimized);
    });
}

EXPORTED InteropStatus InfiniFrame_GetIgnoreCertificateErrorsEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetIgnoreCertificateErrorsEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrame_GetPosition(InfiniFrameWindow* instance, int* x, int* y) {
    ResetOut2(x, y, 0);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(x, "x") || !EnsureOutNotNull(y, "y"))
            return;
        window->GetPosition(x, y);
    });
}

EXPORTED InteropStatus InfiniFrame_GetResizable(InfiniFrameWindow* instance, bool* resizable) {
    ResetOut(resizable, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(resizable, "resizable"))
            return;
        window->GetResizable(resizable);
    });
}

EXPORTED InteropStatus InfiniFrame_GetScreenDpi(InfiniFrameWindow* instance, unsigned int* value) {
    ResetOut(value, static_cast<unsigned int>(0));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->GetScreenDpi();
    });
}

EXPORTED InteropStatus InfiniFrame_GetSize(InfiniFrameWindow* instance, int* width, int* height) {
    ResetOut2(width, height, 0);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(width, "width") || !EnsureOutNotNull(height, "height"))
            return;
        window->GetSize(width, height);
    });
}

EXPORTED InteropStatus InfiniFrame_GetMaxSize(InfiniFrameWindow* instance, int* width, int* height) {
    ResetOut2(width, height, 0);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(width, "width") || !EnsureOutNotNull(height, "height"))
            return;
        window->GetMaxSize(width, height);
    });
}

EXPORTED InteropStatus InfiniFrame_GetMinSize(InfiniFrameWindow* instance, int* width, int* height) {
    ResetOut2(width, height, 0);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(width, "width") || !EnsureOutNotNull(height, "height"))
            return;
        window->GetMinSize(width, height);
    });
}

/// @param[out] value Owned string, caller must free with InfiniFrame_FreeString.
EXPORTED InteropStatus InfiniFrame_GetTitle(InfiniFrameWindow* instance, AutoString* value) {
    ResetOut(value, static_cast<AutoString>(nullptr));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->GetTitle();
    });
}

EXPORTED InteropStatus InfiniFrame_GetTopmost(InfiniFrameWindow* instance, bool* topmost) {
    ResetOut(topmost, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(topmost, "topmost"))
            return;
        window->GetTopmost(topmost);
    });
}

EXPORTED InteropStatus InfiniFrame_GetZoom(InfiniFrameWindow* instance, int* zoom) {
    ResetOut(zoom, 0);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(zoom, "zoom"))
            return;
        window->GetZoom(zoom);
    });
}

EXPORTED InteropStatus InfiniFrame_GetFocused(InfiniFrameWindow* instance, bool* isFocused) {
    ResetOut(isFocused, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(isFocused, "isFocused"))
            return;
        window->GetFocused(isFocused);
    });
}

/// @param[out] value Owned string, caller must free with InfiniFrame_FreeString.
EXPORTED InteropStatus InfiniFrame_GetIconFileName(InfiniFrameWindow* instance, AutoString* value) {
    ResetOut(value, static_cast<AutoString>(nullptr));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->GetIconFileName();
    });
}
}
