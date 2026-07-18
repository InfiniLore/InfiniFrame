// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Embedded/Embedded.h"

#include <InfiniFrameJs.h>
#include <simdutf.h>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace Embedded {
    const std::wstring& InfiniFrameJsUtf16() {
        static const std::wstring cached = [] {
            const auto* source = reinterpret_cast<const char*>(GInfiniframeJsData);
            std::u16string utf16;
            utf16.resize(simdutf::utf16_length_from_utf8(source, GInfiniframeJsSize));
            utf16.resize(simdutf::convert_utf8_to_utf16(source, GInfiniframeJsSize, utf16.data()));
            return std::wstring(utf16.begin(), utf16.end());
        }();
        return cached;
    }

    const std::string& InfiniFrameJsUtf8() {
        static const std::string cached(
                reinterpret_cast<const char*>(GInfiniframeJsData), GInfiniframeJsSize);
        return cached;
    }
}
