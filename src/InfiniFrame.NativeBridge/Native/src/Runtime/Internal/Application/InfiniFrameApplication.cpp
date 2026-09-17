// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Internal/Application/InfiniFrameApplication.h"
#include <atomic>
#include <mutex>
#include <string>
#include <vector>
#include <unordered_set>
#ifdef __APPLE__
#include <Cocoa/Cocoa.h>
#include <condition_variable>
#include "Runtime/Internal/Interop/Types/InfiniFrameWindow.h"
#include "Runtime/Platform/Mac/Window.Cocoa.Internal.h"
#endif
#ifdef __linux__
#include <condition_variable>
#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
#endif
#ifdef _WIN32
#include <windows.h>
#include <shobjidl_core.h>
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
#include "Dependencies/wintoastlib/wintoastlib.h"
#endif
#ifdef __linux__
#include "Runtime/Internal/Interop/Types/InfiniFrameWindow.h"
#endif

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
struct InfiniFrameApplicationImpl {
    mutable std::mutex mutex;
    std::unordered_set<InfiniFrameWindow*> windows;
    std::unordered_set<InfiniFrameWindow*> liveWindows;
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
#ifdef __linux__
    bool running = false;
    std::condition_variable runCompleted;
#endif
#ifdef __APPLE__
    bool running = false;
    std::condition_variable runCompleted;
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
#ifdef __APPLE__
    // NSApplication and WKWebView are process-scoped.  The logical application can
    // be recreated, but all pooled hosts must be released before its native handle
    // disappears so callbacks cannot outlive the application owner.
    if ([NSThread isMainThread]) {
        DrainPooledMacHosts();
    } else {
        dispatch_sync(dispatch_get_main_queue(), ^{
            DrainPooledMacHosts();
        });
    }
#endif
    std::lock_guard lock(_impl->mutex);
    _impl->windows.clear();
    _impl->liveWindows.clear();
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
#ifdef __linux__
    // GTK/WebKit must be initialized by the application, not by the first
    // window. This also establishes the owner thread before construction.
    infiniframe::linux_gtk::ui_thread::EnsureInitialized();
#endif
#ifdef __APPLE__
    // NSApplication is initialized exactly once for the process, while the
    // InfiniFrame application registry remains per application instance.
    InfiniFrameWindow::Register();
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
        // Shutdown is a drain request, not permission to abandon HWNDs. A close can
        // be deferred while WebView2 is creating its controller, so the message pump
        // must continue until every tracked window has reached WM_DESTROY.
        if (_impl->windows.empty()) {
            _impl->running = false;
            return;
        }
    }

    while (true) {
        {
            std::lock_guard lock(_impl->mutex);
            if (_impl->shutdownRequested && _impl->windows.empty()) break;
        }

        MsgWaitForMultipleObjectsEx(0, nullptr, 50, QS_ALLINPUT, MWMO_INPUTAVAILABLE);
        while (PeekMessage(&message, nullptr, 0, 0, PM_REMOVE)) {
            if (message.message == WM_QUIT) {
                std::vector<InfiniFrameWindow*> windows;
                {
                    std::lock_guard lock(_impl->mutex);
                    _impl->shutdownRequested = true;
                    windows.assign(_impl->windows.begin(), _impl->windows.end());
                }
                // WM_QUIT does not dispatch to HWNDs. Post WM_CLOSE so normal
                // teardown drains every tracked window before returning.
                for (InfiniFrameWindow* window : windows)
                    window->Close();
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
#ifdef __linux__
    {
        std::unique_lock lock(_impl->mutex);
        _impl->running = true;
        if (_impl->windows.empty()) {
            _impl->running = false;
            return;
        }
        lock.unlock();
    }
    infiniframe::linux_gtk::ui_thread::EnsureInitialized();
    {
        std::unique_lock lock(_impl->mutex);
        _impl->runCompleted.wait(lock, [this] { return _impl->windows.empty(); });
        _impl->running = false;
    }
#endif
#ifdef __APPLE__
    {
        std::unique_lock lock(_impl->mutex);
        _impl->running = true;
        if (_impl->windows.empty()) {
            _impl->running = false;
            return;
        }
    }

    auto stopWhenComplete = [this] {
        std::unique_lock lock(_impl->mutex);
        _impl->runCompleted.wait(lock, [this] { return _impl->windows.empty() || _impl->liveWindows.empty(); });
        _impl->running = false;
    };

    if ([NSThread isMainThread]) {
        [NSApp run];
        stopWhenComplete();
    } else {
        dispatch_async(dispatch_get_main_queue(), ^{
            if ([NSApp isRunning]) return;
            [NSApp run];
        });
        stopWhenComplete();
    }
#endif
#endif
}

void InfiniFrameApplication::Shutdown() noexcept {
    std::vector<InfiniFrameWindow*> windows;
    {
        std::lock_guard lock(_impl->mutex);
        _impl->shutdownRequested = true;
        windows.assign(_impl->windows.begin(), _impl->windows.end());
    }
#ifdef _WIN32
    {
        std::lock_guard lock(_impl->mutex);
        if (_impl->runThreadId != 0) {
        PostThreadMessage(_impl->runThreadId, WM_QUIT, 0, 0);
        }
    }
#elif defined(__linux__)
    (void)windows;
#elif defined(__APPLE__)
    dispatch_async(dispatch_get_main_queue(), ^{
        for (InfiniFrameWindow* window : windows)
            window->Close();
    });
#endif
}

void InfiniFrameApplication::TrackWindow(InfiniFrameWindow* window) {
    if (window == nullptr) return;
    std::lock_guard lock(_impl->mutex);
    _impl->windows.insert(window);
    _impl->liveWindows.insert(window);
}

void InfiniFrameApplication::UntrackWindow(InfiniFrameWindow* window) noexcept {
    if (window == nullptr) return;
    bool becameEmpty = false;
    {
        std::lock_guard lock(_impl->mutex);
        _impl->windows.erase(window);
        _impl->liveWindows.erase(window);
        becameEmpty = _impl->windows.empty();
    }
#ifdef __linux__
    // The native window destructor is the ownership boundary. Detach only
    // after it has unregistered itself so the GTK loop cannot be joined while
    // managed teardown still owns the native window.
    window->DetachApplication();
    if (becameEmpty) _impl->runCompleted.notify_all();
#endif
#ifdef __APPLE__
    // Deferred destruction can outlive the logical close, but never let the
    // window retain a pointer to an application that has unregistered it.
    window->DetachApplication();
    if (becameEmpty) _impl->runCompleted.notify_all();
#endif
#ifndef __linux__
    (void)becameEmpty;
#endif
}

void InfiniFrameApplication::NotifyWindowClosed(InfiniFrameWindow* window) noexcept {
    if (window == nullptr) return;

    std::lock_guard lock(_impl->mutex);
    _impl->liveWindows.erase(window);
#ifdef _WIN32
    _impl->windows.erase(window);
    window->DetachApplication();
    if (_impl->running && _impl->liveWindows.empty())
        PostThreadMessage(_impl->runThreadId, WM_QUIT, 0, 0);
#endif
#ifdef __linux__
    // Keep the registry entry until the native destructor calls
    // UntrackWindow. This lets managed teardown complete before the GTK loop
    // is joined, while the live-window count reflects the GTK destroy event.
#endif
#ifdef __APPLE__
    // Keep the C++ object registered until deferred destruction has completed,
    // but remove it from the live set so the final logical close ends Run().
    if (_impl->running && _impl->liveWindows.empty()) {
        _impl->runCompleted.notify_all();
        dispatch_async(dispatch_get_main_queue(), ^{
            if ([NSApp isRunning]) {
                [NSApp stop:nil];
                [NSApp postEvent:[NSEvent otherEventWithType:NSEventTypeApplicationDefined
                                                     location:NSZeroPoint
                                                modifierFlags:0 timestamp:0 windowNumber:0
                                                      context:nil subtype:0 data1:0 data2:0]
                          atStart:NO];
            }
        });
    }
#endif
}

std::size_t InfiniFrameApplication::GetWindowCount() const noexcept {
    std::lock_guard lock(_impl->mutex);
    return _impl->liveWindows.size();
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
