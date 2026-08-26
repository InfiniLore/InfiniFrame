#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <string>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// Provides access to the embedded InfiniFrame JavaScript bridge source.
namespace Embedded {
    /// Returns the embedded InfiniFrame.js source as a UTF-16 wide string.
    const std::wstring& InfiniFrameJsUtf16();
    /// Returns the embedded InfiniFrame.js source as a UTF-8 string.
    const std::string& InfiniFrameJsUtf8();
}
