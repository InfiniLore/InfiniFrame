#pragma once
/**
 * @file NativeString.h
 * @brief Ownership helpers for strings returned through the native C ABI.
 */

#ifndef INFINIFRAME_INTEROP_NATIVESTRING_H
#define INFINIFRAME_INTEROP_NATIVESTRING_H

#include "../Types/Basic.h"

#include <cstring>
#include <cstdlib>
#include <cwchar>
#include <string>

#ifdef __linux__
#include <glib.h>
#endif

namespace InfiniFrame::Native::Interop {
    inline AutoString AllocateNativeStringCopy(AutoStringConst value) {
        if (value == nullptr)
            value =
#ifdef _WIN32
                L"";
#else
                "";
#endif

#ifdef _WIN32
        const size_t length = std::wcslen(value);
        auto* copy = new wchar_t[length + 1];
        std::memcpy(copy, value, (length + 1) * sizeof(wchar_t));
        return copy;
#elif __linux__
        return g_strdup(value);
#else
        const size_t length = std::strlen(value);
        auto* copy = static_cast<char*>(std::malloc(length + 1));
        if (copy == nullptr)
            return nullptr;

        std::memcpy(copy, value, length + 1);
        return copy;
#endif
    }

    inline AutoString AllocateNativeStringCopy(const NativeString& value) {
        return AllocateNativeStringCopy(value.c_str());
    }

    inline AutoString* AllocateNativeStringArray(const int count) {
        if (count <= 0)
            return nullptr;

#if defined(_WIN32) || defined(__linux__)
        return new AutoString[count]();
#else
        return static_cast<AutoString*>(std::calloc(static_cast<size_t>(count), sizeof(AutoString)));
#endif
    }

    inline void FreeNativeString(AutoString value) noexcept {
        if (value == nullptr)
            return;

#ifdef _WIN32
        delete[] value;
#elif __linux__
        g_free(value);
#else
        std::free(value);
#endif
    }

    inline void FreeNativeStringArrayContainer(AutoString* values) noexcept {
        if (values == nullptr)
            return;

#if defined(_WIN32) || defined(__linux__)
        delete[] values;
#else
        std::free(values);
#endif
    }

    inline void FreeNativeStringArray(AutoString* values, const int count) noexcept {
        if (values == nullptr)
            return;

        for (int i = 0; i < count; ++i)
            FreeNativeString(values[i]);

        FreeNativeStringArrayContainer(values);
    }
}

#endif // INFINIFRAME_INTEROP_NATIVESTRING_H
