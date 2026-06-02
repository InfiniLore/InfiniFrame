// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrameNative_FreeString(AutoString value) {
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

EXPORTED InteropStatus InfiniFrameNative_FreeStringArray(AutoString* values, const int count) {
    return RunExportStatus([&] {
        if (!EnsureNotNull(values, "values"))
            return;
        if (count < 0)
            throw std::invalid_argument("Argument 'count' must be >= 0.");
        for (int i = 0; i < count; ++i) {
            if (values[i] != nullptr) {
                InfiniFrameNative_FreeString(values[i]);
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

/// @param[out] value Owned string, caller must free with InfiniFrameNative_FreeString.
EXPORTED InteropStatus InfiniFrameNative_GetLastErrorMessage(AutoString* value) {
    // Must NOT go through RunExportStatus, that helper calls SetSuccess() first, which would wipe g_lastErrorMessage 
    // before we can read it.
    ResetOut(value, static_cast<AutoString>(nullptr));
    if (!EnsureOutNotNull(value, "value")) {
        return InteropStatus::OutParameterSetToInvalidNull;
    }
    *value = GetLastErrorMessageCopy();
    return InteropStatus::Success;
}
}
