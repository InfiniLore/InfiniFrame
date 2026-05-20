// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Public/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrame_FreeString(AutoString value) {
    return RunExportStatus([&] {
        if (!EnsureNotNull(value, "value"))
            return;
#ifdef _WIN32
        delete[] value;
#elif __linux__
        g_free(value);
#else
        free(value);
#endif
    });
}

EXPORTED InteropStatus InfiniFrame_FreeStringArray(AutoString* values, const int count) {
    return RunExportStatus([&] {
        if (!EnsureNotNull(values, "values"))
            return;
        if (count < 0)
            throw std::invalid_argument("Argument 'count' must be >= 0.");
        for (int i = 0; i < count; ++i) {
            if (values[i] != nullptr) {
                InfiniFrame_FreeString(values[i]);
            }
        }
#ifdef _WIN32
        delete[] values;
#elif __linux__
        delete[] values;
#else
        free(values);
#endif
    });
}

EXPORTED InteropStatus InfiniFrame_GetLastErrorMessage(AutoString* value) {
    ResetOut(value, static_cast<AutoString>(nullptr));
    return RunExportStatus([&] {
        if (!EnsureOutNotNull(value, "value"))
            return;
        *value = GetLastErrorMessageCopy();
    });
}
}
