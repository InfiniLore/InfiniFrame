// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrameNative_SetClosingCallback(
    InfiniFrameWindow* instance,
    const ClosingCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetClosingCallback(callback);
        });
}

EXPORTED InteropStatus InfiniFrameNative_setClosedCallback(InfiniFrameWindow* instance, const ClosedCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetClosedCallback(callback);
        });
}

EXPORTED InteropStatus InfiniFrameNative_SetFocusInCallback(
    InfiniFrameWindow* instance,
    const FocusInCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetFocusInCallback(callback);
        });
}

EXPORTED InteropStatus InfiniFrameNative_SetFocusOutCallback(
    InfiniFrameWindow* instance,
    const FocusOutCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetFocusOutCallback(callback);
        });
}

EXPORTED InteropStatus InfiniFrameNative_SetMovedCallback(InfiniFrameWindow* instance, const MovedCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetMovedCallback(callback);
        });
}

EXPORTED InteropStatus InfiniFrameNative_SetResizedCallback(
    InfiniFrameWindow* instance,
    const ResizedCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetResizedCallback(callback);
        });
}

EXPORTED InteropStatus InfiniFrameNative_SetFileDroppedCallback(
    InfiniFrameWindow* instance,
    const FileDroppedCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetFileDroppedCallback(callback);
        });
}

EXPORTED InteropStatus InfiniFrameNative_SetDragDropEnabled(InfiniFrameWindow* instance, const int enabled) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetDragDropEnabled(enabled != 0);
        });
}
}