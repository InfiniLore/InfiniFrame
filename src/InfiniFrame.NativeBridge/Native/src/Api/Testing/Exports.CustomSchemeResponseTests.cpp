// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Window/InfiniFrame.h"
#include "Api/Exports/Exports.h"
#include "Runtime/Shared/WebView/CustomSchemeResponse.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
#if defined(INFINIFRAME_BUILD_TEST_EXPORTS)

namespace {
    template <typename CharT>
    infiniframe::ParsedOrigin<CharT> CallParseOrigin(const CharT* value) {
        return infiniframe::ParseOrigin<CharT>(std::basic_string<CharT>(value));
    }

    template <typename CharT>
    bool CallIsSameOrigin(const CharT* left, const CharT* right) {
        return infiniframe::IsSameOrigin<CharT>(
            std::basic_string<CharT>(left),
            std::basic_string<CharT>(right));
    }

    template <typename CharT>
    AutoString CallBuildHeaders(
        const CharT* contentType,
        const CharT* resourceUri,
        const CharT* requestOrigin
    ) {
        std::basic_string<CharT> result = infiniframe::BuildCustomSchemeResponseHeaders<CharT>(
            std::basic_string<CharT>(contentType),
            std::basic_string<CharT>(resourceUri),
            std::basic_string<CharT>(requestOrigin));
        return AllocateStringCopy(result);
    }
}

extern "C" {

EXPORTED InteropStatus InfiniFrameNativeTests_ParseOrigin(
    AutoStringConst value,
    AutoString* scheme,
    AutoString* host,
    AutoString* port,
    int* valid
) {
    if (scheme != nullptr) *scheme = nullptr;
    if (host != nullptr) *host = nullptr;
    if (port != nullptr) *port = nullptr;
    if (valid != nullptr) *valid = 0;

    return RunExportStatus([&] {
        if (!EnsureNotNull(value, "value") ||
            !EnsureNotNull(scheme, "scheme", ::InteropStatus::OutParameterSetToInvalidNull) ||
            !EnsureNotNull(host, "host", ::InteropStatus::OutParameterSetToInvalidNull) ||
            !EnsureNotNull(port, "port", ::InteropStatus::OutParameterSetToInvalidNull) ||
            !EnsureNotNull(valid, "valid", ::InteropStatus::OutParameterSetToInvalidNull)) {
            return;
        }

        auto result = CallParseOrigin(value);
        *valid = result.Valid ? 1 : 0;
        if (result.Valid) {
            *scheme = AllocateStringCopy(result.Scheme);
            *host = AllocateStringCopy(result.Host);
            *port = AllocateStringCopy(result.Port);
        }
    });
}

EXPORTED InteropStatus InfiniFrameNativeTests_IsSameOrigin(
    AutoStringConst left,
    AutoStringConst right,
    int* result
) {
    if (result != nullptr) *result = 0;

    return RunExportStatus([&] {
        if (!EnsureNotNull(left, "left") ||
            !EnsureNotNull(right, "right") ||
            !EnsureNotNull(result, "result", ::InteropStatus::OutParameterSetToInvalidNull)) {
            return;
        }

        *result = CallIsSameOrigin(left, right) ? 1 : 0;
    });
}

EXPORTED InteropStatus InfiniFrameNativeTests_BuildHeaders(
    AutoStringConst contentType,
    AutoStringConst resourceUri,
    AutoStringConst requestOrigin,
    AutoString* headers
) {
    if (headers != nullptr) *headers = nullptr;

    return RunExportStatus([&] {
        if (!EnsureNotNull(contentType, "contentType") ||
            !EnsureNotNull(resourceUri, "resourceUri") ||
            !EnsureNotNull(requestOrigin, "requestOrigin") ||
            !EnsureNotNull(headers, "headers", ::InteropStatus::OutParameterSetToInvalidNull)) {
            return;
        }

        *headers = CallBuildHeaders(contentType, resourceUri, requestOrigin);
    });
}

}

#endif
