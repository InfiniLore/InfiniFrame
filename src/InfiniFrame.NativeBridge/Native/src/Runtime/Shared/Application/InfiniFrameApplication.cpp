// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Application/InfiniFrameApplication.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
InfiniFrameApplication* InfiniFrameApplication::s_instance = nullptr;

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

std::size_t InfiniFrameApplication::GetWindowCount() const noexcept {
    std::lock_guard lock(_mutex);
    return _windows.size();
}
