// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrameNative_GetTransparentEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetTransparentEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetBackgroundColor(InfiniFrameWindow* instance, uint8_t* r, uint8_t* g, uint8_t* b, uint8_t* a) {
    ResetOut(r, static_cast<uint8_t>(0));
    ResetOut(g, static_cast<uint8_t>(0));
    ResetOut(b, static_cast<uint8_t>(0));
    ResetOut(a, static_cast<uint8_t>(0));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(r, "r") || !EnsureOutNotNull(g, "g") || !EnsureOutNotNull(b, "b") || !EnsureOutNotNull(a, "a"))
            return;
        window->GetBackgroundColor(r, g, b, a);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetContextMenuEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetContextMenuEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetZoomEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetZoomEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetDevToolsEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetDevToolsEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetFullScreen(InfiniFrameWindow* instance, bool* fullScreen) {
    ResetOut(fullScreen, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(fullScreen, "fullScreen"))
            return;
        window->GetFullScreen(fullScreen);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetGrantBrowserPermissions(InfiniFrameWindow* instance, bool* grant) {
    ResetOut(grant, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(grant, "grant"))
            return;
        window->GetGrantBrowserPermissions(grant);
    });
}

/// @param[out] value Owned string, caller must free with InfiniFrameNative_FreeString.
EXPORTED InteropStatus InfiniFrameNative_GetUserAgent(InfiniFrameWindow* instance, AutoString* value) {
    ResetOut(value, static_cast<AutoString>(nullptr));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->GetUserAgent();
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetMediaAutoplayEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetMediaAutoplayEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetFileSystemAccessEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetFileSystemAccessEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetWebSecurityEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetWebSecurityEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetJavascriptClipboardAccessEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetJavascriptClipboardAccessEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetMediaStreamEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetMediaStreamEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetSmoothScrollingEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetSmoothScrollingEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetStatusBarEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetStatusBarEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetMaximized(InfiniFrameWindow* instance, bool* isMaximized) {
    ResetOut(isMaximized, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(isMaximized, "isMaximized"))
            return;
        window->GetMaximized(isMaximized);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetMinimized(InfiniFrameWindow* instance, bool* isMinimized) {
    ResetOut(isMinimized, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(isMinimized, "isMinimized"))
            return;
        window->GetMinimized(isMinimized);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetIgnoreCertificateErrorsEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(enabled, "enabled"))
            return;
        window->GetIgnoreCertificateErrorsEnabled(enabled);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetPosition(InfiniFrameWindow* instance, int* x, int* y) {
    ResetOut2(x, y, 0);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(x, "x") || !EnsureOutNotNull(y, "y"))
            return;
        window->GetPosition(x, y);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetResizable(InfiniFrameWindow* instance, bool* resizable) {
    ResetOut(resizable, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(resizable, "resizable"))
            return;
        window->GetResizable(resizable);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetScreenDpi(InfiniFrameWindow* instance, unsigned int* value) {
    ResetOut(value, static_cast<unsigned int>(0));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->GetScreenDpi();
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetSize(InfiniFrameWindow* instance, int* width, int* height) {
    ResetOut2(width, height, 0);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(width, "width") || !EnsureOutNotNull(height, "height"))
            return;
        window->GetSize(width, height);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetMaxSize(InfiniFrameWindow* instance, int* width, int* height) {
    ResetOut2(width, height, 0);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(width, "width") || !EnsureOutNotNull(height, "height"))
            return;
        window->GetMaxSize(width, height);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetMinSize(InfiniFrameWindow* instance, int* width, int* height) {
    ResetOut2(width, height, 0);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(width, "width") || !EnsureOutNotNull(height, "height"))
            return;
        window->GetMinSize(width, height);
    });
}

/// @param[out] value Owned string, caller must free with InfiniFrameNative_FreeString.
EXPORTED InteropStatus InfiniFrameNative_GetTitle(InfiniFrameWindow* instance, AutoString* value) {
    ResetOut(value, static_cast<AutoString>(nullptr));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->GetTitle();
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetTopmost(InfiniFrameWindow* instance, bool* topmost) {
    ResetOut(topmost, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(topmost, "topmost"))
            return;
        window->GetTopmost(topmost);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetZoom(InfiniFrameWindow* instance, int* zoom) {
    ResetOut(zoom, 0);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(zoom, "zoom"))
            return;
        window->GetZoom(zoom);
    });
}

EXPORTED InteropStatus InfiniFrameNative_GetFocused(InfiniFrameWindow* instance, bool* isFocused) {
    ResetOut(isFocused, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(isFocused, "isFocused"))
            return;
        window->GetFocused(isFocused);
    });
}

/// @param[out] value Owned string, caller must free with InfiniFrameNative_FreeString.
EXPORTED InteropStatus InfiniFrameNative_GetIconFileName(InfiniFrameWindow* instance, AutoString* value) {
    ResetOut(value, static_cast<AutoString>(nullptr));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->GetIconFileName();
    });
}
}
