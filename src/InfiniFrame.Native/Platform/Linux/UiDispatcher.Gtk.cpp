#ifdef __linux__

#include "Platform/Linux/WindowImpl.Gtk.h"

#include <condition_variable>
#include <mutex>

namespace {
    std::mutex InvokeLockMutex;

    struct InvokeWaitInfo {
        ACTION callback = nullptr;
        std::condition_variable completionNotifier;
        bool isCompleted = false;
    };

    gboolean InvokeCallback(const gpointer data) {
        auto* waitInfo = reinterpret_cast<InvokeWaitInfo*>(data);
        if (waitInfo->callback != nullptr)
            waitInfo->callback();
        {
            std::lock_guard<std::mutex> guard(InvokeLockMutex);
            waitInfo->isCompleted = true;
        }
        waitInfo->completionNotifier.notify_one();
        return false;
    }
}

void InfiniFrameWindow::Invoke(const ACTION callback) {
    if (callback == nullptr)
        return;

    InvokeWaitInfo waitInfo = {};
    waitInfo.callback = callback;
    gdk_threads_add_idle(InvokeCallback, &waitInfo);

    std::unique_lock<std::mutex> uLock(InvokeLockMutex);
    waitInfo.completionNotifier.wait(
        uLock,
        [&] {
            return waitInfo.isCompleted;
        }
        );
}

#endif
