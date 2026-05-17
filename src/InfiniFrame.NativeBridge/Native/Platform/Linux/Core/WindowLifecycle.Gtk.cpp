#ifdef __linux__

#include "../Window.Gtk.Internal.h"

#include <libnotify/notify.h>

void InfiniFrameWindow::Center() {
    gint windowWidth, windowHeight;
    gtk_window_get_size(GTK_WINDOW(m_impl->_window), &windowWidth, &windowHeight);

    GdkRectangle screen = {0};

    GdkDisplay* d = gdk_display_get_default();
    if (d == nullptr) {
        GtkWidget* dialog = gtk_message_dialog_new(
            nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
            "gdk_display_get_default() returned NULL"
        );
        gtk_dialog_run(GTK_DIALOG(dialog));
        gtk_widget_destroy(dialog);
        return;
    }

    GdkMonitor* m = gdk_display_get_primary_monitor(d);
    if (m == nullptr) {
        m = gdk_display_get_monitor(d, 0);
        if (m == nullptr) {
            GtkWidget* dialog = gtk_message_dialog_new(
                nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
                "gdk_display_get_primary_monitor() returned NULL"
            );
            gtk_dialog_run(GTK_DIALOG(dialog));
            gtk_widget_destroy(dialog);
            return;
        }
    }

    gdk_monitor_get_geometry(m, &screen);

    gtk_window_move(
        GTK_WINDOW(m_impl->_window),
        (screen.width - windowWidth) / 2,
        (screen.height - windowHeight) / 2
    );
}

void InfiniFrameWindow::ClearBrowserAutoFill() {
    // TODO
}

void InfiniFrameWindow::Close() {
    gtk_window_close(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::ShowNotification(const AutoString title, const AutoString message) {
    NotifyNotification* notification = notify_notification_new(title, message, nullptr);
    notify_notification_set_icon_from_pixbuf(notification, gtk_window_get_icon(GTK_WINDOW(m_impl->_window)));
    notify_notification_show(notification, nullptr);
    g_object_unref(G_OBJECT(notification));
}

void InfiniFrameWindow::WaitForExit() {
    g_signal_connect(
        G_OBJECT(m_impl->_window), "destroy",
        G_CALLBACK(
            +[](GtkWidget*, gpointer) {
                gtk_main_quit();
            }
        ),
        nullptr
    );
    gtk_main();
}

void InfiniFrameWindow::CloseWebView() {
    // Not implemented on Linux
}

#endif
