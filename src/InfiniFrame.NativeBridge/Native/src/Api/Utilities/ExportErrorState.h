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
#include "Api/Utilities/ExportStringHelpers.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::exports {
    /// Thread-local storage for the last error message.
    extern thread_local std::string g_lastErrorMessage;
    /// Thread-local storage for the last interop status code.
    extern thread_local InteropStatus g_lastStatus;

    /// Write an InteropStatus to the platform last-error slot (SetLastError on Win32, errno elsewhere).
    /// @param status The status code to store.
    inline void SetLastErrorCode(const InteropStatus status) noexcept {
#ifdef _WIN32
        SetLastError(static_cast<DWORD>(status));
#else
        errno = static_cast<int>(status);
#endif
    }

    /// Clear the platform last-error slot (SetLastError on Win32, errno elsewhere).
    inline void ClearLastErrorCode() noexcept {
#ifdef _WIN32
        SetLastError(0);
#else
        errno = 0;
#endif
    }

    /// Record a failure status and message, and propagate the status to the platform last-error slot.
    /// @param status The failure status code.
    /// @param message Human-readable error description.
    inline void SetFailure(const InteropStatus status, std::string message) noexcept {
        g_lastErrorMessage = std::move(message);
        g_lastStatus = status;
        SetLastErrorCode(status);
    }

    /// Clear the error state and mark the last status as success.
    inline void SetSuccess() noexcept {
        g_lastErrorMessage.clear();
        g_lastStatus = InteropStatus::Success;
        ClearLastErrorCode();
    }

    /// Translate a caught std::exception into an InteropStatus, recording the error message.
    /// Maps std::invalid_argument to InteropStatus::InvalidArgument; all others to OperationFailed.
    /// @param ex The caught exception.
    /// @return The corresponding InteropStatus code.
    inline InteropStatus TranslateException(const std::exception& ex) noexcept {
        if (dynamic_cast<const std::invalid_argument*>(&ex) != nullptr) {
            SetFailure(InteropStatus::InvalidArgument, ex.what());
            return InteropStatus::InvalidArgument;
        }

        SetFailure(InteropStatus::OperationFailed, ex.what());
        return InteropStatus::OperationFailed;
    }

    /// Allocate a copy of the last error message as a C string. Caller owns the result.
    /// @return Newly allocated UTF-8 string, or nullptr if no error message is set.
    inline const char* GetLastErrorMessageCopy() {
        return AllocateErrorMessageString(g_lastErrorMessage);
    }
}
