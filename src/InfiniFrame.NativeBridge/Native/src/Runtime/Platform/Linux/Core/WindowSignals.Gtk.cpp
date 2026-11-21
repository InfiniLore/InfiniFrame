// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <chrono>
#include <string>

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

    const char* webkit_load_event_to_string(const WebKitLoadEvent event) {
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

    const char* webkit_termination_reason_to_string(const WebKitWebProcessTerminationReason reason) {
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

void InfiniFrameWindow::OnConfigureEvent(const int x, const int y, const int width, const int height) {
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

int InfiniFrameWindow::GetActiveEventState() const {
    return m_impl->_activeEventState;
}

void InfiniFrameWindow::SetActiveEventState(const int state) {
    m_impl->_activeEventState = state;
}

void InfiniFrameWindow::OnSurfaceLayout(const int width, const int height) {
    // GTK4 delivers size changes through GdkSurface::layout (no x/y). The position is
    // queried where the platform exposes it (X11); on Wayland it stays at the last known
    // value so no spurious move events are raised.
    if (g_getenv("INFINIFRAME_LINUX_WEBVIEW_DIAGNOSTICS") != nullptr) {
        g_message("[InfiniFrame/Linux] surface layout: %dx%d (last %dx%d)", width, height,
                  m_impl->_lastWidth, m_impl->_lastHeight);
    }
    int x = m_impl->_lastLeft;
    int y = m_impl->_lastTop;
    m_impl->TryGetPosition(&x, &y);
    OnConfigureEvent(x, y, width, height);

    // A SetSize request that landed during the first map/size negotiation was clobbered;
    // re-assert it once now that the toplevel is fully allocated (teardown work flake fix).
    if (m_impl->_pendingResizeWidth > 0 &&
        (width != m_impl->_pendingResizeWidth || height != m_impl->_pendingResizeHeight)) {
        const int targetWidth = m_impl->_pendingResizeWidth;
        const int targetHeight = m_impl->_pendingResizeHeight;
        m_impl->_pendingResizeWidth = 0;
        m_impl->_pendingResizeHeight = 0;
        if (g_getenv("INFINIFRAME_LINUX_WEBVIEW_DIAGNOSTICS") != nullptr) {
            g_message("[InfiniFrame/Linux] re-asserting pending SetSize: %dx%d", targetWidth, targetHeight);
        }
        gtk_window_set_default_size(GTK_WINDOW(m_impl->_window), targetWidth, targetHeight);
    } else if (width == m_impl->_pendingResizeWidth && height == m_impl->_pendingResizeHeight) {
        m_impl->_pendingResizeWidth = 0;
        m_impl->_pendingResizeHeight = 0;
    }
}

void InfiniFrameWindow::OnSurfaceRealized() {
    m_impl->ApplySizeConstraints();
    if (m_impl->_hasPendingMove) {
        m_impl->_hasPendingMove = false;
        m_impl->MoveTo(m_impl->_pendingMoveX, m_impl->_pendingMoveY);
    }
    if (!m_impl->common._iconFileName.empty())
        SetIconFile(m_impl->common._iconFileName.c_str());
}

void InfiniFrameWindow::OnWindowStateEvent(const GdkToplevelState newState) {
    // GTK emitted window-state-event repeatedly for the same logical state (e.g. a focus or geometry change arrives
    // right after a maximize, each carrying the MAXIMIZED bit). Gate every callback on an actual state transition so
    // a single SetMaximized/SetMinimized/restore raises exactly one event, matching the Win32 WM_SIZE handling.
    const bool isMaximized = (newState & GDK_TOPLEVEL_STATE_MAXIMIZED) != 0;
    // Unmapped does not mean minimized: GTK4 reports pre-map/unmap states through the same
    // toplevel notifications, so only the explicit MINIMIZED bit is trusted (migration requirement).
    const bool isMinimized = (newState & GDK_TOPLEVEL_STATE_MINIMIZED) != 0;

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

void on_surface_layout(GdkSurface* surface, const int width, const int height, const gpointer self) {
    (void)surface;
    infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "surface-layout", [&] {
            if (self == nullptr)
                return;
            auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
            instance->OnSurfaceLayout(width, height);
        });
}

void on_surface_layout(GdkSurface* surface, int width, int height, gpointer self);
void on_toplevel_state_notify(GObject* object, GParamSpec* pspec, gpointer self);

void on_window_realized(GtkWidget* widget, const gpointer self) {
    infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "realize", [&] {
            if (self == nullptr)
                return;

            GdkSurface* surface = gtk_native_get_surface(GTK_NATIVE(widget));
            if (surface == nullptr)
                return;

            // Connect surface-level signals here (they cannot exist before realization).
            // Disconnect first so unrealize/realize cycles never stack handlers.
            g_signal_handlers_disconnect_by_data(surface, self);
            g_signal_connect(G_OBJECT(surface), "layout", G_CALLBACK(on_surface_layout), self);
            g_signal_connect(G_OBJECT(surface), "notify::state", G_CALLBACK(on_toplevel_state_notify), self);

            auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
            instance->OnSurfaceRealized();
        });
}

void on_toplevel_state_notify(GObject* object, GParamSpec* pspec, const gpointer self) {
    (void)pspec;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "toplevel-state-notify", [&] {
            if (self == nullptr)
                return;
            if (!GDK_IS_TOPLEVEL(object))
                return;

            auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
            instance->OnWindowStateEvent(gdk_toplevel_get_state(GDK_TOPLEVEL(object)));
        });
}

gboolean on_window_close_request(GtkWindow* window, const gpointer self) {
    (void)window;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "close-request", FALSE, [&] -> gboolean {
            if (self == nullptr)
                return FALSE;

            auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
            const bool cancel = instance->InvokeClose();
            if (cancel)
                return TRUE;

            // The user (or default handler) accepted the close. Disconnect our webview signal handlers and stop any in-flight
            // load before the GTK4 destroy cascade disposes the webview, so none of our callbacks (FlushPendingWebMessages,
            // load/permission/context-menu handlers) can fire against a half-destroyed window. CloseWebView does NOT destroy the
            // webview itself. Explicit destruction from inside this handler triggers WebKit's web-process teardown
            // re-entrantly and aborts (SIGABRT); gtk_window_destroy disposes the webview implicitly once we return FALSE.
            instance->CloseWebView();
            return FALSE;
        });
}

void on_widget_destroyed(GtkWidget* widget, const gpointer self) {
    (void)widget;
    infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "destroy", [&] {
            if (self == nullptr)
                return;

            auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
            instance->MarkDestroyed();
            instance->InvokeClosed();
            instance->ScheduleTeardownCompletion();
        });
}

void on_window_active_notify(GObject* object, GParamSpec* pspec, const gpointer self) {
    (void)pspec;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "notify::is-active", [&] {
            if (self == nullptr)
                return;

            auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
            const bool active = gtk_window_is_active(GTK_WINDOW(object)) != FALSE;
            // Emit focus events only on real activation transitions: GTK4 can deliver an
            // initial notify::is-active at map time, which must not create a spurious
            // focus-out before the first focus-in (migration requirement).
            const int previous = instance->GetActiveEventState();
            const int current = active ? 1 : 0;
            if (previous == current)
                return;
            instance->SetActiveEventState(current);
            if (previous < 0 && !active)
                return;
            if (active)
                instance->InvokeFocusIn();
            else
                instance->InvokeFocusOut();
        });
}

gboolean on_drop_target_drop(
    GtkDropTarget* target,
    const GValue* value,
    const double x,
    const double y,
    const gpointer self
    ) {
    (void)target;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "drop-target-drop", FALSE, [&] -> gboolean {
            if (self == nullptr || value == nullptr)
                return FALSE;

            GdkFileList* fileList = static_cast<GdkFileList*>(g_value_get_boxed(value));
            if (fileList == nullptr)
                return FALSE;

            GSList* files = gdk_file_list_get_files(fileList);
            std::vector<std::string> paths;
            for (GSList* item = files; item != nullptr; item = item->next) {
                GFile* file = G_FILE(item->data);
                gchar* filename = g_file_get_path(file);
                if (filename) {
                    paths.push_back(filename);
                    g_free(filename);
                }
            }
            g_slist_free_full(files, g_object_unref);

            std::vector<const char*> autoStrings;
            autoStrings.reserve(paths.size());
            for (const auto& p : paths) {
                autoStrings.push_back(p.c_str());
            }

            auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
            instance->InvokeFileDropped(
                autoStrings.data(), static_cast<int>(autoStrings.size()),
                static_cast<int>(x), static_cast<int>(y));
            return TRUE;
        });
}

gboolean on_webview_context_menu(
    WebKitWebView* web_view,
    WebKitContextMenu* context_menu,
    WebKitHitTestResult* hit_test_result,
    const gboolean triggered_with_keyboard,
    const gpointer self
    ) {
    (void)web_view;
    (void)context_menu;
    (void)hit_test_result;
    (void)triggered_with_keyboard;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "context-menu", TRUE, [&] -> gboolean {
            if (self == nullptr)
                return TRUE;

            auto* instance = reinterpret_cast<InfiniFrameWindow*>(self);
            bool contextMenuEnabled = false;
            instance->GetContextMenuEnabled(&contextMenuEnabled);
            return !contextMenuEnabled;
        });
}

gboolean on_permission_request(WebKitWebView* web_view, WebKitPermissionRequest* request, const gpointer user_data) {
    (void)web_view;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "permission-request", TRUE, [&] -> gboolean {
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

void on_webview_load_changed(WebKitWebView* web_view, const WebKitLoadEvent load_event, const gpointer user_data) {
    infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "load-changed", [&] {
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
                instance->ShowAfterInitialNavigation();
                instance->CompleteNavigationAndSignalReady(0, true, 0, nullptr);
            }
        });
}

gboolean on_webview_load_failed(
    WebKitWebView* web_view,
    const WebKitLoadEvent load_event,
    gchar* failing_uri,
    GError* error,
    const gpointer user_data
    ) {
    (void)web_view;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "load-failed", FALSE, [&] -> gboolean {
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
            instance->ShowAfterInitialNavigation();
            instance->CompleteNavigationAndSignalReady(
                0, false, error ? error->code : 0,
                error ? error->message : "WebKit navigation failed"
                );

            if (!linux_webview_diagnostics_enabled())
                return FALSE;

            g_warning(
                "[InfiniFrame/Linux] WebKit load-failed: event=%s uri=%s error=%s",
                webkit_load_event_to_string(load_event),
                failing_uri ? failing_uri : "<null>", error ? error->message : "<null>"
                );
            return FALSE;
        });
}

void on_webview_process_terminated(
    WebKitWebView* web_view,
    const WebKitWebProcessTerminationReason reason,
    const gpointer user_data
    ) {
    (void)web_view;
    infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "web-process-terminated", [&] {
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
                "[InfiniFrame/Linux] WebKit web process terminated: reason=%s",
                webkit_termination_reason_to_string(reason)
                );
        });
}

void on_webview_surface_layout(GdkSurface* surface, const int width, const int height, const gpointer user_data) {
    (void)surface;
    (void)user_data;
    infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "webview-surface-layout", [&] {
            if (!linux_webview_diagnostics_enabled())
                return;

            g_message(
                "[InfiniFrame/Linux] WebView surface layout: %dx%d", width, height
                );
        });
}

gboolean on_webview_decide_policy(
    WebKitWebView* web_view,
    WebKitPolicyDecision* decision,
    const WebKitPolicyDecisionType decision_type,
    const gpointer user_data
    ) {
    (void)web_view;
    return infiniframe::linux_gtk::RunGtkCallbackNoThrow(
        "decide-policy", FALSE, [&] -> gboolean {
            if (user_data == nullptr || decision == nullptr)
                return FALSE;

            if (decision_type != WEBKIT_POLICY_DECISION_TYPE_NAVIGATION_ACTION)
                return FALSE;

            auto* instance = reinterpret_cast<InfiniFrameWindow*>(user_data);
            NavigationStartingCallback callback = instance->GetNavigationStartingCallback();
            if (callback == nullptr)
                return FALSE;

            WebKitNavigationPolicyDecision* navDecision = WEBKIT_NAVIGATION_POLICY_DECISION(decision);
            WebKitNavigationAction* action = webkit_navigation_policy_decision_get_navigation_action(navDecision);
            WebKitNavigationType navType = webkit_navigation_action_get_navigation_type(action);
            WebKitURIRequest* request = webkit_navigation_action_get_request(action);
            const gchar* uri = webkit_uri_request_get_uri(request);
            if (uri == nullptr)
                return FALSE;
            bool isUserInitiated = (navType == WEBKIT_NAVIGATION_TYPE_LINK_CLICKED ||
                navType == WEBKIT_NAVIGATION_TYPE_FORM_SUBMITTED);
            bool isRedirect = (navType == WEBKIT_NAVIGATION_TYPE_OTHER);
            bool isMainFrame = true;

            int cancel = callback(
                static_cast<const char*>(uri), isUserInitiated ? 1 : 0, isRedirect ? 1 : 0, isMainFrame ? 1 : 0);
            if (cancel) {
                webkit_policy_decision_ignore(decision);
                return TRUE;
            }
            return FALSE;
        });
}
