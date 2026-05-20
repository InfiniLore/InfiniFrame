// macOS lifecycle methods that require no Obj-C runtime.
// Cocoa-dependent lifecycle operations (Center, Close, WaitForExit, ShowNotification)
// live in the companion WindowLifecycle.Cocoa.mm.

#include "Public/InfiniFrameWindow.h"

void InfiniFrameWindow::ClearBrowserAutoFill()
{
    // Not implemented on macOS.
}

void InfiniFrameWindow::CloseWebView()
{
    // Not implemented on macOS.
}
