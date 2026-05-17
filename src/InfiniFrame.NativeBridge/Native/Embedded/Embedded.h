#pragma once

#include "InfiniFrameJs.h"
#include <simdutf.h>

namespace Embedded {
inline const std::wstring& InfiniFrameJsUtf16() {
    static const std::wstring cached = [] {
        const auto* src = reinterpret_cast<const char*>(gInfiniframeJsData);

        std::u16string temp;
        temp.resize(simdutf::utf16_length_from_utf8(src, gInfiniframeJsSize));

        const size_t written = simdutf::convert_utf8_to_utf16(src, gInfiniframeJsSize, temp.data());

        temp.resize(written);

        return std::wstring(temp.begin(), temp.end());
    }();

    return cached;
}

inline const std::string& InfiniFrameJsUtf8() {
    static const std::string cached = [] {
        const auto* src = reinterpret_cast<const char*>(gInfiniframeJsData);
        return std::string(src, gInfiniframeJsSize);
    }();
    return cached;
}
} // namespace Embedded
