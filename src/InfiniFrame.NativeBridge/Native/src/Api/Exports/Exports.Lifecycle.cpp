// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
#ifdef __APPLE__
#include <dispatch/dispatch.h>
#include <pthread.h>
#include "Runtime/Platform/Mac/MacDiagnostics.h"
#endif
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrameNative_ctor(InfiniFrameInitParams* initParams, InfiniFrameWindow** value) {
    ResetOut(value, static_cast<InfiniFrameWindow*>(nullptr));
    return RunExportStatus([&] {
        if (!EnsureOutNotNull(value, "value"))
            return;
        if (initParams == nullptr)
            throw std::invalid_argument("Argument 'initParams' is null.");
        if (initParams->StructSize != static_cast<int>(sizeof(InfiniFrameInitParams))) {
            throw std::invalid_argument("InfiniFrameInitParams.Size does not match native struct size.");
        }
        auto instance = std::make_unique<InfiniFrameWindow>(initParams);
        *value = instance.release();
    });
}

EXPORTED InteropStatus InfiniFrameNative_dtor(InfiniFrameWindow* instance) {
    return RunExportStatus([&] {
        if (!EnsureNotNull(instance, "instance"))
            return;
#ifdef __APPLE__
        if (infiniframe::macos::IsInsideNativeCallback()) {
            // SafeHandle disposal is legal from a managed event handler. The delegate
            // currently executing still has a raw pointer to this instance, so delete it
            // on the main queue after the callback frame has unwound.
            instance->PrepareForDeferredDestruction();
            if (pthread_main_np() != 0) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    delete instance;
                });
            } else {
                dispatch_async(dispatch_get_global_queue(QOS_CLASS_USER_INITIATED, 0), ^{
                    infiniframe::macos::WaitForNativeCallbacksToExit();
                    dispatch_async(dispatch_get_main_queue(), ^{
                        delete instance;
                    });
                });
            }
            return;
        }
#endif
        std::unique_ptr<InfiniFrameWindow> guard{instance};
    });
}

EXPORTED InteropStatus InfiniFrameNative_Close(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Close(); });
}

EXPORTED InteropStatus InfiniFrameNative_WaitForExit(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->WaitForExit(); });
}
}
