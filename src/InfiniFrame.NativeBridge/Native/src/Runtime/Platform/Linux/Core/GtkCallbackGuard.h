#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <exception>
#include <utility>

#include <glib.h>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::linux_gtk {
    template <typename F>
    void RunGtkCallbackNoThrow(const char* callbackName, F&& callback) noexcept {
        try {
            std::forward<F>(callback)();
        } catch (const std::exception& ex) {
            g_warning("[InfiniFrame/Linux] %s failed: %s", callbackName, ex.what());
        } catch (...) {
            g_warning("[InfiniFrame/Linux] %s failed with an unknown native exception.", callbackName);
        }
    }

    template <typename T, typename F>
    T RunGtkCallbackNoThrow(const char* callbackName, T fallback, F&& callback) noexcept {
        try {
            return std::forward<F>(callback)();
        } catch (const std::exception& ex) {
            g_warning("[InfiniFrame/Linux] %s failed: %s", callbackName, ex.what());
        } catch (...) {
            g_warning("[InfiniFrame/Linux] %s failed with an unknown native exception.", callbackName);
        }

        return fallback;
    }
}
