// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <string>
#include <simdutf.h>

#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
std::wstring Utf8ToWide(const char* source) {
    if (source == nullptr)
        return {};

    const size_t utf8Length = strlen(source);
    if (utf8Length == 0)
        return {};

    if (const auto validation = simdutf::validate_utf8_with_errors(source, utf8Length); validation.is_err())
        return {};

    std::u16string utf16(simdutf::utf16_length_from_utf8(source, utf8Length), u'\0');
    const size_t written =
        simdutf::convert_valid_utf8_to_utf16(source, utf8Length, reinterpret_cast<char16_t*>(utf16.data()));
    utf16.resize(written);

    return {reinterpret_cast<const wchar_t*>(utf16.data()), utf16.size()};
}

std::string WideToUtf8(const wchar_t* source) {
    if (source == nullptr)
        return {};

    const size_t utf16Length = wcslen(source);
    if (utf16Length == 0)
        return {};

    const auto* utf16 = reinterpret_cast<const char16_t*>(source);
    if (const auto validation = simdutf::validate_utf16_with_errors(utf16, utf16Length); validation.is_err())
        return {};

    std::string utf8(simdutf::utf8_length_from_utf16(utf16, utf16Length), '\0');
    const size_t written = simdutf::convert_valid_utf16_to_utf8(utf16, utf16Length, utf8.data());
    utf8.resize(written);

    return utf8;
}
