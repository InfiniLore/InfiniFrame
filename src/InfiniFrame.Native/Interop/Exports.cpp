#include "Interop/ExportApi.h"

#define EXPORTED INFINIFRAME_NATIVE_EXPORT

using namespace InfiniFrame::Native::Interop;

/**
 * @file Exports.cpp
 * @brief C API for InfiniFrame native interop
 *
 * Memory management:
 * - InfiniFrame_ctor returns ownership to caller (.NET side)
 * - InfiniFrame_dtor transfers ownership back and destroys instance
 * - All string returns (AutoString) must be freed with InfiniFrame_FreeString
 *
 * Thread safety:
 * - All methods except Invoke must be called from UI thread
 * - Invoke marshals calls to UI thread safely
 */

extern "C" {
    /**
     * @brief Get transparent enabled status
     * @param instance InfiniFrame instance
     * @param enabled Output: transparent enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetTransparentEnabled(InfiniFrameWindow* instance, bool* enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetTransparentEnabled(enabled);
        }, enabled);
    }

    /**
     * @brief Get context menu enabled status
     * @param instance InfiniFrame instance
     * @param enabled Output: context menu enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetContextMenuEnabled(InfiniFrameWindow* instance, bool* enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetContextMenuEnabled(enabled);
        }, enabled);
    }

    /**
     * @brief Get zoom enabled status
     * @param instance InfiniFrame instance
     * @param enabled Output: zoom enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetZoomEnabled(InfiniFrameWindow* instance, bool* enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetZoomEnabled(enabled);
        }, enabled);
    }

    /**
     * @brief Get dev tools enabled status
     * @param instance InfiniFrame instance
     * @param enabled Output: dev tools enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetDevToolsEnabled(InfiniFrameWindow* instance, bool* enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetDevToolsEnabled(enabled);
        }, enabled);
    }

    /**
     * @brief Get full screen status
     * @param instance InfiniFrame instance
     * @param fullScreen Output: full screen status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetFullScreen(InfiniFrameWindow* instance, bool* fullScreen) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetFullScreen(fullScreen);
        }, fullScreen);
    }

    /**
     * @brief Get grant browser permissions status
     * @param instance InfiniFrame instance
     * @param grant Output: grant browser permissions status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetGrantBrowserPermissions(InfiniFrameWindow* instance, bool* grant) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetGrantBrowserPermissions(grant);
        }, grant);
    }

    /**
     * @brief Get user agent string
     * @param instance InfiniFrame instance
     * @return User agent string
     */
    EXPORTED AutoString InfiniFrame_GetUserAgent(InfiniFrameWindow* instance) {
        return RunWindowReturnExport(instance, static_cast<AutoString>(nullptr), [](InfiniFrameWindow& window) {
            return window.GetUserAgent();
        });
    }

    /**
     * @brief Get media autoplay enabled status
     * @param instance InfiniFrame instance
     * @param enabled Output: media autoplay enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetMediaAutoplayEnabled(InfiniFrameWindow* instance, bool* enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetMediaAutoplayEnabled(enabled);
        }, enabled);
    }

    /**
     * @brief Get file system access enabled status
     * @param instance InfiniFrame instance
     * @param enabled Output: file system access enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetFileSystemAccessEnabled(InfiniFrameWindow* instance, bool* enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetFileSystemAccessEnabled(enabled);
        }, enabled);
    }

    /**
     * @brief Get web security enabled status
     * @param instance InfiniFrame instance
     * @param enabled Output: web security enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetWebSecurityEnabled(InfiniFrameWindow* instance, bool* enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetWebSecurityEnabled(enabled);
        }, enabled);
    }

    /**
     * @brief Get JavaScript clipboard access enabled status
     * @param instance InfiniFrame instance
     * @param enabled Output: JavaScript clipboard access enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetJavascriptClipboardAccessEnabled(InfiniFrameWindow* instance, bool* enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetJavascriptClipboardAccessEnabled(enabled);
        }, enabled);
    }

    /**
     * @brief Get media stream enabled status
     * @param instance InfiniFrame instance
     * @param enabled Output: media stream enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetMediaStreamEnabled(InfiniFrameWindow* instance, bool* enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetMediaStreamEnabled(enabled);
        }, enabled);
    }

    /**
     * @brief Get smooth scrolling enabled status
     * @param instance InfiniFrame instance
     * @param enabled Output: smooth scrolling enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetSmoothScrollingEnabled(InfiniFrameWindow* instance, bool* enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetSmoothScrollingEnabled(enabled);
        }, enabled);
    }

    /**
     * @brief Get maximized status
     * @param instance InfiniFrame instance
     * @param isMaximized Output: maximized status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetMaximized(InfiniFrameWindow* instance, bool* isMaximized) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetMaximized(isMaximized);
        }, isMaximized);
    }

    /**
     * @brief Get minimized status
     * @param instance InfiniFrame instance
     * @param isMinimized Output: minimized status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetMinimized(InfiniFrameWindow* instance, bool* isMinimized) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetMinimized(isMinimized);
        }, isMinimized);
    }

    /**
     * @brief Get ignore certificate errors enabled status
     * @param instance InfiniFrame instance
     * @param disabled Output: ignore certificate errors enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetIgnoreCertificateErrorsEnabled(InfiniFrameWindow* instance, bool* disabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetIgnoreCertificateErrorsEnabled(disabled);
        }, disabled);
    }

    /**
     * @brief Get window position
     * @param instance InfiniFrame instance
     * @param x Output: X coordinate
     * @param y Output: Y coordinate
     */
    EXPORTED NativeStatusCode InfiniFrame_GetPosition(InfiniFrameWindow* instance, int* x, int* y) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetPosition(x, y);
        }, x, y);
    }

    /**
     * @brief Get resizable status
     * @param instance InfiniFrame instance
     * @param resizable Output: resizable status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetResizable(InfiniFrameWindow* instance, bool* resizable) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetResizable(resizable);
        }, resizable);
    }

    /**
     * @brief Get screen DPI
     * @param instance InfiniFrame instance
     * @return Screen DPI value
     */
    EXPORTED unsigned int InfiniFrame_GetScreenDpi(InfiniFrameWindow* instance) {
        return RunWindowReturnExport(instance, 0u, [](InfiniFrameWindow& window) {
            return window.GetScreenDpi();
        });
    }

    /**
     * @brief Get window size
     * @param instance InfiniFrame instance
     * @param width Output: window width
     * @param height Output: window height
     */
    EXPORTED NativeStatusCode InfiniFrame_GetSize(InfiniFrameWindow* instance, int* width, int* height) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetSize(width, height);
        }, width, height);
    }

    /**
     * @brief Get the window maximum size constraints
     * @param instance InfiniFrame instance
     * @param width Output: maximum window width
     * @param height Output: maximum window height
     */
    EXPORTED NativeStatusCode InfiniFrame_GetMaxSize(InfiniFrameWindow* instance, int* width, int* height) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetMaxSize(width, height);
        }, width, height);
    }

    /**
     * @brief Get the window minimum size constraints
     * @param instance InfiniFrame instance
     * @param width Output: minimum window width
     * @param height Output: minimum window height
     */
    EXPORTED NativeStatusCode InfiniFrame_GetMinSize(InfiniFrameWindow* instance, int* width, int* height) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetMinSize(width, height);
        }, width, height);
    }

    /**
     * @brief Get window title
     * @param instance InfiniFrame instance
     * @return Window title string
     */
    EXPORTED AutoString InfiniFrame_GetTitle(InfiniFrameWindow* instance) {
        return RunWindowReturnExport(instance, static_cast<AutoString>(nullptr), [](InfiniFrameWindow& window) {
            return window.GetTitle();
        });
    }

    /**
     * @brief Get topmost status
     * @param instance InfiniFrame instance
     * @param topmost Output: topmost status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetTopmost(InfiniFrameWindow* instance, bool* topmost) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetTopmost(topmost);
        }, topmost);
    }

    /**
     * @brief Get zoom level
     * @param instance InfiniFrame instance
     * @param zoom Output: zoom level percentage
     */
    EXPORTED NativeStatusCode InfiniFrame_GetZoom(InfiniFrameWindow* instance, int* zoom) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetZoom(zoom);
        }, zoom);
    }

    /**
     * @brief Get focused status
     * @param instance InfiniFrame instance
     * @param isFocused Output: focused status
     */
    EXPORTED NativeStatusCode InfiniFrame_GetFocused(InfiniFrameWindow* instance, bool* isFocused) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.GetFocused(isFocused);
        }, isFocused);
    }

    /**
     * @brief Get icon file name
     * @param instance InfiniFrame instance
     * @return Icon file name string
     */
    EXPORTED AutoString InfiniFrame_GetIconFileName(InfiniFrameWindow* instance) {
        return RunWindowReturnExport(instance, static_cast<AutoString>(nullptr), [](InfiniFrameWindow& window) {
            return window.GetIconFileName();
        });
    }

    /**
     * @brief Set transparent enabled status
     * @param instance InfiniFrame instance
     * @param enabled Transparent enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_SetTransparentEnabled(InfiniFrameWindow* instance, const bool enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetTransparentEnabled(enabled);
        });
    }

    /**
     * @brief Set context menu enabled status
     * @param instance InfiniFrame instance
     * @param enabled Context menu enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_SetContextMenuEnabled(InfiniFrameWindow* instance, const bool enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetContextMenuEnabled(enabled);
        });
    }

    /**
     * @brief Set zoom enabled status
     * @param instance InfiniFrame instance
     * @param enabled Zoom enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_SetZoomEnabled(InfiniFrameWindow* instance, const bool enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetZoomEnabled(enabled);
        });
    }

    /**
     * @brief Set dev tools enabled status
     * @param instance InfiniFrame instance
     * @param enabled Dev tools enabled status
     */
    EXPORTED NativeStatusCode InfiniFrame_SetDevToolsEnabled(InfiniFrameWindow* instance, const bool enabled) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetDevToolsEnabled(enabled);
        });
    }

    /**
     * @brief Set full screen status
     * @param instance InfiniFrame instance
     * @param fullScreen Full screen status
     */
    EXPORTED NativeStatusCode InfiniFrame_SetFullScreen(InfiniFrameWindow* instance, const bool fullScreen) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetFullScreen(fullScreen);
        });
    }

    /**
     * @brief Set window icon from file
     * @param instance InfiniFrame instance
     * @param filename Icon file path
     */
    EXPORTED NativeStatusCode InfiniFrame_SetIconFile(InfiniFrameWindow* instance, const AutoString filename) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetIconFile(filename);
        });
    }

    /**
     * @brief Set maximized status
     * @param instance InfiniFrame instance
     * @param maximized Maximized status
     */
    EXPORTED NativeStatusCode InfiniFrame_SetMaximized(InfiniFrameWindow* instance, const bool maximized) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetMaximized(maximized);
        });
    }

    /**
     * @brief Set maximum window size
     * @param instance InfiniFrame instance
     * @param width Maximum width
     * @param height Maximum height
     */
    EXPORTED NativeStatusCode InfiniFrame_SetMaxSize(InfiniFrameWindow* instance, const int width, const int height) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetMaxSize(width, height);
        });
    }

    /**
     * @brief Set minimized status
     * @param instance InfiniFrame instance
     * @param minimized Minimized status
     */
    EXPORTED NativeStatusCode InfiniFrame_SetMinimized(InfiniFrameWindow* instance, const bool minimized) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetMinimized(minimized);
        });
    }

    /**
     * @brief Set minimum window size
     * @param instance InfiniFrame instance
     * @param width Minimum width
     * @param height Minimum height
     */
    EXPORTED NativeStatusCode InfiniFrame_SetMinSize(InfiniFrameWindow* instance, const int width, const int height) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetMinSize(width, height);
        });
    }

    /**
     * @brief Set window position
     * @param instance InfiniFrame instance
     * @param x X coordinate
     * @param y Y coordinate
     */
    EXPORTED NativeStatusCode InfiniFrame_SetPosition(InfiniFrameWindow* instance, const int x, const int y) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetPosition(x, y);
        });
    }

    /**
     * @brief Set resizable status
     * @param instance InfiniFrame instance
     * @param resizable Resizable status
     */
    EXPORTED NativeStatusCode InfiniFrame_SetResizable(InfiniFrameWindow* instance, const bool resizable) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetResizable(resizable);
        });
    }

    /**
     * @brief Set window size
     * @param instance InfiniFrame instance
     * @param width Window width
     * @param height Window height
     */
    EXPORTED NativeStatusCode InfiniFrame_SetSize(InfiniFrameWindow* instance, const int width, const int height) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetSize(width, height);
        });
    }

    /**
     * @brief Set window title
     * @param instance InfiniFrame instance
     * @param title Window title string
     */
    EXPORTED NativeStatusCode InfiniFrame_SetTitle(InfiniFrameWindow* instance, const AutoString title) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetTitle(title);
        });
    }

    /**
     * @brief Set topmost status
     * @param instance InfiniFrame instance
     * @param topmost Topmost status
     */
    EXPORTED NativeStatusCode InfiniFrame_SetTopmost(InfiniFrameWindow* instance, const bool topmost) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetTopmost(topmost);
        });
    }

    /**
     * @brief Set zoom level
     * @param instance InfiniFrame instance
     * @param zoom Zoom level percentage
     */
    EXPORTED NativeStatusCode InfiniFrame_SetZoom(InfiniFrameWindow* instance, const int zoom) {
        return RunWindowExportStatus(instance, [=](InfiniFrameWindow& window) {
            window.SetZoom(zoom);
        });
    }

}
