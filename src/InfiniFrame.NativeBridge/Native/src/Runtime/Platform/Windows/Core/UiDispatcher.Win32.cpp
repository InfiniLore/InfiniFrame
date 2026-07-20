// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "chrono"

#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
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

        OutputDebugStringW(L"InfiniFrame dispatch watchdog: result=TimedOut callback=Suppressed platform=Windows.\n");
        return;
    }

    uLock.unlock();
    delete waitInfo;
}
