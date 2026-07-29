#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::macos {
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

}
