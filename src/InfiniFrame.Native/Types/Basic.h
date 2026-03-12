#pragma once
/**
 * @file Basic.h
 * @brief Basic type definitions for cross-platform interop
 */

#ifndef INFINIFRAME_TYPES_BASIC_H
#define INFINIFRAME_TYPES_BASIC_H

#include <string>
#include <memory>
#include <cstring>
#include <cwchar>

namespace InfiniFrame {

// ============================================================================
// String Deleters
// ============================================================================

namespace detail {

struct WideStringDeleter {
    void operator()(wchar_t* p) const noexcept {
        if (p) delete[] p;
    }
};

struct CStringDeleter {
    void operator()(char* p) const noexcept {
        if (p) std::free(p);
    }
};

} // namespace detail

// ============================================================================
// Native String Type
// ============================================================================

#ifdef _WIN32
using NativeString = std::wstring;
#else
using NativeString = std::string;
#endif

// ============================================================================
// AutoString (C API Interop)
// ============================================================================

#ifdef _WIN32
using AutoString = wchar_t*;
using AutoStringConst = const wchar_t*;
#else
using AutoString = char*;
using AutoStringConst = const char*;
#endif

// ============================================================================
// String Converters
// ============================================================================

inline NativeString ToNativeString(AutoStringConst str) {
    if (str == nullptr) return NativeString{};
#ifdef _WIN32
    return NativeString{str};
#else
    return NativeString{str};
#endif
}

inline NativeString ToNativeString(AutoString str) {
    return ToNativeString(static_cast<AutoStringConst>(str));
}

} // namespace InfiniFrame

#endif // INFINIFRAME_TYPES_BASIC_H
