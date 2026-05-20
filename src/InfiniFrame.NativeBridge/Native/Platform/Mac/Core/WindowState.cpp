// Pure-C++ state getters and no-op setters for the macOS platform.
// These methods only read/write InfiniFrameWindowImpl fields (via ImplBase()) or return
// compile-time constants, so they compile as plain C++ without Objective-C++.
// Methods that directly access NSWindow/WKWebView live in the companion WindowState.Cocoa.mm.

#include "Public/InfiniFrameWindow.h"
#include "Public/InfiniFrameWindowImpl.h"
#include "Utils/StringCopy.h"

// ---------------------------------------------------------------------------
// Feature-flag getters — read InfiniFrameWindowImpl fields
// ---------------------------------------------------------------------------

void InfiniFrameWindow::GetTransparentEnabled(bool* enabled) const
{
    *enabled = false;
}

void InfiniFrameWindow::GetContextMenuEnabled(bool* enabled) const
{
    *enabled = ImplBase()->_contextMenuEnabled;
}

void InfiniFrameWindow::GetZoomEnabled(bool* enabled) const
{
    *enabled = ImplBase()->_zoomEnabled;
}

void InfiniFrameWindow::GetDevToolsEnabled(bool* enabled) const
{
    *enabled = ImplBase()->_devToolsEnabled;
}

void InfiniFrameWindow::GetGrantBrowserPermissions(bool* enabled) const
{
    *enabled = ImplBase()->_grantBrowserPermissions;
}

AutoString InfiniFrameWindow::GetUserAgent() const
{
    return AllocateStringCopy(ImplBase()->_userAgent);
}

void InfiniFrameWindow::GetMediaAutoplayEnabled(bool* enabled) const
{
    *enabled = true;
}

void InfiniFrameWindow::GetFileSystemAccessEnabled(bool* enabled) const
{
    *enabled = ImplBase()->_fileSystemAccessEnabled;
}

void InfiniFrameWindow::GetSmoothScrollingEnabled(bool* enabled) const
{
    *enabled = false;
}

void InfiniFrameWindow::GetWebSecurityEnabled(bool* enabled) const
{
    *enabled = ImplBase()->_webSecurityEnabled;
}

void InfiniFrameWindow::GetJavascriptClipboardAccessEnabled(bool* enabled) const
{
    *enabled = ImplBase()->_javascriptClipboardAccessEnabled;
}

void InfiniFrameWindow::GetMediaStreamEnabled(bool* enabled) const
{
    *enabled = ImplBase()->_mediaStreamEnabled;
}

void InfiniFrameWindow::GetIgnoreCertificateErrorsEnabled(bool* enabled) const
{
    *enabled = ImplBase()->_ignoreCertificateErrorsEnabled;
}

// ---------------------------------------------------------------------------
// Constant getters
// ---------------------------------------------------------------------------

unsigned int InfiniFrameWindow::GetScreenDpi() const
{
    return 72;
}

// ---------------------------------------------------------------------------
// String-field getters
// ---------------------------------------------------------------------------

AutoString InfiniFrameWindow::GetTitle() const
{
    return AllocateStringCopy(ImplBase()->_windowTitle);
}

AutoString InfiniFrameWindow::GetIconFileName() const
{
    return AllocateStringCopy(ImplBase()->_iconFileName);
}

// ---------------------------------------------------------------------------
// Composite state operation — delegates entirely to other InfiniFrameWindow methods
// ---------------------------------------------------------------------------

void InfiniFrameWindow::Restore()
{
    bool minimized = false;
    bool maximized = false;
    GetMinimized(&minimized);
    GetMaximized(&maximized);
    if (minimized) SetMinimized(false);
    if (maximized) SetMaximized(false);
}

// ---------------------------------------------------------------------------
// No-op setters — features not supported on macOS
// ---------------------------------------------------------------------------

void InfiniFrameWindow::SetTransparentEnabled(bool enabled)
{
    (void)enabled;
}

void InfiniFrameWindow::SetContextMenuEnabled(bool enabled)
{
    (void)enabled;
}

void InfiniFrameWindow::SetZoomEnabled(bool enabled)
{
    (void)enabled;
}
