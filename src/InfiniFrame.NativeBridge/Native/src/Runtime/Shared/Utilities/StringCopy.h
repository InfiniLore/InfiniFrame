#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <cstring>
#include <cstdlib>
#include <string>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// Allocate a new null-terminated C string copy of @p str. Caller owns the result.
/// @param str The source string to copy.
/// @return Pointer to the newly allocated string, or nullptr if empty on some platforms.
inline char* AllocateStringCopy(const std::string& str) {
    const size_t len = str.length();
    auto* copy = new char[len + 1];
    std::memcpy(copy, str.c_str(), len + 1);
    return copy;
}

#ifdef _WIN32
/// Convert a wide string to a newly allocated UTF-8 string (caller owns the result).
inline char* AllocateUtf8FromWide(const std::wstring& wstr) {
    if (wstr.empty()) {
        auto* copy = new char[1];
        copy[0] = '\0';
        return copy;
    }
    const int utf8Count = WideCharToMultiByte(
        CP_UTF8, 0, wstr.c_str(), static_cast<int>(wstr.size()), nullptr, 0, nullptr, nullptr);
    if (utf8Count <= 0)
        return nullptr;
    auto* copy = new char[utf8Count + 1];
    WideCharToMultiByte(CP_UTF8, 0, wstr.c_str(), static_cast<int>(wstr.size()), copy, utf8Count, nullptr, nullptr);
    copy[utf8Count] = '\0';
    return copy;
}
#endif
