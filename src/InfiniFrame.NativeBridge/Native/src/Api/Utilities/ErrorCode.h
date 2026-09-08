#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <string>
#include <system_error>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// Error codes used throughout the InfiniFrame native layer.
enum class ErrorCode {
    /// Operation completed successfully.
    Success = 0,
    /// A required argument was null or otherwise invalid.
    InvalidArgument,
    /// The subsystem has not been initialized.
    NotInitialized,
    /// The current platform does not support this feature.
    PlatformNotSupported,
    /// An error occurred in the underlying WebView.
    WebViewError,
    /// A string encoding or conversion error occurred.
    EncodingError,
    /// A memory allocation or access error occurred.
    MemoryError,
    /// An I/O or file-system error occurred.
    IoError,
    /// A null pointer was encountered unexpectedly.
    NullPointer,
    /// The requested interface is not available on this platform.
    InterfaceNotAvailable,
    /// A property read or write failed.
    PropertyAccessFailed,
    /// The target window could not be found.
    WindowNotFound
};

/// Returns the InfiniFrame error category for use with std::error_code.
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

/// Create an std::error_code from an ErrorCode value.
inline std::error_code make_error_code(const ErrorCode e) noexcept {
    return {static_cast<int>(e), errorCategory()};
}

template <> struct std::is_error_code_enum<ErrorCode> : true_type {};
