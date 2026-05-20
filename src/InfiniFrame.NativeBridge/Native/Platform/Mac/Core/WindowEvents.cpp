// Pure-C++ implementations of callback registration and event invocation for the macOS platform.
// These methods access only InfiniFrameWindowImpl fields (no Cocoa types) via ImplBase(), so
// they compile as plain C++ without Objective-C++.
// GetAllMonitors (needs NSScreen) lives in the companion WindowEvents.Cocoa.mm.

#include "Public/InfiniFrameWindow.h"
#include "Public/InfiniFrameWindowImpl.h"

InfiniFrameDialog* InfiniFrameWindow::GetDialog() const
{
    return ImplBase()->_dialog.get();
}

void InfiniFrameWindow::AddCustomSchemeName(const AutoStringConst scheme)
{
    if (scheme)
        ImplBase()->_customSchemeNames.emplace_back(scheme);
}

void InfiniFrameWindow::SetClosingCallback(const ClosingCallback callback)
{
    ImplBase()->_closingCallback = callback;
}

void InfiniFrameWindow::SetClosedCallback(const ClosedCallback callback)
{
    ImplBase()->_closedCallback = callback;
}

void InfiniFrameWindow::SetFocusInCallback(const FocusInCallback callback)
{
    ImplBase()->_focusInCallback = callback;
}

void InfiniFrameWindow::SetFocusOutCallback(const FocusOutCallback callback)
{
    ImplBase()->_focusOutCallback = callback;
}

void InfiniFrameWindow::SetMovedCallback(const MovedCallback callback)
{
    ImplBase()->_movedCallback = callback;
}

void InfiniFrameWindow::SetResizedCallback(const ResizedCallback callback)
{
    ImplBase()->_resizedCallback = callback;
}

void InfiniFrameWindow::SetMaximizedCallback(const MaximizedCallback callback)
{
    ImplBase()->_maximizedCallback = callback;
}

void InfiniFrameWindow::SetRestoredCallback(const RestoredCallback callback)
{
    ImplBase()->_restoredCallback = callback;
}

void InfiniFrameWindow::SetMinimizedCallback(const MinimizedCallback callback)
{
    ImplBase()->_minimizedCallback = callback;
}

[[nodiscard]] bool InfiniFrameWindow::InvokeClose() const noexcept
{
    if (ImplBase()->_closingCallback)
        return ImplBase()->_closingCallback();
    return false;
}

void InfiniFrameWindow::InvokeClosed() const noexcept
{
    if (ImplBase()->_closedCallback)
        ImplBase()->_closedCallback();
}

void InfiniFrameWindow::InvokeFocusIn() const noexcept
{
    if (ImplBase()->_focusInCallback)
        ImplBase()->_focusInCallback();
}

void InfiniFrameWindow::InvokeFocusOut() const noexcept
{
    if (ImplBase()->_focusOutCallback)
        ImplBase()->_focusOutCallback();
}

void InfiniFrameWindow::InvokeMove(int x, int y) const noexcept
{
    if (ImplBase()->_movedCallback)
        ImplBase()->_movedCallback(x, y);
}

void InfiniFrameWindow::InvokeResize(int width, int height) const noexcept
{
    if (ImplBase()->_resizedCallback)
        ImplBase()->_resizedCallback(width, height);
}

void InfiniFrameWindow::InvokeMaximized() const noexcept
{
    if (ImplBase()->_maximizedCallback)
        ImplBase()->_maximizedCallback();
}

void InfiniFrameWindow::InvokeRestored() const noexcept
{
    if (ImplBase()->_restoredCallback)
        ImplBase()->_restoredCallback();
}

void InfiniFrameWindow::InvokeMinimized() const noexcept
{
    if (ImplBase()->_minimizedCallback)
        ImplBase()->_minimizedCallback();
}
