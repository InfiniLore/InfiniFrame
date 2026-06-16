// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <string>

#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::Impl::configure_webkit_remote_debugging() const {
    if (_webContext) {
        webkit_web_context_set_remote_debugging_enabled(_webContext, _remoteDebuggingPort > 0);
        if (_remoteDebuggingPort > 0) {
            g_message("[InfiniFrame/Linux] Remote debugging enabled on port %d (per-context).", _remoteDebuggingPort);
        } else {
            g_message("[InfiniFrame/Linux] Remote debugging disabled.");
        }
    }
    // If _webContext is not yet created, the port is stored in _remoteDebuggingPort
    // and will be applied when the context is created in Show().
}
