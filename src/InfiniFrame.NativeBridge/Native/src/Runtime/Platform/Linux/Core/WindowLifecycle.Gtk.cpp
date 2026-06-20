// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
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

    // Disconnect every signal whose user_data is this instance so our callbacks can't fire after the window starts
    // tearing down. The webview itself is destroyed implicitly by GTK when the parent window is destroyed.
    // Explicit destruction here (gtk_widget_destroy, terminate_web_process, pumping events) triggers WebKit's web
    // process cleanup from inside a GTK signal handler, which causes SIGABRT on libwebkit2gtk-4.1.
    // Do not unref _webContext here either. CloseWebView runs from the GTK delete-event path, and releasing the
    // context from this re-entrant teardown path can trigger the same WebKitGTK abort.
    g_signal_handlers_disconnect_by_data(webview, this);
    webkit_web_view_stop_loading(WEBKIT_WEB_VIEW(webview));
}
