// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Application/InfiniFrameApplication.h"
#include "Runtime/Shared/Application/InfiniFrameApplicationImpl.h"
#include "Runtime/Shared/Application/ApplicationInitParams.h"
#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
InfiniFrameApplication::InfiniFrameApplication(ApplicationInitParams* params) {
    m_impl = std::make_unique<Impl>();
    infiniframe::linux_gtk::ui_thread::EnsureInitialized();
}

InfiniFrameApplication::~InfiniFrameApplication() {
    infiniframe::linux_gtk::ui_thread::Shutdown();
}

void InfiniFrameApplication::TrackWindow(InfiniFrameWindow* window) {
    std::lock_guard lock(m_impl->_windowListMutex);
    m_impl->_windows.push_back(window);
}

void InfiniFrameApplication::UntrackWindow(InfiniFrameWindow* window) {
    std::lock_guard lock(m_impl->_windowListMutex);
    auto it = std::remove(m_impl->_windows.begin(), m_impl->_windows.end(), window);
    m_impl->_windows.erase(it, m_impl->_windows.end());

    if (m_impl->_windows.empty() && !m_impl->_shutdownRequested.load(std::memory_order_acquire)) {
        Shutdown();
    }
}

bool InfiniFrameApplication::HasWindows() const {
    std::lock_guard lock(m_impl->_windowListMutex);
    return !m_impl->_windows.empty();
}

bool InfiniFrameApplication::IsShutdownRequested() const {
    return m_impl->_shutdownRequested.load(std::memory_order_acquire);
}
