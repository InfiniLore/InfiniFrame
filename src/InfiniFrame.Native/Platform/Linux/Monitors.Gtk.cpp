#ifdef __linux__

#include "Platform/Linux/WindowImpl.Gtk.h"

unsigned int InfiniFrameWindow::GetScreenDpi() const {
    GdkScreen* screen = gtk_window_get_screen(GTK_WINDOW(m_impl->_window));
    gdouble dpi = gdk_screen_get_resolution(screen);
    if (dpi < 0)
        return 96;

    return static_cast<unsigned int>(dpi);
}

void InfiniFrameWindow::GetAllMonitors(const GetAllMonitorsCallback callback) const {
    if (callback == nullptr)
        return;

    GdkScreen* screen = gtk_window_get_screen(GTK_WINDOW(m_impl->_window));
    GdkDisplay* display = gdk_screen_get_display(screen);
    const int count = gdk_display_get_n_monitors(display);
    for (int i = 0; i < count; i++) {
        GdkMonitor* monitor = gdk_display_get_monitor(display, i);
        Monitor props = {};
        gdk_monitor_get_geometry(monitor, reinterpret_cast<GdkRectangle*>(&props.monitor));
        gdk_monitor_get_workarea(monitor, reinterpret_cast<GdkRectangle*>(&props.work));
        props.scale = gdk_monitor_get_scale_factor(monitor);

        if (!callback(&props))
            break;
    }
}

#endif
