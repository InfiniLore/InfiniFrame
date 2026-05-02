#pragma once
/**
 * @file NativeBuffer.h
 * @brief Ownership helpers for buffers returned across the native interop boundary.
 */

#ifndef INFINIFRAME_INTEROP_NATIVEBUFFER_H
#define INFINIFRAME_INTEROP_NATIVEBUFFER_H

#ifdef _WIN32
#include <objbase.h>
#else
#include <cstdlib>
#endif

#include <memory>

namespace InfiniFrame::Native::Interop {
    inline void FreeNativeBuffer(void* buffer) noexcept {
        if (buffer == nullptr)
            return;

#ifdef _WIN32
        CoTaskMemFree(buffer);
#else
        std::free(buffer);
#endif
    }

    struct NativeBufferDeleter {
        void operator()(void* buffer) const noexcept {
            FreeNativeBuffer(buffer);
        }
    };

    using NativeBufferPtr = std::unique_ptr<void, NativeBufferDeleter>;

    inline NativeBufferPtr AdoptNativeBuffer(void* buffer) noexcept {
        return NativeBufferPtr(buffer);
    }
}

#endif // INFINIFRAME_INTEROP_NATIVEBUFFER_H
