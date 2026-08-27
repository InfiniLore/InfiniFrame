// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Registers a custom URL scheme name.
/// @param instance The window handle.
/// @param scheme Null-terminated scheme name string to register.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_AddCustomSchemeName(InfiniFrameWindow* instance, const char* scheme) { // NOLINT(*-identifier-naming)
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (!EnsureNotNull(scheme, "scheme")) {
                return;
            }
            window->AddCustomSchemeName(scheme);
        });
}
}