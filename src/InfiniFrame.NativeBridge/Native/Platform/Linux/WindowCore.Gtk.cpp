#ifdef __linux__

#include <X11/Xlib.h>
#include <libnotify/notify.h>

#include "Window.Gtk.Internal.h"

InfiniFrameWindow::InfiniFrameWindow(InfiniFrameInitParams* initParams) :
    m_impl(std::make_unique<Impl>()) {
    XInitThreads();
    gtk_init(nullptr, nullptr);
    notify_init(initParams->Title);

    if (initParams->Size != sizeof(InfiniFrameInitParams)) {
        GtkWidget* dialog = gtk_message_dialog_new(
            nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
            "Initial parameters passed are %i bytes, but expected %lu bytes.",
            initParams->Size, sizeof(InfiniFrameInitParams)
            );
        gtk_dialog_run(GTK_DIALOG(dialog));
        gtk_widget_destroy(dialog);
        exit(0);
    }

    m_impl->InitializeFromParams(initParams);
    m_impl->ConfigureInitialWindow(this, initParams);
    m_impl->ApplyInitialWindowState(this, initParams);
    m_impl->ConnectWindowSignals(this);

    // Register custom schemes before first navigation to avoid first-load races.
    m_impl->AddCustomSchemeHandlers();

    Show(false);

    m_impl->ConnectWebViewSignals(this);

    if (initParams->Transparent)
        SetTransparentEnabled(true);

    if (m_impl->_zoom != 100.0)
        SetZoom(m_impl->_zoom);
}

InfiniFrameWindow::~InfiniFrameWindow() {
    notify_uninit();
    gtk_widget_destroy(m_impl->_window);
}

#endif
