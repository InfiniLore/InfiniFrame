// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Registers a callback for window close requests.
/// @param instance The window handle.
/// @param callback Callback invoked when the window close is requested.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetClosingCallback(
    InfiniFrameWindow* instance,
    const ClosingCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetClosingCallback(callback);
        });
}

/// @brief Registers a callback for window closed event.
/// @param instance The window handle.
/// @param callback Callback invoked when the window is closed.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetClosedCallback(InfiniFrameWindow* instance, const ClosedCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetClosedCallback(callback);
        });
}

/// @brief Registers a callback for window focus gained.
/// @param instance The window handle.
/// @param callback Callback invoked when the window gains focus.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetFocusInCallback(
    InfiniFrameWindow* instance,
    const FocusInCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetFocusInCallback(callback);
        });
}

/// @brief Registers a callback for window focus lost.
/// @param instance The window handle.
/// @param callback Callback invoked when the window loses focus.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetFocusOutCallback(
    InfiniFrameWindow* instance,
    const FocusOutCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetFocusOutCallback(callback);
        });
}

/// @brief Registers a callback for window position changes.
/// @param instance The window handle.
/// @param callback Callback invoked when the window is moved.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetMovedCallback(InfiniFrameWindow* instance, const MovedCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetMovedCallback(callback);
        });
}

/// @brief Registers a callback for window size changes.
/// @param instance The window handle.
/// @param callback Callback invoked when the window is resized.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetResizedCallback(
    InfiniFrameWindow* instance,
    const ResizedCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetResizedCallback(callback);
        });
}

/// @brief Registers a callback for file drop events.
/// @param instance The window handle.
/// @param callback Callback invoked when a file is dropped on the window.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetFileDroppedCallback(
    InfiniFrameWindow* instance,
    const FileDroppedCallback callback) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetFileDroppedCallback(callback);
        });
}

/// @brief Enables or disables file drag-and-drop on the window.
/// @param instance The window handle.
/// @param enabled Non-zero to enable drag-and-drop, zero to disable.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_SetDragDropEnabled(InfiniFrameWindow* instance, const int enabled) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            window->SetDragDropEnabled(enabled != 0);
        });
}
}