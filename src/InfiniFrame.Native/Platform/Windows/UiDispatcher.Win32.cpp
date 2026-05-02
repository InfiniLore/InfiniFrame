#include "WindowImpl.Win32.h"
#include "WindowProc.Win32.h"

#include <chrono>
#include <mutex>

void InfiniFrameWindow::Invoke(ACTION callback) {
    if (callback == nullptr)
        return;

    if (m_impl->_hWnd == nullptr || !IsWindow(m_impl->_hWnd))
        return;

    InfiniFrame::Platform::Windows::InvokeWaitInfo waitInfo = {};
    if (!PostMessage(
        m_impl->_hWnd,
        InfiniFrame::Platform::Windows::InvokeMessage,
        reinterpret_cast<WPARAM>(callback),
        reinterpret_cast<LPARAM>(&waitInfo)
        ))
        return;

    std::unique_lock<std::mutex> uLock(InfiniFrame::Platform::Windows::InvokeLockMutex);
    const bool completed = waitInfo.completionNotifier.wait_for(
        uLock,
        std::chrono::seconds(15),
        [&] {
            return waitInfo.isCompleted;
        }
        );

    if (!completed)
        OutputDebugStringW(L"InfiniFrameWindow::Invoke timed out waiting for UI thread callback.\n");
}
