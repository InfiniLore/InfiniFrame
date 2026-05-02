#ifdef __linux__

#include "Platform/Linux/WindowImpl.Gtk.h"

#include <libnotify/notify.h>

void InfiniFrameWindow::Impl::InitializeNotifications(const AutoStringConst appName) const {
    notify_init(appName == nullptr ? "InfiniFrame" : appName);
}

void InfiniFrameWindow::Impl::ShutdownNotifications() const noexcept {
    notify_uninit();
}

void InfiniFrameWindow::ShowNotification(const AutoString title, const AutoString message) {
    NotifyNotification* notification = notify_notification_new(
        title == nullptr ? "" : title,
        message == nullptr ? "" : message,
        nullptr
        );
    notify_notification_set_icon_from_pixbuf(notification, gtk_window_get_icon(GTK_WINDOW(m_impl->_window)));
    notify_notification_show(notification, nullptr);
    g_object_unref(G_OBJECT(notification));
}

#endif
