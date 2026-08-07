// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrameNative_AddCustomSchemeName(InfiniFrameWindow* instance, const char* scheme) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(scheme, "scheme"))
            return;
        window->AddCustomSchemeName(scheme);
    });
}
}
