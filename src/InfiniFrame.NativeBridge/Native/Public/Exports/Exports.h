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
