// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Application/InfiniFrameApplication.h"
#ifdef _WIN32
#include <windows.h>
#include <shobjidl_core.h>
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
#endif

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
InfiniFrameApplication* InfiniFrameApplication::s_instance = nullptr;

#ifdef _WIN32
namespace {
    std::wstring ToWindowsString(const char* value) {
        if (value == nullptr || value[0] == '\0') return {};
        const int length = MultiByteToWideChar(CP_UTF8, 0, value, -1, nullptr, 0);
        if (length <= 1) return {};
        std::wstring result(static_cast<std::size_t>(length), L'\0');
        MultiByteToWideChar(CP_UTF8, 0, value, -1, result.data(), length);
        result.resize(static_cast<std::size_t>(length - 1));
        return result;
    }
}
#endif

InfiniFrameApplication::InfiniFrameApplication() {
    if (s_instance != nullptr)
        throw std::logic_error("Only one InfiniFrameApplication may exist at a time.");
    s_instance = this;
}

InfiniFrameApplication::~InfiniFrameApplication() {
    std::lock_guard lock(_mutex);
    _windows.clear();
    if (s_instance == this) s_instance = nullptr;
}

InfiniFrameApplication* InfiniFrameApplication::GetInstance() noexcept {
    return s_instance;
}

void InfiniFrameApplication::Register() {
    std::lock_guard lock(_mutex);
#ifdef _WIN32
    InfiniFrameWindow::Register(GetModuleHandle(nullptr));
    if (_registered) return;
    if (!_appUserModelId.empty()) {
        const std::wstring appUserModelId = ToWindowsString(_appUserModelId.c_str());
        const HRESULT result = SetCurrentProcessExplicitAppUserModelID(appUserModelId.c_str());
        if (FAILED(result))
            throw std::runtime_error("Could not set the application Windows AppUserModelID.");
    }
#endif
    _registered = true;
    _shutdownRequested = false;
}

void InfiniFrameApplication::Configure(
    const char* webView2RuntimePath,
    const char* notificationRegistrationId,
    const char* appUserModelId,
    const char* defaultNotificationIcon
) {
    std::lock_guard lock(_mutex);
    _webView2RuntimePath = webView2RuntimePath == nullptr ? "" : webView2RuntimePath;
    _notificationRegistrationId = notificationRegistrationId == nullptr ? "" : notificationRegistrationId;
    _appUserModelId = appUserModelId == nullptr ? "" : appUserModelId;
    _defaultNotificationIcon = defaultNotificationIcon == nullptr ? "" : defaultNotificationIcon;
}

#ifdef _WIN32
void InfiniFrameApplication::EnsureNotificationsInitialized(const char* appName) {
    std::lock_guard lock(_mutex);
    if (_notificationsRegistered) return;
    const std::string& identity = !_notificationRegistrationId.empty()
        ? _notificationRegistrationId
        : _appUserModelId;
    if (identity.empty()) return;

    const char* effectiveAppName = appName != nullptr && appName[0] != '\0' ? appName : identity.c_str();
    const std::wstring windowsAppName = ToWindowsString(effectiveAppName);
    const std::wstring windowsIdentity = ToWindowsString(identity.c_str());
    WinToastLib::WinToast::instance()->setAppName(windowsAppName);
    WinToastLib::WinToast::instance()->setAppUserModelId(windowsIdentity);
    if (!WinToastLib::WinToast::instance()->initialize())
        throw std::runtime_error("Could not initialize application notifications.");
    _notificationsRegistered = true;
}
#endif

void InfiniFrameApplication::Run() noexcept {
#ifdef _WIN32
    {
        std::lock_guard lock(_mutex);
        _runThreadId = GetCurrentThreadId();
        _running = true;
        if (_windows.empty() || _shutdownRequested) {
            _running = false;
            return;
        }
    }

    MSG message = {};
    while (GetMessage(&message, nullptr, 0, 0) > 0) {
        TranslateMessage(&message);
        DispatchMessage(&message);
    }

    std::lock_guard lock(_mutex);
    _running = false;
    _runThreadId = 0;
#else
    // Other platforms retain their existing loop until their application
    // lifecycle integrations are implemented.
#endif
}

void InfiniFrameApplication::Shutdown() noexcept {
    std::lock_guard lock(_mutex);
    _shutdownRequested = true;
#ifdef _WIN32
    if (_running && _runThreadId != 0)
        PostThreadMessage(_runThreadId, WM_QUIT, 0, 0);
#endif
}

void InfiniFrameApplication::TrackWindow(InfiniFrameWindow* window) {
    if (window == nullptr) return;
    std::lock_guard lock(_mutex);
    _windows.insert(window);
}

void InfiniFrameApplication::UntrackWindow(InfiniFrameWindow* window) noexcept {
    if (window == nullptr) return;
    std::lock_guard lock(_mutex);
    _windows.erase(window);
}

void InfiniFrameApplication::NotifyWindowClosed(InfiniFrameWindow* window) noexcept {
    if (window == nullptr) return;

    std::lock_guard lock(_mutex);
    _windows.erase(window);
#ifdef _WIN32
    if (_running && _windows.empty())
        PostThreadMessage(_runThreadId, WM_QUIT, 0, 0);
#endif
}

std::size_t InfiniFrameApplication::GetWindowCount() const noexcept {
    std::lock_guard lock(_mutex);
    return _windows.size();
}

const char* InfiniFrameApplication::GetWebView2RuntimePath() const noexcept {
    return _webView2RuntimePath.c_str();
}

const char* InfiniFrameApplication::GetNotificationRegistrationId() const noexcept {
    return _notificationRegistrationId.c_str();
}

const char* InfiniFrameApplication::GetAppUserModelId() const noexcept {
    return _appUserModelId.c_str();
}

const char* InfiniFrameApplication::GetDefaultNotificationIcon() const noexcept {
    return _defaultNotificationIcon.c_str();
}

bool InfiniFrameApplication::HasNotificationRegistration() const noexcept {
    return _notificationsRegistered;
}
