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

    template <typename T> void ResetOut(T* outValue, const T fallback = {}) noexcept {
        if (outValue != nullptr) {
            *outValue = fallback;
        }
    }

    template <typename T> void ResetOut2(T* first, T* second, const T fallback = {}) noexcept {
        ResetOut(first, fallback);
        ResetOut(second, fallback);
    }

    template <typename T> bool EnsureNotNull(
        T* value, const char* argumentName, const InteropStatus status = InteropStatus::InvalidArgument
    ) noexcept {
        if (value != nullptr) {
            return true;
        }

        SetFailure(status, std::string("Argument '") + argumentName + "' is null.");
        return false;
    }

    template <typename T> bool EnsureOutNotNull(T* value, const char* argumentName) noexcept {
        return exports::EnsureNotNull(value, argumentName, InteropStatus::OutParameterSetToInvalidNull);
    }
} // namespace infiniframe::exports