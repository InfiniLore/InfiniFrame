#ifdef __APPLE__

#include "../Window.Cocoa.Internal.h"

InfiniFrameDialog* InfiniFrameWindow::GetDialog() const
{
    return m_impl->_dialog.get();
}

void InfiniFrameWindow::AddCustomSchemeName(const AutoStringConst scheme)
{
    if (scheme)
        m_impl->_customSchemeNames.emplace_back(scheme);
}

void InfiniFrameWindow::SetClosingCallback(const ClosingCallback callback)
{
    m_impl->_closingCallback = callback;
}

void InfiniFrameWindow::SetClosedCallback(const ClosedCallback callback)
{
    m_impl->_closedCallback = callback;
}

void InfiniFrameWindow::SetFocusInCallback(const FocusInCallback callback)
{
    m_impl->_focusInCallback = callback;
}

void InfiniFrameWindow::SetFocusOutCallback(const FocusOutCallback callback)
{
    m_impl->_focusOutCallback = callback;
}

void InfiniFrameWindow::SetMovedCallback(const MovedCallback callback)
{
    m_impl->_movedCallback = callback;
}

void InfiniFrameWindow::SetResizedCallback(const ResizedCallback callback)
{
    m_impl->_resizedCallback = callback;
}

void InfiniFrameWindow::SetMaximizedCallback(const MaximizedCallback callback)
{
    m_impl->_maximizedCallback = callback;
}

void InfiniFrameWindow::SetRestoredCallback(const RestoredCallback callback)
{
    m_impl->_restoredCallback = callback;
}

void InfiniFrameWindow::SetMinimizedCallback(const MinimizedCallback callback)
{
    m_impl->_minimizedCallback = callback;
}

[[nodiscard]] bool InfiniFrameWindow::InvokeClose() const noexcept
{
    if (m_impl->_closingCallback)
        return m_impl->_closingCallback();
    return false;
}

void InfiniFrameWindow::InvokeClosed() const noexcept
{
    if (m_impl->_closedCallback)
        m_impl->_closedCallback();
}

void InfiniFrameWindow::InvokeFocusIn() const noexcept
{
    if (m_impl->_focusInCallback)
        m_impl->_focusInCallback();
}

void InfiniFrameWindow::InvokeFocusOut() const noexcept
{
    if (m_impl->_focusOutCallback)
        m_impl->_focusOutCallback();
}

void InfiniFrameWindow::InvokeMove(int x, int y) const noexcept
{
    if (m_impl->_movedCallback)
        m_impl->_movedCallback(x, y);
}

void InfiniFrameWindow::InvokeResize(int width, int height) const noexcept
{
    if (m_impl->_resizedCallback)
        m_impl->_resizedCallback(width, height);
}

void InfiniFrameWindow::InvokeMaximized() const noexcept
{
    if (m_impl->_maximizedCallback)
        m_impl->_maximizedCallback();
}

void InfiniFrameWindow::InvokeRestored() const noexcept
{
    if (m_impl->_restoredCallback)
        m_impl->_restoredCallback();
}

void InfiniFrameWindow::InvokeMinimized() const noexcept
{
    if (m_impl->_minimizedCallback)
        m_impl->_minimizedCallback();
}

#endif
