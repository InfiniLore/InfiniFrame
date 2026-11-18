// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <gdk/x11/gdkx.h>

#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
#include "Runtime/Internal/Application/InfiniFrameApplication.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    void on_webview_finalized(const gpointer userData, GObject* object) {
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
    m_impl->_lifecycleClosed.wait(
        lock, [&] {
            return m_impl->_destroyed;
        });
}

void InfiniFrameWindow::Center() {
    int windowWidth = 0;
    int windowHeight = 0;
    GetSize(&windowWidth, &windowHeight);

    GdkDisplay* display = gdk_display_get_default();
    if (display == nullptr) {
        // GTK4 removed gtk_dialog_run; a nested-loop error dialog is not permitted.
        g_warning("gdk_display_get_default() returned NULL; cannot center the window.");
        return;
    }

    GdkRectangle screen = {};

    GdkSurface* surface = gtk_native_get_surface(GTK_NATIVE(m_impl->_window));
    GdkMonitor* monitor = surface != nullptr
        ? gdk_display_get_monitor_at_surface(display, surface)
        : nullptr;
    if (monitor == nullptr) {
        // GTK4 removed gdk_display_get_primary_monitor; the X11 backend still exposes it
        // (deprecated 4.18, no core replacement — see blockers-resolution.md B5).
        if (GDK_IS_X11_DISPLAY(display)) {
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
            monitor = gdk_x11_display_get_primary_monitor(GDK_X11_DISPLAY(display));
G_GNUC_END_IGNORE_DEPRECATIONS
        }
    }
    if (monitor == nullptr) {
        GListModel* monitors = gdk_display_get_monitors(display);
        if (g_list_model_get_n_items(monitors) > 0) {
            GdkMonitor* first = GDK_MONITOR(g_list_model_get_item(monitors, 0));
            gdk_monitor_get_geometry(first, &screen);
            g_object_unref(first);
            m_impl->MoveTo((screen.width - windowWidth) / 2, (screen.height - windowHeight) / 2);
            return;
        }
        g_warning("No display monitor found for centering.");
        return;
    }

    gdk_monitor_get_geometry(monitor, &screen);

    m_impl->MoveTo((screen.width - windowWidth) / 2, (screen.height - windowHeight) / 2);
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
        webkit_user_content_manager_unregister_script_message_handler(contentManager, "infiniFrameInterop", nullptr);
    }

    // The window's destroy signal runs before GtkContainer's default handler destroys its WebKit child. Retain this
    // instance until the child's final weak notification and do not report backend teardown while WebKit still owns it.
    g_object_weak_ref(G_OBJECT(webview), on_webview_finalized, this);
    g_signal_handlers_disconnect_by_data(webview, this);
    webkit_web_view_stop_loading(WEBKIT_WEB_VIEW(webview));
}

void InfiniFrameWindow::NotifyWebViewFinalized() {
    m_impl->_webview = nullptr;
    {
        std::lock_guard lock(m_impl->_lifecycleMutex);
        m_impl->_webviewFinalized = true;
    }
    m_impl->_lifecycleClosed.notify_all();
    ScheduleTeardownCompletion();
}

void InfiniFrameWindow::ScheduleTeardownCompletion() {
    if (!m_impl->_windowDestroyed || !m_impl->_webviewFinalized || m_impl->_teardownCompletionScheduled)
        return;

    m_impl->_teardownCompletionScheduled = true;
    CompleteOperationsForClose();
    CompleteNavigationForClose();
    CompleteDialogsForClose();
    if (!infiniframe::linux_gtk::ui_thread::InvokeIdle(
        [this] {
            SignalTeardown();
            if (!m_impl->_applicationNotified && _application != nullptr) {
                m_impl->_applicationNotified = true;
                _application->NotifyWindowClosed(this);
            }
            {
                std::lock_guard lock(m_impl->_lifecycleMutex);
                m_impl->_teardownCompleted = true;
            }
            m_impl->_lifecycleClosed.notify_all();
        })) {
        SignalTeardown();
        if (!m_impl->_applicationNotified && _application != nullptr) {
            m_impl->_applicationNotified = true;
            _application->NotifyWindowClosed(this);
        }
        {
            std::lock_guard lock(m_impl->_lifecycleMutex);
            m_impl->_teardownCompleted = true;
        }
        m_impl->_lifecycleClosed.notify_all();
    }
}