#pragma once
/**
 * @file DarkMode.h
 * @brief Win32 dark-mode helpers using undocumented UxTheme APIs
 *
 * Provides runtime detection and application of Windows dark mode for the
 * non-client area (title bar, borders). All functions are noexcept and safe
 * to call even when the underlying APIs are unavailable
 */

#include <Windows.h>

/** @brief Detect available dark-mode APIs at runtime and cache the results. Must be called once at startup */
void InitDarkModeSupport() noexcept;

/**
 * @brief Check whether the current Windows theme is dark
 * @return true if the system is in dark mode
 */
[[nodiscard]] bool IsDarkModeEnabled() noexcept;

/**
 * @brief Apply or remove dark mode colouring on a window's non-client area
 * @param hwnd   Target window handle
 * @param enable true to enable dark title bar, false to restore light title bar
 */
void EnableDarkMode(HWND hwnd, bool enable) noexcept;

/**
 * @brief Force a repaint of the non-client area (title bar / borders) of a window.
 * Call after toggling dark mode to make the change immediately visible.
 * @param hwnd Target window handle
 */
void RefreshNonClientArea(HWND hwnd) noexcept;

/**
 * @brief Check whether a WM_SETTINGCHANGE lParam signals a colour-scheme change
 * @param l_param lParam from a WM_SETTINGCHANGE message
 * @return true if the message indicates an immersive colour-scheme change
 */
[[nodiscard]] bool IsColorSchemeChange(LPARAM l_param) noexcept;

// ============================================================================
// Internal UxTheme / DWM types (undocumented Win32 API surface)
// These are required to call the dark-mode APIs at runtime via GetProcAddress
// ============================================================================

/** @brief Controls whether the immersive colour cache is used or refreshed */
enum IMMERSIVE_HC_CACHE_MODE {
    IHCM_USE_CACHED_VALUE = 0, /// Use the previously cached value
    IHCM_REFRESH = 1,          /// Force a refresh of the cached value
};

/** @brief Application colour-mode preference passed to SetPreferredAppMode */
enum PreferredAppMode {
    Default = 0, /// Follow the system setting
    AllowDark = 1, /// Allow dark mode if the system is dark
    ForceDark = 2, /// Always use dark mode
    ForceLight = 3, /// Always use light mode
    Max = 4, /// Sentinel value; not a valid mode
};

/** @brief Window composition attribute identifiers used with SetWindowCompositionAttribute */
enum WINDOWCOMPOSITIONATTRIB {
    WCA_UNDEFINED = 0,
    WCA_NCRENDERING_ENABLED = 1,  /// Non-client rendering enabled flag
    WCA_NCRENDERING_POLICY = 2,  /// Non-client rendering policy
    WCA_TRANSITIONS_FORCEDISABLED = 3,
    WCA_ALLOW_NCPAINT = 4,
    WCA_CAPTION_BUTTON_BOUNDS = 5,
    WCA_NONCLIENT_RTL_LAYOUT = 6,
    WCA_FORCE_ICONIC_REPRESENTATION = 7,
    WCA_EXTENDED_FRAME_BOUNDS = 8,
    WCA_HAS_ICONIC_BITMAP = 9,
    WCA_THEME_ATTRIBUTES = 10,
    WCA_NCRENDERING_EXILED = 11,
    WCA_NCADORNMENTINFO = 12,
    WCA_EXCLUDED_FROM_LIVEPREVIEW = 13,
    WCA_VIDEO_OVERLAY_ACTIVE = 14,
    WCA_FORCE_ACTIVEWINDOW_APPEARANCE = 15,
    WCA_DISALLOW_PEEK = 16,
    WCA_CLOAK = 17,
    WCA_CLOAKED = 18,
    WCA_ACCENT_POLICY = 19,
    WCA_FREEZE_REPRESENTATION = 20,
    WCA_EVER_UNCLOAKED = 21,
    WCA_VISUAL_OWNER = 22,
    WCA_HOLOGRAPHIC = 23,
    WCA_EXCLUDED_FROM_DDA = 24,
    WCA_PASSIVEUPDATEMODE = 25,
    WCA_USEDARKMODECOLORS = 26, /// Enable dark mode colours for non-client area
    WCA_LAST = 27,
};

/** @brief Parameter struct for SetWindowCompositionAttribute */
struct WINDOWCOMPOSITIONATTRIBDATA {
    WINDOWCOMPOSITIONATTRIB Attrib; /// Attribute to get or set
    PVOID pvData;                   /// Pointer to attribute-specific data
    SIZE_T cbData;                  /// Size of the data pointed to by pvData
};
