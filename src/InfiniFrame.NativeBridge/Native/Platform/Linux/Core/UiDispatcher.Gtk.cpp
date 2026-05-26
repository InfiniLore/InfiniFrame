// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <condition_variable>
#include <mutex>

#include "Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    struct InvokeWaitInfo {
        ACTION callback;
        std::mutex callMutex;                    // per-call, not global
        std::condition_variable completionNotifier;
        bool isCompleted = false;
    };

    gboolean invokeCallback(const gpointer data) {
        auto* waitInfo = reinterpret_cast<InvokeWaitInfo*>(data);
        waitInfo->callback();
        {
            std::lock_guard<std::mutex> guard(waitInfo->callMutex);
            waitInfo->isCompleted = true;
        }
        waitInfo->completionNotifier.notify_one();
        return false;
    }
} // namespace

void InfiniFrameWindow::Invoke(const ACTION callback) {
    // GTK APIs are thread-affine. Use the captured owner thread rather than main-context ownership because
    // g_main_context_is_owner() can be false outside active dispatch while still on the right GTK thread.
    if (m_impl->IsGtkThread()) {
        callback();
        return;
    }

    InvokeWaitInfo waitInfo;
    waitInfo.callback = callback;
    gdk_threads_add_idle(invokeCallback, &waitInfo);

    std::unique_lock<std::mutex> uLock(waitInfo.callMutex);
    waitInfo.completionNotifier.wait(uLock, [&] { return waitInfo.isCompleted; });
}
