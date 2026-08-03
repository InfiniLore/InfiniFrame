// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <chrono>
#include <string>

#include <webkit2/webkit2.h>

#include "Runtime/Platform/Linux/Core/GtkCallbackGuard.h"
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
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

    int64_t unix_timestamp_milliseconds_utc() {
        return std::chrono::duration_cast<std::chrono::milliseconds>(
                   std::chrono::system_clock::now().time_since_epoch()
               )
            .count();
    }
} 

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
    // GTK emits window-state-event repeatedly for the same logical state (e.g. a focus or geometry change arrives
    // right after a maximize, each carrying the MAXIMIZED bit). Gate every callback on an actual state transition so
    // a single SetMaximized/SetMinimized/restore raises exactly one event, matching the Win32 WM_SIZE handling.
    const bool isMaximized = (newState & GDK_WINDOW_STATE_MAXIMIZED) != 0;
    const bool isMinimized = (newState & GDK_WINDOW_STATE_ICONIFIED) || !gtk_widget_get_mapped(m_impl->_window);

    if (isMaximized) {
        if (!m_impl->_maximized) {
            m_impl->_maximized = true;
            m_impl->_minimized = false;
            InvokeMaximized();
        }
    } else if (isMinimized) {
        if (!m_impl->_minimized) {
            m_impl->_maximized = false;
            m_impl->_minimized = true;
            InvokeMinimized();
        }
    } else if (m_impl->_maximized || m_impl->_minimized) {
        m_impl->_maximized = false;
        m_impl->_minimized = false;
        InvokeRestored();
    }
}

gboolean on_configure_event(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    (void)widget;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow("configure-event", FALSE, [&] -> gboolean {
        if (event != nullptr && event->type == GDK_CONFIGURE && self != nullptr) {
            auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
            instance->OnConfigureEvent(
                event->configure.x, event->configure.y, event->configure.width, event->configure.height
            );
        }
        return FALSE;
    });
}

gboolean on_window_state_event(GtkWidget* widget, GdkEventWindowState* event, const gpointer self) {
    (void)widget;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow("window-state-event", FALSE, [&] -> gboolean {
        if (event == nullptr || self == nullptr)
            return FALSE;

        auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
        instance->OnWindowStateEvent(event->new_window_state);
        return TRUE;
    });
}

gboolean on_widget_deleted(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    (void)widget;
    (void)event;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow("delete-event", FALSE, [&] -> gboolean {
        if (self == nullptr)
            return FALSE;

        auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
        const bool cancel = instance->InvokeClose();
        if (cancel)
            return TRUE;

        // The user (or default handler) accepted the close. Disconnect our webview signal handlers and stop any in-flight
        // load before the GtkContainer destroy cascade disposes the webview, so none of our callbacks (FlushPendingWebMessages,
        // load/permission/context-menu handlers) can fire against a half-destroyed window. CloseWebView does NOT destroy the
        // webview itself. Explicit destruction from inside this signal handler triggers WebKit's web-process teardown
        // re-entrantly and aborts (SIGABRT); GtkContainer disposes the webview implicitly once we return FALSE.
        instance->CloseWebView();
        return FALSE;
    });
}

void on_widget_destroyed(GtkWidget* widget, const gpointer self) {
    (void)widget;
    infiniframe::linux_gtk::RunGtkCallbackNoThrow("destroy", [&] {
        if (self == nullptr)
            return;

        auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
        instance->MarkDestroyed();
        instance->InvokeClosed();
        instance->ScheduleTeardownCompletion();
    });
}

gboolean on_focus_in_event(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    (void)widget;
    (void)event;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow("focus-in-event", FALSE, [&] -> gboolean {
        if (self == nullptr)
            return FALSE;

        auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
        instance->InvokeFocusIn();
        return FALSE;
    });
}

gboolean on_focus_out_event(GtkWidget* widget, GdkEvent* event, const gpointer self) {
    (void)widget;
    (void)event;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow("focus-out-event", FALSE, [&] -> gboolean {
        if (self == nullptr)
            return FALSE;

        auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
        instance->InvokeFocusOut();
        return FALSE;
    });
}

gboolean on_webview_context_menu(
    WebKitWebView* web_view,
    GtkWidget* default_menu,
    WebKitHitTestResult* hit_test_result,
    gboolean triggered_with_keyboard,
    const gpointer self
) {
    (void)web_view;
    (void)default_menu;
    (void)hit_test_result;
    (void)triggered_with_keyboard;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow("context-menu", TRUE, [&] -> gboolean {
        if (self == nullptr)
            return TRUE;

        auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
        bool contextMenuEnabled = false;
        instance->GetContextMenuEnabled(&contextMenuEnabled);
        return !contextMenuEnabled;
    });
}

gboolean on_permission_request(WebKitWebView* web_view, WebKitPermissionRequest* request, gpointer user_data) {
    (void)web_view;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow("permission-request", TRUE, [&] -> gboolean {
        if (request == nullptr)
            return TRUE;
        if (user_data == nullptr) {
            webkit_permission_request_deny(request);
            return TRUE;
        }

        auto* instance = reinterpret_cast<InfiniFrameWindow*>(user_data);
        bool grant = false;
        instance->GetGrantBrowserPermissions(&grant);
        if (grant)
            webkit_permission_request_allow(request);
        else
            webkit_permission_request_deny(request);
        return TRUE;
    });
}

void on_webview_load_changed(WebKitWebView* web_view, WebKitLoadEvent load_event, gpointer user_data) {
    infiniframe::linux_gtk::RunGtkCallbackNoThrow("load-changed", [&] {
        if (web_view == nullptr || user_data == nullptr)
            return;

        auto* instance = reinterpret_cast<InfiniFrameWindow*>(user_data);
        const char* uri = webkit_web_view_get_uri(web_view);
        std::string payload = std::string{"{\"loadEvent\":\""} + webkit_load_event_to_string(load_event) + "\"}";
        instance->InvokeDebugEvent(
            "Navigation",
            webkit_load_event_to_string(load_event),
            "Info",
            uri,
            0,
            unix_timestamp_milliseconds_utc(),
            payload.c_str()
        );

        if (linux_webview_diagnostics_enabled()) {
            g_message(
                "[InfiniFrame/Linux] WebKit load-changed: event=%s uri=%s", webkit_load_event_to_string(load_event),
                uri ? uri : "<null>"
            );
        }

        if (load_event == WEBKIT_LOAD_FINISHED) {
            instance->FlushPendingWebMessages();
            instance->CompleteNavigationAndSignalReady(0, true, 0, nullptr);
        }
    });
}

gboolean on_webview_load_failed(
    WebKitWebView* web_view, WebKitLoadEvent load_event, gchar* failing_uri, GError* error, gpointer user_data
) {
    (void)web_view;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow("load-failed", FALSE, [&] -> gboolean {
        if (user_data == nullptr)
            return FALSE;

        auto* instance = reinterpret_cast<InfiniFrameWindow*>(user_data);
        std::string payload = std::string{"{\"loadEvent\":\""} + webkit_load_event_to_string(load_event) + "\"}";
        instance->InvokeDebugEvent(
            "ScriptError",
            error ? error->message : "WebKit load failed",
            "Error",
            failing_uri,
            error ? error->code : 0,
            unix_timestamp_milliseconds_utc(),
            payload.c_str()
        );
        instance->CompleteNavigationAndSignalReady(
            0, false, error ? error->code : 0,
            error ? error->message : "WebKit navigation failed"
        );

        if (!linux_webview_diagnostics_enabled())
            return FALSE;

        g_warning(
            "[InfiniFrame/Linux] WebKit load-failed: event=%s uri=%s error=%s", webkit_load_event_to_string(load_event),
            failing_uri ? failing_uri : "<null>", error ? error->message : "<null>"
        );
        return FALSE;
    });
}

void on_webview_process_terminated(
    WebKitWebView* web_view, WebKitWebProcessTerminationReason reason, gpointer user_data
) {
    (void)web_view;
    infiniframe::linux_gtk::RunGtkCallbackNoThrow("web-process-terminated", [&] {
        if (user_data == nullptr)
            return;

        auto* instance = reinterpret_cast<InfiniFrameWindow*>(user_data);
        std::string payload =
            std::string{"{\"terminationReason\":\""} + webkit_termination_reason_to_string(reason) + "\"}";
        instance->InvokeDebugEvent(
            "Process",
            "WebKit web process terminated",
            "Error",
            nullptr,
            static_cast<int>(reason),
            unix_timestamp_milliseconds_utc(),
            payload.c_str()
        );

        g_warning(
            "[InfiniFrame/Linux] WebKit web process terminated: reason=%s", webkit_termination_reason_to_string(reason)
        );
    });
}

void on_webview_size_allocate(GtkWidget* widget, GtkAllocation* allocation, gpointer user_data) {
    (void)widget;
    (void)user_data;
    infiniframe::linux_gtk::RunGtkCallbackNoThrow("size-allocate", [&] {
        if (!linux_webview_diagnostics_enabled())
            return;

        g_message(
            "[InfiniFrame/Linux] WebView size-allocate: %dx%d", allocation ? allocation->width : -1,
            allocation ? allocation->height : -1
        );
    });
}

gboolean on_webview_decide_policy(
    WebKitWebView* web_view, WebKitPolicyDecision* decision,
    WebKitPolicyDecisionType decision_type, gpointer user_data
) {
    (void)web_view;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow("decide-policy", FALSE, [&] -> gboolean {
        if (user_data == nullptr || decision == nullptr)
            return FALSE;

        if (decision_type != WEBKIT_POLICY_DECISION_TYPE_NAVIGATION_ACTION)
            return FALSE;

        auto* instance = reinterpret_cast<InfiniFrameWindow*>(user_data);
        NavigationStartingCallback callback = instance->GetNavigationStartingCallback();
        if (callback == nullptr)
            return FALSE;

        WebKitNavigationPolicyRequest* navRequest = WEBKIT_NAVIGATION_POLICY_REQUEST(decision);
        const gchar* uri = webkit_navigation_policy_request_get_uri(navRequest);
        if (uri == nullptr)
            return FALSE;

        WebKitNavigationAction* action = webkit_navigation_policy_get_navigation_action(navRequest);
        WebKitNavigationType navType = webkit_navigation_action_get_navigation_action_type(action);
        bool isUserInitiated = (navType == WEBKIT_NAVIGATION_TYPE_LINK_CLICKED ||
                                navType == WEBKIT_NAVIGATION_TYPE_FORM_SUBMITTED);
        bool isRedirect = (navType == WEBKIT_NAVIGATION_TYPE_OTHER);
        bool isMainFrame = true;

        int cancel = callback((AutoString)uri, isUserInitiated ? 1 : 0, isRedirect ? 1 : 0, isMainFrame ? 1 : 0);
        if (cancel) {
            webkit_policy_decision_ignore(decision);
            return TRUE;
        }
        return FALSE;
    });
}
