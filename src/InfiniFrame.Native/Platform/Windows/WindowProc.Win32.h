#pragma once
/**
 * @file WindowProc.Win32.h
 * @brief Private Win32 message dispatch helpers for InfiniFrameWindow.
 */

#ifndef INFINIFRAME_PLATFORM_WINDOWS_WINDOWPROC_WIN32_H
#define INFINIFRAME_PLATFORM_WINDOWS_WINDOWPROC_WIN32_H

#include <condition_variable>
#include <mutex>

#include <windows.h>

#include "Types/Callbacks.h"

class InfiniFrameWindow;

namespace InfiniFrame::Platform::Windows {
    inline constexpr UINT InvokeMessage = WM_USER + 0x0002;

    struct InvokeWaitInfo {
        ACTION callback;
        std::condition_variable completionNotifier;
        bool isCompleted;
    };

    extern std::mutex InvokeLockMutex;
    extern thread_local HWND MessageLoopRootWindowHandle;

    HBRUSH DarkBackgroundBrush() noexcept;
    HBRUSH LightBackgroundBrush() noexcept;
    void TrackWindowInstance(HWND hwnd, InfiniFrameWindow* instance);
}

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

#endif // INFINIFRAME_PLATFORM_WINDOWS_WINDOWPROC_WIN32_H
