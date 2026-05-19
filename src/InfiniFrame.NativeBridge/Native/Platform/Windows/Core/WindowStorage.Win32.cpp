// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <filesystem>
#include <format>

#include "Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
bool EnsureDirectoryWritable(const std::wstring& directoryPath) {
    if (directoryPath.empty())
        return false;

    std::error_code createError;
    std::filesystem::create_directories(directoryPath, createError);
    if (createError)
        return false;

    const std::wstring probePath = std::format(
        L"{}\\{}.tmp", directoryPath,
        std::format(
            L".infiniframe-wv2-write-check-{}-{}-{}", GetCurrentProcessId(), GetCurrentThreadId(), GetTickCount64()
        )
    );

    HANDLE probeHandle = CreateFileW(
        probePath.c_str(), GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE, nullptr,
        CREATE_ALWAYS, FILE_ATTRIBUTE_TEMPORARY, nullptr
    );

    if (probeHandle == INVALID_HANDLE_VALUE)
        return false;

    CloseHandle(probeHandle);
    DeleteFileW(probePath.c_str());
    return true;
}
