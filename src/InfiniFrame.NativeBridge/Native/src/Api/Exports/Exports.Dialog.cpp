// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @param[out] values Owned string array, caller must free with InfiniFrameNative_FreeStringArray(values, resultCount).
EXPORTED InteropStatus InfiniFrameNative_ShowOpenFile(
    InfiniFrameWindow* inst,
    const char* title,
    const char* defaultPath,
    const bool MultiSelect,
    const char** filters,
    const int FilterCount,
    int* resultCount,
    const char*** values
) {
    ResetOut(resultCount, 0);
    ResetOut(values, static_cast<const char**>(nullptr));
    return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(resultCount, "resultCount")) {
            return;
        }
        if (!EnsureOutNotNull(values, "values")) {
            return;
        }
        if (FilterCount < 0) {
            throw std::invalid_argument("Argument 'filterCount' must be >= 0.");
        }
        *values = window->GetDialog()->ShowOpenFile(
            NullToEmpty(title), NullToEmpty(defaultPath), MultiSelect, filters, FilterCount, resultCount
        );
    });
}

/// @param[out] values Owned string array, caller must free with InfiniFrameNative_FreeStringArray(values, resultCount).
EXPORTED InteropStatus InfiniFrameNative_ShowOpenFolder(
    InfiniFrameWindow* inst,
    const char* title,
    const char* defaultPath,
    const bool multiSelect,
    int* resultCount,
    const char*** values
) {
    ResetOut(resultCount, 0);
    ResetOut(values, static_cast<const char**>(nullptr));
    return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(resultCount, "resultCount")) {
            return;
        }
        if (!EnsureOutNotNull(values, "values")) {
            return;
        }
        *values =
            window->GetDialog()->ShowOpenFolder(NullToEmpty(title), NullToEmpty(defaultPath), multiSelect, resultCount);
    });
}

/// @param[out] value Owned string, caller must free with InfiniFrameNative_FreeString.
EXPORTED InteropStatus InfiniFrameNative_ShowSaveFile(
    InfiniFrameWindow* inst,
    const char* title,
    const char* defaultPath,
    const char** filters,
    const int filterCount,
    const char* defaultFileName,
    const char** value
) {
    ResetOut(value, static_cast<const char*>(nullptr));
    return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        if (filterCount < 0)
            throw std::invalid_argument("Argument 'filterCount' must be >= 0.");
        *value = window->GetDialog()->ShowSaveFile(
            NullToEmpty(title), NullToEmpty(defaultPath), filters, filterCount, NullToEmpty(defaultFileName)
        );
    });
}

EXPORTED InteropStatus InfiniFrameNative_ShowMessage(
    InfiniFrameWindow* inst,
    const char* title,
    const char* text,
    const DialogButtons buttons,
    const DialogIcon icon,
    DialogResult* value
) {
    ResetOut(value, DialogResult::Cancel);
    return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = window->GetDialog()->ShowMessage(NullToEmpty(title), NullToEmpty(text), buttons, icon);
    });
}

EXPORTED InteropStatus InfiniFrameNative_BeginShowOpenFile(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const char* title,
    const char* defaultPath,
    const bool multiSelect,
    const char** filters,
    const int filterCount,
    const FileDialogCompletedCallback completion,
    void* completionContext
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (operationId == 0 || completion == nullptr || filterCount < 0)
            throw std::invalid_argument("Invalid asynchronous open-file dialog arguments.");
        window->BeginShowOpenFile(
            operationId, NullToEmpty(title), NullToEmpty(defaultPath), multiSelect, filters, filterCount, completion,
            completionContext
        );
    });
}

EXPORTED InteropStatus InfiniFrameNative_BeginShowOpenFolder(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const char* title,
    const char* defaultPath,
    const bool multiSelect,
    const FileDialogCompletedCallback completion,
    void* completionContext
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (operationId == 0 || completion == nullptr)
            throw std::invalid_argument("Invalid asynchronous open-folder dialog arguments.");
        window->BeginShowOpenFolder(
            operationId, NullToEmpty(title), NullToEmpty(defaultPath), multiSelect, completion, completionContext
        );
    });
}

EXPORTED InteropStatus InfiniFrameNative_BeginShowSaveFile(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const char* title,
    const char* defaultPath,
    const char** filters,
    const int filterCount,
    const char* defaultFileName,
    const FileDialogCompletedCallback completion,
    void* completionContext
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (operationId == 0 || completion == nullptr || filterCount < 0)
            throw std::invalid_argument("Invalid asynchronous save-file dialog arguments.");
        window->BeginShowSaveFile(
            operationId, NullToEmpty(title), NullToEmpty(defaultPath), filters, filterCount,
            NullToEmpty(defaultFileName), completion, completionContext
        );
    });
}

EXPORTED InteropStatus InfiniFrameNative_BeginShowMessage(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const char* title,
    const char* text,
    const DialogButtons buttons,
    const DialogIcon icon,
    const OperationCompletedCallback completion,
    void* completionContext
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (operationId == 0 || completion == nullptr)
            throw std::invalid_argument("Invalid asynchronous message-dialog arguments.");
        window->BeginShowMessage(
            operationId, NullToEmpty(title), NullToEmpty(text), buttons, icon, completion, completionContext
        );
    });
}

EXPORTED InteropStatus
InfiniFrameNative_CancelDialog(InfiniFrameWindow* instance, const uint64_t operationId, bool* cancelled) {
    ResetOut(cancelled, false);
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(cancelled, "cancelled"))
            return;
        *cancelled = window->CancelDialog(operationId);
    });
}
}
