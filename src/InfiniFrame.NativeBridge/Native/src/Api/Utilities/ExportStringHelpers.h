#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef __linux__
#include <glib.h>
#endif

#ifdef _WIN32
#include <Windows.h>
#endif

#include <string>
#include <cstring>

#include "Runtime/Shared/Window/InfiniFrame.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::exports {
    /// Allocate a new C string from an error message string. Returns nullptr if the input is empty.
    /// @param value The error message to copy.
    /// @return Newly allocated null-terminated UTF-8 string, or nullptr if empty. Caller owns the result.
    inline const char* AllocateErrorMessageString(const std::string& value) {
        if (value.empty()) {
            return nullptr;
        }

        return AllocateStringCopy(value);
    }

    /// Return @p value if non-null, otherwise return a pointer to an empty string.
    /// @param value A possibly-null C string.
    /// @return @p value or a pointer to "".
    inline const char* NullToEmpty(const char* value) noexcept {
        static constexpr char empty[] = "";
        return value != nullptr ? value : const_cast<const char*>(empty);
    }

    /// Duplicate a null-terminated C string. Returns nullptr if the input is nullptr.
    /// @param str The source string to duplicate.
    /// @return Newly allocated null-terminated copy, or nullptr. Caller owns the result.
    inline const char* DuplicateString(const char* str) {
        if (str == nullptr) {
            return nullptr;
        }

        const size_t len = strlen(str);
        auto* copy = new char[len + 1];
        memcpy(copy, str, len + 1);
        return copy;
    }
}
