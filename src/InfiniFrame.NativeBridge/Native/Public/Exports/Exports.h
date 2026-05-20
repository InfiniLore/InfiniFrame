#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef __linux__
#include <glib.h>
#endif

#ifdef _WIN32
#define EXPORTED __declspec(dllexport)
#else
#define EXPORTED
#endif

#include "Public/InfiniFrame.h"
#include "Utils/ExportGuards.h"
// ---------------------------------------------------------------------------------------------------------------------
// String Ownership Contract
// ---------------------------------------------------------------------------------------------------------------------
// Every exported function follows these pointer-ownership rules:
//
//  OWNED (caller-frees):
//    Any AutoString written to an out-parameter (AutoString*) is heap-allocated
//    by the native layer. The caller MUST free it with InfiniFrame_FreeString().
//    Multi-string results (AutoString** from dialog APIs) must be freed with
//    InfiniFrame_FreeStringArray(values, count).
//
//    Affected exports:
//      InfiniFrame_GetUserAgent, InfiniFrame_GetTitle, InfiniFrame_GetIconFileName,
//      InfiniFrame_GetLastErrorMessage, InfiniFrame_ShowOpenFile,
//      InfiniFrame_ShowOpenFolder, InfiniFrame_ShowSaveFile
//
//  BORROWED (callee does not take ownership):
//    All AutoString input parameters (const AutoString) are borrowed for the
//    duration of the call. The native layer copies what it needs internally.
//    The caller retains ownership and may free after the call returns.
//
//  NULL semantics:
//    Returning nullptr from an owned-string function means "no value" (e.g. no
//    file selected). The caller must still check before calling FreeString.
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using infiniframe::exports::EnsureNotNull;
using infiniframe::exports::GetLastErrorMessageCopy;
using infiniframe::exports::ResetOut;
using infiniframe::exports::ResetOut2;
using infiniframe::exports::RunExportStatus;
using infiniframe::exports::RunReturnExport;
using infiniframe::exports::RunWindowExportStatus;
using infiniframe::exports::RunWindowReturnExport;

template <typename T> bool EnsureOutNotNull(T* value, const char* argumentName) noexcept {
    return infiniframe::exports::EnsureNotNull(value, argumentName, InteropStatus::OutParameterSetToInvalidNull);
}

inline AutoString NullToEmpty(const AutoString value) noexcept {
#ifdef _WIN32
    static const wchar_t empty[] = L"";
#else
    static const char empty[] = "";
#endif
    return value != nullptr ? value : const_cast<AutoString>(empty);
}
