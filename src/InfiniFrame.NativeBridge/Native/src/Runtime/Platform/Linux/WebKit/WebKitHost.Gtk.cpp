// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <webkit2/webkit2.h>

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
extern void on_webview_size_allocate(GtkWidget* widget, GtkAllocation* allocation, gpointer user_data);
extern gboolean on_webview_decide_policy(
    WebKitWebView* web_view,
    WebKitPolicyDecision* decision,
    WebKitPolicyDecisionType decision_type,
    gpointer user_data
    );

void InfiniFrameWindow::Show(const bool isAlreadyShown) {
    (void)isAlreadyShown;

    // Early out if the webview has already been created.
    if (m_impl->_webview) {
        return;
    }

    // Prepare the WebKit and graphics subsystems.
    m_impl->configure_webkit_remote_debugging();
    infiniframe::linux_gtk::ConfigureGraphicsEnvironment();

    // Create a new WebKit context and web view, then configure settings and
    // custom scheme handlers.  The context is no longer needed after the view
    // takes ownership.
    m_impl->_webContext = webkit_web_context_new();

    m_impl->_webview = webkit_web_view_new_with_context(m_impl->_webContext);

    m_impl->set_webkit_settings();
    m_impl->AddCustomSchemeHandlers();
    if (m_impl->_webContext != nullptr)
        g_object_unref(m_impl->_webContext);
    m_impl->_webContext = nullptr;

    // Attach the web view to the GTK window and make it fill the available space.
    WebKitUserContentManager* contentManager = webkit_web_view_get_user_content_manager(
        WEBKIT_WEB_VIEW(m_impl->_webview));

    gtk_container_add(GTK_CONTAINER(m_impl->_window), m_impl->_webview);
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
        reinterpret_cast<void*>(m_impl->_webMessageReceivedCallback)
        );
    webkit_user_content_manager_register_script_message_handler(contentManager, "infiniFrameInterop");

    // Connect WebKit signals for load lifecycle, process termination, sizing,
    // and navigation policy decisions.
    g_signal_connect(G_OBJECT(m_impl->_webview), "load-changed", G_CALLBACK(on_webview_load_changed), this);
    g_signal_connect(G_OBJECT(m_impl->_webview), "load-failed", G_CALLBACK(on_webview_load_failed), this);
    g_signal_connect(
        G_OBJECT(m_impl->_webview), "web-process-terminated", G_CALLBACK(on_webview_process_terminated), this
        );
    g_signal_connect(G_OBJECT(m_impl->_webview), "size-allocate", G_CALLBACK(on_webview_size_allocate), this);
    g_signal_connect(G_OBJECT(m_impl->_webview), "decide-policy", G_CALLBACK(on_webview_decide_policy), this);

    // Navigate to the initial content.  Show an error dialog if neither URL
    // nor raw string was provided.
    if (!m_impl->_startUrl.empty()) {
        NavigateToUrl(const_cast<const char*>(m_impl->_startUrl.c_str()));
    } else if (!m_impl->_startString.empty()) {
        NavigateToString(const_cast<const char*>(m_impl->_startString.c_str()));
    } else {
        GtkWidget* dialog = gtk_message_dialog_new(
            nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
            "Neither StartUrl nor StartString was specified"
            );
        gtk_dialog_run(GTK_DIALOG(dialog));
        gtk_widget_destroy(dialog);
        return;
    }

    gtk_widget_show_all(m_impl->_window);
}

void InfiniFrameWindow::AttachWebView() {
    // On Linux, WebView is attached in Show()
}