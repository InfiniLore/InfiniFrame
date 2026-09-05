#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <cstddef>
#include <mutex>
#include <stdexcept>
#include <string>
#include <unordered_set>

class InfiniFrameWindow;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/**
 * @brief Process-scoped owner and registry for native InfiniFrame windows.
 *
 * The registry is independent of the platform message loop so the existing
 * window ABI can remain compatible while application-loop ownership is added.
 */
class InfiniFrameApplication {
    public:
    InfiniFrameApplication();
    ~InfiniFrameApplication();

    InfiniFrameApplication(const InfiniFrameApplication&) = delete;
    InfiniFrameApplication& operator=(const InfiniFrameApplication&) = delete;

    [[nodiscard]] static InfiniFrameApplication* GetInstance() noexcept;
    void Register();
    void Configure(
        const char* webView2RuntimePath,
        const char* notificationRegistrationId,
        const char* appUserModelId,
        const char* defaultNotificationIcon
    );
    void Run() noexcept;
    void Shutdown() noexcept;
    void TrackWindow(InfiniFrameWindow* window);
    void UntrackWindow(InfiniFrameWindow* window) noexcept;
    void NotifyWindowClosed(InfiniFrameWindow* window) noexcept;
    [[nodiscard]] std::size_t GetWindowCount() const noexcept;
    [[nodiscard]] const char* GetWebView2RuntimePath() const noexcept;
    [[nodiscard]] const char* GetNotificationRegistrationId() const noexcept;
    [[nodiscard]] const char* GetAppUserModelId() const noexcept;
    [[nodiscard]] const char* GetDefaultNotificationIcon() const noexcept;

    private:
    static InfiniFrameApplication* s_instance;
    mutable std::mutex _mutex;
    std::unordered_set<InfiniFrameWindow*> _windows;
    bool _registered = false;
    bool _shutdownRequested = false;
    std::string _webView2RuntimePath;
    std::string _notificationRegistrationId;
    std::string _appUserModelId;
    std::string _defaultNotificationIcon;
#ifdef _WIN32
    unsigned long _runThreadId = 0;
    bool _running = false;
#endif
};
