// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrameNative_NavigateToString(InfiniFrameWindow* instance, const AutoString content) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(content, "content"))
            return;
        window->NavigateToString(content);
    });
}

EXPORTED InteropStatus InfiniFrameNative_NavigateToUrl(InfiniFrameWindow* instance, const AutoString url) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(url, "url"))
            return;
        window->NavigateToUrl(url);
    });
}

EXPORTED InteropStatus InfiniFrameNative_SendWebMessage(InfiniFrameWindow* instance, const AutoString message) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        window->SendWebMessage(NullToEmpty(message));
    });
}

EXPORTED InteropStatus InfiniFrameNative_BeginNavigateToString(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const AutoString content,
    const OperationCompletedCallback completion,
    void* completionContext
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (operationId == 0 || completion == nullptr || !EnsureNotNull(content, "content"))
            return;
        window->BeginNavigateToString(operationId, content, completion, completionContext);
    });
}

EXPORTED InteropStatus InfiniFrameNative_BeginNavigateToUrl(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const AutoString url,
    const OperationCompletedCallback completion,
    void* completionContext
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (operationId == 0 || completion == nullptr || !EnsureNotNull(url, "url"))
            return;
        window->BeginNavigateToUrl(operationId, url, completion, completionContext);
    });
}

EXPORTED InteropStatus InfiniFrameNative_CancelNavigation(InfiniFrameWindow* instance, const uint64_t operationId) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (operationId == 0)
            throw std::invalid_argument("Argument 'operationId' must be non-zero.");
        window->CancelNavigation(operationId);
    });
}

/// @param[out] value Owned string, caller must free with InfiniFrameNative_FreeString.
EXPORTED InteropStatus InfiniFrameNative_GetCurrentUrl(InfiniFrameWindow* instance, AutoString* value) {
    ResetOut(value, static_cast<AutoString>(nullptr));
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->GetCurrentUrl();
    });
}
}
