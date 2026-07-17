#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <cstdint>

#include "Basic.h"
#include "Monitor.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/** @brief Generic parameterless action callback */
using ACTION = void (*)();

/**
 * @brief Called when the WebView receives a message posted from JavaScript via window.chrome.webview.postMessage
 * @param message UTF-8 encoded message string
 * @param origin UTF-8 encoded sender origin URL, or null if unavailable
 */
using WebMessageReceivedCallback = void (*)(AutoString message, AutoString origin);

/**
 * @brief Called when a debug/diagnostic event is produced by the platform WebView runtime.
 * @param kind Event kind (Console, ScriptError, Navigation, Network, Process, Runtime)
 * @param message Human-readable message for the event
 * @param level Severity level where available (Debug/Info/Warning/Error)
 * @param uri Related URI where available
 * @param statusCode Status or native code when available, or 0 when unavailable
 * @param timestampUnixMillisecondsUtc UTC timestamp in unix milliseconds
 * @param platformPayload Optional platform-specific payload string
 */
using DebugEventCallback = void (*)(
    AutoString kind,
    AutoString message,
    AutoString level,
    AutoString uri,
    int statusCode,
    int64_t timestampUnixMillisecondsUtc,
    AutoString platformPayload
);

/** Version 1 custom-scheme response body kinds. Kind 2 is reserved for a future pull-based stream ABI. */
enum class CustomSchemeBodyKind : uint32_t {
    Buffered = 1,
    Stream = 2
};

using ReleaseCustomSchemeResponseCallback = void (*)(void* ownerContext);

/**
 * @brief Versioned custom-scheme response descriptor shared with .NET.
 *
 * The native caller owns this descriptor. The producer owns Body, ContentTypeUtf8, and OwnerContext until native calls
 * Release(OwnerContext) exactly once. Native must not free any field directly. ReservedRead/ReservedSeek are ABI space
 * for a future streaming body kind and must be null for buffered responses.
 */
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

static_assert(sizeof(uintptr_t) != 8 || sizeof(CustomSchemeResponse) == 72, "Unexpected 64-bit response ABI layout");

/**
 * @brief Called when the WebView requests a custom-scheme resource.
 * @param url Platform-native URL (UTF-16 on Windows, UTF-8 on Unix); borrowed for the duration of the call
 * @param response Caller-owned, zero-initialized output descriptor
 * @return Non-zero if a response was produced; zero for not found or handler failure
 */
using WebResourceRequestedCallback = int (*)(AutoString url, CustomSchemeResponse* response);

/**
 * @brief Called once per monitor during a GetAllMonitors enumeration.
 * @param monitor Pointer to a Monitor describing geometry and DPI scale for one display
 * @return Non-zero to continue enumeration, zero to stop
 */
using GetAllMonitorsCallback = int (*)(const Monitor* monitor);

/**
 * @brief Called when the window is resized.
 * @param width New client-area width in pixels
 * @param height New client-area height in pixels
 */
using ResizedCallback = void (*)(int width, int height);

/** @brief Called when the window is maximized */
using MaximizedCallback = void (*)();

/** @brief Called when the window is restored from a maximized or minimized state */
using RestoredCallback = void (*)();

/** @brief Called when the window is minimized */
using MinimizedCallback = void (*)();

/**
 * @brief Called when the window is moved.
 * @param x New left edge in screen pixels
 * @param y New top edge in screen pixels
 */
using MovedCallback = void (*)(int x, int y);

/**
 * @brief Called when the user attempts to close the window.
 * @return true to allow the window to close, false to cancel closing
 */
using ClosingCallback = bool (*)();

/** @brief Called when the window is closed */
using ClosedCallback = void (*)();

/** @brief Called when the window gains keyboard focus */
using FocusInCallback = void (*)();

/** @brief Called when the window loses keyboard focus */
using FocusOutCallback = void (*)();
