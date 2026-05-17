#include <algorithm>
#include <chrono>
#include <comdef.h>
#include <cstdlib>
#include <cstdarg>
#include <cstdio>
#include <cstring>
#include <cwchar>
#include <filesystem>
#include <Shellscalingapi.h>
#include <Shlwapi.h>
#include <WebView2EnvironmentOptions.h>
#include <windows.h>
#include <wrl.h>
#include <string>

#include <format>

#include "Core/InfiniFrameDialog.h"
#include "Core/InfiniFrameWindow.h"
#include <simdutf.h>
#include "DarkMode.h"
#include "ToastHandler.h"
#include "Utils/Common.h"
#include "Window.Win32.Context.h"
#include "Window.Win32.Internal.h"

#include "Embedded/Embedded.h"

#pragma comment(lib, "Shcore.lib")
#pragma comment(lib, "Urlmon.lib")

using namespace WinToastLib;
using namespace Microsoft::WRL;

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
const wchar_t* CLASS_NAME = L"InfiniFrame";
std::atomic<HINSTANCE> _hInstance{nullptr};
thread_local HWND messageLoopRootWindowHandle = nullptr;
wchar_t _webview2RuntimePath[MAX_PATH];
std::mutex webview2RuntimePathMutex;

static_assert(sizeof(wchar_t) == sizeof(char16_t));

bool IsTeardownTraceEnabled() {
    static const bool enabled = [] {
        wchar_t value[32] = {};
        const DWORD len = GetEnvironmentVariableW(L"INFINIFRAME_TRACE_TEARDOWN", value, _countof(value));
        if (len == 0 || len >= _countof(value))
            return false;

        return _wcsicmp(value, L"1") == 0
            || _wcsicmp(value, L"true") == 0
            || _wcsicmp(value, L"yes") == 0
            || _wcsicmp(value, L"on") == 0;
    }();

    return enabled;
}

void TraceTeardown(const wchar_t* format, ...) {
    if (!IsTeardownTraceEnabled())
        return;

    wchar_t message[1024] = {};
    va_list args;
    va_start(args, format);
    _vsnwprintf_s(message, _countof(message), _TRUNCATE, format, args);
    va_end(args);

    const std::wstring line = std::format(
        L"[InfiniFrame][teardown][tid={}] {}\n",
        GetCurrentThreadId(),
        message
        );
    OutputDebugStringW(line.c_str());
    std::fwprintf(stderr, L"%ls", line.c_str());
    std::fflush(stderr);
}

std::wstring Utf8ToWide(const AutoString source) {
    if (source == nullptr)
        return {};

    const auto* utf8 = reinterpret_cast<const char*>(source);
    const size_t utf8Length = strlen(utf8);
    if (utf8Length == 0)
        return {};

    if (const auto validation = simdutf::validate_utf8_with_errors(utf8, utf8Length); validation.is_err())
        return {};

    std::u16string utf16(simdutf::utf16_length_from_utf8(utf8, utf8Length), u'\0');
    const size_t written = simdutf::convert_valid_utf8_to_utf16(
        utf8,
        utf8Length,
        reinterpret_cast<char16_t*>(utf16.data())
        );
    utf16.resize(written);

    return {
        reinterpret_cast<const wchar_t*>(utf16.data()),
        utf16.size()
    };
}

std::string WideToUtf8(const AutoString source) {
    if (source == nullptr)
        return {};

    const size_t utf16Length = wcslen(source);
    if (utf16Length == 0)
        return {};

    const auto* utf16 = reinterpret_cast<const char16_t*>(source);
    if (const auto validation = simdutf::validate_utf16_with_errors(utf16, utf16Length); validation.is_err())
        return {};

    std::string utf8(simdutf::utf8_length_from_utf16(utf16, utf16Length), '\0');
    const size_t written = simdutf::convert_valid_utf16_to_utf8(
        utf16,
        utf16Length,
        utf8.data()
        );
    utf8.resize(written);

    return utf8;
}

bool EnsureDirectoryWritable(const std::wstring& directoryPath) {
    if (directoryPath.empty())
        return false;

    std::error_code createError;
    std::filesystem::create_directories(directoryPath, createError);
    if (createError)
        return false;

    const std::wstring probePath = std::format(
        L"{}\\{}.tmp",
        directoryPath,
        std::format(L".infiniframe-wv2-write-check-{}-{}-{}", GetCurrentProcessId(), GetCurrentThreadId(), GetTickCount64())
        );

    HANDLE probeHandle = CreateFileW(
        probePath.c_str(),
        GENERIC_WRITE,
        FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE,
        nullptr,
        CREATE_ALWAYS,
        FILE_ATTRIBUTE_TEMPORARY,
        nullptr
        );

    if (probeHandle == INVALID_HANDLE_VALUE)
        return false;

    CloseHandle(probeHandle);
    DeleteFileW(probePath.c_str());
    return true;
}

InfiniFrameWindow* LookupWindowInstance(const HWND hwnd) {
    return reinterpret_cast<InfiniFrameWindow*>(GetWindowLongPtr(hwnd, GWLP_USERDATA));
}

HWND ResolveParentWindowHandle(InfiniFrameWindow* parent) {
    if (parent == nullptr)
        return nullptr;

    HWND parentHwnd = parent->getHwnd();
    if (parentHwnd == nullptr || !IsWindow(parentHwnd))
        return nullptr;

    return parentHwnd;
}

void InfiniFrameWindow::WaitForExit() {
    ApplyPendingOwnerWindow(m_impl.get(), L"wait_for_exit");

    messageLoopRootWindowHandle = m_impl->_hWnd;
    TraceTeardown(L"WaitForExit start instance=%p hwnd=%p", this, m_impl->_hWnd);

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
    TraceTeardown(L"WaitForExit end instance=%p hwnd=%p", this, m_impl->_hWnd);
}
