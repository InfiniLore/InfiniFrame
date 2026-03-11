#pragma once

#include "Types.h"
#include "Monitor.h"

using ACTION = void (*)();
using WebMessageReceivedCallback = void (*)(AutoString message);
using WebResourceRequestedCallback = void *(*)(AutoString url, int *outNumBytes, AutoString *outContentType);
using GetAllMonitorsCallback = int (*)(const Monitor *monitor);
using ResizedCallback = void (*)(int width, int height);
using MaximizedCallback = void (*)();
using RestoredCallback = void (*)();
using MinimizedCallback = void (*)();
using MovedCallback = void (*)(int x, int y);
using ClosingCallback = bool (*)();
using FocusInCallback = void (*)();
using FocusOutCallback = void (*)();
