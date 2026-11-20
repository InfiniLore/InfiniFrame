// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Embedded/Embedded.h"
#include "Runtime/Platform/Linux/Core/LinuxGraphicsEnvironment.Gtk.h"
#include "Runtime/Platform/Linux/WebKit/WebKit.Gtk.Internal.h"
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
extern void on_webview_load_changed(WebKitWebView* web_view, WebKitLoadEvent load_event, gpointer user_data);
extern gboolean on_webview_load_failed(
    WebKitWebView* web_view,
    WebKitLoadEvent load_event,
    gchar* failing_uri,
    GError* error,
    gpointer user_data
    );
extern void on_webview_process_terminated(
    WebKitWebView* web_view,
    WebKitWebProcessTerminationReason reason,
    gpointer user_data
    );
extern void on_webview_surface_layout(GdkSurface* surface, int width, int height, gpointer user_data);
extern gboolean on_webview_decide_policy(
    WebKitWebView* web_view,
    WebKitPolicyDecision* decision,
    WebKitPolicyDecisionType decision_type,
    gpointer user_data
    );

namespace {
    // GTK4 removed GtkWidget "size-allocate"; webview size diagnostics observe the
    // toplevel surface layout instead (same dimensions: the webview fills the window).
    void on_webview_realized(GtkWidget* widget, const gpointer self) {
        GdkSurface* surface = gtk_native_get_surface(GTK_NATIVE(widget));
        if (surface != nullptr)
            g_signal_connect(G_OBJECT(surface), "layout", G_CALLBACK(on_webview_surface_layout), self);
    }
}

void InfiniFrameWindow::Show(const bool isAlreadyShown) {
    (void)isAlreadyShown;

    // Early out if the webview has already been created.
    if (m_impl->_webview) {
        return;
    }

    // Prepare the WebKit and graphics subsystems. The remote-debugging environment
    // must be configured before the first WebKit object exists (process-wide WebKit inspector requirement).
    m_impl->configure_webkit_remote_debugging();
    infiniframe::linux_gtk::ConfigureGraphicsEnvironment();

    // Create per-window WebKit context and network session, then the web view bound to both
    // (construct-only properties; webkit_web_view_new_with_context is gone in 6.0).
    // A per-window network session preserves per-window TLS-error policy behavior and keeps
    // the GTK3 parity of shared default storage directories.
    m_impl->_webContext = webkit_web_context_new();
    WebKitNetworkSession* networkSession = webkit_network_session_new(nullptr, nullptr);

    m_impl->_webview = GTK_WIDGET(
        g_object_new(
            WEBKIT_TYPE_WEB_VIEW,
            "web-context", m_impl->_webContext,
            "network-session", networkSession,
            nullptr
            )
        );
    g_object_unref(networkSession);

    m_impl->set_webkit_settings();
    m_impl->AddCustomSchemeHandlers();
    if (m_impl->_webContext != nullptr)
        g_object_unref(m_impl->_webContext);
    m_impl->_webContext = nullptr;

    // Attach the web view to the GTK window and make it fill the available space.
    // RelayoutWindow places the webview (and the menu bar, if any) correctly.
    WebKitUserContentManager* contentManager = webkit_web_view_get_user_content_manager(
        WEBKIT_WEB_VIEW(m_impl->_webview));

    m_impl->RelayoutWindow();
    gtk_widget_set_hexpand(m_impl->_webview, TRUE);
    gtk_widget_set_vexpand(m_impl->_webview, TRUE);

    // Inject the core InfiniFrame bridge script that enables native<->web messaging.
    const auto& jsCode = Embedded::InfiniFrameJsUtf8();

    WebKitUserScript* script = webkit_user_script_new(
        jsCode.c_str(), WEBKIT_USER_CONTENT_INJECT_ALL_FRAMES, WEBKIT_USER_SCRIPT_INJECT_AT_DOCUMENT_START, nullptr,
        nullptr
        );

    webkit_user_content_manager_add_script(contentManager, script);
    webkit_user_script_unref(script);

    // Register the "infiniFrameInterop" message handler so the web content can
    // send structured messages back to the host via
    // window.webkit.messageHandlers.infiniFrameInterop.postMessage().
    m_impl->_webMessageSignalHandlerId = g_signal_connect(
        contentManager, "script-message-received::infiniFrameInterop", G_CALLBACK(gtk_webkit::HandleWebMessage),
        reinterpret_cast<void*>(m_impl->common._webMessageReceivedCallback)
        );
    webkit_user_content_manager_register_script_message_handler(contentManager, "infiniFrameInterop", nullptr);

    // Connect WebKit signals for load lifecycle, process termination, sizing,
    // and navigation policy decisions.
    g_signal_connect(G_OBJECT(m_impl->_webview), "load-changed", G_CALLBACK(on_webview_load_changed), this);
    g_signal_connect(G_OBJECT(m_impl->_webview), "load-failed", G_CALLBACK(on_webview_load_failed), this);
    g_signal_connect(
        G_OBJECT(m_impl->_webview), "web-process-terminated", G_CALLBACK(on_webview_process_terminated), this
        );
    g_signal_connect(G_OBJECT(m_impl->_window), "realize", G_CALLBACK(on_webview_realized), this);
    g_signal_connect(G_OBJECT(m_impl->_webview), "decide-policy", G_CALLBACK(on_webview_decide_policy), this);

    // Navigate to the initial content. Log an error if neither URL nor raw string
    // was provided (GTK4 forbids the old nested-loop error dialog).
    if (!m_impl->common._startUrl.empty()) {
        NavigateToUrl(const_cast<const char*>(m_impl->common._startUrl.c_str()));
    } else if (!m_impl->common._startString.empty()) {
        NavigateToString(const_cast<const char*>(m_impl->common._startString.c_str()));
    } else {
        g_warning("Neither StartUrl nor StartString was specified");
        return;
    }

    gtk_widget_set_visible(GTK_WIDGET(m_impl->_window), TRUE);
}

void InfiniFrameWindow::AttachWebView() {
    // On Linux, WebView is attached in Show()
}