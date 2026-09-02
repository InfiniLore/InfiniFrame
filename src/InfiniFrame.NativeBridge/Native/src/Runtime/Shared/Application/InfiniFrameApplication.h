#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef _WIN32
#include <Windows.h>
#endif
#ifdef __APPLE__
#include <Cocoa/Cocoa.h>
#endif
#ifdef __linux__
#include <gtk/gtk.h>
#endif

#include <memory>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
struct ApplicationInitParams;
class InfiniFrameWindow;

/**
 * @brief Application-level singleton managing platform registration, message loop, and window collection.
 *
 * One instance per process. Must be created before any windows and destroyed after all windows.
 * Uses PIMPL idiom for platform-specific state encapsulation.
 */
class InfiniFrameApplication {
    public:
    /**
         * @brief Construct a new InfiniFrameApplication.
         * @param params Application initialization parameters.
         */
    explicit InfiniFrameApplication(ApplicationInitParams* params);

    /**
         * @brief Destroy InfiniFrameApplication.
         */
    ~InfiniFrameApplication();

    // ── Singleton access ───────────────────────────────────────────────
    /// Get the global application instance (null if none exists).
    [[nodiscard]] static InfiniFrameApplication* GetInstance();

    // ── Platform registration (one-time, called before any windows) ──────
#ifdef _WIN32
    /// Register the Win32 window class and set DPI awareness.
    /// @param hInstance The application instance handle.
    void Register(HINSTANCE hInstance);

    /// Get the stored HINSTANCE.
    /// @return The HINSTANCE passed to Register().
    [[nodiscard]] HINSTANCE GetHInstance() const;

    /// Get the AppUserModelId set during construction.
    [[nodiscard]] const std::wstring& GetAppUserModelId() const;

    /// Get the notification registration ID set during construction.
    [[nodiscard]] const std::wstring& GetNotificationRegistrationId() const;

    /// Get the WebView2 runtime path set during construction.
    [[nodiscard]] const std::wstring& GetWebView2RuntimePath() const;
#endif

#ifdef __APPLE__
    /// Set up NSApplication delegate and activation policy.
    void Register();
#endif

    // ── Message loop ──────────────────────────────────────────────────────
    /// Block until all windows are closed or Shutdown() is called. Runs the platform event loop.
    void Run();

    /// Signal the message loop to exit. Safe to call from any thread.
    void Shutdown();

    // ── Window management ─────────────────────────────────────────────────
    /// Track a window as owned by this application.
    void TrackWindow(InfiniFrameWindow* window);

    /// Remove a window from tracking. If no windows remain, triggers Shutdown().
    void UntrackWindow(InfiniFrameWindow* window);

    /// Check if any windows are still tracked.
    [[nodiscard]] bool HasWindows() const;

    // ── Process-wide state ────────────────────────────────────────────────
    /// Check if Shutdown() has been called.
    [[nodiscard]] bool IsShutdownRequested() const;

    private:
    struct Impl;
    std::unique_ptr<Impl> m_impl;
    static InfiniFrameApplication* s_instance;

    friend class InfiniFrameWindow;
};
