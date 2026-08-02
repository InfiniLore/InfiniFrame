// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
#include "Runtime/Shared/Operations/NativeOperation.h"
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::Invoke(const ACTION callback) {
    if (callback == nullptr) {
        return;
    }

    infiniframe::linux_gtk::ui_thread::InvokeSync([callback] { callback(); });
}

bool InfiniFrameWindow::ScheduleOperation(const std::shared_ptr<NativeOperation>& operation) {
    return infiniframe::linux_gtk::ui_thread::InvokeAsync([operation] { operation->Execute(); });
}
