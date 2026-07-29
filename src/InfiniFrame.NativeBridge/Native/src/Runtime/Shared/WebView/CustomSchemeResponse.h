#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <algorithm>
#include <string>
#include <cstddef>
#include <cstring>

#include "Runtime/Shared/Types/Callbacks.h"
// ---------------------------------------------------------------------------------------------------------------------
// Cross-platform CORS/header helper for custom-scheme responses.
//
// Each platform's WebView has its own response API (ICoreWebView2WebResourceResponse on
// Windows, webkit_uri_scheme_request_finish on Linux, WKURLSchemeTask on macOS), but the
// CORS header policy is identical: reflect an origin only when it has the same
// scheme/host/effective-port tuple as the requested resource.
//
// Usage:
//   auto headers = infiniframe::BuildCustomSchemeResponseHeaders<wchar_t>(contentType, uri, origin);
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
        static constexpr const char* AllowOriginPrefix       = "Access-Control-Allow-Origin: ";
        static constexpr const char* AllowCredentials        = "Access-Control-Allow-Credentials: true";
        static constexpr const char* VaryOrigin              = "Vary: Origin";
    };

    template <>
    struct SchemeResponseTraits<wchar_t> {
        static constexpr const wchar_t* ContentTypePrefix    = L"Content-Type: ";
        static constexpr const wchar_t* CrLf                 = L"\r\n";
        static constexpr const wchar_t* AllowOriginPrefix    = L"Access-Control-Allow-Origin: ";
        static constexpr const wchar_t* AllowCredentials     = L"Access-Control-Allow-Credentials: true";
        static constexpr const wchar_t* VaryOrigin           = L"Vary: Origin";
    };

    template <typename CharT>
    struct ParsedOrigin {
        std::basic_string<CharT> Scheme;
        std::basic_string<CharT> Host;
        std::basic_string<CharT> Port;
        bool Valid = false;
    };

    template <typename CharT>
    ParsedOrigin<CharT> ParseOrigin(const std::basic_string<CharT>& value) {
        ParsedOrigin<CharT> result;
        const auto delimiter = value.find(std::basic_string<CharT>{static_cast<CharT>(':'), static_cast<CharT>('/'), static_cast<CharT>('/')});
        if (delimiter == std::basic_string<CharT>::npos || delimiter == 0) return result;

        const auto authorityStart = delimiter + 3;
        auto authorityEnd = value.find_first_of(
            std::basic_string<CharT>{static_cast<CharT>('/'), static_cast<CharT>('?'), static_cast<CharT>('#')},
            authorityStart);
        if (authorityEnd == std::basic_string<CharT>::npos) authorityEnd = value.size();
        if (authorityEnd == authorityStart) return result;

        auto authority = value.substr(authorityStart, authorityEnd - authorityStart);
        if (authority.find(static_cast<CharT>('@')) != std::basic_string<CharT>::npos) return result;
        auto portSeparator = authority.rfind(static_cast<CharT>(':'));
        if (portSeparator != std::basic_string<CharT>::npos) {
            result.Host = authority.substr(0, portSeparator);
            result.Port = authority.substr(portSeparator + 1);
            if (result.Port.empty()) return result;
        } else {
            result.Host = authority;
        }
        if (result.Host.empty()) return result;

        result.Scheme = value.substr(0, delimiter);
        auto lower = [](CharT character) {
            return character >= static_cast<CharT>('A') && character <= static_cast<CharT>('Z')
                ? static_cast<CharT>(character + (static_cast<CharT>('a') - static_cast<CharT>('A')))
                : character;
        };
        std::transform(result.Scheme.begin(), result.Scheme.end(), result.Scheme.begin(), lower);
        std::transform(result.Host.begin(), result.Host.end(), result.Host.begin(), lower);
        if (result.Port.empty()) {
            if (result.Scheme == std::basic_string<CharT>{static_cast<CharT>('h'), static_cast<CharT>('t'), static_cast<CharT>('t'), static_cast<CharT>('p')})
                result.Port = std::basic_string<CharT>{static_cast<CharT>('8'), static_cast<CharT>('0')};
            else if (result.Scheme == std::basic_string<CharT>{static_cast<CharT>('h'), static_cast<CharT>('t'), static_cast<CharT>('t'), static_cast<CharT>('p'), static_cast<CharT>('s')})
                result.Port = std::basic_string<CharT>{static_cast<CharT>('4'), static_cast<CharT>('4'), static_cast<CharT>('3')};
        }
        result.Valid = true;
        return result;
    }

    template <typename CharT>
    bool IsSameOrigin(const std::basic_string<CharT>& left, const std::basic_string<CharT>& right) {
        auto leftOrigin = ParseOrigin(left);
        auto rightOrigin = ParseOrigin(right);
        return leftOrigin.Valid && rightOrigin.Valid &&
            leftOrigin.Scheme == rightOrigin.Scheme &&
            leftOrigin.Host == rightOrigin.Host &&
            leftOrigin.Port == rightOrigin.Port;
    }

    template <typename CharT>
    std::basic_string<CharT> BuildCustomSchemeResponseHeaders(
        const std::basic_string<CharT>& contentType,
        const std::basic_string<CharT>& resourceUri,
        const std::basic_string<CharT>& requestOrigin
    ) {
        using T = SchemeResponseTraits<CharT>;
        std::basic_string<CharT> h;
        h += T::ContentTypePrefix;
        h += contentType;
        if (IsSameOrigin(resourceUri, requestOrigin)) {
            h += T::CrLf;
            h += T::AllowOriginPrefix;
            h += requestOrigin;
            h += T::CrLf;
            h += T::AllowCredentials;
            h += T::CrLf;
            h += T::VaryOrigin;
        }
        return h;
    }

} 
