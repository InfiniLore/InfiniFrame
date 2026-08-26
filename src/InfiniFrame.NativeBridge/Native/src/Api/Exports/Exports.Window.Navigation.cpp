// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Navigates the window to render raw HTML content.
/// @param instance The window handle.
/// @param content Null-terminated HTML string to render.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_NavigateToString(InfiniFrameWindow* instance, const char* content) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(content, "content"))
                return;
            window->NavigateToString(content);
        });
}

/// @brief Navigates the window to a URL.
/// @param instance The window handle.
/// @param url Null-terminated URL string to navigate to.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_NavigateToUrl(InfiniFrameWindow* instance, const char* url) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(url, "url"))
                return;
            window->NavigateToUrl(url);
        });
}

/// @brief Sends a string message to the browser's JavaScript context.
/// @param instance The window handle.
/// @param message Null-terminated message string to send.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SendWebMessage(InfiniFrameWindow* instance, const char* message) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SendWebMessage(NullToEmpty(message));
        });
}

/// @brief Begins an async navigation to raw HTML content with completion callback.
/// @param instance The window handle.
/// @param operationId Non-zero identifier for this async operation.
/// @param content Null-terminated HTML string to render.
/// @param completion Callback invoked when the navigation completes.
/// @param completionContext User-defined context passed to the callback.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_BeginNavigateToString(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const char* content,
    const OperationCompletedCallback completion,
    void* completionContext
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (operationId == 0 || completion == nullptr || !EnsureNotNull(content, "content"))
                return;
            window->BeginNavigateToString(operationId, content, completion, completionContext);
        });
}

/// @brief Begins an async navigation to a URL with completion callback.
/// @param instance The window handle.
/// @param operationId Non-zero identifier for this async operation.
/// @param url Null-terminated URL string to navigate to.
/// @param completion Callback invoked when the navigation completes.
/// @param completionContext User-defined context passed to the callback.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_BeginNavigateToUrl(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const char* url,
    const OperationCompletedCallback completion,
    void* completionContext
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (operationId == 0 || completion == nullptr || !EnsureNotNull(url, "url"))
                return;
            window->BeginNavigateToUrl(operationId, url, completion, completionContext);
        });
}

/// @brief Cancels a pending async navigation.
/// @param instance The window handle.
/// @param operationId Non-zero identifier of the operation to cancel.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_CancelNavigation(InfiniFrameWindow* instance, const uint64_t operationId) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (operationId == 0)
                throw std::invalid_argument("Argument 'operationId' must be non-zero.");
            window->CancelNavigation(operationId);
        });
}

/// @brief Gets the current page URL.
/// @param instance The window handle.
/// @param[out] value Owned string, caller must free with InfiniFrameNative_FreeString.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetCurrentUrl(InfiniFrameWindow* instance, const char** value) {
    ResetOut(value, static_cast<const char*>(nullptr));
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureOutNotNull(value, "value"))
                return;
            *value = window->GetCurrentUrl();
        });
}
}