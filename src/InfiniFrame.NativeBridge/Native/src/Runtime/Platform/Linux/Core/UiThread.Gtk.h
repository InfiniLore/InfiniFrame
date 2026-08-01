#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <functional>

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::linux_gtk::ui_thread {
    void EnsureInitialized();
    bool IsCurrentThread();
    bool InvokeAsync(std::function<void()> callback);
    bool InvokeIdle(std::function<void()> callback);
    void InvokeSync(std::function<void()> callback);
}
