#pragma once

#include "Embedded/InfiniFrameHostJs/InfiniFrameHostJs.h"

namespace Embedded {
    inline const std::wstring& InfiniFrameHostJsUtf16() {
        static const std::wstring cached = [] {
            const auto* src = reinterpret_cast<const char*>(g_infiniframe_host_js_data);

            std::wstring result;
            result.resize(simdutf::utf16_length_from_utf8(src, g_infiniframe_host_js_size));

            const size_t written = simdutf::convert_utf8_to_utf16(
                src,
                g_infiniframe_host_js_size,
                reinterpret_cast<char16_t*>(result.data())
            );

            result.resize(written);
            return result;
        }();

        return cached;
    }
}
