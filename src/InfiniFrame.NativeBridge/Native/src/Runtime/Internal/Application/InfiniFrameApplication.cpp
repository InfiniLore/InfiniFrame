// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Internal/Application/InfiniFrameApplication.h"
#include <atomic>
#include <mutex>
#include <string>
#include <unordered_set>
#ifdef _WIN32
#include <windows.h>
#include <shobjidl_core.h>
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
#include "Dependencies/wintoastlib/wintoastlib.h"
#endif

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
struct InfiniFrameApplicationImpl {
    mutable std::mutex mutex;
    std::unordered_set<InfiniFrameWindow*> windows;
    std::unordered_set<InfiniFrameWindow*> closedWindows;
    bool registered = false;
    bool shutdownRequested = false;
    std::string webView2RuntimePath;
    std::string notificationRegistrationId;
    std::string appUserModelId;
    std::string defaultNotificationIcon;
    bool notificationsRegistered = false;
#ifdef _WIN32
    unsigned long runThreadId = 0;
    bool running = false;
#endif
};

namespace {
    std::atomic<InfiniFrameApplication*> applicationInstance = nullptr;
}

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

InfiniFrameApplication::InfiniFrameApplication()
    : _impl(std::make_unique<InfiniFrameApplicationImpl>()) {
    InfiniFrameApplication* expected = nullptr;
    if (!applicationInstance.compare_exchange_strong(expected, this, std::memory_order_acq_rel))
        throw std::runtime_error("Only one InfiniFrameApplication may exist per process.");
}

InfiniFrameApplication::~InfiniFrameApplication() {
    std::lock_guard lock(_impl->mutex);
    _impl->windows.clear();
    _impl->closedWindows.clear();
    InfiniFrameApplication* expected = this;
    applicationInstance.compare_exchange_strong(expected, nullptr, std::memory_order_acq_rel);
}

InfiniFrameApplication* InfiniFrameApplication::GetInstance() noexcept {
    return applicationInstance.load(std::memory_order_acquire);
}

void InfiniFrameApplication::Register() {
    std::lock_guard lock(_impl->mutex);
#ifdef _WIN32
    InfiniFrameWindow::Register(GetModuleHandle(nullptr));
    if (_impl->registered) return;
    if (!_impl->appUserModelId.empty()) {
        const std::wstring appUserModelId = ToWindowsString(_impl->appUserModelId.c_str());
        const HRESULT result = SetCurrentProcessExplicitAppUserModelID(appUserModelId.c_str());
        if (FAILED(result))
            throw std::runtime_error("Could not set the application Windows AppUserModelID.");
    }

#endif
    _impl->registered = true;
}

void InfiniFrameApplication::Configure(const InfiniFrameApplicationInitParams& parameters) {
    std::lock_guard lock(_impl->mutex);
    _impl->webView2RuntimePath = parameters.WebView2RuntimePath == nullptr ? "" : parameters.WebView2RuntimePath;
    _impl->notificationRegistrationId = parameters.NotificationRegistrationId == nullptr ? "" : parameters.NotificationRegistrationId;
    _impl->appUserModelId = parameters.AppUserModelId == nullptr ? "" : parameters.AppUserModelId;
    _impl->defaultNotificationIcon = parameters.DefaultNotificationIcon == nullptr ? "" : parameters.DefaultNotificationIcon;
}

#ifdef _WIN32
void InfiniFrameApplication::EnsureNotificationsInitialized(const char* appName) {
    std::lock_guard lock(_impl->mutex);
    if (_impl->notificationsRegistered) return;
    const std::string identity = !_impl->notificationRegistrationId.empty()
        ? _impl->notificationRegistrationId
        : !_impl->appUserModelId.empty() ? _impl->appUserModelId
        : appName != nullptr && appName[0] != '\0' ? appName
        : "InfiniFrame";

    const char* effectiveAppName = appName != nullptr && appName[0] != '\0' ? appName : identity.c_str();
    const std::wstring windowsAppName = ToWindowsString(effectiveAppName);
    const std::wstring windowsIdentity = ToWindowsString(identity.c_str());
    WinToastLib::setDebugOutputEnabled(false);
    WinToastLib::WinToast::instance()->setAppName(windowsAppName);
    WinToastLib::WinToast::instance()->setAppUserModelId(windowsIdentity);
    if (!WinToastLib::WinToast::instance()->initialize())
        throw std::runtime_error("Could not initialize application notifications.");
    _impl->notificationsRegistered = true;
}
#endif

void InfiniFrameApplication::Run() noexcept {
#ifdef _WIN32
    MSG message = {};
    PeekMessage(&message, nullptr, WM_USER, WM_USER, PM_NOREMOVE);

    {
        std::lock_guard lock(_impl->mutex);
        _impl->runThreadId = GetCurrentThreadId();
        _impl->running = true;
        if (_impl->windows.empty() || _impl->shutdownRequested) {
            _impl->running = false;
            return;
        }
    }

    while (true) {
        {
            std::lock_guard lock(_impl->mutex);
            if (_impl->shutdownRequested) break;
        }

        MsgWaitForMultipleObjectsEx(0, nullptr, 50, QS_ALLINPUT, MWMO_INPUTAVAILABLE);
        while (PeekMessage(&message, nullptr, 0, 0, PM_REMOVE)) {
            if (message.message == WM_QUIT) {
                std::lock_guard lock(_impl->mutex);
                _impl->shutdownRequested = true;
                break;
            }
            TranslateMessage(&message);
            DispatchMessage(&message);
        }
    }

    std::lock_guard lock(_impl->mutex);
    _impl->running = false;
    _impl->runThreadId = 0;
#else
    // Other platforms retain their existing loop until their application
    // lifecycle integrations are implemented.
#endif
}

void InfiniFrameApplication::Shutdown() noexcept {
    std::lock_guard lock(_impl->mutex);
    _impl->shutdownRequested = true;
#ifdef _WIN32
    if (_impl->running && _impl->runThreadId != 0)
        PostThreadMessage(_impl->runThreadId, WM_QUIT, 0, 0);
#endif
}

void InfiniFrameApplication::TrackWindow(InfiniFrameWindow* window) {
    if (window == nullptr) return;
    std::lock_guard lock(_impl->mutex);
    _impl->windows.insert(window);
    _impl->closedWindows.erase(window);
}

void InfiniFrameApplication::UntrackWindow(InfiniFrameWindow* window) noexcept {
    if (window == nullptr) return;
    std::lock_guard lock(_impl->mutex);
    _impl->windows.erase(window);
    _impl->closedWindows.erase(window);
}

void InfiniFrameApplication::NotifyWindowClosed(InfiniFrameWindow* window) noexcept {
    if (window == nullptr) return;

    std::lock_guard lock(_impl->mutex);
    if (_impl->windows.contains(window))
        _impl->closedWindows.insert(window);
#ifdef _WIN32
    if (_impl->running && !_impl->windows.empty() && _impl->closedWindows.size() == _impl->windows.size())
        PostThreadMessage(_impl->runThreadId, WM_QUIT, 0, 0);
#endif
}

std::size_t InfiniFrameApplication::GetWindowCount() const noexcept {
    std::lock_guard lock(_impl->mutex);
    return _impl->windows.size();
}

const char* InfiniFrameApplication::GetWebView2RuntimePath() const noexcept {
    thread_local std::string value;
    std::lock_guard lock(_impl->mutex);
    value = _impl->webView2RuntimePath;
    return value.c_str();
}

const char* InfiniFrameApplication::GetNotificationRegistrationId() const noexcept {
    thread_local std::string value;
    std::lock_guard lock(_impl->mutex);
    value = _impl->notificationRegistrationId;
    return value.c_str();
}

const char* InfiniFrameApplication::GetAppUserModelId() const noexcept {
    thread_local std::string value;
    std::lock_guard lock(_impl->mutex);
    value = _impl->appUserModelId;
    return value.c_str();
}

const char* InfiniFrameApplication::GetDefaultNotificationIcon() const noexcept {
    thread_local std::string value;
    std::lock_guard lock(_impl->mutex);
    value = _impl->defaultNotificationIcon;
    return value.c_str();
}

bool InfiniFrameApplication::HasNotificationRegistration() const noexcept {
    return _impl->notificationsRegistered;
}
