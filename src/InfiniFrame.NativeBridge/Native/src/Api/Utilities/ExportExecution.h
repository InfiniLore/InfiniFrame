#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <exception>
#include <utility>

#include "Runtime/Shared/Window/InfiniFrame.h"
#include "Runtime/Shared/Utilities/InteropStatus.h"
#include "ExportErrorState.h"
#include "ExportValidation.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::exports {
    /// Execute a callable, catching all exceptions and translating them to an InteropStatus.
    /// @tparam Fn Callable type.
    /// @param fn The callable to execute.
    /// @return InteropStatus::Success on success, or an error status on failure.
    template <typename Fn> InteropStatus RunExportStatus(Fn&& fn) noexcept {
        try {
            SetSuccess();
            std::forward<Fn>(fn)();
            if (g_lastStatus != InteropStatus::Success) {
                return g_lastStatus;
            }
            SetSuccess();
            return InteropStatus::Success;
        } catch (const std::exception& ex) {
            return TranslateException(ex);
        } catch (...) {
            SetFailure(InteropStatus::OperationFailed, "Unknown native exception.");
            return InteropStatus::OperationFailed;
        }
    }

    /// Execute a callable that requires a valid window instance, with null-check and exception safety.
    /// @tparam Fn Callable type.
    /// @param instance Pointer to the window (must be non-null).
    /// @param fn The callable receiving the validated window pointer.
    /// @return InteropStatus::Success on success, or an error status on failure.
    template <typename Fn> InteropStatus RunWindowExportStatus(InfiniFrameWindow* instance, Fn&& fn) noexcept {
        return RunExportStatus(
            [&] {
                if (!EnsureNotNull(instance, "instance")) {
                    return;
                }

                std::forward<Fn>(fn)(instance);
            });
    }

    /// Execute a callable that returns a value, with window null-check and exception safety.
    /// @tparam T Return type.
    /// @tparam Fn Callable type.
    /// @param instance Pointer to the window (must be non-null).
    /// @param fallback Value returned on error.
    /// @param fn The callable receiving the validated window pointer and returning T.
    /// @return The value returned by @p fn, or @p fallback on error.
    template <typename T, typename Fn>
    T RunWindowReturnExport(InfiniFrameWindow* instance, T fallback, Fn&& fn) noexcept {
        try {
            if (!EnsureNotNull(instance, "instance")) {
                return fallback;
            }

            T value = std::forward<Fn>(fn)(instance);
            SetSuccess();
            return value;
        } catch (const std::exception& ex) {
            TranslateException(ex);
            return fallback;
        } catch (...) {
            SetFailure(InteropStatus::OperationFailed, "Unknown native exception.");
            return fallback;
        }
    }

    /// Execute a callable that returns a value, with exception safety.
    /// @tparam T Return type.
    /// @tparam Fn Callable type.
    /// @param fallback Value returned on error.
    /// @param fn The callable returning T.
    /// @return The value returned by @p fn, or @p fallback on error.
    template <typename T, typename Fn> T RunReturnExport(T fallback, Fn&& fn) noexcept {
        try {
            T value = std::forward<Fn>(fn)();
            SetSuccess();
            return value;
        } catch (const std::exception& ex) {
            TranslateException(ex);
            return fallback;
        } catch (...) {
            SetFailure(InteropStatus::OperationFailed, "Unknown native exception.");
            return fallback;
        }
    }
}
