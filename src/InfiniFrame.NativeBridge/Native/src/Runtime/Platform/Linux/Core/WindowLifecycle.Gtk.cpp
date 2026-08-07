// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    void on_webview_finalized(gpointer userData, GObject* object) {
        (void)object;
        if (userData != nullptr)
            static_cast<InfiniFrameWindow*>(userData)->NotifyWebViewFinalized();
    }
}

void InfiniFrameWindow::MarkDestroyed() {
    {
        std::lock_guard lock(m_impl->_lifecycleMutex);
        m_impl->_destroyed = true;
    }
    m_impl->_windowDestroyed = true;
    m_impl->_window = nullptr;
    m_impl->_lifecycleClosed.notify_all();
}

bool InfiniFrameWindow::IsDestroyed() const {
    std::lock_guard lock(m_impl->_lifecycleMutex);
    return m_impl->_destroyed;
}

void InfiniFrameWindow::WaitUntilDestroyed() {
    std::unique_lock lock(m_impl->_lifecycleMutex);
    m_impl->_lifecycleClosed.wait(lock, [&] { return m_impl->_destroyed; });
}

void InfiniFrameWindow::Center() {
    gint windowWidth, windowHeight;
    gtk_window_get_size(GTK_WINDOW(m_impl->_window), &windowWidth, &windowHeight);

    GdkRectangle screen = {};

    GdkDisplay* display = gdk_display_get_default();
    if (display == nullptr) {
        GtkWidget* dialog = gtk_message_dialog_new(
            nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
            "gdk_display_get_default() returned NULL"
        );
        gtk_dialog_run(GTK_DIALOG(dialog));
        gtk_widget_destroy(dialog);
        return;
    }

    GdkMonitor* monitor = gdk_display_get_primary_monitor(display);
    if (monitor == nullptr) {
        monitor = gdk_display_get_monitor(display, 0);
        if (monitor == nullptr) {
            GtkWidget* dialog = gtk_message_dialog_new(
                nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE,
                "gdk_display_get_primary_monitor() returned NULL"
            );
            gtk_dialog_run(GTK_DIALOG(dialog));
            gtk_widget_destroy(dialog);
            return;
        }
    }

    gdk_monitor_get_geometry(monitor, &screen);

    gtk_window_move(GTK_WINDOW(m_impl->_window), (screen.width - windowWidth) / 2, (screen.height - windowHeight) / 2);
}

void InfiniFrameWindow::ClearBrowserAutoFill() {
    // ClearBrowserAutoFill is Windows-only (WebView2).
    // The managed layer logs a warning and returns on non-Windows platforms,
    // but the native export must exist for all platforms.
}

void InfiniFrameWindow::Close() {
    gtk_window_close(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::WaitForExit() {
    WaitUntilDestroyed();
}

void InfiniFrameWindow::CloseWebView() {
    if (m_impl->_webviewClosed)
        return;
    m_impl->_webviewClosed = true;

    GtkWidget* webview = m_impl->_webview;
    if (webview == nullptr) {
        m_impl->_webviewFinalized = true;
        return;
    }

    // Disconnect every signal whose user_data is this instance so our callbacks can't fire after the window starts
    // tearing down. The webview itself is destroyed implicitly by GTK when the parent window is destroyed.
    // Explicit destruction here (gtk_widget_destroy, terminate_web_process, pumping events) triggers WebKit's web
    // process cleanup from inside a GTK signal handler, which causes SIGABRT on libwebkit2gtk-4.1.
    // Do not unref _webContext here either. CloseWebView runs from the GTK delete-event path, and releasing the
    // context from this re-entrant teardown path can trigger the same WebKitGTK abort.
    WebKitUserContentManager* contentManager =
        webkit_web_view_get_user_content_manager(WEBKIT_WEB_VIEW(webview));
    if (contentManager != nullptr) {
        if (m_impl->_webMessageSignalHandlerId != 0) {
            g_signal_handler_disconnect(contentManager, m_impl->_webMessageSignalHandlerId);
            m_impl->_webMessageSignalHandlerId = 0;
        }
        webkit_user_content_manager_unregister_script_message_handler(contentManager, "infiniFrameInterop");
    }

    // The window's destroy signal runs before GtkContainer's default handler destroys its WebKit child. Retain this
    // instance until the child's final weak notification and do not report backend teardown while WebKit still owns it.
    g_object_weak_ref(G_OBJECT(webview), on_webview_finalized, this);
    g_signal_handlers_disconnect_by_data(webview, this);
    webkit_web_view_stop_loading(WEBKIT_WEB_VIEW(webview));
}

void InfiniFrameWindow::NotifyWebViewFinalized() {
    m_impl->_webview = nullptr;
    m_impl->_webviewFinalized = true;
    ScheduleTeardownCompletion();
}

void InfiniFrameWindow::ScheduleTeardownCompletion() {
    if (!m_impl->_windowDestroyed || !m_impl->_webviewFinalized || m_impl->_teardownCompletionScheduled)
        return;

    m_impl->_teardownCompletionScheduled = true;
    CompleteOperationsForClose();
    CompleteNavigationForClose();
    CompleteDialogsForClose();
    if (!infiniframe::linux_gtk::ui_thread::InvokeIdle([this] { SignalTeardown(); }))
        SignalTeardown();
}
