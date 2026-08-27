// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Sets the taskbar progress indicator state and value.
/// @param instance The window handle.
/// @param state Progress state (e.g., normal, error, paused).
/// @param current Current progress value.
/// @param total Maximum progress value.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetTaskbarProgress(
    InfiniFrameWindow* instance,
    const int state,
    const uint64_t current,
    const uint64_t total
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetTaskbarProgress(state, current, total);
        });
}

/// @brief Clears the taskbar progress indicator.
/// @param instance The window handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_ClearTaskbarProgress(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->ClearTaskbarProgress();
        });
}

/// @brief Starts a taskbar flash notification.
/// @param instance The window handle.
/// @param mode Flash mode (e.g., all, until foreground).
/// @param count Number of times to flash.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetTaskbarFlash(
    InfiniFrameWindow* instance,
    const int mode,
    const uint32_t count
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetTaskbarFlash(mode, count);
        });
}

/// @brief Stops the taskbar flash notification.
/// @param instance The window handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_StopTaskbarFlash(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->StopTaskbarFlash();
        });
}

/// @brief Queries whether taskbar progress is supported on this platform.
/// @param instance The window handle.
/// @param[out] supported Receives true if taskbar progress is supported.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetTaskbarProgressSupported(
    InfiniFrameWindow* instance,
    bool* supported
    ) {
    ResetOut(supported, false);
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(supported, "supported"))
                return;
            window->GetTaskbarProgressSupported(supported);
        });
}
}