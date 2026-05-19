// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Utils/Common.h"
#include "Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
static_assert(sizeof(wchar_t) == sizeof(char16_t));

const wchar_t* CLASS_NAME = L"InfiniFrame";
std::atomic<HINSTANCE> _hInstance{nullptr};
thread_local HWND messageLoopRootWindowHandle = nullptr;
wchar_t _webview2RuntimePath[MAX_PATH];
std::mutex webview2RuntimePathMutex;
