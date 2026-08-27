// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Frees a string allocated by the native layer.
/// @param value Pointer to the string to free.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_FreeString(const char* value) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(value, "value"))
                return;
            delete[] value;
        });
}

/// @brief Frees a string array allocated by the native layer.
/// @param values Pointer to the string array to free.
/// @param count Number of elements in the array.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_FreeStringArray(const char** values, const int count) {
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(values, "values"))
                return;
            if (count < 0)
                throw std::invalid_argument("Argument 'count' must be >= 0.");
            for (int i = 0; i < count; ++i) {
                if (values[i] != nullptr) {
                    InfiniFrameNative_FreeString(values[i]);
                }
            }
            delete[] values;
        });
}

/// @brief Gets the last error message.
/// @param[out] value Owned string, caller must free with InfiniFrameNative_FreeString.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetLastErrorMessage(const char** value) {
    // Must NOT go through RunExportStatus, that helper calls SetSuccess() first, which would wipe g_lastErrorMessage 
    // before we can read it.
    ResetOut(value, static_cast<const char*>(nullptr));
    if (!EnsureOutNotNull(value, "value")) {
        return InteropStatus::OutParameterSetToInvalidNull;
    }
    *value = GetLastErrorMessageCopy();
    return InteropStatus::Success;
}
}