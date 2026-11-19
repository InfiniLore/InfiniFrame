// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <libnotify/notify.h>

#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
#include "Runtime/Internal/Interop/Exports/ExportStringHelpers.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using namespace infiniframe::exports;

namespace {
    // GTK4 removed gtk_window_get_icon. The window icon file (kept by SetIconFile) is
    // loaded into a pixbuf for the notification icon instead.
    GdkPixbuf* LoadNotificationIcon(const std::string& iconFileName) {
        if (iconFileName.empty())
            return nullptr;
        return gdk_pixbuf_new_from_file(iconFileName.c_str(), nullptr);
    }

    void ApplyNotificationIcon(NotifyNotification* notification, const std::string& iconFileName) {
        GdkPixbuf* icon = LoadNotificationIcon(iconFileName);
        notify_notification_set_icon_from_pixbuf(notification, icon);
        if (icon != nullptr)
            g_object_unref(G_OBJECT(icon));
    }
}

void InfiniFrameWindow::ShowNotification(const char* title, const char* message) {
    NotifyNotification* notification = notify_notification_new(title, message, nullptr);
    ApplyNotificationIcon(notification, m_impl->common._iconFileName);
    notify_notification_show(notification, nullptr);
    g_object_unref(G_OBJECT(notification));
}

void InfiniFrameWindow::ShowNotificationWithOptions(
    const char* title,
    const char* body,
    const char* iconPath,
    const int urgency,
    const char* tag
    ) {
    (void)iconPath;
    (void)urgency;
    (void)tag;
    NotifyNotification* notification = notify_notification_new(title, body, nullptr);
    ApplyNotificationIcon(notification, m_impl->common._iconFileName);

    NotifyUrgency libnotifyUrgency = NOTIFY_URGENCY_NORMAL;
    switch (urgency) {
        case 1:
            libnotifyUrgency = NOTIFY_URGENCY_LOW;
            break;
        case 2:
            libnotifyUrgency = NOTIFY_URGENCY_CRITICAL;
            break;
        case 3:
            libnotifyUrgency = NOTIFY_URGENCY_CRITICAL;
            break;
        default:
            libnotifyUrgency = NOTIFY_URGENCY_NORMAL;
            break;
    }
    notify_notification_set_urgency(notification, libnotifyUrgency);

    notify_notification_show(notification, nullptr);
    g_object_unref(G_OBJECT(notification));
}

void InfiniFrameWindow::BeginShowNotification(
    const uint64_t operationId,
    const char* title,
    const char* body,
    const char* iconPath,
    const int urgency,
    const char* tag,
    const OperationCompletedCallback completion,
    void* completionContext
    ) {
    ShowNotificationWithOptions(title, body, iconPath, urgency, tag);

    if (completion) {
        completion(completionContext, operationId, 0, 0, nullptr);
    }
}

void InfiniFrameWindow::CancelNotification(const uint64_t operationId, bool* canceled) {
    (void)operationId;
    if (canceled)
        *canceled = false;
}