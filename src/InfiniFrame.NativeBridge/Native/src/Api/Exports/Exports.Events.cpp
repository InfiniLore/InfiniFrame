// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrame_SetClosingCallback(InfiniFrameWindow* instance, const ClosingCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetClosingCallback(callback); });
}

EXPORTED InteropStatus InfiniFrame_setClosedCallback(InfiniFrameWindow* instance, const ClosedCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetClosedCallback(callback); });
}

EXPORTED InteropStatus InfiniFrame_SetFocusInCallback(InfiniFrameWindow* instance, const FocusInCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetFocusInCallback(callback); });
}

EXPORTED InteropStatus InfiniFrame_SetFocusOutCallback(InfiniFrameWindow* instance, const FocusOutCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetFocusOutCallback(callback); });
}

EXPORTED InteropStatus InfiniFrame_SetMovedCallback(InfiniFrameWindow* instance, const MovedCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetMovedCallback(callback); });
}

EXPORTED InteropStatus InfiniFrame_SetResizedCallback(InfiniFrameWindow* instance, const ResizedCallback callback) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) { window->SetResizedCallback(callback); });
}
}
