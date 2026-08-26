// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
#ifdef __linux__
/// @brief Gets the GTK window handle for the window.
/// @param instance The window handle.
/// @param[out] value Receives the GtkWidget pointer.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_getGtkWindow_linux(InfiniFrameWindow* instance, GtkWidget** value) {
    ResetOut(value, static_cast<GtkWidget*>(nullptr));
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(value, "value"))
                return;
            *value = window->getGtkWindow();
        });
}
#endif
}