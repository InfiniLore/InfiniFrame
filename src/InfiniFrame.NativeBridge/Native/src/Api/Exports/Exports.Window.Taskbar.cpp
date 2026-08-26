// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
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

EXPORTED InteropStatus InfiniFrameNative_ClearTaskbarProgress(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->ClearTaskbarProgress();
        });
}

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

EXPORTED InteropStatus InfiniFrameNative_StopTaskbarFlash(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->StopTaskbarFlash();
        });
}

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