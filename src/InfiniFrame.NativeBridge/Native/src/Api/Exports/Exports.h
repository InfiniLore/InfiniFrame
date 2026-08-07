#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef __linux__
#include <glib.h>
#endif

#ifdef _WIN32
#define EXPORTED __declspec(dllexport)
#elif defined(__GNUC__)
#define EXPORTED __attribute__((visibility("default")))
#else
#define EXPORTED
#endif

#include "Runtime/Shared/Types/Basic.h"
#include "Runtime/Shared/Utilities/InteropStatus.h"
#include "Api/Utilities/Utilities.h"

// ---------------------------------------------------------------------------------------------------------------------
// String Ownership Contract
// ---------------------------------------------------------------------------------------------------------------------
// Every exported function follows these pointer-ownership rules:
//
//  OWNED (caller-frees):
//    Any const char* written to an out-parameter (const char**) is heap-allocated
//    by the native layer. The caller MUST free it with InfiniFrameNative_FreeString().
//    Multi-string results (const char*** from dialog APIs) must be freed with
//    InfiniFrameNative_FreeStringArray(values, count).
//
//  BORROWED (callee does not take ownership):
//    All const char* input parameters (const char*) are borrowed for the
//    duration of the call. The native layer copies what it needs internally.
//    The caller retains ownership and may free after the call returns.
//
//  NULL semantics:
//    Returning nullptr from an owned-string function means "no value" (e.g. no
//    file selected). The caller must still check before calling FreeString.
