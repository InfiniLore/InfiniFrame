#ifdef __linux__

#include <condition_variable>
#include <mutex>

#include "Window.Gtk.Internal.h"

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
            std::lock_guard<std::mutex> guard(invokeLockMutex);
            waitInfo->isCompleted = true;
        }
        waitInfo->completionNotifier.notify_one();
        return false;
    }
}

void InfiniFrameWindow::Invoke(const ACTION callback) {
    InvokeWaitInfo waitInfo = {};
    waitInfo.callback = callback;
    gdk_threads_add_idle(invokeCallback, &waitInfo);

    std::unique_lock<std::mutex> uLock(invokeLockMutex);
    waitInfo.completionNotifier.wait(
        uLock, [&] {
            return waitInfo.isCompleted;
        }
        );
}

#endif
