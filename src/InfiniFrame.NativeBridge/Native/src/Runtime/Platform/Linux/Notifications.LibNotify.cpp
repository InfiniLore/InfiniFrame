// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <libnotify/notify.h>

#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
#include "Api/Utilities/ExportStringHelpers.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using namespace infiniframe::exports;

void InfiniFrameWindow::ShowNotification(const char* title, const char* message) {
    NotifyNotification* notification = notify_notification_new(title, message, nullptr);
    notify_notification_set_icon_from_pixbuf(notification, gtk_window_get_icon(GTK_WINDOW(m_impl->_window)));
    notify_notification_show(notification, nullptr);
    g_object_unref(G_OBJECT(notification));
}

void InfiniFrameWindow::ShowNotificationWithOptions(
    const char* title, const char* body, const char* iconPath, int urgency, const char* tag
) {
    (void)iconPath;
    (void)urgency;
    (void)tag;
    NotifyNotification* notification = notify_notification_new(title, body, nullptr);
    notify_notification_set_icon_from_pixbuf(notification, gtk_window_get_icon(GTK_WINDOW(m_impl->_window)));

    NotifyUrgency libnotifyUrgency = NOTIFY_URGENCY_NORMAL;
    switch (urgency) {
        case 1: libnotifyUrgency = NOTIFY_URGENCY_LOW; break;
        case 2: libnotifyUrgency = NOTIFY_URGENCY_CRITICAL; break;
        case 3: libnotifyUrgency = NOTIFY_URGENCY_CRITICAL; break;
        default: libnotifyUrgency = NOTIFY_URGENCY_NORMAL; break;
    }
    notify_notification_set_urgency(notification, libnotifyUrgency);

    notify_notification_show(notification, nullptr);
    g_object_unref(G_OBJECT(notification));
}

void InfiniFrameWindow::BeginShowNotification(
    uint64_t operationId,
    const char* title, const char* body, const char* iconPath, int urgency, const char* tag,
    OperationCompletedCallback completion, void* completionContext
) {
    ShowNotificationWithOptions(title, body, iconPath, urgency, tag);

    if (completion) {
        completion(completionContext, operationId, 0, 0, nullptr);
    }
}

void InfiniFrameWindow::CancelNotification(uint64_t operationId, bool* canceled) {
    (void)operationId;
    if (canceled) *canceled = false;
}
