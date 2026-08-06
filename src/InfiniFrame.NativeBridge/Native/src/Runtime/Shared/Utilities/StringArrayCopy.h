#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <string>
#include <vector>

#include "Runtime/Shared/Utilities/StringCopy.h"
// ---------------------------------------------------------------------------------------------------------------------
// Owned string array allocation and free, consistent with StringCopy.h.
//
// The caller owns the returned array AND each element. Use FreeStringArray()
// or the platform-matched InfiniFrameNative_FreeStringArray export to release.
// ---------------------------------------------------------------------------------------------------------------------

/// Allocate a zero-initialized array of const char* pointers.
inline const char** AllocateStringArray(const size_t count) {
    if (count == 0)
        return nullptr;
    return new const char*[count]();
}

/// Allocate a string array and copy each element from a vector of platform strings.
#ifdef _WIN32
inline const char** AllocateStringArrayCopy(const std::vector<std::wstring>& strings) {
    if (strings.empty())
        return nullptr;
    auto* arr = AllocateStringArray(strings.size());
    for (size_t i = 0; i < strings.size(); ++i)
        arr[i] = AllocateUtf8FromWide(strings[i]);
    return arr;
}
#else
inline const char** AllocateStringArrayCopy(const std::vector<std::string>& strings) {
    if (strings.empty())
        return nullptr;
    auto* arr = AllocateStringArray(strings.size());
    for (size_t i = 0; i < strings.size(); ++i)
        arr[i] = AllocateStringCopy(strings[i]);
    return arr;
}
#endif

/// Free a string array and all its elements, matching the allocation in
/// AllocateStringArray / AllocateStringCopy.
inline void FreeStringArray(const char** arr, const int count) {
    if (arr == nullptr || count <= 0)
        return;
    for (int i = 0; i < count; ++i) {
        if (arr[i] != nullptr) {
            delete[] arr[i];
        }
    }
    delete[] arr;
}
