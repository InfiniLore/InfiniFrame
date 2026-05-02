#ifdef __APPLE__

#include "Platform/Mac/WindowImpl.Cocoa.h"

void InfiniFrameWindow::GetFullScreen(bool* fullScreen) const
{
    *fullScreen = ([m_impl->_window styleMask] & NSWindowStyleMaskFullScreen) != 0;
}

void InfiniFrameWindow::GetMaximized(bool* isMaximized) const
{
    bool isFullScreen = false;
    GetFullScreen(&isFullScreen);
    if (isFullScreen)
    {
        *isMaximized = false;
        return;
    }

    *isMaximized = [m_impl->_window isZoomed];
}

void InfiniFrameWindow::GetMinimized(bool* isMinimized) const
{
    *isMinimized = [m_impl->_window isMiniaturized];
}

void InfiniFrameWindow::Restore()
{
    bool minimized = false;
    bool maximized = false;
    GetMinimized(&minimized);
    GetMaximized(&maximized);

    if (minimized)
        SetMinimized(false);
    if (maximized)
        SetMaximized(false);
}

void InfiniFrameWindow::SetFullScreen(bool fullScreen)
{
    bool isFullScreen = ([m_impl->_window styleMask] & NSWindowStyleMaskFullScreen) != 0;
    if (fullScreen != isFullScreen)
        [m_impl->_window toggleFullScreen: nil];
}

void InfiniFrameWindow::SetMinimized(bool minimized)
{
    if (m_impl->_window.isMiniaturized == minimized)
        return;

    if (minimized)
        [m_impl->_window miniaturize: nullptr];
    else
        [m_impl->_window deminiaturize: nullptr];
}

void InfiniFrameWindow::SetMaximized(bool maximized)
{
    if (maximized)
    {
        NSRect window = [m_impl->_window frame];
        m_impl->_preMaximizedWidth = window.size.width;
        m_impl->_preMaximizedHeight = window.size.height;
        m_impl->_preMaximizedXPosition = window.origin.x;
        m_impl->_preMaximizedYPosition = window.origin.y;

        NSRect screen = [[m_impl->_window screen] visibleFrame];
        [m_impl->_window setFrame: NSMakeRect(screen.origin.x, screen.origin.y, screen.size.width, screen.size.height)
                          display: YES];
    }
    else if (m_impl->_preMaximizedWidth > 0 && m_impl->_preMaximizedHeight > 0)
    {
        [m_impl->_window setFrame: NSMakeRect(
                                      m_impl->_preMaximizedXPosition,
                                      m_impl->_preMaximizedYPosition,
                                      m_impl->_preMaximizedWidth,
                                      m_impl->_preMaximizedHeight)
                          display: YES];
    }
}

#endif
