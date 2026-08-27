// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
#include "Runtime/Shared/Operations/NativeOperation.h"
#ifdef __linux__
#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
#endif
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
/// @brief Synchronously dispatches a callback to the window's native thread.
/// @param instance The window handle.
/// @param callback Action to execute on the native thread.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_Invoke(InfiniFrameWindow* instance, const ACTION callback) {
#ifdef __linux__
    (void)instance;
    return RunExportStatus(
        [&] {
            if (callback == nullptr)
                throw std::invalid_argument("Argument 'callback' is null.");

            infiniframe::linux_gtk::ui_thread::InvokeSync(
                [callback] {
                    callback();
                });
        });
#else
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (callback == nullptr)
                throw std::invalid_argument("Argument 'callback' is null.");
            window->Invoke(callback);
        });
#endif
}

/// @brief Begins an async dispatch to the window's native thread with completion callback.
/// @param instance The window handle.
/// @param operationId Non-zero identifier for this async operation.
/// @param callback Context action to execute on the native thread.
/// @param callbackContext User-defined context passed to the callback.
/// @param completion Callback invoked when the dispatch completes.
/// @param completionContext User-defined context passed to the completion callback.
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_BeginInvoke(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const ContextAction callback,
    void* callbackContext,
    const OperationCompletedCallback completion,
    void* completionContext
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (operationId == 0)
                throw std::invalid_argument("Argument 'operationId' must be non-zero.");
            if (callback == nullptr)
                throw std::invalid_argument("Argument 'callback' is null.");
            if (completion == nullptr)
                throw std::invalid_argument("Argument 'completion' is null.");
            if (!window->BeginInvoke(operationId, callback, callbackContext, completion, completionContext))
                throw std::runtime_error("The asynchronous dispatch could not be queued.");
        });
}

/// @brief Cancels a pending async operation.
/// @param instance The window handle.
/// @param operationId Non-zero identifier of the operation to cancel.
/// @param result Cancellation result code (TimedOut, Cancelled, or WindowClosed).
/// @return InteropStatus
EXPORTED InteropStatus InfiniFrameNative_CancelOperation(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const int32_t result
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (operationId == 0)
                throw std::invalid_argument("Argument 'operationId' must be non-zero.");
            if (result != static_cast<int32_t>(NativeOperationResult::TimedOut)
                && result != static_cast<int32_t>(NativeOperationResult::Cancelled)
                && result != static_cast<int32_t>(NativeOperationResult::WindowClosed))
                throw std::invalid_argument("Argument 'result' is not cancellable.");
            window->CancelOperation(operationId, static_cast<NativeOperationResult>(result));
        });
}
}