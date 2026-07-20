#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <string>
#include <cstddef>
#include <cstring>

#include "Runtime/Shared/Types/Callbacks.h"
// ---------------------------------------------------------------------------------------------------------------------
// Cross-platform CORS/header helper for custom-scheme responses.
//
// Each platform's WebView has its own response API (ICoreWebView2WebResourceResponse on
// Windows, webkit_uri_scheme_request_finish on Linux, WKURLSchemeTask on macOS), but the
// CORS header policy is identical: allow all methods/headers, reflect or wildcard the
// origin, and include credentials when an origin is present.
//
// Usage:
//   std::wstring headers = infiniframe::BuildCorsResponseHeaders<wchar_t>(L"application/json", origin);
//   std::string  headers = infiniframe::BuildCorsResponseHeaders<char>("application/json", origin);
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe {

    static constexpr std::size_t MaxCustomSchemeContentTypeBytes = 1024;

    /** Calls the producer-provided release function exactly once on every native exit path. */
    class CustomSchemeResponseLease final {
    public:
        explicit CustomSchemeResponseLease(CustomSchemeResponse& response) noexcept : _response(response) {}
        CustomSchemeResponseLease(const CustomSchemeResponseLease&) = delete;
        CustomSchemeResponseLease& operator=(const CustomSchemeResponseLease&) = delete;

        ~CustomSchemeResponseLease() {
            if (_response.OwnerContext != nullptr && _response.Release != nullptr) {
                _response.Release(_response.OwnerContext);
                _response.OwnerContext = nullptr;
            }
        }

    private:
        CustomSchemeResponse& _response;
    };

    /** Validate the complete v1 buffered-response prefix before dereferencing producer-owned pointers. */
    inline bool IsValidBufferedCustomSchemeResponse(const CustomSchemeResponse& response) noexcept {
        constexpr std::size_t requiredSize = offsetof(CustomSchemeResponse, ReservedRead);
        if (response.StructSize < requiredSize ||
            response.AbiVersion != CustomSchemeResponse::CurrentAbiVersion ||
            response.BodyKind != static_cast<uint32_t>(CustomSchemeBodyKind::Buffered) ||
            response.StatusCode < 100 || response.StatusCode > 599 ||
            response.ContentLength > CustomSchemeResponse::MaxBufferedBodyBytes ||
            (response.ContentLength != 0 && response.Body == nullptr) ||
            response.ContentTypeUtf8 == nullptr ||
            response.OwnerContext == nullptr || response.Release == nullptr) {
            return false;
        }

        return std::memchr(response.ContentTypeUtf8, '\0', MaxCustomSchemeContentTypeBytes + 1) != nullptr;
    }

    template <typename CharT>
    struct SchemeResponseTraits;

    template <>
    struct SchemeResponseTraits<char> {
        static constexpr const char* ContentTypePrefix       = "Content-Type: ";
        static constexpr const char* CrLf                    = "\r\n";
        static constexpr const char* AllowMethods            = "Access-Control-Allow-Methods: GET, HEAD, OPTIONS";
        static constexpr const char* AllowHeaders            = "Access-Control-Allow-Headers: *";
        static constexpr const char* AllowOriginPrefix       = "Access-Control-Allow-Origin: ";
        static constexpr const char* AllowCredentials        = "Access-Control-Allow-Credentials: true";
        static constexpr const char* VaryOrigin              = "Vary: Origin";
        static constexpr const char* WildcardOrigin          = "*";
    };

    template <>
    struct SchemeResponseTraits<wchar_t> {
        static constexpr const wchar_t* ContentTypePrefix    = L"Content-Type: ";
        static constexpr const wchar_t* CrLf                 = L"\r\n";
        static constexpr const wchar_t* AllowMethods         = L"Access-Control-Allow-Methods: GET, HEAD, OPTIONS";
        static constexpr const wchar_t* AllowHeaders         = L"Access-Control-Allow-Headers: *";
        static constexpr const wchar_t* AllowOriginPrefix    = L"Access-Control-Allow-Origin: ";
        static constexpr const wchar_t* AllowCredentials     = L"Access-Control-Allow-Credentials: true";
        static constexpr const wchar_t* VaryOrigin           = L"Vary: Origin";
        static constexpr const wchar_t* WildcardOrigin       = L"*";
    };

    /// Build a complete CORS response header string for a custom-scheme response.
    /// @param contentType   MIME type of the response body (e.g. "application/json").
    /// @param requestOrigin The Origin header from the request, or empty for wildcard.
    /// @return Multi-line header string using "\r\n" delimiters.
    template <typename CharT>
    std::basic_string<CharT> BuildCorsResponseHeaders(
        const std::basic_string<CharT>& contentType,
        const std::basic_string<CharT>& requestOrigin
    ) {
        using T = SchemeResponseTraits<CharT>;
        std::basic_string<CharT> h;
        h += T::ContentTypePrefix;
        h += contentType;
        h += T::CrLf;
        h += T::AllowMethods;
        h += T::CrLf;
        h += T::AllowHeaders;
        if (!requestOrigin.empty()) {
            h += T::CrLf;
            h += T::AllowOriginPrefix;
            h += requestOrigin;
            h += T::CrLf;
            h += T::AllowCredentials;
            h += T::CrLf;
            h += T::VaryOrigin;
        } else {
            h += T::CrLf;
            h += T::AllowOriginPrefix;
            h += T::WildcardOrigin;
        }
        return h;
    }

} 
