// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <webkit2/webkit2.h>

#include "Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    bool linux_webview_diagnostics_enabled() {
        const char* value = g_getenv("INFINIFRAME_LINUX_WEBVIEW_DIAGNOSTICS");
        return value != nullptr && value[0] != '\0' && g_strcmp0(value, "0") != 0;
    }

    const char* webkit_load_event_to_string(WebKitLoadEvent event) {
        switch (event) {
            case WEBKIT_LOAD_STARTED:
                return "started";
            case WEBKIT_LOAD_REDIRECTED:
                return "redirected";
            case WEBKIT_LOAD_COMMITTED:
                return "committed";
            case WEBKIT_LOAD_FINISHED:
                return "finished";
            default:
                return "unknown";
        }
    }

    const char* webkit_termination_reason_to_string(WebKitWebProcessTerminationReason reason) {
        switch (reason) {
            case WEBKIT_WEB_PROCESS_CRASHED:
                return "crashed";
            case WEBKIT_WEB_PROCESS_EXCEEDED_MEMORY_LIMIT:
                return "exceeded-memory-limit";
            case WEBKIT_WEB_PROCESS_TERMINATED_BY_API:
                return "terminated-by-api";
            default:
                return "unknown";
        }
    }
} // namespace

void InfiniFrameWindow::OnConfigureEvent(int x, int y, int width, int height) {
    if (m_impl->_lastLeft != x || m_impl->_lastTop != y) {
        InvokeMove(x, y);
        m_impl->_lastLeft = x;
        m_impl->_lastTop = y;
    }

    if (m_impl->_lastHeight != height || m_impl->_lastWidth != width) {
        InvokeResize(width, height);
        m_impl->_lastWidth = width;
        m_impl->_lastHeight = height;
    }
}

void InfiniFrameWindow::OnWindowStateEvent(GdkWindowState newState) {
    if (newState & GDK_WINDOW_STATE_MAXIMIZED) {
        InvokeMaximized();
    } else if ((newState & GDK_WINDOW_STATE_ICONIFIED) || !gtk_widget_get_mapped(m_impl->_window)) {
        InvokeMinimized();
    } else if (!(newState & GDK_WINDOW_STATE_MAXIMIZED) && !(newState & GDK_WINDOW_STATE_ICONIFIED)) {
        InvokeRestored();
    }
}

gboolean on_configure_event(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    if (event->type == GDK_CONFIGURE) {
        auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
        instance->OnConfigureEvent(
            event->configure.x, event->configure.y, event->configure.width, event->configure.height
        );
    }
    return FALSE;
}

gboolean on_window_state_event(GtkWidget* widget, GdkEventWindowState* event, const gpointer self) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    instance->OnWindowStateEvent(event->new_window_state);
    return TRUE;
}

gboolean on_widget_deleted(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    return instance->InvokeClose();
}

void on_widget_destroyed(GtkWidget* widget, const gpointer self) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    instance->InvokeClosed();
}

gboolean on_focus_in_event(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    instance->InvokeFocusIn();
    return FALSE;
}

gboolean on_focus_out_event(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    instance->InvokeFocusOut();
    return FALSE;
}

gboolean on_webview_context_menu(
    WebKitWebView* web_view,
    GtkWidget* default_menu,
    WebKitHitTestResult* hit_test_result,
    gboolean triggered_with_keyboard,
    const gpointer self
) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    bool contextMenuEnabled = false;
    instance->GetContextMenuEnabled(&contextMenuEnabled);
    return !contextMenuEnabled;
}

gboolean on_permission_request(WebKitWebView* web_view, WebKitPermissionRequest* request, gpointer user_data) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(user_data);
    bool grant = false;
    instance->GetGrantBrowserPermissions(&grant);
    if (grant)
        webkit_permission_request_allow(request);
    else
        webkit_permission_request_deny(request);
    return TRUE;
}

void on_webview_load_changed(WebKitWebView* web_view, WebKitLoadEvent load_event, gpointer user_data) {
    if (!linux_webview_diagnostics_enabled())
        return;

    const char* uri = webkit_web_view_get_uri(web_view);
    g_message(
        "[InfiniFrame/Linux] WebKit load-changed: event=%s uri=%s", webkit_load_event_to_string(load_event),
        uri ? uri : "<null>"
    );
}

gboolean on_webview_load_failed(
    WebKitWebView* web_view, WebKitLoadEvent load_event, gchar* failing_uri, GError* error, gpointer user_data
) {
    if (!linux_webview_diagnostics_enabled())
        return FALSE;

    g_warning(
        "[InfiniFrame/Linux] WebKit load-failed: event=%s uri=%s error=%s", webkit_load_event_to_string(load_event),
        failing_uri ? failing_uri : "<null>", error ? error->message : "<null>"
    );
    return FALSE;
}

void on_webview_process_terminated(
    WebKitWebView* web_view, WebKitWebProcessTerminationReason reason, gpointer user_data
) {
    g_warning(
        "[InfiniFrame/Linux] WebKit web process terminated: reason=%s", webkit_termination_reason_to_string(reason)
    );
}

void on_webview_size_allocate(GtkWidget* widget, GtkAllocation* allocation, gpointer user_data) {
    if (!linux_webview_diagnostics_enabled())
        return;

    g_message(
        "[InfiniFrame/Linux] WebView size-allocate: %dx%d", allocation ? allocation->width : -1,
        allocation ? allocation->height : -1
    );
}
