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