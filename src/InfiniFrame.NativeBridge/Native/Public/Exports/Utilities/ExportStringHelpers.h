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

#include "Public/InfiniFrame.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::exports {
#ifdef _WIN32
    inline AutoString AllocateErrorMessageString(const std::string& value) {
        if (value.empty()) {
            return nullptr;
        }

        const int wideCount =
            MultiByteToWideChar(CP_UTF8, 0, value.c_str(), static_cast<int>(value.size()), nullptr, 0);
        if (wideCount <= 0) {
            return nullptr;
        }

        auto* buffer = new wchar_t[wideCount + 1];
        const int converted =
            MultiByteToWideChar(CP_UTF8, 0, value.c_str(), static_cast<int>(value.size()), buffer, wideCount);
        if (converted <= 0) {
            delete[] buffer;
            return nullptr;
        }

        buffer[converted] = L'\0';
        return buffer;
    }
#else
    inline AutoString AllocateErrorMessageString(const std::string& value) {
        if (value.empty()) {
            return nullptr;
        }

        return AllocateStringCopy(value);
    }
#endif
    
    inline AutoString NullToEmpty(const AutoString value) noexcept {
#ifdef _WIN32
        static const wchar_t empty[] = L"";
#else
        static const char empty[] = "";
#endif
        return value != nullptr ? value : const_cast<AutoString>(empty);
    }

#ifdef _WIN32
    inline AutoString DuplicateString(const AutoStringConst str) {
        if (str == nullptr) {
            return nullptr;
        }

        const size_t len = wcslen(str);
        auto* copy = new wchar_t[len + 1];
        wcscpy_s(copy, len + 1, str);
        return copy;
    }
#else
    inline AutoString DuplicateString(const AutoStringConst str) {
        if (str == nullptr) {
            return nullptr;
        }

        const size_t len = strlen(str);
        auto* copy = new char[len + 1];
        strcpy(copy, str);
        return copy;
    }
#endif
}