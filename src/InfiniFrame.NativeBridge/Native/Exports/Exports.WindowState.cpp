#include "Core/Exports.h"

extern "C" {
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
}
