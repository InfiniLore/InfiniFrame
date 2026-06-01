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

#include "Public/InfiniFrame.h"
#include "Utils/InteropStatus.h"
#include "Public/Exports/Utilities/ExportStringHelpers.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::exports {
    inline thread_local std::string g_lastErrorMessage;
    inline thread_local auto g_lastStatus = InteropStatus::Success;

    inline void SetLastErrorCode(const InteropStatus status) noexcept {
#ifdef _WIN32
        SetLastError(static_cast<DWORD>(status));
#else
        errno = static_cast<int>(status);
#endif
    }

    inline void ClearLastErrorCode() noexcept {
#ifdef _WIN32
        SetLastError(0);
#else
        errno = 0;
#endif
    }

    inline void SetFailure(const InteropStatus status, std::string message) noexcept {
        g_lastErrorMessage = std::move(message);
        g_lastStatus = status;
        SetLastErrorCode(status);
    }

    inline void SetSuccess() noexcept {
        g_lastErrorMessage.clear();
        g_lastStatus = InteropStatus::Success;
        ClearLastErrorCode();
    }

    inline InteropStatus TranslateException(const std::exception& ex) noexcept {
        if (dynamic_cast<const std::invalid_argument*>(&ex) != nullptr) {
            SetFailure(InteropStatus::InvalidArgument, ex.what());
            return InteropStatus::InvalidArgument;
        }

        SetFailure(InteropStatus::OperationFailed, ex.what());
        return InteropStatus::OperationFailed;
    }

    inline AutoString GetLastErrorMessageCopy() {
        return AllocateErrorMessageString(g_lastErrorMessage);
    }
}