// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <condition_variable>
#include <functional>
#include <mutex>

#include "Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    struct GtkSyncInvokeState {
        std::mutex mutex;
        std::condition_variable condition;
        bool completed = false;
        std::function<void()> action;
    };

    gboolean run_gtk_sync_invoke(gpointer data) {
        auto* state = reinterpret_cast<GtkSyncInvokeState*>(data);
        state->action();
        {
            std::lock_guard<std::mutex> lock(state->mutex);
            state->completed = true;
        }
        state->condition.notify_one();
        return G_SOURCE_REMOVE;
    }

    void invoke_on_gtk_thread_and_wait(const std::function<void()>& action) {
        GtkSyncInvokeState state;
        state.action = action;
        g_main_context_invoke(nullptr, run_gtk_sync_invoke, &state);
        std::unique_lock<std::mutex> lock(state.mutex);
        state.condition.wait(lock, [&state] { return state.completed; });
    }
} // namespace

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
    // TODO
}

void InfiniFrameWindow::Close() {
    if (m_impl->_window == nullptr || !GTK_IS_WINDOW(m_impl->_window))
        return;

    if (!m_impl->IsGtkThread()) {
        invoke_on_gtk_thread_and_wait([this] {
            if (m_impl->_window != nullptr && GTK_IS_WINDOW(m_impl->_window))
                gtk_window_close(GTK_WINDOW(m_impl->_window));
        });
        return;
    }

    gtk_window_close(GTK_WINDOW(m_impl->_window));
}

void InfiniFrameWindow::WaitForExit() {
    if (m_impl->IsGtkThread()) {
        // Called on the GTK worker thread — this happens when C# dispatches WaitForExit() via
        // Invoke(), which routes through the native UiDispatcher to the GTK thread.
        // Blocking on _destroyedCv here would freeze the GTK event loop so close events could
        // never be processed.  Instead run a nested GMainLoop that keeps dispatching GTK events
        // until OnWidgetDestroyed() quits it.
        if (m_impl->_windowDestroyed)
            return;
        GMainLoop* loop = g_main_loop_new(nullptr, FALSE);
        m_impl->_exitLoop = loop;
        g_main_loop_run(loop);   // processes GTK events; returns when OnWidgetDestroyed calls g_main_loop_quit
        m_impl->_exitLoop = nullptr;
        g_main_loop_unref(loop);
        return;
    }
    // Called from a non-GTK thread: block on CV until OnWidgetDestroyed notifies it.
    std::unique_lock<std::mutex> lk(m_impl->_destroyedMutex);
    m_impl->_destroyedCv.wait(lk, [this] { return m_impl->_windowDestroyed; });
}

void InfiniFrameWindow::CloseWebView() {
    if (!m_impl->IsGtkThread()) {
        invoke_on_gtk_thread_and_wait([this] { CloseWebView(); });
        return;
    }

    GtkWidget* webview = m_impl->_webview;
    if (webview == nullptr)
        return;

    // Clear the pointer first — this is the idempotency guard; OnWidgetDestroyed() will skip
    // the webview if it sees nullptr here.
    m_impl->_webview = nullptr;
    // _webContext is the process-global static singleton; we do not own its reference.
    m_impl->_webContext = nullptr;

    // Disconnect every signal whose user_data is this instance so callbacks cannot fire
    // during WebKit teardown.
    g_signal_handlers_disconnect_by_data(webview, this);

    // Stop any in-flight load before widget teardown.
    webkit_web_view_stop_loading(WEBKIT_WEB_VIEW(webview));

    // Hold a temporary ref to control destruction order independently of the container parent.
    g_object_ref(webview);
    if (GtkWidget* parent = gtk_widget_get_parent(webview))
        gtk_container_remove(GTK_CONTAINER(parent), webview);
    gtk_widget_destroy(webview);
    g_object_unref(webview);
}
