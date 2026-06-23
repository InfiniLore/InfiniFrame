// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <X11/Xlib.h>
#include <condition_variable>
#include <exception>
#include <functional>
#include <libnotify/notify.h>
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

    struct InvokeState {
        std::function<void()> callback;
        std::mutex completionMutex;
        std::condition_variable completion;
        std::exception_ptr failure = nullptr;
        bool completed = false;
    };

    gboolean InvokeOnOwnerContext(gpointer userData) {
        auto* state = static_cast<InvokeState*>(userData);
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
}

namespace infiniframe::linux_gtk::ui_thread {
    void EnsureInitialized() {
        std::call_once(initializeOnce, [] {
            std::thread worker([] {
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

                auto* loop = g_main_loop_new(ownerContext, FALSE);
                g_main_loop_run(loop);
                g_main_loop_unref(loop);
            });

            worker.detach();

            std::unique_lock lock(initializeMutex);
            initializeCompleted.wait(lock, [] { return initialized; });
        });
    }

    bool IsCurrentThread() {
        EnsureInitialized();
        return std::this_thread::get_id() == ownerThreadId;
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

        InvokeState state;
        state.callback = std::move(callback);

        g_main_context_invoke_full(ownerContext, G_PRIORITY_DEFAULT, InvokeOnOwnerContext, &state, nullptr);

        std::unique_lock lock(state.completionMutex);
        state.completion.wait(lock, [&state] { return state.completed; });

        if (state.failure != nullptr) {
            std::rethrow_exception(state.failure);
        }
    }
}
