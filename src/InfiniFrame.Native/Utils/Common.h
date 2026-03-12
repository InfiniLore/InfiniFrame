#pragma once
/**
 * @file Common.h
 * @brief Common utilities for cross-platform development
 */

#ifndef INFINIFRAME_COMMON_H
#define INFINIFRAME_COMMON_H

#include <string>
#include <string_view>
#include <memory>
#include <expected>
#include <functional>
#include <optional>
#include <algorithm>
#include <system_error>

#ifdef _WIN32
#include <windows.h>
#endif

namespace InfiniFrame {

// ============================================================================
// Constants
// ============================================================================

inline constexpr int MaxWindowDimension = 10000;
inline constexpr int MinWindowDimension = 50;
inline constexpr int DefaultWindowWidth = 800;
inline constexpr int DefaultWindowHeight = 600;

// ============================================================================
// Error Codes
// ============================================================================

enum class ErrorCode {
    Success = 0,
    InvalidArgument,
    NotInitialized,
    PlatformNotSupported,
    WebViewError,
    EncodingError,
    MemoryError,
    IoError,
    NullPointer,
    InterfaceNotAvailable,
    PropertyAccessFailed,
    WindowNotFound
};

// ============================================================================
// Error Category
// ============================================================================

inline const std::error_category& errorCategory() noexcept {
    struct InfiniFrameCategory : std::error_category {
        const char* name() const noexcept override {
            return "InfiniFrame";
        }

        std::string message(int ev) const override {
            switch (static_cast<ErrorCode>(ev)) {
                case ErrorCode::Success: return "Success";
                case ErrorCode::InvalidArgument: return "Invalid argument";
                case ErrorCode::NotInitialized: return "Not initialized";
                case ErrorCode::PlatformNotSupported: return "Platform not supported";
                case ErrorCode::WebViewError: return "WebView error";
                case ErrorCode::EncodingError: return "Encoding error";
                case ErrorCode::MemoryError: return "Memory error";
                case ErrorCode::IoError: return "I/O error";
                case ErrorCode::NullPointer: return "Null pointer";
                case ErrorCode::InterfaceNotAvailable: return "Interface not available";
                case ErrorCode::PropertyAccessFailed: return "Property access failed";
                case ErrorCode::WindowNotFound: return "Window not found";
                default: return "Unknown error";
            }
        }
    };
    static const InfiniFrameCategory category;
    return category;
}

inline std::error_code make_error_code(ErrorCode e) noexcept {
    return {static_cast<int>(e), errorCategory()};
}

} // namespace InfiniFrame

namespace std {
template<>
struct is_error_code_enum<InfiniFrame::ErrorCode> : true_type {};
}

namespace InfiniFrame {

// ============================================================================
// Result Type
// ============================================================================

template<typename T>
using Result = std::expected<T, ErrorCode>;

// ============================================================================
// RAII Wrappers (Windows)
// ============================================================================

#ifdef _WIN32

struct HBRUSHDeleter {
    void operator()(void* h) const noexcept {
        if (h) DeleteObject(static_cast<HBRUSH>(h));
    }
};

struct HICONDeleter {
    void operator()(void* h) const noexcept {
        if (h) DestroyIcon(static_cast<HICON>(h));
    }
};

struct HDCDeleter {
    void operator()(void* h) const noexcept {
        if (h) DeleteDC(static_cast<HDC>(h));
    }
};

using UniqueHBRUSH = std::unique_ptr<void, HBRUSHDeleter>;
using UniqueHICON = std::unique_ptr<void, HICONDeleter>;
using UniqueHDC = std::unique_ptr<void, HDCDeleter>;

#endif

// ============================================================================
// Helper Functions
// ============================================================================

template<typename T>
[[nodiscard]] constexpr T clampDimension(T value, T minVal = MinWindowDimension, T maxVal = MaxWindowDimension) {
    return std::clamp(value, minVal, maxVal);
}

#ifdef _WIN32
inline std::wstring Utf8ToWide(std::string_view utf8) {
    if (utf8.empty()) return {};
    return std::wstring{utf8.begin(), utf8.end()};
}

inline std::string WideToUtf8(std::wstring_view wide) {
    if (wide.empty()) return {};
    return std::string{wide.begin(), wide.end()};
}
#endif

} // namespace InfiniFrame

#endif // INFINIFRAME_COMMON_H
