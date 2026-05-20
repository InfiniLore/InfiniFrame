// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
InfiniFrameDialog* InfiniFrameWindow::GetDialog() const {
    return m_impl->_dialog.get();
}

void InfiniFrameWindow::AddCustomSchemeName(const AutoStringConst scheme) {
    if (scheme == nullptr) {
        return;
    }

    m_impl->_customSchemeNames.emplace_back(scheme);
}

void InfiniFrameWindow::SetClosingCallback(const ClosingCallback callback) {
    m_impl->_closingCallback = callback;
}

void InfiniFrameWindow::SetClosedCallback(const ClosedCallback callback) {
    m_impl->_closedCallback = callback;
}

void InfiniFrameWindow::SetFocusInCallback(const FocusInCallback callback) {
    m_impl->_focusInCallback = callback;
}

void InfiniFrameWindow::SetFocusOutCallback(const FocusOutCallback callback) {
    m_impl->_focusOutCallback = callback;
}

void InfiniFrameWindow::SetMovedCallback(const MovedCallback callback) {
    m_impl->_movedCallback = callback;
}

void InfiniFrameWindow::SetResizedCallback(const ResizedCallback callback) {
    m_impl->_resizedCallback = callback;
}

void InfiniFrameWindow::SetMaximizedCallback(const MaximizedCallback callback) {
    m_impl->_maximizedCallback = callback;
}

void InfiniFrameWindow::SetRestoredCallback(const RestoredCallback callback) {
    m_impl->_restoredCallback = callback;
}

void InfiniFrameWindow::SetMinimizedCallback(const MinimizedCallback callback) {
    m_impl->_minimizedCallback = callback;
}

[[nodiscard]] bool InfiniFrameWindow::InvokeClose() const noexcept {
    if (m_impl->_closingCallback == nullptr) {
        return false;
    }

    return m_impl->_closingCallback();
}

void InfiniFrameWindow::InvokeClosed() const noexcept {
    if (m_impl->_closedCallback == nullptr) {
        return;
    }

    m_impl->_closedCallback();
}

void InfiniFrameWindow::InvokeFocusIn() const noexcept {
    if (m_impl->_focusInCallback == nullptr) {
        return;
    }

    m_impl->_focusInCallback();
}

void InfiniFrameWindow::InvokeFocusOut() const noexcept {
    if (m_impl->_focusOutCallback == nullptr) {
        return;
    }

    m_impl->_focusOutCallback();
}

void InfiniFrameWindow::InvokeMove(int x, int y) const noexcept {
    if (m_impl->_movedCallback == nullptr) {
        return;
    }

    m_impl->_movedCallback(x, y);
}

void InfiniFrameWindow::InvokeResize(int width, int height) const noexcept {
    if (m_impl->_resizedCallback == nullptr) {
        return;
    }

    m_impl->_resizedCallback(width, height);
}

void InfiniFrameWindow::InvokeMaximized() const noexcept {
    if (m_impl->_maximizedCallback == nullptr) {
        return;
    }

    m_impl->_maximizedCallback();
}

void InfiniFrameWindow::InvokeRestored() const noexcept {
    if (m_impl->_restoredCallback == nullptr) {
        return;
    }

    m_impl->_restoredCallback();
}

void InfiniFrameWindow::InvokeMinimized() const noexcept {
    if (m_impl->_minimizedCallback == nullptr) {
        return;
    }

    m_impl->_minimizedCallback();
}