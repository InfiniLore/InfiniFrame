#pragma once
/**
 * @file Callbacks.h
 * @brief C-style callback type definitions for interop
 */

#ifndef INFINIFRAME_TYPES_CALLBACKS_H
#define INFINIFRAME_TYPES_CALLBACKS_H

#include "Basic.h"
#include "Dialog.h"

// ============================================================================
// C-style Callbacks (for C# interop)
// ============================================================================

/** @brief Generic parameterless action callback */
using ACTION = void (*)();

/**
 * @brief Called when the WebView receives a message posted from JavaScript via window.chrome.webview.postMessage
 * @param message UTF-8 encoded message string
 */
using WebMessageReceivedCallback = void (*)(AutoString message);

/**
 * @brief Called when the WebView requests a custom-scheme resource.
 * The handler must return a heap-allocated buffer and set outNumBytes and outContentType.
 * @param url UTF-8 URL of the requested resource
 * @param outNumBytes Output: byte length of the returned buffer
 * @param outContentType Output: MIME type string (e.g. "text/html")
 * @return Heap-allocated response body; ownership is transferred to the caller
 */
using WebResourceRequestedCallback = void *(*)(AutoString url, int *outNumBytes, AutoString *outContentType);

/**
 * @brief Called once per monitor during a GetAllMonitors enumeration.
 * @param monitor Pointer to a Monitor describing geometry and DPI scale for one display
 * @return Non-zero to continue enumeration, zero to stop
 */
using GetAllMonitorsCallback = int (*)(const Monitor *monitor);

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

/** @brief Called when the window gains keyboard focus */
using FocusInCallback = void (*)();

/** @brief Called when the window loses keyboard focus */
using FocusOutCallback = void (*)();

#endif // INFINIFRAME_TYPES_CALLBACKS_H