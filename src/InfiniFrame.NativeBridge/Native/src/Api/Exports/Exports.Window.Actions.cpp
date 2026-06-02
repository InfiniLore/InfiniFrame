// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrame_Center(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Center(); });
}

EXPORTED InteropStatus InfiniFrame_ClearBrowserAutoFill(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->ClearBrowserAutoFill(); });
}

EXPORTED InteropStatus InfiniFrame_Restore(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Restore(); });
}

EXPORTED InteropStatus InfiniFrame_SetFocused(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->SetFocused(); });
}

EXPORTED InteropStatus InfiniFrame_ShowNotification(
    InfiniFrameWindow* instance,
    const AutoString title,
    const AutoString body
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->ShowNotification(NullToEmpty(title), NullToEmpty(body));
    });
}
}
