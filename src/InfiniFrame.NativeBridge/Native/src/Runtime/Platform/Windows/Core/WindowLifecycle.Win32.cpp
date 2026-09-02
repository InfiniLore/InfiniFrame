// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
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
    // Block until _destroyed is set. Use a timed wait so we can periodically
    // yield to the OS scheduler, allowing WM_USER_INVOKE messages posted by
    // other threads to be dispatched by the application's message loop or
    // by our own message pump below.
    while (true) {
        {
            std::unique_lock lock(m_impl->_lifecycleMutex);
            if (m_impl->_destroyed)
                return;
            m_impl->_lifecycleClosed.wait_for(lock, std::chrono::milliseconds(50), [this] {
                return m_impl->_destroyed;
            });
        }
        // Dispatch any pending messages for this thread to keep the
        // message queue alive (needed for Invoke from other threads).
        MSG msg;
        while (PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE)) {
            if (msg.message == WM_QUIT) {
                // Post WM_QUIT back so application loops can see it.
                PostThreadMessage(GetCurrentThreadId(), WM_QUIT, 0, 0);
                return;
            }
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
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