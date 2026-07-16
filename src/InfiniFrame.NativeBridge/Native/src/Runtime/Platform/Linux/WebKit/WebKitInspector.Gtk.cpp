// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <cerrno>
#include <cstring>
#include <stdexcept>
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
}

void InfiniFrameWindow::Impl::configure_webkit_remote_debugging() const {
    if (_remoteDebuggingPort <= 0) {
        g_unsetenv(InspectorServerEnvVar);
        g_unsetenv(InspectorHttpServerEnvVar);

        g_message("[InfiniFrame/Linux] Remote debugging disabled.");
        return;
    }

    std::string binding = BuildInspectorBinding(_remoteDebuggingPort);
    if (!g_setenv(InspectorServerEnvVar, binding.c_str(), TRUE))
        ThrowEnvMutationFailure("set", InspectorServerEnvVar);
    if (!g_setenv(InspectorHttpServerEnvVar, binding.c_str(), TRUE))
        ThrowEnvMutationFailure("set", InspectorHttpServerEnvVar);

    g_message(
        "[InfiniFrame/Linux] Remote debugging enabled on loopback %s (env: %s, %s).",
        binding.c_str(),
        InspectorServerEnvVar,
        InspectorHttpServerEnvVar
    );
}
