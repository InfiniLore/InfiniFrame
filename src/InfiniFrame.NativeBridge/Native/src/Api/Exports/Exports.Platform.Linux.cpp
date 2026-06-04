// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
#ifdef __linux__
EXPORTED InteropStatus InfiniFrameNative_getGtkWindow_linux(InfiniFrameWindow* instance, GtkWidget** value) {
    ResetOut(value, static_cast<GtkWidget*>(nullptr));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->getGtkWindow();
    });
}
#endif
}
