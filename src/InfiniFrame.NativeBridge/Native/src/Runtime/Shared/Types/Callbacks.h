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

/**
 * @brief Called when the WebView requests a custom-scheme resource.
 * The handler must return a heap-allocated buffer and set outNumBytes and outContentType.
 * @param url UTF-8 URL of the requested resource
 * @param outNumBytes Output: byte length of the returned buffer
 * @param outContentType Output: MIME type string (e.g. "text/html")
 * @return Heap-allocated response body; ownership is transferred to the caller
 */
using WebResourceRequestedCallback = void* (*)(AutoString url, int* outNumBytes, AutoString* outContentType);

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
