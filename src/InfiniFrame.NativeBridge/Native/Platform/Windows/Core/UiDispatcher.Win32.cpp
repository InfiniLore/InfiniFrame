#include "chrono"
#include "../Window.Win32.Context.h"

void InfiniFrameWindow::WaitForExit() {
    auto* impl = m_impl.get();
    ApplyPendingOwnerWindow(impl, L"wait_for_exit");

    messageLoopRootWindowHandle = impl->_hWnd;
    TraceTeardown(L"WaitForExit start instance=%p hwnd=%p", this, impl->_hWnd);

    MSG msg = {};
    while (true) {
        const int getMessageResult = GetMessage(&msg, nullptr, 0, 0);
        if (getMessageResult == -1) {
            TraceTeardown(L"WaitForExit GetMessage failed err=%lu", GetLastError());
            break;
        }
        if (getMessageResult == 0)
            break;

        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    messageLoopRootWindowHandle = nullptr;
    TraceTeardown(L"WaitForExit end instance=%p hwnd=%p", this, impl->_hWnd);
}

void InfiniFrameWindow::Invoke(ACTION callback) {
    if (!callback) {
        return;
    }

    auto* impl = m_impl.get();
    if (impl->_hWnd == nullptr || IsWindow(impl->_hWnd) == 0) {
        return;
    }

    auto* waitInfo = new InvokeWaitInfo();
    if (!PostMessage(
            impl->_hWnd, WM_USER_INVOKE, reinterpret_cast<WPARAM>(callback), reinterpret_cast<LPARAM>(waitInfo)
        )) {
        delete waitInfo;
        return;
    }

    std::unique_lock<std::mutex> uLock(waitInfo->mutex);
    const bool completed =
        waitInfo->completionNotifier.wait_for(uLock, std::chrono::seconds(15), [&] { return waitInfo->isCompleted; });

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
