// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Application/InfiniFrameApplication.h"
#include "Runtime/Shared/Application/InfiniFrameApplicationImpl.h"
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameApplication::Run() {
    m_impl->_messageLoopThreadId = GetCurrentThreadId();

    MSG msg = {};
    while (!m_impl->_shutdownRequested.load(std::memory_order_acquire)) {
        const int getMessageResult = GetMessage(&msg, nullptr, 0, 0);
        if (getMessageResult == -1)
            break;
        if (getMessageResult == 0)
            break;

        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }
}

void InfiniFrameApplication::Shutdown() {
    bool expected = false;
    if (!m_impl->_shutdownRequested.compare_exchange_strong(expected, true, std::memory_order_acq_rel))
        return;

    DWORD threadId = m_impl->_messageLoopThreadId;
    if (threadId != 0 && threadId != GetCurrentThreadId()) {
        PostThreadMessage(threadId, WM_QUIT, 0, 0);
    } else {
        PostQuitMessage(0);
    }
}
