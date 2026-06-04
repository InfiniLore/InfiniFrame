// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <mutex>

#include "Runtime/Platform/Windows/DarkMode.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using RtlGetNtVersionNumbers_f = void(WINAPI*)(LPDWORD, LPDWORD, LPDWORD);

using SetWindowCompositionAttribute_f = HRESULT(WINAPI*)(HWND, WINDOWCOMPOSITIONATTRIBDATA*);

using ShouldAppsUseDarkMode_f = BOOLEAN(WINAPI*)();

using AllowDarkModeForWindow_f = BOOLEAN(WINAPI*)(HWND, BOOLEAN);

using RefreshImmersiveColorPolicyState_f = void(WINAPI*)();

using IsDarkModeAllowedForWindow_f = BOOLEAN(WINAPI*)(HWND);

using GetIsImmersiveColorUsingHighContrast_f = BOOLEAN(WINAPI*)(IMMERSIVE_HC_CACHE_MODE);

using SetPreferredAppMode_f = PreferredAppMode(WINAPI*)(PreferredAppMode);

static SetWindowCompositionAttribute_f setWindowCompositionAttribute = nullptr;
static ShouldAppsUseDarkMode_f shouldAppsUseDarkMode = nullptr;
static AllowDarkModeForWindow_f allowDarkModeForWindow = nullptr;
static RefreshImmersiveColorPolicyState_f refreshImmersiveColorPolicyState = nullptr;
static IsDarkModeAllowedForWindow_f isDarkModeAllowedForWindow = nullptr;
static GetIsImmersiveColorUsingHighContrast_f getIsImmersiveColorUsingHighContrast = nullptr;
static SetPreferredAppMode_f setPreferredAppMode = nullptr;

static constexpr DWORD wiN10MinimumBuildDarkMode = 18362;

static std::once_flag flagInitDarkModeSupport;

namespace {
    class ModuleHandle {
        public:
        ~ModuleHandle() {
            if (_handle != nullptr) {
                FreeLibrary(_handle);
            }
        }

        void reset(HMODULE handle) {
            if (_handle != nullptr) {
                FreeLibrary(_handle);
            }
            _handle = handle;
        }

        auto get() const -> HMODULE {
            return _handle;
        }

        private:
        HMODULE _handle = nullptr;
    };

    ModuleHandle gUxtheme;
}

static void EnableDarkModeForApp() noexcept {
    if (setPreferredAppMode != nullptr) {
        setPreferredAppMode(AllowDark);
    }
}

[[nodiscard]] static auto GetBuildNumber() noexcept -> DWORD {
    auto rtlGetNtVersionNumbers = reinterpret_cast<RtlGetNtVersionNumbers_f>(
        GetProcAddress(GetModuleHandleW(L"ntdll.dll"), "RtlGetNtVersionNumbers")
    );

    if (rtlGetNtVersionNumbers == nullptr) {
        return 0;
    }

    DWORD major = 0;
    DWORD minor = 0;
    DWORD build = 0;
    rtlGetNtVersionNumbers(&major, &minor, &build);
    build &= ~0xF0000000;
    return build;
}

[[nodiscard]] static auto IsHighContrast() noexcept -> bool {
    HIGHCONTRASTW highContrast;
    highContrast.cbSize = sizeof(highContrast);
    if (SystemParametersInfoW(SPI_GETHIGHCONTRAST, sizeof(highContrast), &highContrast, FALSE) == TRUE) {
        return (highContrast.dwFlags & HCF_HIGHCONTRASTON) > 0;
    }
    return false;
}

static void InitDarkModeSupportOnce() noexcept {
    const auto buildNumber = GetBuildNumber();

    if (buildNumber < wiN10MinimumBuildDarkMode) {
        return;
    }

    gUxtheme.reset(LoadLibraryExW(L"uxtheme.dll", nullptr, LOAD_LIBRARY_SEARCH_SYSTEM32));

    if (gUxtheme.get() == nullptr) {
        return;
    }

    refreshImmersiveColorPolicyState =
        reinterpret_cast<RefreshImmersiveColorPolicyState_f>(GetProcAddress(gUxtheme.get(), MAKEINTRESOURCEA(104)));

    getIsImmersiveColorUsingHighContrast =
        reinterpret_cast<GetIsImmersiveColorUsingHighContrast_f>(GetProcAddress(gUxtheme.get(), MAKEINTRESOURCEA(106)));

    shouldAppsUseDarkMode =
        reinterpret_cast<ShouldAppsUseDarkMode_f>(GetProcAddress(gUxtheme.get(), MAKEINTRESOURCEA(132)));

    allowDarkModeForWindow =
        reinterpret_cast<AllowDarkModeForWindow_f>(GetProcAddress(gUxtheme.get(), MAKEINTRESOURCEA(133)));

    setPreferredAppMode =
        reinterpret_cast<SetPreferredAppMode_f>(GetProcAddress(gUxtheme.get(), MAKEINTRESOURCEA(135)));

    isDarkModeAllowedForWindow =
        reinterpret_cast<IsDarkModeAllowedForWindow_f>(GetProcAddress(gUxtheme.get(), MAKEINTRESOURCEA(137)));

    setWindowCompositionAttribute = reinterpret_cast<SetWindowCompositionAttribute_f>(
        GetProcAddress(GetModuleHandleW(L"user32.dll"), "SetWindowCompositionAttribute")
    );

    if (refreshImmersiveColorPolicyState != nullptr && shouldAppsUseDarkMode != nullptr &&
        allowDarkModeForWindow != nullptr && setPreferredAppMode != nullptr && isDarkModeAllowedForWindow != nullptr) {
        EnableDarkModeForApp();
        refreshImmersiveColorPolicyState();
    }
}

void InitDarkModeSupport() noexcept {
    std::call_once(flagInitDarkModeSupport, InitDarkModeSupportOnce);
}

auto IsDarkModeEnabled() noexcept -> bool {
    if (shouldAppsUseDarkMode == nullptr) {
        return false;
    }
    return (shouldAppsUseDarkMode() == TRUE) && !IsHighContrast();
}

void EnableDarkMode(const HWND hwnd, const bool enable) noexcept {
    if (allowDarkModeForWindow == nullptr) {
        return;
    }
    allowDarkModeForWindow(hwnd, enable ? TRUE : FALSE);
}

void RefreshNonClientArea(const HWND hwnd) noexcept {
    if (isDarkModeAllowedForWindow == nullptr || shouldAppsUseDarkMode == nullptr) {
        return;
    }

    BOOL dark = FALSE;
    if (isDarkModeAllowedForWindow(hwnd) == TRUE && shouldAppsUseDarkMode() == TRUE && !IsHighContrast()) {
        dark = TRUE;
    }

    if (setWindowCompositionAttribute != nullptr) {
        WINDOWCOMPOSITIONATTRIBDATA data = {WCA_USEDARKMODECOLORS, &dark, sizeof(dark)};
        setWindowCompositionAttribute(hwnd, &data);
    }
}

auto IsColorSchemeChange(const LPARAM lParam) noexcept -> bool {
    bool returnValue = false;
    if (lParam > 0) {
        bool isImmersiveColorSet = false;
        __try {
            isImmersiveColorSet =
                CompareStringOrdinal(reinterpret_cast<LPCWCH>(lParam), -1, L"ImmersiveColorSet", -1, TRUE) == CSTR_EQUAL;
        } __except (EXCEPTION_EXECUTE_HANDLER) {
            isImmersiveColorSet = false;
        }

        if (isImmersiveColorSet) {
            if (refreshImmersiveColorPolicyState != nullptr) {
                refreshImmersiveColorPolicyState();
            }
            returnValue = true;
        }
    }

    if (getIsImmersiveColorUsingHighContrast != nullptr) {
        getIsImmersiveColorUsingHighContrast(IHCM_REFRESH);
    }
    return returnValue;
}
