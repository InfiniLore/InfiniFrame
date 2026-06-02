// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
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
