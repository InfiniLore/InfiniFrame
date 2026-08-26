// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Centers the window on the primary monitor.
/// @param instance The window handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Center(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->Center();
        });
}

/// @brief Clears browser auto-fill data.
/// @param instance The window handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_ClearBrowserAutoFill(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->ClearBrowserAutoFill();
        });
}

/// @brief Restores the window from maximized or minimized state.
/// @param instance The window handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Restore(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->Restore();
        });
}

/// @brief Brings the window to the foreground and gives it keyboard focus.
/// @param instance The window handle.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetFocused(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->SetFocused();
        });
}

/// @brief Shows a simple desktop notification with title and body.
/// @param instance The window handle.
/// @param title The notification title.
/// @param body The notification body text.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_ShowNotification(
    InfiniFrameWindow* instance,
    const char* title,
    const char* body
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->ShowNotification(NullToEmpty(title), NullToEmpty(body));
        });
}

/// @brief Shows a desktop notification with options (icon, urgency, actions).
/// @param instance The window handle.
/// @param title The notification title.
/// @param body The notification body text.
/// @param iconPath Path to the notification icon file.
/// @param urgency Notification urgency level (0 = low, 1 = normal, 2 = critical).
/// @param tag Optional tag to identify or replace the notification.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_ShowNotificationWithOptions(
    InfiniFrameWindow* instance,
    const char* title,
    const char* body,
    const char* iconPath,
    const int urgency,
    const char* tag
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->ShowNotificationWithOptions(
                NullToEmpty(title), NullToEmpty(body), NullToEmpty(iconPath), urgency, NullToEmpty(tag)
                );
        });
}

/// @brief Begins showing an async notification with a completion callback.
/// @param instance The window handle.
/// @param operationId Unique identifier for this async operation.
/// @param title The notification title.
/// @param body The notification body text.
/// @param iconPath Path to the notification icon file.
/// @param urgency Notification urgency level (0 = low, 1 = normal, 2 = critical).
/// @param tag Optional tag to identify or replace the notification.
/// @param completion Callback invoked when the notification action is completed.
/// @param completionContext User-defined context passed to the completion callback.
/// @return InteropStatus
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
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->BeginShowNotification(
                operationId,
                NullToEmpty(title), NullToEmpty(body), NullToEmpty(iconPath), urgency, NullToEmpty(tag),
                completion, completionContext
                );
        });
}

/// @brief Cancels a pending async notification.
/// @param instance The window handle.
/// @param operationId The operation identifier of the notification to cancel.
/// @param canceled Output flag indicating whether the notification was successfully canceled.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_CancelNotification(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    bool* canceled
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->CancelNotification(operationId, canceled);
        });
}
}