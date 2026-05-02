#pragma once
/**
 * @file CustomSchemeResponse.h
 * @brief Shared helpers for custom-scheme callback responses.
 */

#ifndef INFINIFRAME_SHARED_CUSTOMSCHEMERESPONSE_H
#define INFINIFRAME_SHARED_CUSTOMSCHEMERESPONSE_H

#include "Core/InfiniFrame.h"
#include "Interop/NativeBuffer.h"

#include <string_view>
#include <utility>

#ifdef _WIN32
#include <string>
#endif

namespace InfiniFrame::Native::Shared {
#ifdef _WIN32
    inline constexpr AutoStringConst DefaultCustomSchemeContentType = L"application/octet-stream";
    inline constexpr AutoStringConst JsonCustomSchemeContentType = L"application/json";
#else
    inline constexpr AutoStringConst DefaultCustomSchemeContentType = "application/octet-stream";
    inline constexpr AutoStringConst JsonCustomSchemeContentType = "application/json";
#endif

    struct CustomSchemeResponse {
        InfiniFrame::Native::Interop::NativeBufferPtr body;
        InfiniFrame::Native::Interop::NativeBufferPtr contentTypeBuffer;
        int length = 0;
        AutoString contentType = nullptr;

        [[nodiscard]] bool HasBody() const noexcept {
            return body != nullptr && length >= 0;
        }

        [[nodiscard]] AutoStringConst ContentTypeOrDefault() const noexcept {
            return contentType == nullptr ? DefaultCustomSchemeContentType : contentType;
        }
    };

    inline CustomSchemeResponse InvokeCustomSchemeCallback(
        const WebResourceRequestedCallback callback,
        const AutoString url
        ) noexcept {
        if (callback == nullptr)
            return {};

        int numBytes = 0;
        AutoString contentType = nullptr;
        auto body = InfiniFrame::Native::Interop::AdoptNativeBuffer(
            callback(url, &numBytes, &contentType)
            );
        auto contentTypeBuffer = InfiniFrame::Native::Interop::AdoptNativeBuffer(contentType);

        return CustomSchemeResponse{
            std::move(body),
            std::move(contentTypeBuffer),
            numBytes,
            contentType
        };
    }

#ifdef _WIN32
    inline std::wstring BuildCorsResponseHeaders(
        const std::wstring_view contentType,
        const std::wstring_view requestOrigin
        ) {
        std::wstring responseHeaders = L"Content-Type: ";
        if (contentType.empty())
            responseHeaders += DefaultCustomSchemeContentType;
        else
            responseHeaders.append(contentType);
        responseHeaders += L"\r\nAccess-Control-Allow-Methods: GET, HEAD, OPTIONS";
        responseHeaders += L"\r\nAccess-Control-Allow-Headers: *";

        if (!requestOrigin.empty()) {
            responseHeaders += L"\r\nAccess-Control-Allow-Origin: ";
            responseHeaders.append(requestOrigin);
            responseHeaders += L"\r\nAccess-Control-Allow-Credentials: true";
            responseHeaders += L"\r\nVary: Origin";
        }
        else {
            responseHeaders += L"\r\nAccess-Control-Allow-Origin: *";
        }

        return responseHeaders;
    }
#endif
}

#endif // INFINIFRAME_SHARED_CUSTOMSCHEMERESPONSE_H
