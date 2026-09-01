#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <atomic>
#include <mutex>
#include <vector>
#ifdef _WIN32
#include <Windows.h>
#include <string>
#endif
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
class InfiniFrameWindow;
class InfiniFrameApplication;

struct InfiniFrameApplicationImpl {
    std::atomic<bool> _shutdownRequested{false};
    std::mutex _windowListMutex;
    std::vector<InfiniFrameWindow*> _windows;

#ifdef _WIN32
    HINSTANCE _hInstance = nullptr;
    std::wstring _appUserModelId;
    std::wstring _notificationRegistrationId;
    std::wstring _webView2RuntimePath;
    DWORD _messageLoopThreadId = 0;
#endif
};

struct InfiniFrameApplication::Impl : InfiniFrameApplicationImpl {};
