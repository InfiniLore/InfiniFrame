#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <cstddef>
#include <memory>

#include "Runtime/Internal/Interop/InfiniFrameApplicationInitParams.h"

class InfiniFrameWindow;
struct InfiniFrameApplicationImpl;
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
    void Configure(const InfiniFrameApplicationInitParams& parameters);
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
    [[nodiscard]] bool HasNotificationRegistration() const noexcept;
    void EnsureNotificationsInitialized(const char* appName);

    private:
    std::unique_ptr<InfiniFrameApplicationImpl> _impl;
};
