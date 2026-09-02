// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <format>
#include <stdexcept>
#include <string>

#include "Runtime/Shared/Application/InfiniFrameApplication.h"
#include "Runtime/Shared/Application/InfiniFrameApplicationImpl.h"
#include "Runtime/Shared/Application/ApplicationInitParams.h"
#include "Runtime/Platform/Windows/DarkMode.h"
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

using namespace WinToastLib;

InfiniFrameApplication* InfiniFrameApplication::s_instance = nullptr;

InfiniFrameApplication* InfiniFrameApplication::GetInstance() {
    return s_instance;
}

InfiniFrameApplication::InfiniFrameApplication(ApplicationInitParams* params) {
    m_impl = std::make_unique<Impl>();
    s_instance = this;

    if (params == nullptr)
        throw std::invalid_argument("Argument 'params' is null.");

    if (params->StructSize != sizeof(ApplicationInitParams))
        throw std::invalid_argument("ApplicationInitParams size mismatch.");

    // Process-wide: AppUserModelId
    if (params->WindowsAppUserModelId != nullptr && params->WindowsAppUserModelId[0] != '\0') {
        m_impl->_appUserModelId = Utf8ToWide(params->WindowsAppUserModelId);
        const HRESULT result = SetCurrentProcessExplicitAppUserModelID(m_impl->_appUserModelId.c_str());
        if (FAILED(result)) {
            throw std::runtime_error(
                std::format(
                    "Could not set Windows AppUserModelID (HRESULT 0x{:08X}).",
                    static_cast<unsigned long>(result)
                )
            );
        }
    }

    // Process-wide: WinToast
    WinToastLib::setDebugOutputEnabled(false);

    if (params->NotificationRegistrationId != nullptr)
        m_impl->_notificationRegistrationId = Utf8ToWide(params->NotificationRegistrationId);

    // WebView2 runtime path
    if (params->WebView2RuntimePath != nullptr)
        m_impl->_webView2RuntimePath = Utf8ToWide(params->WebView2RuntimePath);
}

InfiniFrameApplication::~InfiniFrameApplication() {
    s_instance = nullptr;
}

void InfiniFrameApplication::Register(const HINSTANCE hInstance) {
    InitDarkModeSupport();

    m_impl->_hInstance = hInstance;
    m_impl->_messageLoopThreadId = GetCurrentThreadId();

    WNDCLASSEX wcx{};
    wcx.cbSize = sizeof(WNDCLASSEX);
    wcx.style = CS_HREDRAW | CS_VREDRAW;
    wcx.lpfnWndProc = WindowProc;
    wcx.hInstance = hInstance;
    wcx.hIcon = LoadIcon(hInstance, IDI_APPLICATION);
    wcx.hCursor = LoadCursor(nullptr, IDC_ARROW);
    wcx.hbrBackground = IsDarkModeEnabled() ? GetDarkBrush() : GetLightBrush();
    wcx.lpszClassName = CLASS_NAME;
    wcx.hIconSm = LoadIcon(hInstance, IDI_APPLICATION);

    if (RegisterClassEx(&wcx) == 0) {
        const DWORD error = GetLastError();
        if (error != ERROR_CLASS_ALREADY_EXISTS)
            throw std::runtime_error("RegisterClassEx failed for window class 'InfiniFrame'.");
    }

    SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

    // Initialize WinToast once at the application level.
    if (!m_impl->_appUserModelId.empty())
        WinToast::instance()->setAppUserModelId(m_impl->_appUserModelId.c_str());
    else if (!m_impl->_notificationRegistrationId.empty())
        WinToast::instance()->setAppUserModelId(m_impl->_notificationRegistrationId.c_str());
    WinToast::instance()->initialize();
}

HINSTANCE InfiniFrameApplication::GetHInstance() const {
    return m_impl->_hInstance;
}

void InfiniFrameApplication::TrackWindow(InfiniFrameWindow* window) {
    std::lock_guard lock(m_impl->_windowListMutex);
    m_impl->_windows.push_back(window);
}

void InfiniFrameApplication::UntrackWindow(InfiniFrameWindow* window) {
    std::lock_guard lock(m_impl->_windowListMutex);
    auto it = std::remove(m_impl->_windows.begin(), m_impl->_windows.end(), window);
    m_impl->_windows.erase(it, m_impl->_windows.end());

    if (m_impl->_windows.empty() && !m_impl->_shutdownRequested.load(std::memory_order_acquire)) {
        Shutdown();
    }
}

bool InfiniFrameApplication::HasWindows() const {
    std::lock_guard lock(m_impl->_windowListMutex);
    return !m_impl->_windows.empty();
}

bool InfiniFrameApplication::IsShutdownRequested() const {
    return m_impl->_shutdownRequested.load(std::memory_order_acquire);
}

const std::wstring& InfiniFrameApplication::GetAppUserModelId() const {
    return m_impl->_appUserModelId;
}

const std::wstring& InfiniFrameApplication::GetNotificationRegistrationId() const {
    return m_impl->_notificationRegistrationId;
}

const std::wstring& InfiniFrameApplication::GetWebView2RuntimePath() const {
    return m_impl->_webView2RuntimePath;
}
