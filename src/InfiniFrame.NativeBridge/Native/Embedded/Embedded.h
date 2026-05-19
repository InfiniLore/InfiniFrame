#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Embedded/InfiniFrameJs/InfiniFrameJs.h"
#include <simdutf.h>
#include <string>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace Embedded {
    inline const std::wstring& InfiniFrameJsUtf16() {
        static const std::wstring Cached = [] {
            const auto* src = reinterpret_cast<const char*>(GInfiniframeJsData);

            std::u16string temp;
            temp.resize(simdutf::utf16_length_from_utf8(src, GInfiniframeJsSize));

            const size_t written = simdutf::convert_utf8_to_utf16(src, GInfiniframeJsSize, temp.data());

            temp.resize(written);

            return std::wstring(temp.begin(), temp.end());
        }();

        return Cached;
    }

    inline const std::string& InfiniFrameJsUtf8() {
        static const std::string Cached = [] {
            const auto* src = reinterpret_cast<const char*>(GInfiniframeJsData);
            return std::string(src, GInfiniframeJsSize);
        }();
        return Cached;
    }
}
