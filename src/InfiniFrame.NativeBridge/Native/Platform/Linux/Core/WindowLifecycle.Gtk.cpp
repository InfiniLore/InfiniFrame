// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::Center() {
    gint windowWidth, windowHeight;
    gtk_window_get_size(GTK_WINDOW(m_impl->_window), &windowWidth, &windowHeight);

    GdkRectangle screen = {};

    GdkDisplay* display = gdk_display_get_default();
    if (display == nullptr) {
        GtkWidget* dialog = gtk_message_dialog_new(
            nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
            "gdk_display_get_default() returned NULL"
        );
        gtk_dialog_run(GTK_DIALOG(dialog));
        gtk_widget_destroy(dialog);
        return;
    }

    GdkMonitor* monitor = gdk_display_get_primary_monitor(display);
    if (monitor == nullptr) {
        monitor = gdk_display_get_monitor(display, 0);
        if (monitor == nullptr) {
            GtkWidget* dialog = gtk_message_dialog_new(
                nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
                "gdk_display_get_primary_monitor() returned NULL"
            );
            gtk_dialog_run(GTK_DIALOG(dialog));
            gtk_widget_destroy(dialog);
            return;
        }
    }

    gdk_monitor_get_geometry(monitor, &screen);

    gtk_window_move(GTK_WINDOW(m_impl->_window), (screen.width - windowWidth) / 2, (screen.height - windowHeight) / 2);
}

void InfiniFrameWindow::ClearBrowserAutoFill() {
    // TODO
}

void InfiniFrameWindow::Close() {
    gtk_window_close(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::WaitForExit() {
    g_signal_connect(
        G_OBJECT(m_impl->_window), "destroy", G_CALLBACK(+[](GtkWidget*, gpointer) { gtk_main_quit(); }), nullptr
    );
    gtk_main();
}

void InfiniFrameWindow::CloseWebView() {
    if (m_impl->_webviewClosed)
        return;
    m_impl->_webviewClosed = true;

    GtkWidget* webview = m_impl->_webview;
    if (webview == nullptr)
        return;

    // Disconnect every signal whose user_data is this instance so callbacks can't fire while the WebKit objects tear
    // themselves down.
    g_signal_handlers_disconnect_by_data(webview, this);

    // Stop any in-flight load before we detach the widget.
    webkit_web_view_stop_loading(WEBKIT_WEB_VIEW(webview));

    // Explicitly detach and destroy the webview before the window destroy cascade runs so WebKit can settle its
    // singletons synchronously instead of being implicitly disposed by GtkContainer. The latter can leave dangling refs
    // in WebKit's singleton context that abort in its atexit handler (exit code 134).
    // NOTE: Do NOT call webkit_web_view_terminate_web_process() here — that sends SIGTERM to the WebKit subprocess and
    // can trigger a SIGABRT via GLib signal handling while we are inside a GTK signal callback. The process-exit
    // SIGABRT from WebKit's own atexit is handled separately by webkit_atexit_bypass() in WebKitHost.Gtk.cpp.
    g_object_ref(webview);
    if (GtkWidget* parent = gtk_widget_get_parent(webview))
        gtk_container_remove(GTK_CONTAINER(parent), webview);
    gtk_widget_destroy(webview);
    g_object_unref(webview);

    m_impl->_webview = nullptr;
}
