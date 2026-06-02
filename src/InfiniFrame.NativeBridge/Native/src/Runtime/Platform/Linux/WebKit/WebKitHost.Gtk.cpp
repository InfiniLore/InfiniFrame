// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <atomic>
#include <cstdlib>
#include <signal.h>
#include <unistd.h>
#include <webkit2/webkit2.h>

#include "Embedded/Embedded.h"
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

namespace {
    // libwebkit2gtk-4.1 registers an atexit() handler when its globals are initialized.That handler walks the default
    // WebKitWebContext singleton and unrefs its members. On Ubuntu 22.04 (WebKit 2.50.4) one of those member destructors 
    // aborts with SIGABRT (process exits with 134) any time a UI process has hosted a WebKitWebView. We can't avoid 
    // creating a webview, and we can't reach into WebKit's globals to tidy them up, so we register a competing atexit 
    // handler AFTER WebKit has initialised its own. atexit() runs handlers in LIFO order, so ours fires first and _exit()s
    // the process, skipping WebKit's crashing cleanup.
    //
    // _exit() bypasses remaining atexit handlers and stdio buffer flushing. The .NET test host writes its TRX/HTML reports 
    // synchronously before returning from main(), and stderr/stdout are line-buffered when not attached to a terminal,
    // so no test output is lost.
    void webkit_atexit_bypass() noexcept {
        std::_Exit(0);
    }

    void register_webkit_atexit_bypass_once() noexcept {
        static std::atomic<bool> registered{false};
        bool expected = false;
        if (registered.compare_exchange_strong(expected, true, std::memory_order_acq_rel))
            std::atexit(webkit_atexit_bypass);
    }
} // namespace

void InfiniFrameWindow::Show(bool isAlreadyShown) {
    if (m_impl->_webview) {
        return;
    }

    struct sigaction oldAction{};
    sigaction(SIGCHLD, nullptr, &oldAction);
    WebKitUserContentManager* contentManager = webkit_user_content_manager_new();
    // Now that libwebkit's globals are guaranteed to be initialised (and its own atexit handler is registered), install
    // ours so it runs first.
    register_webkit_atexit_bypass_once();
    m_impl->_webview = webkit_web_view_new_with_user_content_manager(contentManager);

    m_impl->set_webkit_settings();

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

    g_signal_connect(
        contentManager, "script-message-received::infiniFrameInterop", G_CALLBACK(gtk_webkit::HandleWebMessage),
        reinterpret_cast<void*>(m_impl->_webMessageReceivedCallback)
    );
    webkit_user_content_manager_register_script_message_handler(contentManager, "infiniFrameInterop");

    // webkit_web_view_new_with_user_content_manager keeps its own reference; drop ours so the content manager doesn't
    // leak past the webview's lifetime.
    g_object_unref(contentManager);

    g_signal_connect(G_OBJECT(m_impl->_webview), "load-changed", G_CALLBACK(on_webview_load_changed), this);
    g_signal_connect(G_OBJECT(m_impl->_webview), "load-failed", G_CALLBACK(on_webview_load_failed), this);
    g_signal_connect(
        G_OBJECT(m_impl->_webview), "web-process-terminated", G_CALLBACK(on_webview_process_terminated), this
    );
    g_signal_connect(G_OBJECT(m_impl->_webview), "size-allocate", G_CALLBACK(on_webview_size_allocate), this);

    if (!m_impl->_startUrl.empty()) {
        NavigateToUrl(const_cast<AutoString>(m_impl->_startUrl.c_str()));
    } else if (!m_impl->_startString.empty()) {
        NavigateToString(const_cast<AutoString>(m_impl->_startString.c_str()));
    } else {
        GtkWidget* dialog = gtk_message_dialog_new(
            nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
            "Neither StartUrl nor StartString was specified"
        );
        gtk_dialog_run(GTK_DIALOG(dialog));
        gtk_widget_destroy(dialog);
        sigaction(SIGCHLD, &oldAction, nullptr);
        return;
    }
    sigaction(SIGCHLD, &oldAction, nullptr);

    gtk_widget_show_all(m_impl->_window);
}

void InfiniFrameWindow::AttachWebView() {
    // On Linux, WebView is attached in Show()
}
