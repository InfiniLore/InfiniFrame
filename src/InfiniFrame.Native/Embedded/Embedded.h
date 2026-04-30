#pragma once

#include "InfiniFrameHostJs.h"
#include <simdutf.h>

namespace Embedded {
    inline const std::wstring& InfiniFrameHostJsUtf16() {
        static const std::wstring cached = [] {
            const auto* src = reinterpret_cast<const char*>(g_infiniframe_host_js_data);

            std::u16string temp;
            temp.resize(simdutf::utf16_length_from_utf8(src, g_infiniframe_host_js_size));

            const size_t written = simdutf::convert_utf8_to_utf16(
                src,
                g_infiniframe_host_js_size,
                temp.data()
            );

            temp.resize(written);

            return std::wstring(temp.begin(), temp.end());
        }();

        return cached;
    }
    
    inline const std::string& InfiniFrameHostJsUtf8() {
        static const std::string cached = [] {
            const auto* src = reinterpret_cast<const char*>(g_infiniframe_host_js_data);
            return std::string(src, g_infiniframe_host_js_size);
        }();
        return cached;
    }
}
