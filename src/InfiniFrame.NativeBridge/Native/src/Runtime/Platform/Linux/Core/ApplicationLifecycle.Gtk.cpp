// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Application/InfiniFrameApplication.h"
#include "Runtime/Shared/Application/InfiniFrameApplicationImpl.h"
#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameApplication::Run() {
    gtk_main();
}

void InfiniFrameApplication::Shutdown() {
    bool expected = false;
    if (!m_impl->_shutdownRequested.compare_exchange_strong(expected, true, std::memory_order_acq_rel))
        return;

    infiniframe::linux_gtk::ui_thread::InvokeSync([] {
        gtk_main_quit();
    });
}
