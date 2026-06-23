// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <cerrno>
#include <cstring>
#include <map>
#include <mutex>
#include <string>

#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    constexpr const char* LoopbackAddress = "127.0.0.1";
    constexpr const char* InspectorServerEnvVar = "WEBKIT_INSPECTOR_SERVER";
    constexpr const char* InspectorHttpServerEnvVar = "WEBKIT_INSPECTOR_HTTP_SERVER";

    std::string BuildInspectorBinding(const int port) {
        return std::string{LoopbackAddress} + ":" + std::to_string(port);
    }

    [[noreturn]] void ThrowEnvMutationFailure(const char* operation, const char* variableName) {
        throw std::runtime_error(
            std::string{"Failed to "} + operation + " " + variableName + " for Linux remote debugging: " + std::strerror(errno)
        );
    }

    void UnsetInspectorEnvironment() {
        g_unsetenv(InspectorServerEnvVar);
        g_unsetenv(InspectorHttpServerEnvVar);
    }

    class WebKitInspectorConcurrencyManager {
    public:
        void acquire(InfiniFrameWindow::Impl* window, const int port) {
            if (port <= 0) {
                std::lock_guard lock(_mutex);
                if (_activeWindows.empty())
                    UnsetInspectorEnvironment();

                g_message("[InfiniFrame/Linux] Remote debugging disabled.");
                return;
            }

            std::lock_guard lock(_mutex);

            if (const auto existing = _activeWindows.find(window); existing != _activeWindows.end()) {
                if (existing->second == port)
                    return;

                g_warning(
                    "[InfiniFrame/Linux] Remote debugging conflict for window %p: already registered on port %d, requested port %d.",
                    static_cast<void*>(window),
                    existing->second,
                    port
                );
                throw std::runtime_error("Linux remote debugging port changed while the WebKit inspector is active.");
            }

            if (_activePort != 0 && _activePort != port) {
                g_warning(
                    "[InfiniFrame/Linux] Remote debugging conflict: active inspector port %d, requested port %d for window %p.",
                    _activePort,
                    port,
                    static_cast<void*>(window)
                );
                throw std::runtime_error(
                    "Linux WebKit remote debugging only supports one inspector port per process while inspectors are active."
                );
            }

            const std::string binding = BuildInspectorBinding(port);
            if (_activeWindows.empty()) {
                if (!g_setenv(InspectorServerEnvVar, binding.c_str(), TRUE))
                    ThrowEnvMutationFailure("set", InspectorServerEnvVar);
                if (!g_setenv(InspectorHttpServerEnvVar, binding.c_str(), TRUE))
                    ThrowEnvMutationFailure("set", InspectorHttpServerEnvVar);
                _activePort = port;
            }

            _activeWindows.emplace(window, port);
            window->_webkitRemoteDebuggingRegistered = true;

            g_message(
                "[InfiniFrame/Linux] Remote debugging enabled on loopback %s for window %p (%zu active, env: %s, %s).",
                binding.c_str(),
                static_cast<void*>(window),
                _activeWindows.size(),
                InspectorServerEnvVar,
                InspectorHttpServerEnvVar
            );
        }

        void release(InfiniFrameWindow::Impl* window) noexcept {
            try {
                std::lock_guard lock(_mutex);

                if (_activeWindows.erase(window) == 0)
                    return;

                window->_webkitRemoteDebuggingRegistered = false;

                if (_activeWindows.empty()) {
                    _activePort = 0;
                    UnsetInspectorEnvironment();
                    g_message("[InfiniFrame/Linux] Remote debugging inspector environment released.");
                    return;
                }

                g_message(
                    "[InfiniFrame/Linux] Remote debugging remains active on port %d for %zu window(s).",
                    _activePort,
                    _activeWindows.size()
                );
            }
            catch (...) {
                g_warning("[InfiniFrame/Linux] Ignoring failure while releasing remote debugging inspector state.");
            }
        }

    private:
        std::mutex _mutex;
        std::map<InfiniFrameWindow::Impl*, int> _activeWindows;
        int _activePort = 0;
    };

    WebKitInspectorConcurrencyManager& InspectorManager() {
        static WebKitInspectorConcurrencyManager manager;
        return manager;
    }
}

void InfiniFrameWindow::Impl::configure_webkit_remote_debugging() {
    InspectorManager().acquire(this, _remoteDebuggingPort);
}

void InfiniFrameWindow::Impl::release_webkit_remote_debugging() noexcept {
    if (!_webkitRemoteDebuggingRegistered)
        return;

    InspectorManager().release(this);
}
