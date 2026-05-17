#include <chrono>

#include "Window.Win32.Context.h"

void InfiniFrameWindow::Invoke(ACTION callback) {
    if (!callback)
        return;

    if (m_impl->_hWnd == nullptr || !IsWindow(m_impl->_hWnd))
        return;

    auto* waitInfo = new InvokeWaitInfo();
    if (!PostMessage(
        m_impl->_hWnd, WM_USER_INVOKE, reinterpret_cast<WPARAM>(callback), reinterpret_cast<LPARAM>(waitInfo)
        )) {
        delete waitInfo;
        return;
    }

    std::unique_lock<std::mutex> uLock(waitInfo->mutex);
    const bool completed = waitInfo->completionNotifier.wait_for(
        uLock,
        std::chrono::seconds(15),
        [&] {
            return waitInfo->isCompleted;
        }
        );

    if (!completed) {
        bool deleteWaitInfo = false;
        if (waitInfo->isCompleted)
            deleteWaitInfo = true;
        else
            waitInfo->isAbandoned = true;

        uLock.unlock();

        if (deleteWaitInfo)
            delete waitInfo;

        OutputDebugStringW(L"InfiniFrameWindow::Invoke timed out waiting for UI thread callback.\n");
        return;
    }

    uLock.unlock();
    delete waitInfo;
}
