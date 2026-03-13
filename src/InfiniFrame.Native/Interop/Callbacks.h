#pragma once
/**
 * @file Callbacks.h
 * @brief C-style callback type definitions for interop
 */

#ifndef INFINIFRAME_INTEROP_CALLBACKS_H
#define INFINIFRAME_INTEROP_CALLBACKS_H

#include "../Types/Basic.h"
#include "../Types/Dialog.h"

// ============================================================================
// C-style Callbacks (for C# interop)
// ============================================================================

using ACTION = void (*)();
using WebMessageReceivedCallback = void (*)(AutoString message);
using WebResourceRequestedCallback = void* (*)(AutoString url, int* outNumBytes, AutoString* outContentType);
using GetAllMonitorsCallback = int (*)(const Monitor* monitor);
using ResizedCallback = void (*)(int width, int height);
using MaximizedCallback = void (*)();
using RestoredCallback = void (*)();
using MinimizedCallback = void (*)();
using MovedCallback = void (*)(int x, int y);
using ClosingCallback = bool (*)();
using FocusInCallback = void (*)();
using FocusOutCallback = void (*)();

#endif // INFINIFRAME_INTEROP_CALLBACKS_H
