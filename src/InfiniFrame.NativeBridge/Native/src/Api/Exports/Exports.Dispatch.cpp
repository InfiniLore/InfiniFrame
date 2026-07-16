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
}
