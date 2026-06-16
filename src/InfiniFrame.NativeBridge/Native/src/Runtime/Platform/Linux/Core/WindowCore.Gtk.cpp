// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <mutex>
#include <stdexcept>
#include <string>

#include <X11/Xlib.h>
#include <libnotify/notify.h>

#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
InfiniFrameWindow::InfiniFrameWindow(InfiniFrameInitParams* initParams)
    : m_impl(std::make_unique<Impl>()) {
    static std::once_flag once;
    std::call_once(once, [] {
        XInitThreads();
        gtk_init(nullptr, nullptr);
    });
    notify_init(initParams->Title);

    if (initParams->StructSize != sizeof(InfiniFrameInitParams)) {
        throw std::invalid_argument(
            "Initial parameters passed are " + std::to_string(initParams->StructSize) +
            " bytes, but expected " + std::to_string(sizeof(InfiniFrameInitParams)) + " bytes."
        );
    }

    m_impl->InitializeFromParams(initParams);
    m_impl->ConfigureInitialWindow(this, initParams);
    m_impl->ApplyInitialWindowState(this, initParams);
    m_impl->ConnectWindowSignals(this);
    m_impl->configure_webkit_remote_debugging();

    Show(false);

    m_impl->ConnectWebViewSignals(this);

    if (initParams->Transparent)
        SetTransparentEnabled(true);

    if (m_impl->_zoom != 100.0)
        SetZoom(m_impl->_zoom);
}

InfiniFrameWindow::~InfiniFrameWindow() {
    notify_uninit();
}

InfiniFrameWindowImpl* InfiniFrameWindow::ImplBase() noexcept { return m_impl.get(); }
const InfiniFrameWindowImpl* InfiniFrameWindow::ImplBase() const noexcept { return m_impl.get(); }

GtkWidget* InfiniFrameWindow::getGtkWindow() {
    return m_impl->_window;
}
