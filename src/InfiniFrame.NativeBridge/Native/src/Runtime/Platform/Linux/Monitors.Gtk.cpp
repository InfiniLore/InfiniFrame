// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <gdk/x11/gdkx.h>

#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::GetAllMonitors(const GetAllMonitorsCallback Callback) const {
    if (Callback == nullptr) {
        return;
    }

    GdkDisplay* display = gtk_widget_get_display(GTK_WIDGET(m_impl->_window));
    GListModel* monitors = gdk_display_get_monitors(display);
    const guint monitorCount = g_list_model_get_n_items(monitors);

    for (guint i = 0; i < monitorCount; i++) {
        GdkMonitor* monitor = GDK_MONITOR(g_list_model_get_item(monitors, i));

        Monitor props = {};
        gdk_monitor_get_geometry(monitor, reinterpret_cast<GdkRectangle*>(&props.monitor));
        if (GDK_IS_X11_DISPLAY(display)) {
            // GTK4 removed gdk_monitor_get_workarea from core GDK; the X11 backend still
            // exposes it (deprecated 4.18, no replacement — blockers-resolution.md B5).
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
            gdk_x11_monitor_get_workarea(monitor, reinterpret_cast<GdkRectangle*>(&props.work));
G_GNUC_END_IGNORE_DEPRECATIONS
        } else {
            // Wayland has no work-area concept; the geometry is the best approximation
            // (matches the GTK3-on-Wayland behavior).
            gdk_monitor_get_geometry(monitor, reinterpret_cast<GdkRectangle*>(&props.work));
        }
        props.scale = gdk_monitor_get_scale_factor(monitor);

        g_object_unref(monitor);
        if (Callback(&props) == 0) {
            break;
        }
    }
}