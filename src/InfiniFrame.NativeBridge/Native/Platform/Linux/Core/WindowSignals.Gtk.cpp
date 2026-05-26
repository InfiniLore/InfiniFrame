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

void InfiniFrameWindow::OnWidgetDestroyed() {
    // In the normal close path, CloseWebView() was already called from on_widget_deleted, so
    // _webview is nullptr here and this block is a no-op.
    // In the forced-destroy path (C++ destructor calling gtk_widget_destroy directly), the webview
    // may still be alive. Tear it down explicitly now — before GtkContainer's built-in cleanup
    // cascade runs — to prevent the main-loop deadlock described in on_widget_deleted's comment.
    if (m_impl->_webview != nullptr) {
        CloseWebView();
    }

    // Release our ownership ref taken by g_object_ref_sink() in ConfigureInitialWindow.
    // g_object_run_dispose() holds its own ref during dispose, so this drops the count from
    // 2→1 (not to 0), meaning finalize is deferred until g_object_run_dispose() returns safely.
    if (m_impl->_window != nullptr) {
        g_object_unref(m_impl->_window);
        m_impl->_window = nullptr;
    }

    // Fire the Closed callback BEFORE unblocking WaitForExit(). The native instance must not be
    // freed by TryDestroyNativeInstanceNoThrow while this callback is still executing on the GTK
    // worker thread.
    InvokeClosed();

    {
        std::lock_guard<std::mutex> lk(m_impl->_destroyedMutex);
        m_impl->_windowDestroyed = true;
    }
    m_impl->_destroyedCv.notify_all();

    // Quit the nested GMainLoop that WaitForExit() started when called on the GTK thread.
    if (m_impl->_exitLoop != nullptr)
        g_main_loop_quit(m_impl->_exitLoop);
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
    if (instance->InvokeClose())
        return TRUE;
    // Tear the WebView down before the window destroy cascade. When the webview is left as a
    // GtkContainer child and GTK destroys it implicitly, WebKit's web-process termination is
    // asynchronous and needs GLib main-loop dispatch to complete — but the main loop is blocked
    // inside the cascade, causing a deadlock and a 100% hang. Explicit pre-cascade teardown here
    // avoids that: by the time GTK's cascade runs the window has no children and completes cleanly.
    instance->CloseWebView();
    return FALSE;
}

void on_widget_destroyed(GtkWidget* widget, const gpointer self) {
    auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
    instance->OnWidgetDestroyed();
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
    if (linux_webview_diagnostics_enabled()) {
        const char* uri = webkit_web_view_get_uri(web_view);
        g_message(
            "[InfiniFrame/Linux] WebKit load-changed: event=%s uri=%s", webkit_load_event_to_string(load_event),
            uri ? uri : "<null>"
        );
    }

    if (load_event == WEBKIT_LOAD_FINISHED) {
        auto* instance = reinterpret_cast<InfiniFrameWindow*>(user_data);
        instance->FlushPendingWebMessages();
    }
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
