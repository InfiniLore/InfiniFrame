#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::macos {
    using MainRunLoopWork = void (^)();

    class NativeCallbackScope final {
    public:
        NativeCallbackScope() noexcept;
        ~NativeCallbackScope() noexcept;
        NativeCallbackScope(const NativeCallbackScope&) = delete;
        NativeCallbackScope& operator=(const NativeCallbackScope&) = delete;
    };

    void InstallDiagnostics() noexcept;
    void LogLifecycle(const char* event, const void* instance) noexcept;
    bool IsInsideNativeCallback() noexcept;
    void WaitForNativeCallbacksToExit() noexcept;

    /**
     * Queues WebKit view removal on the AppKit run loop, one operation at a time.
     * WebKit's display-link observer bookkeeping is not safe when several WKWebViews are
     * detached during the same refresh cycle.
     */
    void EnqueueWebKitTeardown(MainRunLoopWork work) noexcept;
}
