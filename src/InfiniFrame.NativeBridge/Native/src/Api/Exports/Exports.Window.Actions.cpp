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
    const char* title,
    const char* body
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->ShowNotification(NullToEmpty(title), NullToEmpty(body));
    });
}

EXPORTED InteropStatus InfiniFrameNative_ShowNotificationWithOptions(
    InfiniFrameWindow* instance,
    const char* title,
    const char* body,
    const char* iconPath,
    const int urgency,
    const char* tag
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->ShowNotificationWithOptions(
            NullToEmpty(title), NullToEmpty(body), NullToEmpty(iconPath), urgency, NullToEmpty(tag)
        );
    });
}

EXPORTED InteropStatus InfiniFrameNative_BeginShowNotification(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const char* title,
    const char* body,
    const char* iconPath,
    const int urgency,
    const char* tag,
    const OperationCompletedCallback completion,
    void* completionContext
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->BeginShowNotification(
            operationId,
            NullToEmpty(title), NullToEmpty(body), NullToEmpty(iconPath), urgency, NullToEmpty(tag),
            completion, completionContext
        );
    });
}

EXPORTED InteropStatus InfiniFrameNative_CancelNotification(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    bool* canceled
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->CancelNotification(operationId, canceled);
    });
}
}
