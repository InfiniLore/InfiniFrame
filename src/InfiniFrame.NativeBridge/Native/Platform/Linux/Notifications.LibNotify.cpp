// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <libnotify/notify.h>

#include "Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::ShowNotification(const AutoString title, const AutoString message) {
    NotifyNotification* notification = notify_notification_new(title, message, nullptr);
    notify_notification_set_icon_from_pixbuf(notification, gtk_window_get_icon(GTK_WINDOW(m_impl->_window)));
    notify_notification_show(notification, nullptr);
    g_object_unref(G_OBJECT(notification));
}
