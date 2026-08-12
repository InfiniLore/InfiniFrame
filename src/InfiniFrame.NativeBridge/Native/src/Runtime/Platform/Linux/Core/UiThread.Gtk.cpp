// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <X11/Xlib.h>
#include <chrono>
#include <condition_variable>
#include <exception>
#include <functional>
#include <libnotify/notify.h>
#include <memory>
#include <mutex>
#include <thread>

#include <gtk/gtk.h>

#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
#include "Runtime/Platform/Linux/Core/LinuxGraphicsEnvironment.Gtk.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    constexpr const char* NotifyAppName = "InfiniFrame";

    std::once_flag initializeOnce;
    std::mutex initializeMutex;
    std::condition_variable initializeCompleted;
    bool initialized = false;
    std::thread::id ownerThreadId = {};
    GMainContext* ownerContext = nullptr;
    std::thread gtkThread;
    GMainLoop* mainLoop = nullptr;

    struct InvokeState {
        std::function<void()> callback;
        std::mutex completionMutex;
        std::condition_variable completion;
        std::exception_ptr failure = nullptr;
        bool completed = false;
        // 0 = queued, 1 = callback owns execution, 2 = timed out/cancelled by the waiter.
        std::atomic<int> state = 0;
    };

    gboolean InvokeOnOwnerContext(const gpointer userData) {
        auto* retainedState = static_cast<std::shared_ptr<InvokeState>*>(userData);
        std::shared_ptr<InvokeState> state = *retainedState;
        // Do not invoke a reverse P/Invoke after the waiting managed call has returned.
        if (state->state.exchange(1, std::memory_order_acq_rel) == 2) {
            std::lock_guard lock(state->completionMutex);
            state->completed = true;
            state->completion.notify_one();
            return G_SOURCE_REMOVE;
        }
        try {
            state->callback();
        }
        catch (...) {
            state->failure = std::current_exception();
        }

        {
            std::lock_guard lock(state->completionMutex);
            state->completed = true;
            state->completion.notify_one();
        }

        return G_SOURCE_REMOVE;
    }

    void ReleaseInvokeState(const gpointer userData) {
        delete static_cast<std::shared_ptr<InvokeState>*>(userData);
    }
}

namespace infiniframe::linux_gtk::ui_thread {
    void EnsureInitialized() {
        std::call_once(initializeOnce, [] {
            gtkThread = std::thread([] {
                infiniframe::linux_gtk::ConfigureGraphicsEnvironment();
                XInitThreads();
                gtk_init(nullptr, nullptr);
                notify_init(NotifyAppName);

                {
                    std::lock_guard lock(initializeMutex);
                    ownerThreadId = std::this_thread::get_id();
                    ownerContext = g_main_context_default();
                    initialized = true;
                    initializeCompleted.notify_all();
                }

                mainLoop = g_main_loop_new(ownerContext, FALSE);
                g_main_loop_run(mainLoop);
                g_main_loop_unref(mainLoop);
                mainLoop = nullptr;
            });

            gtkThread.detach();

            std::unique_lock lock(initializeMutex);
            initializeCompleted.wait(lock, [] { return initialized; });
        });
    }

    void Shutdown() {
        if (!initialized) {
            return;
        }

        if (mainLoop != nullptr && g_main_loop_is_running(mainLoop)) {
            g_main_loop_quit(mainLoop);
        }
    }

    bool IsCurrentThread() {
        EnsureInitialized();
        return std::this_thread::get_id() == ownerThreadId;
    }

    namespace {
        gboolean ExecuteAsync(const gpointer userData) {
            std::unique_ptr<std::function<void()>> callback(static_cast<std::function<void()>*>(userData));
            try {
                (*callback)();
            } catch (...) {
                g_warning("InfiniFrame asynchronous UI callback failed.");
            }
            return G_SOURCE_REMOVE;
        }
    }

    namespace {
        bool AttachAsync(std::function<void()> callback, const int priority) {
            if (!callback)
                return false;
            EnsureInitialized();
            GSource* source = g_idle_source_new();
            g_source_set_priority(source, priority);
            g_source_set_callback(
                source, ExecuteAsync, new std::function<void()>(std::move(callback)), nullptr
            );
            const guint sourceId = g_source_attach(source, ownerContext);
            g_source_unref(source);
            if (sourceId == 0)
                return false;
            return true;
        }
    }

    bool InvokeAsync(std::function<void()> callback) {
        return AttachAsync(std::move(callback), G_PRIORITY_DEFAULT);
    }

    bool InvokeIdle(std::function<void()> callback) {
        // WebKit schedules object and process cleanup at the default priority. A low-priority idle acknowledgement
        // therefore cannot overtake cleanup already queued by destruction of a WebKitWebView.
        return AttachAsync(std::move(callback), G_PRIORITY_LOW);
    }

    void InvokeSync(std::function<void()> callback) {
        if (!callback) {
            return;
        }

        EnsureInitialized();
        if (IsCurrentThread()) {
            callback();
            return;
        }

        auto state = std::make_shared<InvokeState>();
        state->callback = std::move(callback);

        g_main_context_invoke_full(
            ownerContext, G_PRIORITY_DEFAULT, InvokeOnOwnerContext, new std::shared_ptr<InvokeState>(state), ReleaseInvokeState
        );

        std::unique_lock lock(state->completionMutex);
        const bool completed = state->completion.wait_for(lock, std::chrono::seconds(15), [&] { return state->completed; });
        if (!completed) {
            // If the UI callback has not started, suppress it. If it won the race, keep the P/Invoke alive until it
            // completes; returning while it runs would leave native code holding an invalid managed callback.
            int expected = 0;
            if (state->state.compare_exchange_strong(expected, 2, std::memory_order_acq_rel)) {
                g_warning("InfiniFrame UI dispatch timed out; late callback suppressed.");
                return;
            }
            state->completion.wait(lock, [&] { return state->completed; });
        }

        if (state->failure != nullptr) {
            std::rethrow_exception(state->failure);
        }
    }
}

__attribute__((destructor(101)))
static void InfiniFrame_UiThread_Shutdown() {
    // During static destruction GLib/GDK objects are already half-torn-down. Attempting
    // g_main_loop_quit() or thread join here triggers cascading assertion failures
    // (gdk_seat_get_keyboard, g_dbus_connection_call_sync_internal) and SIGABRT.
    // The OS reclaims all resources on process exit, so it is safe to do nothing here.
}
