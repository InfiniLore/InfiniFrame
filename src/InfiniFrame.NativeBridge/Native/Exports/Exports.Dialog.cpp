#include "Core/Exports.h"

extern "C" {
EXPORTED InteropStatus InfiniFrame_ShowOpenFile(InfiniFrameWindow* inst, const AutoString title, const AutoString defaultPath, const bool multiSelect, AutoString* filters, const int filterCount, int* resultCount, AutoString** values) {
    ResetOut(resultCount, 0);
    ResetOut(values, static_cast<AutoString*>(nullptr));
    return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(resultCount, "resultCount")) throw std::invalid_argument("Argument 'resultCount' is null.");
        if (!EnsureNotNull(values, "values")) throw std::invalid_argument("Argument 'values' is null.");
        if (filterCount < 0) throw std::invalid_argument("Argument 'filterCount' must be >= 0.");
        *values = window->GetDialog()->ShowOpenFile(title, defaultPath, multiSelect, filters, filterCount, resultCount);
    });
}

EXPORTED InteropStatus InfiniFrame_ShowOpenFolder(InfiniFrameWindow* inst, const AutoString title, const AutoString defaultPath, const bool multiSelect, int* resultCount, AutoString** values) {
    ResetOut(resultCount, 0);
    ResetOut(values, static_cast<AutoString*>(nullptr));
    return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(resultCount, "resultCount")) throw std::invalid_argument("Argument 'resultCount' is null.");
        if (!EnsureNotNull(values, "values")) throw std::invalid_argument("Argument 'values' is null.");
        *values = window->GetDialog()->ShowOpenFolder(title, defaultPath, multiSelect, resultCount);
    });
}

EXPORTED InteropStatus InfiniFrame_ShowSaveFile(InfiniFrameWindow* inst, const AutoString title, const AutoString defaultPath, AutoString* filters, const int filterCount, const AutoString defaultFileName, AutoString* value) {
    ResetOut(value, static_cast<AutoString>(nullptr));
    return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
        if (filterCount < 0) throw std::invalid_argument("Argument 'filterCount' must be >= 0.");
        *value = window->GetDialog()->ShowSaveFile(title, defaultPath, filters, filterCount, defaultFileName);
    });
}

EXPORTED InteropStatus InfiniFrame_ShowMessage(InfiniFrameWindow* inst, const AutoString title, const AutoString text, const DialogButtons buttons, const DialogIcon icon, DialogResult* value) {
    ResetOut(value, DialogResult::Cancel);
    return RunWindowExportStatus(inst, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(value, "value")) throw std::invalid_argument("Argument 'value' is null.");
        *value = window->GetDialog()->ShowMessage(title, text, buttons, icon);
    });
}
}
