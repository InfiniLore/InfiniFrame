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
EXPORTED InteropStatus InfiniFrameNative_Invoke(InfiniFrameWindow* instance, const ACTION callback) {
#ifdef __linux__
    (void)instance;
    return RunExportStatus([&] {
        if (callback == nullptr)
            throw std::invalid_argument("Argument 'callback' is null.");

        infiniframe::linux_gtk::ui_thread::InvokeSync([callback] { callback(); });
    });
#else
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
        if (callback == nullptr)
            throw std::invalid_argument("Argument 'callback' is null.");
        window->Invoke(callback);
    });
#endif
}

EXPORTED InteropStatus InfiniFrameNative_BeginInvoke(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const ContextAction callback,
    void* callbackContext,
    const OperationCompletedCallback completion,
    void* completionContext
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
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

EXPORTED InteropStatus InfiniFrameNative_CancelOperation(
    InfiniFrameWindow* instance,
    const uint64_t operationId,
    const int32_t result
) {
    return RunWindowExportStatus(instance, [&](InfiniFrameWindow* window) {
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
