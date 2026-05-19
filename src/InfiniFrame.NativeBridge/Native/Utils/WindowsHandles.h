#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef _WIN32
#include <memory>
#include <windows.h>
#endif
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
#ifdef _WIN32
struct HBRUSHDeleter {
    void operator()(void* h) const noexcept {
        if (h)
            DeleteObject(h);
    }
};

struct HICONDeleter {
    void operator()(void* h) const noexcept {
        if (h)
            DestroyIcon(static_cast<HICON>(h));
    }
};

struct HDCDeleter {
    void operator()(void* h) const noexcept {
        if (h)
            DeleteDC(static_cast<HDC>(h));
    }
};

using UniqueHBRUSH = std::unique_ptr<void, HBRUSHDeleter>;
using UniqueHICON = std::unique_ptr<void, HICONDeleter>;
using UniqueHDC = std::unique_ptr<void, HDCDeleter>;

#endif