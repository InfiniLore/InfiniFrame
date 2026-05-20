// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Public/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @param[out] values Owned string array — caller must free with InfiniFrame_FreeStringArray(values, resultCount).
EXPORTED InteropStatus InfiniFrame_ShowOpenFile(
    InfiniFrameWindow* inst,
    const AutoString title,
    const AutoString defaultPath,
    const bool multiSelect,
    AutoString* filters,
    const int filterCount,
    int* resultCount,
    AutoString** values
) {
    ResetOut(resultCount, 0);
    ResetOut(values, static_cast<AutoString*>(nullptr));
    return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(resultCount, "resultCount"))
            return;
        if (!EnsureOutNotNull(values, "values"))
            return;
        if (filterCount < 0)
            throw std::invalid_argument("Argument 'filterCount' must be >= 0.");
        *values = window->GetDialog()->ShowOpenFile(
            NullToEmpty(title), NullToEmpty(defaultPath), multiSelect, filters, filterCount, resultCount
        );
    });
}

/// @param[out] values Owned string array — caller must free with InfiniFrame_FreeStringArray(values, resultCount).
EXPORTED InteropStatus InfiniFrame_ShowOpenFolder(
    InfiniFrameWindow* inst,
    const AutoString title,
    const AutoString defaultPath,
    const bool multiSelect,
    int* resultCount,
    AutoString** values
) {
    ResetOut(resultCount, 0);
    ResetOut(values, static_cast<AutoString*>(nullptr));
    return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
        if (!EnsureOutNotNull(resultCount, "resultCount"))
            return;
        if (!EnsureOutNotNull(values, "values"))
            return;
        *values =
            window->GetDialog()->ShowOpenFolder(NullToEmpty(title), NullToEmpty(defaultPath), multiSelect, resultCount);
    });
}

/// @param[out] value Owned string — caller must free with InfiniFrame_FreeString.
EXPORTED InteropStatus InfiniFrame_ShowSaveFile(
    InfiniFrameWindow* inst,
    const AutoString title,
    const AutoString defaultPath,
    AutoString* filters,
    const int filterCount,
    const AutoString defaultFileName,
    AutoString* value
) {
    ResetOut(value, static_cast<AutoString>(nullptr));
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

EXPORTED InteropStatus InfiniFrame_ShowMessage(
    InfiniFrameWindow* inst,
    const AutoString title,
    const AutoString text,
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
}
