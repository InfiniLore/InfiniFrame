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
// or the platform-matched InfiniFrame_FreeStringArray export to release.
// ---------------------------------------------------------------------------------------------------------------------

/// Allocate a zero-initialized array of AutoString pointers.
inline AutoString* AllocateStringArray(const size_t count) {
    if (count == 0)
        return nullptr;
#ifdef _WIN32
    return new AutoString[count]();
#elif __linux__
    return new AutoString[count]();
#else
    auto* arr = static_cast<AutoString*>(calloc(count, sizeof(AutoString)));
    return arr;
#endif
}

/// Allocate a string array and copy each element from a vector of platform strings.
#ifdef _WIN32
inline AutoString* AllocateStringArrayCopy(const std::vector<std::wstring>& strings) {
    if (strings.empty())
        return nullptr;
    auto* arr = AllocateStringArray(strings.size());
    for (size_t i = 0; i < strings.size(); ++i)
        arr[i] = AllocateStringCopy(strings[i]);
    return arr;
}
#else
inline AutoString* AllocateStringArrayCopy(const std::vector<std::string>& strings) {
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
inline void FreeStringArray(AutoString* arr, const int count) {
    if (arr == nullptr || count <= 0)
        return;
    for (int i = 0; i < count; ++i) {
        if (arr[i] != nullptr) {
#ifdef _WIN32
            delete[] arr[i];
#elif __linux__
            g_free(arr[i]);
#else
            free(arr[i]);
#endif
        }
    }
#ifdef _WIN32
    delete[] arr;
#elif __linux__
    delete[] arr;
#else
    free(arr);
#endif
}
