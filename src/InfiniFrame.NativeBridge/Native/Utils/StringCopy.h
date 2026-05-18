#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <cstring>
#include <cstdlib>
#include <string>

#ifdef __linux__
#include <glib.h>
#endif
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
#ifdef _WIN32
inline wchar_t* AllocateStringCopy(const std::wstring& str) {
    const size_t len = str.length();
    auto* copy = new wchar_t[len + 1];
    std::memcpy(copy, str.c_str(), (len + 1) * sizeof(wchar_t));
    return copy;
}

#elif __linux__
inline char* AllocateStringCopy(const std::string& str) {
    return g_strdup(str.c_str());
}

#elif __APPLE__
inline char* AllocateStringCopy(const std::string& str) {
    const size_t len = str.length();
    char* copy = static_cast<char*>(malloc(len + 1));
    std::memcpy(copy, str.c_str(), len + 1);
    return copy;
}

#else
inline char* AllocateStringCopy(const std::string& str) {
    const size_t len = str.length();
    char* copy = static_cast<char*>(malloc(len + 1));
    std::memcpy(copy, str.c_str(), len + 1);
    return copy;
}
#endif
