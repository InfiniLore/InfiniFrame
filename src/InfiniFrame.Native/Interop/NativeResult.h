#pragma once
/**
 * @file NativeResult.h
 * @brief Helpers for keeping the C ABI boundary exception-safe.
 */

#ifndef INFINIFRAME_INTEROP_NATIVERESULT_H
#define INFINIFRAME_INTEROP_NATIVERESULT_H

#include "../Core/InfiniFrameWindow.h"

#include <cerrno>
#include <cstdint>
#include <cstring>
#include <stdexcept>
#include <string>
#include <utility>

namespace InfiniFrame::Native::Interop {
    enum class NativeStatusCode : int32_t {
        Success = 0,
        InvalidArgument = EINVAL,
        OperationFailed = EFAULT
    };

    constexpr int ExportSuccess = static_cast<int>(NativeStatusCode::Success);
    constexpr int ExportInvalidArgument = static_cast<int>(NativeStatusCode::InvalidArgument);
    constexpr int ExportOperationFailed = static_cast<int>(NativeStatusCode::OperationFailed);

    inline thread_local NativeString LastExportErrorMessage;

    inline NativeString ToNativeErrorMessage(const char* message) {
        if (message == nullptr)
            return {};

#ifdef _WIN32
        const int utf16Length = MultiByteToWideChar(CP_UTF8, MB_ERR_INVALID_CHARS, message, -1, nullptr, 0);
        if (utf16Length > 0) {
            std::wstring result(static_cast<size_t>(utf16Length - 1), L'\0');
            MultiByteToWideChar(CP_UTF8, MB_ERR_INVALID_CHARS, message, -1, result.data(), utf16Length);
            return result;
        }

        std::wstring fallback;
        fallback.reserve(std::strlen(message));
        for (const unsigned char* current = reinterpret_cast<const unsigned char*>(message); *current != '\0'; ++current)
            fallback.push_back(static_cast<wchar_t>(*current));
        return fallback;
#else
        return message;
#endif
    }

    inline void SetExportErrorMessage(const char* message) noexcept {
        try {
            LastExportErrorMessage = ToNativeErrorMessage(message);
        }
        catch (...) {
            LastExportErrorMessage.clear();
        }
    }

    inline void ClearExportErrorMessage() noexcept {
        LastExportErrorMessage.clear();
    }

    [[nodiscard]] inline const NativeString& GetExportErrorMessage() noexcept {
        return LastExportErrorMessage;
    }

    [[nodiscard]] inline NativeStatusCode ToNativeStatusCode(const int error) noexcept {
        return static_cast<NativeStatusCode>(error);
    }

    inline NativeStatusCode SetExportLastError(const int error) noexcept {
#ifdef _WIN32
        SetLastError(static_cast<DWORD>(error));
#else
        errno = error;
#endif
        return ToNativeStatusCode(error);
    }

    inline NativeStatusCode SetExportSuccess() noexcept {
        ClearExportErrorMessage();
        return SetExportLastError(ExportSuccess);
    }

    inline NativeStatusCode SetExportInvalidArgument(const char* message = "Invalid native argument.") noexcept {
        SetExportErrorMessage(message);
        return SetExportLastError(ExportInvalidArgument);
    }

    inline NativeStatusCode SetExportOperationFailed(const char* message = "Native operation failed.") noexcept {
        SetExportErrorMessage(message);
        return SetExportLastError(ExportOperationFailed);
    }

    template <typename T>
    void ResetOutput(T* output) noexcept {
        if (output != nullptr)
            *output = {};
    }

    template <typename... Outputs>
    void ResetOutputs(Outputs*... outputs) noexcept {
        (ResetOutput(outputs), ...);
    }

    template <typename... Outputs>
    bool HasOutputs(Outputs*... outputs) noexcept {
        return ((outputs != nullptr) && ...);
    }

    template <typename Action>
    void RunExport(Action&& action) noexcept {
        try {
            std::forward<Action>(action)();
            SetExportSuccess();
        }
        catch (const std::invalid_argument& ex) {
            SetExportInvalidArgument(ex.what());
        }
        catch (const std::exception& ex) {
            SetExportOperationFailed(ex.what());
        }
        catch (...) {
            SetExportOperationFailed("Unknown native exception.");
        }
    }

    template <typename Action>
    NativeStatusCode RunExportStatus(Action&& action) noexcept {
        try {
            std::forward<Action>(action)();
            return SetExportSuccess();
        }
        catch (const std::invalid_argument& ex) {
            return SetExportInvalidArgument(ex.what());
        }
        catch (const std::exception& ex) {
            return SetExportOperationFailed(ex.what());
        }
        catch (...) {
            return SetExportOperationFailed("Unknown native exception.");
        }
    }

    template <typename Result, typename Action>
    Result RunReturnExport(const Result fallback, Action&& action) noexcept {
        try {
            Result result = std::forward<Action>(action)();
            SetExportSuccess();
            return result;
        }
        catch (const std::invalid_argument& ex) {
            SetExportInvalidArgument(ex.what());
            return fallback;
        }
        catch (const std::exception& ex) {
            SetExportOperationFailed(ex.what());
            return fallback;
        }
        catch (...) {
            SetExportOperationFailed("Unknown native exception.");
            return fallback;
        }
    }

    template <typename Action, typename... Outputs>
    void RunWindowExport(InfiniFrameWindow* instance, Action&& action, Outputs*... outputs) noexcept {
        ResetOutputs(outputs...);
        if (instance == nullptr || !HasOutputs(outputs...)) {
            SetExportInvalidArgument();
            return;
        }

        RunExport([&] {
            std::forward<Action>(action)(*instance);
        });
    }

    template <typename Action, typename... Outputs>
    NativeStatusCode RunWindowExportStatus(
        InfiniFrameWindow* instance,
        Action&& action,
        Outputs*... outputs
        ) noexcept {
        ResetOutputs(outputs...);
        if (instance == nullptr || !HasOutputs(outputs...))
            return SetExportInvalidArgument();

        return RunExportStatus([&] {
            std::forward<Action>(action)(*instance);
        });
    }

    template <typename Result, typename Action, typename... Outputs>
    Result RunWindowReturnExport(
        InfiniFrameWindow* instance,
        const Result fallback,
        Action&& action,
        Outputs*... outputs
        ) noexcept {
        ResetOutputs(outputs...);
        if (instance == nullptr || !HasOutputs(outputs...)) {
            SetExportInvalidArgument();
            return fallback;
        }

        return RunReturnExport(fallback, [&] {
            return std::forward<Action>(action)(*instance);
        });
    }
}

#endif // INFINIFRAME_INTEROP_NATIVERESULT_H
