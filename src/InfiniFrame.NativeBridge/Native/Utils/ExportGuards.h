#pragma once

#include <cerrno>
#include <exception>
#include <stdexcept>
#include <string>
#include <utility>

#include "../Public/InfiniFrame.h"

#ifdef _WIN32
#include <Windows.h>
#endif

enum class InteropStatus : int {
    Success = 0,
    InvalidArgument = 22,
    OutParameterSetToInvalidNull = 2001,
    OperationFailed = 14
};

namespace infiniframe::exports {
    namespace detail {
        inline thread_local std::string g_lastErrorMessage;
        inline thread_local InteropStatus g_lastStatus = InteropStatus::Success;

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

#ifdef _WIN32
        inline AutoString AllocateErrorMessageString(const std::string& value) {
            if (value.empty()) {
                return nullptr;
            }

            const int wideCount =
                MultiByteToWideChar(CP_UTF8, 0, value.c_str(), static_cast<int>(value.size()), nullptr, 0);
            if (wideCount <= 0) {
                return nullptr;
            }

            auto* buffer = new wchar_t[wideCount + 1];
            const int converted =
                MultiByteToWideChar(CP_UTF8, 0, value.c_str(), static_cast<int>(value.size()), buffer, wideCount);
            if (converted <= 0) {
                delete[] buffer;
                return nullptr;
            }

            buffer[converted] = L'\0';
            return buffer;
        }
#else
        inline AutoString AllocateErrorMessageString(const std::string& value) {
            if (value.empty()) {
                return nullptr;
            }

            return AllocateStringCopy(value);
        }
#endif
    } // namespace detail

    inline AutoString GetLastErrorMessageCopy() {
        return detail::AllocateErrorMessageString(detail::g_lastErrorMessage);
    }

    template <typename T> void ResetOut(T* outValue, const T fallback = {}) noexcept {
        if (outValue != nullptr) {
            *outValue = fallback;
        }
    }

    template <typename T> void ResetOut2(T* first, T* second, const T fallback = {}) noexcept {
        ResetOut(first, fallback);
        ResetOut(second, fallback);
    }

    template <typename T>
    bool EnsureNotNull(
        T* value, const char* argumentName, const InteropStatus status = InteropStatus::InvalidArgument
    ) noexcept {
        if (value != nullptr) {
            return true;
        }

        detail::SetFailure(status, std::string("Argument '") + argumentName + "' is null.");
        return false;
    }

    template <typename Fn> InteropStatus RunExportStatus(Fn&& fn) noexcept {
        try {
            detail::SetSuccess();
            std::forward<Fn>(fn)();
            if (detail::g_lastStatus != InteropStatus::Success) {
                return detail::g_lastStatus;
            }
            detail::SetSuccess();
            return InteropStatus::Success;
        } catch (const std::exception& ex) {
            return detail::TranslateException(ex);
        } catch (...) {
            detail::SetFailure(InteropStatus::OperationFailed, "Unknown native exception.");
            return InteropStatus::OperationFailed;
        }
    }

    template <typename Fn> InteropStatus RunWindowExportStatus(InfiniFrameWindow* instance, Fn&& fn) noexcept {
        return RunExportStatus([&] {
            if (!EnsureNotNull(instance, "instance")) {
                return;
            }

            std::forward<Fn>(fn)(instance);
        });
    }

    template <typename T, typename Fn>
    T RunWindowReturnExport(InfiniFrameWindow* instance, T fallback, Fn&& fn) noexcept {
        try {
            if (!EnsureNotNull(instance, "instance")) {
                return fallback;
            }

            T value = std::forward<Fn>(fn)(instance);
            detail::SetSuccess();
            return value;
        } catch (const std::exception& ex) {
            detail::TranslateException(ex);
            return fallback;
        } catch (...) {
            detail::SetFailure(InteropStatus::OperationFailed, "Unknown native exception.");
            return fallback;
        }
    }

    template <typename T, typename Fn> T RunReturnExport(T fallback, Fn&& fn) noexcept {
        try {
            T value = std::forward<Fn>(fn)();
            detail::SetSuccess();
            return value;
        } catch (const std::exception& ex) {
            detail::TranslateException(ex);
            return fallback;
        } catch (...) {
            detail::SetFailure(InteropStatus::OperationFailed, "Unknown native exception.");
            return fallback;
        }
    }
}
