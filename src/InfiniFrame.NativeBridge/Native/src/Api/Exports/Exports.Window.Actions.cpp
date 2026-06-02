// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrameNative_Center(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Center(); });
}

EXPORTED InteropStatus InfiniFrameNative_ClearBrowserAutoFill(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->ClearBrowserAutoFill(); });
}

EXPORTED InteropStatus InfiniFrameNative_Restore(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Restore(); });
}

EXPORTED InteropStatus InfiniFrameNative_SetFocused(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->SetFocused(); });
}

EXPORTED InteropStatus InfiniFrameNative_ShowNotification(
    InfiniFrameWindow* instance,
    const AutoString title,
    const AutoString body
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->ShowNotification(NullToEmpty(title), NullToEmpty(body));
    });
}
}
