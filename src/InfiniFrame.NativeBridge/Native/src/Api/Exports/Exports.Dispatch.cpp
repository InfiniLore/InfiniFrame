// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrame_Invoke(InfiniFrameWindow* instance, const ACTION callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (callback == nullptr)
            throw std::invalid_argument("Argument 'callback' is null.");
        window->Invoke(callback);
    });
}
}
