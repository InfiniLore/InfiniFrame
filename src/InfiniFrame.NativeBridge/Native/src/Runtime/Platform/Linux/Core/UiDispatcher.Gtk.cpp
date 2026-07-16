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
            // Notify while still holding the lock. The waiter in Invoke() may observe isCompleted before ever
            // blocking, return immediately and destroy the stack-allocated waitInfo. Signalling after releasing the
            // lock would then touch a freed condition_variable (use-after-free -> SIGSEGV -> PAL_SEHException ->
            // terminate -> exit 134). Holding the lock guarantees the waiter cannot re-acquire it (and thus cannot
            // return/destroy waitInfo) until notify_one has finished and the lock is released here.
            waitInfo->completionNotifier.notify_one();
        }
        return false;
    }
} 

void InfiniFrameWindow::Invoke(const ACTION callback) {
    InvokeWaitInfo waitInfo = {};
    waitInfo.callback = callback;
    gdk_threads_add_idle(invokeCallback, &waitInfo);

    std::unique_lock uLock(invokeLockMutex);
    waitInfo.completionNotifier.wait(uLock, [&] { return waitInfo.isCompleted; });
}
