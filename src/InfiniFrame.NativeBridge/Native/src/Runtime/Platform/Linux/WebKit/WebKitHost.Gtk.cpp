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
    WebKitWebView* web_view, WebKitLoadEvent load_event, gchar* failing_uri, GError* error, gpointer user_data
);
extern void on_webview_process_terminated(
    WebKitWebView* web_view, WebKitWebProcessTerminationReason reason, gpointer user_data
);
extern void on_webview_size_allocate(GtkWidget* widget, GtkAllocation* allocation, gpointer user_data);
extern gboolean on_webview_decide_policy(
    WebKitWebView* web_view, WebKitPolicyDecision* decision,
    WebKitPolicyDecisionType decision_type, gpointer user_data
);

void InfiniFrameWindow::Show(bool isAlreadyShown) {
    (void)isAlreadyShown;
    if (m_impl->_webview) {
        return;
    }

    m_impl->configure_webkit_remote_debugging();
    infiniframe::linux_gtk::ConfigureGraphicsEnvironment();

    m_impl->_webContext = webkit_web_context_new();

    m_impl->_webview = webkit_web_view_new_with_context(m_impl->_webContext);

    m_impl->set_webkit_settings();
    m_impl->AddCustomSchemeHandlers();
    if (m_impl->_webContext != nullptr)
        g_object_unref(m_impl->_webContext);
    m_impl->_webContext = nullptr;

    WebKitUserContentManager* contentManager = webkit_web_view_get_user_content_manager(WEBKIT_WEB_VIEW(m_impl->_webview));

    gtk_container_add(GTK_CONTAINER(m_impl->_window), m_impl->_webview);
    gtk_widget_set_hexpand(m_impl->_webview, TRUE);
    gtk_widget_set_vexpand(m_impl->_webview, TRUE);

    const auto& jsCode = Embedded::InfiniFrameJsUtf8();

    WebKitUserScript* script = webkit_user_script_new(
        jsCode.c_str(), WEBKIT_USER_CONTENT_INJECT_ALL_FRAMES, WEBKIT_USER_SCRIPT_INJECT_AT_DOCUMENT_START, nullptr,
        nullptr
    );

    webkit_user_content_manager_add_script(contentManager, script);
    webkit_user_script_unref(script);

    {
        static constexpr char kBrowserShortcutsJs[] =
            "(function(){"
            "if(window.__infiniframe_browserShortcutsEnabled===undefined)"
            "window.__infiniframe_browserShortcutsEnabled=true;"
            "document.addEventListener('keydown',function(e){"
            "if(window.__infiniframe_browserShortcutsEnabled)return;"
            "var c=e.ctrlKey||e.metaKey,s=e.shiftKey,k=e.key.toLowerCase();"
            "if(c&&(k==='t'||k==='n'||k==='w'||k==='r'||k==='p'||k==='u'||k==='j'|"
            "|k==='l'||k==='i'||k==='o'||k==='h'||(s&&k==='i'))){"
            "e.preventDefault();e.stopPropagation();return false;}"
            "if(k==='f11'){e.preventDefault();e.stopPropagation();return false;}"
            "},true);"
            "})();";
        WebKitUserScript* shortcutsScript = webkit_user_script_new(
            kBrowserShortcutsJs,
            WEBKIT_USER_CONTENT_INJECT_ALL_FRAMES,
            WEBKIT_USER_SCRIPT_INJECT_AT_DOCUMENT_START,
            nullptr, nullptr
        );
    webkit_user_content_manager_add_script(contentManager, shortcutsScript);
    webkit_user_script_unref(shortcutsScript);
    }

    {
        static constexpr char kContextMenuJs[] =
            "(function(){"
            "if(window.__infiniframe_contextMenuEnabled===undefined)"
            "window.__infiniframe_contextMenuEnabled=true;"
            "document.addEventListener('contextmenu',function(e){"
            "if(!window.__infiniframe_contextMenuEnabled){"
            "e.preventDefault();e.stopPropagation();return false;}"
            "},true);"
            "})();";
        WebKitUserScript* contextMenuScript = webkit_user_script_new(
            kContextMenuJs,
            WEBKIT_USER_CONTENT_INJECT_ALL_FRAMES,
            WEBKIT_USER_SCRIPT_INJECT_AT_DOCUMENT_START,
            nullptr, nullptr
        );
        webkit_user_content_manager_add_script(contentManager, contextMenuScript);
        webkit_user_script_unref(contextMenuScript);
    }

    {
        static constexpr char kZoomDisabledJs[] =
            "(function(){"
            "if(window.__infiniframe_zoomEnabled===undefined)"
            "window.__infiniframe_zoomEnabled=true;"
            "document.addEventListener('wheel',function(e){"
            "if(!window.__infiniframe_zoomEnabled&&(e.ctrlKey||e.metaKey)){"
            "e.preventDefault();e.stopPropagation();return false;}"
            "},true);"
            "document.addEventListener('keydown',function(e){"
            "if(!window.__infiniframe_zoomEnabled){"
            "var c=e.ctrlKey||e.metaKey,k=e.key;"
            "if((c&&(k==='+'||k==='-'||k==='='||k==='0'))||"
            "(k==='F5')||(c&&k==='0')){"
            "e.preventDefault();e.stopPropagation();return false;}"
            "}"
            "},true);"
            "})();";
        WebKitUserScript* zoomScript = webkit_user_script_new(
            kZoomDisabledJs,
            WEBKIT_USER_CONTENT_INJECT_ALL_FRAMES,
            WEBKIT_USER_SCRIPT_INJECT_AT_DOCUMENT_START,
            nullptr, nullptr
        );
        webkit_user_content_manager_add_script(contentManager, zoomScript);
        webkit_user_script_unref(zoomScript);
    }

    m_impl->_webMessageSignalHandlerId = g_signal_connect(
        contentManager, "script-message-received::infiniFrameInterop", G_CALLBACK(gtk_webkit::HandleWebMessage),
        reinterpret_cast<void*>(m_impl->_webMessageReceivedCallback)
    );
    webkit_user_content_manager_register_script_message_handler(contentManager, "infiniFrameInterop");

    {
        std::string initJs;
        if (!m_impl->_contextMenuEnabled)
            initJs += "window.__infiniframe_contextMenuEnabled=false;";
        if (!m_impl->_zoomEnabled)
            initJs += "window.__infiniframe_zoomEnabled=false;";
        if (!m_impl->_browserShortcutsEnabled)
            initJs += "window.__infiniframe_browserShortcutsEnabled=false;";
        if (!initJs.empty()) {
            webkit_web_view_evaluate_javascript(
                WEBKIT_WEB_VIEW(m_impl->_webview), initJs.c_str(), -1,
                nullptr, nullptr, nullptr, nullptr, nullptr
            );
        }
    }

    g_signal_connect(G_OBJECT(m_impl->_webview), "load-changed", G_CALLBACK(on_webview_load_changed), this);
    g_signal_connect(G_OBJECT(m_impl->_webview), "load-failed", G_CALLBACK(on_webview_load_failed), this);
    g_signal_connect(
        G_OBJECT(m_impl->_webview), "web-process-terminated", G_CALLBACK(on_webview_process_terminated), this
    );
    g_signal_connect(G_OBJECT(m_impl->_webview), "size-allocate", G_CALLBACK(on_webview_size_allocate), this);
    g_signal_connect(G_OBJECT(m_impl->_webview), "decide-policy", G_CALLBACK(on_webview_decide_policy), this);

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
