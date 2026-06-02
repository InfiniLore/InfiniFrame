// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#pragma comment(lib, "Urlmon.lib")
#include <Shlwapi.h>

#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
bool InfiniFrameWindow::EnsureWebViewIsInstalled() {
    LPWSTR versionInfo = nullptr;
    HRESULT ensureInstalledResult = GetAvailableCoreWebView2BrowserVersionString(nullptr, &versionInfo);
    if (versionInfo != nullptr)
        CoTaskMemFree(versionInfo);

    if (ensureInstalledResult != S_OK)
        return InstallWebView2();

    return true;
}

bool InfiniFrameWindow::InstallWebView2() {
    auto srcURL = L"https://go.microsoft.com/fwlink/p/?LinkId=2124703";
    auto destFile = L"MicrosoftEdgeWebview2Setup.exe";

    if (S_OK == URLDownloadToFile(nullptr, srcURL, destFile, 0, nullptr)) {
        std::wstring command = L"MicrosoftEdgeWebview2Setup.exe";

        STARTUPINFO si;
        PROCESS_INFORMATION pi;

        ZeroMemory(&si, sizeof(si));
        si.cb = sizeof(si);
        ZeroMemory(&pi, sizeof(pi));

        bool success = CreateProcess(nullptr, command.data(), nullptr, nullptr, FALSE, 0, nullptr, nullptr, &si, &pi);

        if (success) {
            WaitForSingleObject(pi.hProcess, INFINITE);
            CloseHandle(pi.hProcess);
            CloseHandle(pi.hThread);
        }

        return success;
    }

    return false;
}

void InfiniFrameWindow::SetWebView2RuntimePath(const AutoString pathToWebView2) {
    if (pathToWebView2 == nullptr)
        return;

    std::wstring widePath = Utf8ToWide(pathToWebView2);
    std::lock_guard<std::mutex> lock(webview2RuntimePathMutex);
    wcsncpy_s(_webview2RuntimePath, widePath.c_str(), _countof(_webview2RuntimePath));
}
