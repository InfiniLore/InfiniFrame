#include "Public/Exports/Exports.h"

extern "C" {
EXPORTED InteropStatus InfiniFrame_ctor(InfiniFrameInitParams* initParams, InfiniFrameWindow** value) {
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

EXPORTED InteropStatus InfiniFrame_dtor(InfiniFrameWindow* instance) {
    return RunExportStatus([&] {
        if (!EnsureNotNull(instance, "instance"))
            return;
        std::unique_ptr<InfiniFrameWindow> guard{instance};
    });
}

EXPORTED InteropStatus InfiniFrame_Close(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->Close(); });
}

EXPORTED InteropStatus InfiniFrame_WaitForExit(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(instance, [](InfiniFrameWindow* window) { window->WaitForExit(); });
}
}
