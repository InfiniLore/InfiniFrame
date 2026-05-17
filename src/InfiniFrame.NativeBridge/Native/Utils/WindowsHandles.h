#pragma once

#ifndef INFINIFRAME_UTILS_WINDOWS_HANDLES_H
#define INFINIFRAME_UTILS_WINDOWS_HANDLES_H

#ifdef _WIN32

#include <memory>
#include <windows.h>

struct HBRUSHDeleter {
    void operator()(void* h) const noexcept {
        if (h)
            DeleteObject(static_cast<HBRUSH>(h));
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

#endif // INFINIFRAME_UTILS_WINDOWS_HANDLES_H
