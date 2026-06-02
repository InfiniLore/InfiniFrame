// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <condition_variable>
#include <mutex>

#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    std::mutex invokeLockMutex;

    struct InvokeWaitInfo {
        ACTION callback;
        std::condition_variable completionNotifier;
        bool isCompleted;
    };

    gboolean invokeCallback(const gpointer data) {
        auto* waitInfo = reinterpret_cast<InvokeWaitInfo*>(data);
        waitInfo->callback();
        {
            std::lock_guard guard(invokeLockMutex);
            waitInfo->isCompleted = true;
        }
        waitInfo->completionNotifier.notify_one();
        return false;
    }
} // namespace

void InfiniFrameWindow::Invoke(const ACTION callback) {
    InvokeWaitInfo waitInfo = {};
    waitInfo.callback = callback;
    gdk_threads_add_idle(invokeCallback, &waitInfo);

    std::unique_lock uLock(invokeLockMutex);
    waitInfo.completionNotifier.wait(uLock, [&] { return waitInfo.isCompleted; });
}
