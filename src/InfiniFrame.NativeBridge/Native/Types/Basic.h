#pragma once
/**
 * @file Basic.h
 * @brief Basic type definitions for cross-platform interop
 */

#include <string>

// ---------------------------------------------------------------------------------------------------------------------
// Native String Type
// ---------------------------------------------------------------------------------------------------------------------

#ifdef _WIN32
using NativeString = std::wstring;
#else
using NativeString = std::string;
#endif

// ---------------------------------------------------------------------------------------------------------------------
// AutoString (C API Interop)
// ---------------------------------------------------------------------------------------------------------------------

#ifdef _WIN32
using AutoString = wchar_t*;
using AutoStringConst = const wchar_t*;
#else
using AutoString = char*;
using AutoStringConst = const char*;
#endif
