// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <atomic>
#include <stdexcept>
#include <string>
#include <mutex>
#include <condition_variable>
#include <functional>
#include <csignal>
#include <execinfo.h>
#include <unistd.h>
#include <pthread.h>
#include <thread>

// Declared in WebKitHost.Gtk.cpp — arms the SIGABRT bypass when the last window is gone.
extern void InfiniFrame_ArmWebKitTeardown() noexcept;

#include <X11/Xlib.h>
#include <libnotify/notify.h>

#include "Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

// g_gtk_worker_thread_id has external linkage so WindowInitialization.Gtk.cpp can read it.
std::thread::id g_gtk_worker_thread_id;

namespace {
    std::recursive_mutex g_linux_window_lifecycle_mutex;
    std::atomic<int> g_active_window_count{0};
    std::once_flag g_gtk_worker_once;
    std::once_flag g_sigabrt_trace_once;
    std::mutex g_notify_mutex;
    int g_notify_ref_count = 0;
    bool g_notify_initialized = false;

    GMainLoop* g_gtk_permanent_loop = nullptr;
    std::mutex g_gtk_worker_ready_mutex;
    std::condition_variable g_gtk_worker_ready_cv;
    bool g_gtk_worker_ready = false;

    struct GtkSyncInvokeState {
        std::mutex mutex;
        std::condition_variable condition;
        bool completed = false;
        std::function<void()> action;
    };

    bool linux_native_sigabrt_trace_enabled() {
        const char* value = g_getenv("INFINIFRAME_LINUX_NATIVE_SIGABRT_TRACE");
        return value != nullptr && value[0] != '\0' && g_strcmp0(value, "0") != 0;
    }

    void sigabrt_backtrace_handler(int) {
        void* frames[128];
        const int frame_count = backtrace(frames, static_cast<int>(std::size(frames)));
        const unsigned long tid = static_cast<unsigned long>(pthread_self());
        dprintf(STDERR_FILENO, "[InfiniFrame/Linux] SIGABRT on pthread=%lu\n", tid);
        if (frame_count > 0)
            backtrace_symbols_fd(frames, frame_count, STDERR_FILENO);
        signal(SIGABRT, SIG_DFL);
        raise(SIGABRT);
    }

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

    void start_gtk_worker_thread() {
        std::thread([] {
            XInitThreads();
            gtk_init(nullptr, nullptr);

            g_gtk_worker_thread_id = std::this_thread::get_id();

            {
                std::lock_guard<std::mutex> lk(g_notify_mutex);
                notify_init("InfiniFrame");
                g_notify_initialized = true;
            }

            g_gtk_permanent_loop = g_main_loop_new(nullptr, FALSE);

            {
                std::lock_guard<std::mutex> lk(g_gtk_worker_ready_mutex);
                g_gtk_worker_ready = true;
            }
            g_gtk_worker_ready_cv.notify_all();

            g_main_loop_run(g_gtk_permanent_loop); // runs for process lifetime
        }).detach();

        std::unique_lock<std::mutex> lk(g_gtk_worker_ready_mutex);
        g_gtk_worker_ready_cv.wait(lk, [] { return g_gtk_worker_ready; });
    }

    void acquire_linux_runtime(const char* /*app_name*/) {
        std::call_once(g_gtk_worker_once, start_gtk_worker_thread);

        std::call_once(g_sigabrt_trace_once, [] {
            if (!linux_native_sigabrt_trace_enabled())
                return;

            struct sigaction sa {};
            sa.sa_handler = sigabrt_backtrace_handler;
            sigemptyset(&sa.sa_mask);
            sa.sa_flags = SA_RESETHAND;
            sigaction(SIGABRT, &sa, nullptr);
        });

        std::lock_guard<std::mutex> lock(g_notify_mutex);
        // notify_init() was called in start_gtk_worker_thread(); only increment ref-count here.
        ++g_notify_ref_count;
    }

    void release_linux_runtime() {
        std::lock_guard<std::mutex> lock(g_notify_mutex);
        if (g_notify_ref_count <= 0)
            return;

        --g_notify_ref_count;
        if (g_notify_ref_count == 0 && g_notify_initialized && g_main_context_is_owner(g_main_context_default())) {
            notify_uninit();
            g_notify_initialized = false;
        }
    }
}

InfiniFrameWindow::InfiniFrameWindow(InfiniFrameInitParams* initParams)
    : m_impl(std::make_unique<Impl>()) {
    std::lock_guard<std::recursive_mutex> lifecycle_guard(g_linux_window_lifecycle_mutex);
    ++g_active_window_count;
    acquire_linux_runtime(initParams->Title);

    if (initParams->StructSize != sizeof(InfiniFrameInitParams)) {
        throw std::invalid_argument(
            "Initial parameters passed are " + std::to_string(initParams->StructSize) +
            " bytes, but expected " + std::to_string(sizeof(InfiniFrameInitParams)) + " bytes."
        );
    }

    // InitializeFromParams is pure C++ — safe to call on the calling thread.
    m_impl->InitializeFromParams(initParams);

    // All GTK/WebKit calls must execute on the permanent GTK worker thread.
    invoke_on_gtk_thread_and_wait([this, initParams] {
        m_impl->ConfigureInitialWindow(this, initParams);
        m_impl->ApplyInitialWindowState(this, initParams);
        m_impl->ConnectWindowSignals(this);

        // Register custom schemes before first navigation to avoid first-load races.
        m_impl->AddCustomSchemeHandlers();

        Show(false);

        m_impl->ConnectWebViewSignals(this);

        if (m_impl->_transparentEnabled)
            SetTransparentEnabled(true);

        if (m_impl->_zoom != 100.0)
            SetZoom(m_impl->_zoom);
    });
}

InfiniFrameWindow::~InfiniFrameWindow() {
    std::lock_guard<std::recursive_mutex> lifecycle_guard(g_linux_window_lifecycle_mutex);

    // If the window was not closed through the normal path (WaitForExit/WaitForClose), force-destroy
    // it now. gtk_widget_destroy fires the "destroy" signal synchronously, so OnWidgetDestroyed()
    // runs within the dispatch and handles all signal cleanup, pointer nulling, and CV notification.
    if (!m_impl->_windowDestroyed && m_impl->_window != nullptr && GTK_IS_WIDGET(m_impl->_window)) {
        if (!m_impl->IsGtkThread()) {
            invoke_on_gtk_thread_and_wait([this] {
                if (!m_impl->_windowDestroyed && m_impl->_window != nullptr && GTK_IS_WIDGET(m_impl->_window))
                    gtk_widget_destroy(m_impl->_window);
            });
        } else {
            gtk_widget_destroy(m_impl->_window);
        }
    }

    // _webContext is the process-global static context; we do not own its reference.
    m_impl->_webContext = nullptr;
    release_linux_runtime();

    // When the last window is destroyed, arm the SIGABRT bypass so WebKit's own background-thread
    // cleanup abort() is suppressed.
    if (--g_active_window_count == 0) {
        InfiniFrame_ArmWebKitTeardown();
    }
}

InfiniFrameWindowImpl* InfiniFrameWindow::ImplBase() noexcept { return m_impl.get(); }
const InfiniFrameWindowImpl* InfiniFrameWindow::ImplBase() const noexcept { return m_impl.get(); }
