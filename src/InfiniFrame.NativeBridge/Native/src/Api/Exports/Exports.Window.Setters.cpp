// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Enables or disables window transparency.
/// @param instance The window handle.
/// @param enabled True to enable transparency, false to disable.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetTransparentEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetTransparentEnabled(enabled);
        });
}

/// @brief Sets the window background color from RGBA components.
/// @param instance The window handle.
/// @param r Red component (0-255).
/// @param g Green component (0-255).
/// @param b Blue component (0-255).
/// @param a Alpha component (0-255).
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetBackgroundColor(
    InfiniFrameWindow* instance,
    const uint8_t r,
    const uint8_t g,
    const uint8_t b,
    const uint8_t a) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetBackgroundColor(r, g, b, a);
        });
}

/// @brief Enables or disables the browser context menu.
/// @param instance The window handle.
/// @param enabled True to enable the context menu, false to disable.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetContextMenuEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetContextMenuEnabled(enabled);
        });
}

/// @brief Enables or disables media autoplay.
/// @param instance The window handle.
/// @param enabled True to enable media autoplay, false to disable.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetMediaAutoplayEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetMediaAutoplayEnabled(enabled);
        });
}

/// @brief Sets the browser user agent string.
/// @param instance The window handle.
/// @param userAgent The user agent string to set.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetUserAgent(InfiniFrameWindow* instance, const char* userAgent) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetUserAgent(userAgent);
        });
}

/// @brief Enables or disables user zoom controls.
/// @param instance The window handle.
/// @param enabled True to enable zoom, false to disable.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetZoomEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetZoomEnabled(enabled);
        });
}

/// @brief Enables or disables the URL hover status bar.
/// @param instance The window handle.
/// @param enabled True to enable the status bar, false to disable.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetStatusBarEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetStatusBarEnabled(enabled);
        });
}

/// @brief Enables or disables browser keyboard shortcuts.
/// @param instance The window handle.
/// @param enabled True to enable browser shortcuts, false to disable.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetBrowserShortcutsEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetBrowserShortcutsEnabled(enabled);
        });
}

/// @brief Enables or disables developer tools.
/// @param instance The window handle.
/// @param enabled True to enable developer tools, false to disable.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetDevToolsEnabled(InfiniFrameWindow* instance, const bool enabled) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetDevToolsEnabled(enabled);
        });
}

/// @brief Enters or exits full-screen mode.
/// @param instance The window handle.
/// @param fullScreen True to enter full-screen, false to exit.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetFullScreen(InfiniFrameWindow* instance, const bool fullScreen) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetFullScreen(fullScreen);
        });
}

/// @brief Sets the window icon from a file path.
/// @param instance The window handle.
/// @param filename Path to the icon file.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetIconFile(InfiniFrameWindow* instance, const char* filename) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetIconFile(NullToEmpty(filename));
        });
}

/// @brief Maximizes or restores the window.
/// @param instance The window handle.
/// @param maximized True to maximize, false to restore.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetMaximized(InfiniFrameWindow* instance, const bool maximized) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetMaximized(maximized);
        });
}

/// @brief Sets the maximum window size.
/// @param instance The window handle.
/// @param width Maximum width in pixels.
/// @param height Maximum height in pixels.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetMaxSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetMaxSize(width, height);
        });
}

/// @brief Minimizes the window.
/// @param instance The window handle.
/// @param minimized True to minimize the window.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetMinimized(InfiniFrameWindow* instance, const bool minimized) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetMinimized(minimized);
        });
}

/// @brief Sets the minimum window size.
/// @param instance The window handle.
/// @param width Minimum width in pixels.
/// @param height Minimum height in pixels.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetMinSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetMinSize(width, height);
        });
}

/// @brief Sets the window position.
/// @param instance The window handle.
/// @param x Left coordinate in screen pixels.
/// @param y Top coordinate in screen pixels.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetPosition(InfiniFrameWindow* instance, const int x, const int y) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetPosition(x, y);
        });
}

/// @brief Enables or disables window resizability.
/// @param instance The window handle.
/// @param resizable True to enable resizing, false to disable.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetResizable(InfiniFrameWindow* instance, const bool resizable) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetResizable(resizable);
        });
}

/// @brief Sets the window size.
/// @param instance The window handle.
/// @param width Width in pixels.
/// @param height Height in pixels.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetSize(InfiniFrameWindow* instance, const int width, const int height) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetSize(width, height);
        });
}

/// @brief Sets the window title.
/// @param instance The window handle.
/// @param title The title string to set.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetTitle(InfiniFrameWindow* instance, const char* title) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetTitle(NullToEmpty(title));
        });
}

/// @brief Sets or unsets the always-on-top flag.
/// @param instance The window handle.
/// @param topmost True to set always-on-top, false to unset.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetTopmost(InfiniFrameWindow* instance, const bool topmost) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetTopmost(topmost);
        });
}

/// @brief Sets the zoom level.
/// @param instance The window handle.
/// @param zoom Zoom level as a percentage (e.g. 100 for 100%).
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetZoom(InfiniFrameWindow* instance, const int zoom) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetZoom(zoom);
        });
}
}