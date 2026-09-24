#pragma once

#include <cstdint>

/** Version 1 custom-scheme response body kinds. Kind 2 is reserved for streaming. */
enum class CustomSchemeBodyKind : uint32_t {
    Buffered = 1,
    Stream = 2
};

using ReleaseCustomSchemeResponseCallback = void (*)(void* ownerContext);

/** Versioned response descriptor shared with the managed ABI. */
struct CustomSchemeResponse {
    static constexpr uint32_t CurrentAbiVersion = 1;
    static constexpr uint64_t MaxBufferedBodyBytes = 256ULL * 1024ULL * 1024ULL;

    uint32_t StructSize;
    uint32_t AbiVersion;
    uint32_t StatusCode;
    uint32_t BodyKind;
    uint64_t ContentLength;
    const uint8_t* Body;
    const char* ContentTypeUtf8;
    void* OwnerContext;
    ReleaseCustomSchemeResponseCallback Release;
    void* ReservedRead;
    void* ReservedSeek;
};

static_assert(sizeof(uintptr_t) != 8 || sizeof(CustomSchemeResponse) == 72,
              "Unexpected 64-bit response ABI layout");
