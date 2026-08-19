// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <stdexcept>
#include <string>

#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
InfiniFrameWindow::InfiniFrameWindow(InfiniFrameInitParams* initParams)
    : m_impl(std::make_unique<Impl>()) {
    infiniframe::linux_gtk::ui_thread::EnsureInitialized();

    if (initParams->StructSize != sizeof(InfiniFrameInitParams)) {
        throw std::invalid_argument(
            "Initial parameters passed are " + std::to_string(initParams->StructSize) +
            " bytes, but expected " + std::to_string(sizeof(InfiniFrameInitParams)) + " bytes."
        );
    }

    infiniframe::linux_gtk::ui_thread::InvokeSync([this, initParams] {
        m_impl->InitializeFromParams(initParams);
        m_impl->ConfigureInitialWindow(this, initParams);
        m_impl->ApplyInitialWindowState(this, initParams);
        m_impl->ConnectWindowSignals(this);

        if (initParams->MenuBarJson != nullptr && initParams->MenuBarJson[0] != '\0')
            ApplyInitMenuBar(initParams->MenuBarJson);

        Show(false);

        m_impl->ConnectWebViewSignals(this);

        if (initParams->Transparent)
            SetTransparentEnabled(true);

        if (m_impl->_backgroundColorR != 0 || m_impl->_backgroundColorG != 0 || m_impl->_backgroundColorB != 0 || m_impl->_backgroundColorA != 0)
            SetBackgroundColor(m_impl->_backgroundColorR, m_impl->_backgroundColorG, m_impl->_backgroundColorB, m_impl->_backgroundColorA);

        if (m_impl->_zoom != 100.0)
            SetZoom(m_impl->_zoom);
    });
}

InfiniFrameWindow::~InfiniFrameWindow() {
    infiniframe::linux_gtk::ui_thread::InvokeSync([this] {
        if (m_impl->_window != nullptr) {
            g_signal_handlers_disconnect_by_data(m_impl->_window, this);
            gtk_widget_destroy(m_impl->_window);
            m_impl->_window = nullptr;
        }

        if (m_impl->_webview != nullptr) {
            g_signal_handlers_disconnect_by_data(m_impl->_webview, this);
            m_impl->_webview = nullptr;
        }

        m_impl->_webContext = nullptr;

        {
            std::lock_guard lock(m_impl->_lifecycleMutex);
            m_impl->_destroyed = true;
        }
        m_impl->_lifecycleClosed.notify_all();
    });
}

InfiniFrameWindowImpl* InfiniFrameWindow::ImplBase() noexcept { return m_impl.get(); }
const InfiniFrameWindowImpl* InfiniFrameWindow::ImplBase() const noexcept { return m_impl.get(); }

GtkWidget* InfiniFrameWindow::getGtkWindow() {
    return m_impl->_window;
}
