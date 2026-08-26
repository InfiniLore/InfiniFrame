// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
#ifdef __APPLE__
/// @brief Registers the macOS window class.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_register_mac() {
    return RunExportStatus(
        [] {
            InfiniFrameWindow::Register();
        });
}

/// @brief Gets the NSWindow handle for the window.
/// @param instance The window handle.
/// @param[out] value Receives the NSWindow pointer as void*.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_getNSWindow_mac(InfiniFrameWindow* instance, void** value) {
    ResetOut(value, static_cast<void*>(nullptr));
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(value, "value"))
                return;

            *value = static_cast<void*>(window->getNSWindow());
        });
}
#endif
}