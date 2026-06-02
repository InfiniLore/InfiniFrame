// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::GetAllMonitors(const GetAllMonitorsCallback callback) const {
    if (callback == nullptr) {
        return;
    }

    GdkScreen* screen = gtk_window_get_screen(GTK_WINDOW(m_impl->_window));
    GdkDisplay* display = gdk_screen_get_display(screen);

    const int MonitorCount = gdk_display_get_n_monitors(display);

    for (int i = 0; i < MonitorCount; i++) {
        GdkMonitor* monitor = gdk_display_get_monitor(display, i);

        Monitor props = {};
        gdk_monitor_get_geometry(monitor, reinterpret_cast<GdkRectangle*>(&props.monitor));
        gdk_monitor_get_workarea(monitor, reinterpret_cast<GdkRectangle*>(&props.work));
        props.scale = gdk_monitor_get_scale_factor(monitor);

        if (callback(&props) == 0) {
            break;
        }
    }
}
