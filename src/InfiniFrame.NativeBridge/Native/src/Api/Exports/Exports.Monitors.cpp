// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Gets information about all connected displays.
/// @param instance The window handle.
/// @param callback Callback invoked with the monitor information.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_GetAllMonitors(
    InfiniFrameWindow* instance,
    const GetAllMonitorsCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (callback == nullptr)
                throw std::invalid_argument("Argument 'callback' is null.");
            window->GetAllMonitors(callback);
        });
}
}