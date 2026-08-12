// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef _WIN32
#include <Windows.h>
#include <shobjidl.h>

#include "Runtime/Shared/Window/InfiniFrameWindow.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::SetTaskbarProgress(const int state, const uint64_t current, const uint64_t total) {
    HWND hWnd = getHwnd();
    if (!hWnd) return;

    ITaskbarList3* pTaskbarList = nullptr;
    HRESULT hr = CoCreateInstance(
        CLSID_TaskbarList,
        nullptr,
        CLSCTX_INPROC_SERVER,
        IID_ITaskbarList3,
        reinterpret_cast<void**>(&pTaskbarList)
    );

    if (FAILED(hr) || !pTaskbarList) return;

    hr = pTaskbarList->HrInit();
    if (FAILED(hr)) {
        pTaskbarList->Release();
        return;
    }

    TBPFLAG flag = TBPF_NOPROGRESS;
    switch (state) {
        case 0: flag = TBPF_NOPROGRESS; break;
        case 1: flag = TBPF_INDETERMINATE; break;
        case 2: flag = TBPF_NORMAL; break;
        case 3: flag = TBPF_ERROR; break;
        case 4: flag = TBPF_PAUSED; break;
        default: flag = TBPF_NOPROGRESS; break;
    }

    pTaskbarList->SetProgressValue(hWnd, current, total);
    pTaskbarList->SetProgressState(hWnd, flag);
    pTaskbarList->Release();
}

void InfiniFrameWindow::ClearTaskbarProgress() {
    HWND hWnd = getHwnd();
    if (!hWnd) return;

    ITaskbarList3* pTaskbarList = nullptr;
    HRESULT hr = CoCreateInstance(
        CLSID_TaskbarList,
        nullptr,
        CLSCTX_INPROC_SERVER,
        IID_ITaskbarList3,
        reinterpret_cast<void**>(&pTaskbarList)
    );

    if (FAILED(hr) || !pTaskbarList) return;

    hr = pTaskbarList->HrInit();
    if (FAILED(hr)) {
        pTaskbarList->Release();
        return;
    }

    pTaskbarList->SetProgressState(hWnd, TBPF_NOPROGRESS);
    pTaskbarList->Release();
}

void InfiniFrameWindow::SetTaskbarFlash(const int mode, const uint32_t count) {
    HWND hWnd = getHwnd();
    if (!hWnd) return;

    FLASHWINFO fi = {};
    fi.cbSize = sizeof(FLASHWINFO);
    fi.hwnd = hWnd;

    switch (mode) {
        case 0: fi.dwFlags = FLASHW_STOP; break;
        case 1: fi.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG; break;
        case 2: fi.dwFlags = FLASHW_ALL | FLASHW_TIMER; fi.uCount = count; break;
        case 3: fi.dwFlags = FLASHW_ALL | FLASHW_TIMER | FLASHW_TIMERNOFG; fi.uCount = count; break;
        default: fi.dwFlags = FLASHW_STOP; break;
    }

    FlashWindowEx(&fi);
}

void InfiniFrameWindow::StopTaskbarFlash() {
    HWND hWnd = getHwnd();
    if (!hWnd) return;

    FLASHWINFO fi = {};
    fi.cbSize = sizeof(FLASHWINFO);
    fi.hwnd = hWnd;
    fi.dwFlags = FLASHW_STOP;
    FlashWindowEx(&fi);
}

void InfiniFrameWindow::GetTaskbarProgressSupported(bool* supported) const {
    if (supported) *supported = true;
}

#endif
