#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <cstddef>
#include <mutex>
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
    void TrackWindow(InfiniFrameWindow* window);
    void UntrackWindow(InfiniFrameWindow* window) noexcept;
    [[nodiscard]] std::size_t GetWindowCount() const noexcept;

    private:
    static InfiniFrameApplication* s_instance;
    mutable std::mutex _mutex;
    std::unordered_set<InfiniFrameWindow*> _windows;
};
