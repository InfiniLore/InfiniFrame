# Examples

Runnable examples demonstrating different InfiniFrame integration patterns

## Overview

| Example                                                                                | Integration     | Demonstrates                                                  |
|----------------------------------------------------------------------------------------|-----------------|---------------------------------------------------------------|
| [BlazorWebView](InfiniFrameExample.BlazorWebView/)                                     | `BlazorWebView` | Basic Blazor app in a native window                           |
| [BlazorWebView.MultiWindowSample](InfiniFrameExample.BlazorWebView.MultiWindowSample/) | `BlazorWebView` | Multiple independent windows with different Blazor components |
| [WebApp.Blazor](InfiniFrameExample.WebApp.Blazor/)                                     | `WebServer`     | Blazor Server hosted via ASP.NET Core                         |
| [WebApp.React](InfiniFrameExample.WebApp.React/)                                       | `WebServer`     | React frontend with custom scheme handler and web messaging   |
| [WebApp.Vue](InfiniFrameExample.WebApp.Vue/)                                           | `WebServer`     | Vue.js frontend with all built-in JS message handlers         |

## Running an Example

All examples require the native `InfiniFrame.Native` library to be built first

```bash
# From the repo root — build the native library for your platform
cmake -S src/InfiniFrame.Native -B artifacts/native/windows/x64/Debug -DCMAKE_BUILD_TYPE=Debug
cmake --build artifacts/native/windows/x64/Debug

# Then run an example
dotnet run --project examples/InfiniFrameExample.BlazorWebView
```

## Guides

Each example maps to a documentation guide:

- BlazorWebView → [Blazor WebView Guide](../docs/Guides/Blazor.md)
- WebApp.Blazor, WebApp.React, WebApp.Vue → [Web Server Guide](../docs/Guides/WebServer.md)
- WebApp.React → [Core Window Guide](../docs/Guides/CoreWindow.md) (custom schemes, messaging)
- WebApp.Vue → [JavaScript Interop Guide](../docs/Guides/JsInterop.md) (built-in message handlers)
