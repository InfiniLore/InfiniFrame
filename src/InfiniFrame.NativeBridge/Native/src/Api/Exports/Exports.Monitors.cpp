// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrame_GetAllMonitors(InfiniFrameWindow* instance, const GetAllMonitorsCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (callback == nullptr)
            throw std::invalid_argument("Argument 'callback' is null.");
        window->GetAllMonitors(callback);
    });
}
}
