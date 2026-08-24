// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <cstdarg>
#include <cstdio>
#include <format>
#include <string>

#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Determines whether teardown trace logging is enabled in the system.
 *
 * This function typically checks a configuration setting or a runtime
 * flag that specifies if detailed logging or tracing should be
 * performed during the teardown phase of a system or application
 * component.
 *
 * @return true if teardown trace logging is enabled, false otherwise.
 */
bool IsTeardownTraceEnabled() {
    static const bool enabled = [] {
        wchar_t value[32] = {};
        const DWORD len = GetEnvironmentVariableW(L"INFINIFRAME_TRACE_TEARDOWN", value, _countof(value));
        if (len == 0 || len >= _countof(value))
            return false;

        return _wcsicmp(value, L"1") == 0 || _wcsicmp(value, L"true") == 0 || _wcsicmp(value, L"yes") == 0 ||
            _wcsicmp(value, L"on") == 0;
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

    const std::wstring line = std::format(L"[InfiniFrame][teardown][tid={}] {}\n", GetCurrentThreadId(), message);
    OutputDebugStringW(line.c_str());
    std::fwprintf(stderr, L"%ls", line.c_str());
    std::fflush(stderr);
}