// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrame_AddCustomSchemeName(InfiniFrameWindow* instance, const AutoString scheme) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (!EnsureNotNull(scheme, "scheme"))
            return;
        window->AddCustomSchemeName(scheme);
    });
}
}
