// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
#ifdef __APPLE__
EXPORTED InteropStatus InfiniFrameNative_register_mac() {
    return RunExportStatus([] { InfiniFrameWindow::Register(); });
}

EXPORTED InteropStatus InfiniFrameNative_getNSWindow_mac(InfiniFrameWindow* instance, void** value) {
    ResetOut(value, static_cast<void*>(nullptr));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;

        *value = static_cast<void*>(window->getNSWindow());
    });
}
#endif
}
