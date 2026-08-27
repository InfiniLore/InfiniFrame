// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Queries whether window transparency is enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if transparency is enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetTransparentEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetTransparentEnabled(enabled);
        });
}

/// @brief Gets the window background color as RGBA components.
/// @param instance The window handle.
/// @param r Output red component (0-255).
/// @param g Output green component (0-255).
/// @param b Output blue component (0-255).
/// @param a Output alpha component (0-255).
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetBackgroundColor(
    InfiniFrameWindow* instance,
    uint8_t* r,
    uint8_t* g,
    uint8_t* b,
    uint8_t* a) {
    ResetOut(r, static_cast<uint8_t>(0));
    ResetOut(g, static_cast<uint8_t>(0));
    ResetOut(b, static_cast<uint8_t>(0));
    ResetOut(a, static_cast<uint8_t>(0));
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(r, "r") || !EnsureOutNotNull(g, "g") || !EnsureOutNotNull(b, "b") || !
                EnsureOutNotNull(a, "a"))
                return;
            window->GetBackgroundColor(r, g, b, a);
        });
}

/// @brief Queries whether the browser context menu is enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if the context menu is enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetContextMenuEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetContextMenuEnabled(enabled);
        });
}

/// @brief Queries whether user zoom is enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if user zoom is enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetZoomEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetZoomEnabled(enabled);
        });
}

/// @brief Queries whether developer tools are enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if developer tools are enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetDevToolsEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetDevToolsEnabled(enabled);
        });
}

/// @brief Queries whether the window is in full-screen mode.
/// @param instance The window handle.
/// @param fullScreen Output flag set to true if the window is full-screen.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetFullScreen(InfiniFrameWindow* instance, bool* fullScreen) {
    ResetOut(fullScreen, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(fullScreen, "fullScreen"))
                return;
            window->GetFullScreen(fullScreen);
        });
}

/// @brief Queries whether browser permissions are auto-granted.
/// @param instance The window handle.
/// @param grant Output flag set to true if browser permissions are auto-granted.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetGrantBrowserPermissions(InfiniFrameWindow* instance, bool* grant) {
    ResetOut(grant, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(grant, "grant"))
                return;
            window->GetGrantBrowserPermissions(grant);
        });
}

/// @brief Gets the browser user agent string.
/// @param instance The window handle.
/// @param value Output pointer to the user agent string. Caller must free with InfiniFrameNative_FreeString.
/// @return InteropStatus
/// @param[out] value Owned string, caller must free with InfiniFrameNative_FreeString.
EXPORTED InteropStatus InfiniFrameNative_GetUserAgent(InfiniFrameWindow* instance, const char** value) {
    ResetOut(value, static_cast<const char*>(nullptr));
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(value, "value"))
                return;
            *value = window->GetUserAgent();
        });
}

/// @brief Queries whether media autoplay is enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if media autoplay is enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetMediaAutoplayEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetMediaAutoplayEnabled(enabled);
        });
}

/// @brief Queries whether file system access is enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if file system access is enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetFileSystemAccessEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetFileSystemAccessEnabled(enabled);
        });
}

/// @brief Queries whether web security is enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if web security is enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetWebSecurityEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetWebSecurityEnabled(enabled);
        });
}

/// @brief Queries whether JavaScript clipboard access is enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if JavaScript clipboard access is enabled.
/// @return InteropStatus
EXPORTED InteropStatus
InfiniFrameNative_GetJavascriptClipboardAccessEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetJavascriptClipboardAccessEnabled(enabled);
        });
}

/// @brief Queries whether camera/microphone access is enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if media stream access is enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetMediaStreamEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetMediaStreamEnabled(enabled);
        });
}

/// @brief Queries whether smooth scrolling is enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if smooth scrolling is enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetSmoothScrollingEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetSmoothScrollingEnabled(enabled);
        });
}

/// @brief Queries whether the URL hover status bar is enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if the status bar is enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetStatusBarEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetStatusBarEnabled(enabled);
        });
}

/// @brief Queries whether browser keyboard shortcuts are enabled.
/// @param instance The window handle.
/// @param enabled Output flag set to true if browser shortcuts are enabled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetBrowserShortcutsEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetBrowserShortcutsEnabled(enabled);
        });
}

/// @brief Queries whether the window is maximized.
/// @param instance The window handle.
/// @param isMaximized Output flag set to true if the window is maximized.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetMaximized(InfiniFrameWindow* instance, bool* isMaximized) {
    ResetOut(isMaximized, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(isMaximized, "isMaximized"))
                return;
            window->GetMaximized(isMaximized);
        });
}

/// @brief Queries whether the window is minimized.
/// @param instance The window handle.
/// @param isMinimized Output flag set to true if the window is minimized.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetMinimized(InfiniFrameWindow* instance, bool* isMinimized) {
    ResetOut(isMinimized, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(isMinimized, "isMinimized"))
                return;
            window->GetMinimized(isMinimized);
        });
}

/// @brief Queries whether SSL certificate errors are ignored.
/// @param instance The window handle.
/// @param enabled Output flag set to true if certificate errors are ignored.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetIgnoreCertificateErrorsEnabled(InfiniFrameWindow* instance, bool* enabled) {
    ResetOut(enabled, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(enabled, "enabled"))
                return;
            window->GetIgnoreCertificateErrorsEnabled(enabled);
        });
}

/// @brief Gets the window position (left, top).
/// @param instance The window handle.
/// @param x Output left coordinate in screen pixels.
/// @param y Output top coordinate in screen pixels.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetPosition(InfiniFrameWindow* instance, int* x, int* y) {
    ResetOut2(x, y, 0);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(x, "x") || !EnsureOutNotNull(y, "y"))
                return;
            window->GetPosition(x, y);
        });
}

/// @brief Queries whether the window is resizable.
/// @param instance The window handle.
/// @param resizable Output flag set to true if the window is resizable.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetResizable(InfiniFrameWindow* instance, bool* resizable) {
    ResetOut(resizable, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(resizable, "resizable"))
                return;
            window->GetResizable(resizable);
        });
}

/// @brief Gets the current screen DPI.
/// @param instance The window handle.
/// @param value Output DPI value.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetScreenDpi(InfiniFrameWindow* instance, unsigned int* value) {
    ResetOut(value, static_cast<unsigned int>(0));
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(value, "value"))
                return;
            *value = window->GetScreenDpi();
        });
}

/// @brief Gets the window size (width, height).
/// @param instance The window handle.
/// @param width Output width in pixels.
/// @param height Output height in pixels.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetSize(InfiniFrameWindow* instance, int* width, int* height) {
    ResetOut2(width, height, 0);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(width, "width") || !EnsureOutNotNull(height, "height"))
                return;
            window->GetSize(width, height);
        });
}

/// @brief Gets the maximum window size.
/// @param instance The window handle.
/// @param width Output maximum width in pixels.
/// @param height Output maximum height in pixels.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetMaxSize(InfiniFrameWindow* instance, int* width, int* height) {
    ResetOut2(width, height, 0);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(width, "width") || !EnsureOutNotNull(height, "height"))
                return;
            window->GetMaxSize(width, height);
        });
}

/// @brief Gets the minimum window size.
/// @param instance The window handle.
/// @param width Output minimum width in pixels.
/// @param height Output minimum height in pixels.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetMinSize(InfiniFrameWindow* instance, int* width, int* height) {
    ResetOut2(width, height, 0);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(width, "width") || !EnsureOutNotNull(height, "height"))
                return;
            window->GetMinSize(width, height);
        });
}

/// @brief Gets the window title.
/// @param instance The window handle.
/// @param value Output pointer to the title string. Caller must free with InfiniFrameNative_FreeString.
/// @return InteropStatus
/// @param[out] value Owned string, caller must free with InfiniFrameNative_FreeString.
EXPORTED InteropStatus InfiniFrameNative_GetTitle(InfiniFrameWindow* instance, const char** value) {
    ResetOut(value, static_cast<const char*>(nullptr));
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(value, "value"))
                return;
            *value = window->GetTitle();
        });
}

/// @brief Queries whether the window is always-on-top.
/// @param instance The window handle.
/// @param topmost Output flag set to true if the window is always-on-top.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetTopmost(InfiniFrameWindow* instance, bool* topmost) {
    ResetOut(topmost, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(topmost, "topmost"))
                return;
            window->GetTopmost(topmost);
        });
}

/// @brief Gets the zoom level as a percentage.
/// @param instance The window handle.
/// @param zoom Output zoom level (e.g. 100 for 100%).
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetZoom(InfiniFrameWindow* instance, int* zoom) {
    ResetOut(zoom, 0);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(zoom, "zoom"))
                return;
            window->GetZoom(zoom);
        });
}

/// @brief Queries whether the window has keyboard focus.
/// @param instance The window handle.
/// @param isFocused Output flag set to true if the window is focused.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetFocused(InfiniFrameWindow* instance, bool* isFocused) {
    ResetOut(isFocused, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(isFocused, "isFocused"))
                return;
            window->GetFocused(isFocused);
        });
}

/// @brief Gets the icon file name of the window.
/// @param instance The window handle.
/// @param value Output pointer to the icon file name string. Caller must free with InfiniFrameNative_FreeString.
/// @return InteropStatus
/// @param[out] value Owned string, caller must free with InfiniFrameNative_FreeString.
EXPORTED InteropStatus InfiniFrameNative_GetIconFileName(InfiniFrameWindow* instance, const char** value) {
    ResetOut(value, static_cast<const char*>(nullptr));
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(value, "value"))
                return;
            *value = window->GetIconFileName();
        });
}
}