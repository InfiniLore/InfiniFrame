// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <atomic>
#include <signal.h>
#include <unistd.h>
#include <mutex>
#include <webkit2/webkit2.h>

#include "Embedded/Embedded.h"
#include "Platform/Linux/WebKit/WebKit.Gtk.Internal.h"
#include "Platform/Linux/Window.Gtk.Internal.h"
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
    // Armed by InfiniFrame_ArmWebKitTeardown() once the last InfiniFrameWindow is destroyed.
    // After that point all GTK/WebKit activity is complete; any subsequent SIGABRT is from
    // WebKit's own background cleanup and should be suppressed rather than propagated.
    std::atomic<bool> g_webkit_teardown_active{false};
} // namespace

// Called from InfiniFrameWindow::~InfiniFrameWindow when the last window instance is destroyed.
void InfiniFrame_ArmWebKitTeardown() noexcept {
    g_webkit_teardown_active.store(true, std::memory_order_relaxed);
}

void InfiniFrameWindow::Show(bool isAlreadyShown) {
    static std::mutex showMutex;
    std::lock_guard<std::mutex> showGuard(showMutex);

    if (m_impl->_webview) {
        return;
    }

    // Install a SIGABRT bypass as a safety net against WebKit abort() calls.
    // The process-global WebKit context uses a permanent static reference (never unref'd) so GLib
    // never finalizes it and the known process-exit abort() path no longer fires. This handler
    // guards against any other unforeseen abort() originating from WebKit internals: it only calls
    // _exit(0) once g_webkit_teardown_active is armed (after the last window is destroyed),
    // so real crashes during active test execution still propagate normally.
    static bool sigabrtHandlerInstalled = false;
    if (!sigabrtHandlerInstalled) {
        sigabrtHandlerInstalled = true;
        struct sigaction sa{};
        sa.sa_handler = [](int) noexcept {
            if (g_webkit_teardown_active.load(std::memory_order_relaxed)) {
                _exit(0);
            }
            signal(SIGABRT, SIG_DFL);
            raise(SIGABRT);
        };
        sigemptyset(&sa.sa_mask);
        sa.sa_flags = 0;
        sigaction(SIGABRT, &sa, nullptr);
    }

    // Flush pending GLib events to allow WebKit's previous web process to finish shutting down
    // before creating a new WebView. Without this, if the previous web process is still
    // terminating asynchronously, webkit_web_view_new_with_context() hits an internal assertion
    // and calls abort(). This call is safe here because Show() always runs on the GTK worker thread.
    while (g_main_context_pending(nullptr))
        g_main_context_iteration(nullptr, FALSE);

    struct sigaction oldAction{};
    sigaction(SIGCHLD, nullptr, &oldAction);
    m_impl->_webview = webkit_web_view_new_with_context(m_impl->_webContext);
    auto* contentManager = webkit_web_view_get_user_content_manager(WEBKIT_WEB_VIEW(m_impl->_webview));

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
