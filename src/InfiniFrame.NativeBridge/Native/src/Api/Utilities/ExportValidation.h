#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef __linux__
#include <glib.h>
#endif

#ifdef _WIN32
#include <Windows.h>
#endif

#include <cerrno>
#include <exception>
#include <stdexcept>
#include <string>
#include <utility>

#include "Runtime/Shared/Window/InfiniFrame.h"
#include "Runtime/Shared/Utilities/InteropStatus.h"
#include "ExportErrorState.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::exports {
    /// Reset an output parameter to a fallback value if the pointer is non-null.
    /// @tparam T Value type.
    /// @param outValue Pointer to the output parameter (may be null).
    /// @param fallback Value to assign (default-constructed if omitted).
    template <typename T> void ResetOut(T* outValue, const T fallback = {}) noexcept {
        if (outValue != nullptr) {
            *outValue = fallback;
        }
    }

    /// Reset two output parameters to a fallback value if their pointers are non-null.
    /// @tparam T Value type.
    /// @param first Pointer to the first output parameter (may be null).
    /// @param second Pointer to the second output parameter (may be null).
    /// @param fallback Value to assign (default-constructed if omitted).
    template <typename T> void ResetOut2(T* first, T* second, const T fallback = {}) noexcept {
        ResetOut(first, fallback);
        ResetOut(second, fallback);
    }

    /// Assert that a pointer is non-null, recording a failure if it is null.
    /// @tparam T Pointee type.
    /// @param value The pointer to check.
    /// @param argumentName Human-readable name of the argument for error messages.
    /// @param status InteropStatus to set on failure.
    /// @return true if non-null, false if null (failure has been recorded).
    template <typename T> bool EnsureNotNull(
        T* value,
        const char* argumentName,
        const InteropStatus status = InteropStatus::InvalidArgument
        ) noexcept {
        if (value != nullptr) {
            return true;
        }

        SetFailure(status, std::string("Argument '") + argumentName + "' is null.");
        return false;
    }

    /// Assert that an output parameter pointer is non-null, recording a failure if it is null.
    /// Uses InteropStatus::OutParameterSetToInvalidNull as the error code.
    /// @tparam T Pointee type.
    /// @param value The output pointer to check.
    /// @param argumentName Human-readable name of the argument for error messages.
    /// @return true if non-null, false if null (failure has been recorded).
    template <typename T> bool EnsureOutNotNull(T* value, const char* argumentName) noexcept {
        return exports::EnsureNotNull(value, argumentName, InteropStatus::OutParameterSetToInvalidNull);
    }
}
