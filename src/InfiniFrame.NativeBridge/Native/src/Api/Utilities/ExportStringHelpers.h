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

#include "Runtime/Shared/Window/InfiniFrame.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::exports {
    inline const char* AllocateErrorMessageString(const std::string& value) {
        if (value.empty()) {
            return nullptr;
        }

        return AllocateStringCopy(value);
    }

    inline const char* NullToEmpty(const char* value) noexcept {
        static const char empty[] = "";
        return value != nullptr ? value : const_cast<const char*>(empty);
    }

    inline const char* DuplicateString(const char* str) {
        if (str == nullptr) {
            return nullptr;
        }

        const size_t len = strlen(str);
        auto* copy = new char[len + 1];
        strcpy(copy, str);
        return copy;
    }
}