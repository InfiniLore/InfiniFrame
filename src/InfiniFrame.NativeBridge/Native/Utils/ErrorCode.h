#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <string>
#include <system_error>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
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

inline const std::error_category& errorCategory() noexcept {
    struct InfiniFrameCategory : std::error_category {
        const char* name() const noexcept override {
            return "InfiniFrame";
        }

        std::string message(int ev) const override {
            switch (static_cast<ErrorCode>(ev)) {
                case ErrorCode::Success:
                    return "Success";
                case ErrorCode::InvalidArgument:
                    return "Invalid argument";
                case ErrorCode::NotInitialized:
                    return "Not initialized";
                case ErrorCode::PlatformNotSupported:
                    return "Platform not supported";
                case ErrorCode::WebViewError:
                    return "WebView error";
                case ErrorCode::EncodingError:
                    return "Encoding error";
                case ErrorCode::MemoryError:
                    return "Memory error";
                case ErrorCode::IoError:
                    return "I/O error";
                case ErrorCode::NullPointer:
                    return "Null pointer";
                case ErrorCode::InterfaceNotAvailable:
                    return "Interface not available";
                case ErrorCode::PropertyAccessFailed:
                    return "Property access failed";
                case ErrorCode::WindowNotFound:
                    return "Window not found";
                default:
                    return "Unknown error";
            }
        }
    };
    static const InfiniFrameCategory category;
    return category;
}

inline std::error_code make_error_code(const ErrorCode e) noexcept {
    return {static_cast<int>(e), errorCategory()};
}

template <> struct std::is_error_code_enum<ErrorCode> : true_type {}; 
