// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
#include "Runtime/Shared/Application/InfiniFrameApplication.h"
#include "Runtime/Shared/Application/InfiniFrameApplicationImpl.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::Center() {
    int screenDpi = GetDpiForWindow(m_impl->_hWnd);
    int screenHeight = GetSystemMetricsForDpi(SM_CYSCREEN, screenDpi);
    int screenWidth = GetSystemMetricsForDpi(SM_CXSCREEN, screenDpi);

    RECT windowRect = {};
    GetWindowRect(m_impl->_hWnd, &windowRect);
    int windowHeight = windowRect.bottom - windowRect.top;
    int windowWidth = windowRect.right - windowRect.left;

    int left = (screenWidth / 2) - (windowWidth / 2);
    int top = (screenHeight / 2) - (windowHeight / 2);

    SetPosition(left, top);
}

void InfiniFrameWindow::Close() {
    PostMessage(m_impl->_hWnd, WM_CLOSE, 0, 0);
}

void InfiniFrameWindow::MarkDestroyed() {
    {
        std::lock_guard lock(m_impl->_lifecycleMutex);
        m_impl->_destroyed = true;
    }
    m_impl->_lifecycleClosed.notify_all();
}

bool InfiniFrameWindow::IsDestroyed() const {
    std::lock_guard lock(m_impl->_lifecycleMutex);
    return m_impl->_destroyed;
}

void InfiniFrameWindow::WaitForExit() {
    // If the application owns the message loop (Run() is active), just wait for _destroyed.
    // The application's message loop processes WM_USER_INVOKE and other messages.
    // If no application message loop is active, pump messages ourselves so that
    // Invoke() from other threads can complete.
    if (m_impl->_application != nullptr && m_impl->_application->IsMessageLoopRunning()) {
        std::unique_lock lock(m_impl->_lifecycleMutex);
        m_impl->_lifecycleClosed.wait(lock, [&] { return m_impl->_destroyed; });
        return;
    }

    // No application message loop — pump messages while waiting for destroy.
    MSG msg = {};
    while (true) {
        {
            std::lock_guard lock(m_impl->_lifecycleMutex);
            if (m_impl->_destroyed)
                return;
        }

        // PeekMessage with nullptr hwnd retrieves ALL messages for this thread,
        // including WM_USER_INVOKE posted to our window.
        while (PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE)) {
            if (msg.message == WM_QUIT)
                return;
            // Only dispatch messages intended for our window or thread messages.
            if (msg.hwnd == nullptr || msg.hwnd == m_impl->_hWnd) {
                TranslateMessage(&msg);
                DispatchMessage(&msg);
            }
            {
                std::lock_guard lock(m_impl->_lifecycleMutex);
                if (m_impl->_destroyed)
                    return;
            }
        }

        MsgWaitForMultipleObjects(0, nullptr, FALSE, 50, QS_ALLINPUT);
    }
}

namespace {
    DWORD CALLBACK CompleteTeardown(void* context) {
        static_cast<InfiniFrameWindow*>(context)->SignalTeardown();
        return 0;
    }
}

void InfiniFrameWindow::ScheduleTeardownCompletion() {
    CompleteOperationsForClose();
    CompleteNavigationForClose();
    CompleteDialogsForClose();
    if (!QueueUserWorkItem(CompleteTeardown, this, WT_EXECUTEONLYONCE))
        SignalTeardown();
}