#ifdef __linux__

#include "Platform/Linux/WindowImpl.Gtk.h"

void InfiniFrameWindow::GetFullScreen(bool* fullScreen) const {
    *fullScreen = m_impl->_isFullScreen;
}

void InfiniFrameWindow::Restore() {
    gtk_window_present(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::SetFullScreen(const bool fullScreen) {
    if (fullScreen)
        gtk_window_fullscreen(GTK_WINDOW(m_impl->_window));
    else
        gtk_window_unfullscreen(GTK_WINDOW(m_impl->_window));

    m_impl->_isFullScreen = fullScreen;
}

void InfiniFrameWindow::SetMinimized(const bool minimized) {
    if (minimized)
        gtk_window_iconify(GTK_WINDOW(m_impl->_window));
    else
        gtk_window_deiconify(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::SetMaximized(const bool maximized) {
    if (maximized)
        gtk_window_maximize(GTK_WINDOW(m_impl->_window));
    else
        gtk_window_unmaximize(GTK_WINDOW(m_impl->_window));
}

#endif
