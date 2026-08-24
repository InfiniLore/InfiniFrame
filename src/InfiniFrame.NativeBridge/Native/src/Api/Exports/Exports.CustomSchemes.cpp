// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
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