# Examples

Runnable examples demonstrating different InfiniFrame integration patterns

## Overview

| Example                                                                          | Integration      | Demonstrates                                                  |
|----------------------------------------------------------------------------------|------------------|---------------------------------------------------------------|
| [BlazorWebView](InfiniFrameExample.BlazorWebView/)                               | `BlazorWebView`  | Basic Blazor app in a native window                           |
| [NativeMenu](InfiniFrameExample.NativeMenu/)                                     | `NativeMenu`     | Native menu integration                                       |
| [TrimAotSmoke](InfiniFrameExample.TrimAotSmoke/)                                 | `TrimAotSmoke`   | Trim and AOT compilation smoke test                           |
| [SingleFileExe](SingleFileExe/InfiniFrameExample.SingleFileExe/)                 | `SingleFile`     | Embedded wwwroot in a single-file executable                  |
| [SingleFileExe.Vue](SingleFileExe/InfiniFrameExample.SingleFileExe.Vue/)         | `SingleFile`     | Vue.js with single-file packaging                             |
| [SingleFileExe.React](SingleFileExe/InfiniFrameExample.SingleFileExe.React/)     | `SingleFile`     | React with single-file packaging                              |
| [SingleFileExe.MudBlazor](SingleFileExe/InfiniFrameExample.SingleFileExe.MudBlazor/) | `SingleFile` | Blazor + MudBlazor with single-file packaging                 |
| [WebApp](WebApp/InfiniFrameExample.WebApp/)                                      | `WebServer`      | Basic web app via ASP.NET Core                                |
| [WebApp.Blazor](WebApp/InfiniFrameExample.WebApp.Blazor/)                        | `WebServer`      | Blazor Server hosted via ASP.NET Core                         |
| [WebApp.React](WebApp/InfiniFrameExample.WebApp.React/)                          | `WebServer`      | React frontend with custom scheme handler and web messaging   |
| [WebApp.Vue](WebApp/InfiniFrameExample.WebApp.Vue/)                              | `WebServer`      | Vue.js frontend with all built-in JS message handlers         |

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
- NativeMenu → [Core Window Guide](../docs/Guides/CoreWindow.md) (native menus)
- SingleFileExe → [Pack Tool Guide](../docs/Guides/PackTool.md) (single-file publishing)
- WebApp, WebApp.Blazor, WebApp.React, WebApp.Vue → [Web Server Guide](../docs/Guides/WebServer.md)
- WebApp.React → [Core Window Guide](../docs/Guides/CoreWindow.md) (custom schemes, messaging)
- WebApp.Vue → [JavaScript Interop Guide](../docs/Guides/JsInterop.md) (built-in message handlers)
