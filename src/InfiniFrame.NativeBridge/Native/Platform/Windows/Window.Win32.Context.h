#pragma once

#ifndef INFINIFRAME_PLATFORM_WINDOWS_WINDOW_WIN32_CONTEXT_H
#define INFINIFRAME_PLATFORM_WINDOWS_WINDOW_WIN32_CONTEXT_H

#include <atomic>
#include <condition_variable>
#include <mutex>
#include <string>

#include <windows.h>

#include "Core/InfiniFrameWindow.h"
#include "Window.Win32.Internal.h"

inline constexpr UINT WM_USER_INVOKE = WM_USER + 0x0002;

extern std::atomic<HINSTANCE> _hInstance;
extern thread_local HWND messageLoopRootWindowHandle;
extern wchar_t _webview2RuntimePath[MAX_PATH];
extern std::mutex webview2RuntimePathMutex;
extern const wchar_t* CLASS_NAME;

struct InvokeWaitInfo {
    std::mutex mutex;
    std::condition_variable completionNotifier;
    bool isCompleted = false;
    bool isAbandoned = false;
};

bool IsTeardownTraceEnabled();
void TraceTeardown(const wchar_t* format, ...);
std::wstring Utf8ToWide(AutoString source);
std::string WideToUtf8(AutoString source);
bool EnsureDirectoryWritable(const std::wstring& directoryPath);
InfiniFrameWindow* LookupWindowInstance(HWND hwnd);
HWND ResolveParentWindowHandle(InfiniFrameWindow* parent);
HBRUSH GetDarkBrush();
HBRUSH GetLightBrush();

template <typename TImpl>
inline void ApplyPendingOwnerWindow(TImpl* impl, const wchar_t* phase) {
    if (impl == nullptr)
        return;
    if (impl->_ownerAssigned)
        return;

    if (impl->_pendingOwnerHwnd == nullptr || impl->_hWnd == nullptr)
        return;

    if (impl->_pendingOwnerHwnd == impl->_hWnd)
        return;

    if (!IsWindow(impl->_pendingOwnerHwnd) || !IsWindow(impl->_hWnd))
        return;

    SetLastError(0);
    const LONG_PTR previousOwner = SetWindowLongPtr(
        impl->_hWnd,
        GWLP_HWNDPARENT,
        reinterpret_cast<LONG_PTR>(impl->_pendingOwnerHwnd)
        );
    const DWORD lastError = GetLastError();

    if (previousOwner == 0 && lastError != 0) {
        TraceTeardown(
            L"ApplyPendingOwnerWindow failed phase=%ls child=%p owner=%p err=%lu",
            phase,
            impl->_hWnd,
            impl->_pendingOwnerHwnd,
            lastError
            );
        return;
    }

    impl->_ownerAssigned = true;

    const DWORD childThreadId = GetWindowThreadProcessId(impl->_hWnd, nullptr);
    const DWORD ownerThreadId = GetWindowThreadProcessId(impl->_pendingOwnerHwnd, nullptr);
    TraceTeardown(
        L"ApplyPendingOwnerWindow success phase=%ls child=%p owner=%p childTid=%lu ownerTid=%lu prev=%p",
        phase,
        impl->_hWnd,
        impl->_pendingOwnerHwnd,
        childThreadId,
        ownerThreadId,
        reinterpret_cast<void*>(previousOwner)
        );
}

#endif // INFINIFRAME_PLATFORM_WINDOWS_WINDOW_WIN32_CONTEXT_H
