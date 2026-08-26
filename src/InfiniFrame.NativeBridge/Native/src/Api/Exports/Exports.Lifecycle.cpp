// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Api/Exports/Exports.h"
#ifdef __linux__
#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
#endif
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern "C" {
EXPORTED InteropStatus InfiniFrameNative_ctor(InfiniFrameInitParams* initParams, InfiniFrameWindow** value) {
    ResetOut(value, static_cast<InfiniFrameWindow*>(nullptr));
    return RunExportStatus(
        [&] {
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
    return RunExportStatus(
        [&] {
            if (!EnsureNotNull(instance, "instance"))
                return;
#ifdef __APPLE__
            // WKWebView close is asynchronous.  SafeHandle may be disposed immediately after
            // Close (and from a reverse P/Invoke callback), so the native instance takes ownership
            // of its own final deletion and performs it only after AppKit's close boundary.
            instance->ScheduleDeferredDestruction();
            return;
#endif
            std::unique_ptr<InfiniFrameWindow> guard{instance};
        });
}

EXPORTED InteropStatus InfiniFrameNative_Close(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->Close();
        });
}

EXPORTED InteropStatus InfiniFrameNative_WaitForExit(InfiniFrameWindow* instance) {
    return RunWindowExportStatus(
        instance, [](InfiniFrameWindow* window) {
            window->WaitForExit();
        });
}

EXPORTED InteropStatus InfiniFrameNative_SetReadyCallback(
    InfiniFrameWindow* instance,
    const ContextAction callback,
    void* context
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (callback == nullptr)
                throw std::invalid_argument("Argument 'callback' is null.");
            window->SetReadyCallback(callback, context);
        });
}

EXPORTED InteropStatus InfiniFrameNative_SetTeardownCallback(
    InfiniFrameWindow* instance,
    const ContextAction callback,
    void* context
    ) {
    return RunWindowExportStatus(
        instance, [&](InfiniFrameWindow* window) {
            if (callback == nullptr)
                throw std::invalid_argument("Argument 'callback' is null.");
            window->SetTeardownCallback(callback, context);
        });
}

#ifdef __linux__
EXPORTED InteropStatus InfiniFrameNative_Shutdown() {
    return RunExportStatus(
        [] {
            infiniframe::linux_gtk::ui_thread::Shutdown();
        });
}
#endif
}