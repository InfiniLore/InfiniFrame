# Breaking Changes from Photino

InfiniFrame is a full, independent rework of Photino.NET, Photino.Native, and the rest of the Photino ecosystem. 
It's not a drop-in replacement, and some API decisions and general design choices are intentionally different.
This document walks through what changed to help you migrate.

## Table of Contents

- [Package and Namespace](#package-and-namespace)
- [Entry Point and Builder API](#entry-point-and-builder-api)
- [Runtime Window API](#runtime-window-api)
- [Javascript Debugging](#javascript-debugging)
- [Event System](#event-system)
- [Web Messaging and Message Routing](#web-messaging-and-message-routing)
- [Web Security, CORS, and Trusted Origins](#web-security-cors-and-trusted-origins)
- [Logging](#logging)
- [Native C++ Interface](#native-c-interface)
- [Known Photino Issues Addressed](#known-photino-issues-addressed)
- [Removed or Replaced Features](#removed-or-replaced-features)

## Package and Namespace

| Aspect                   | Photino                                 | InfiniFrame                          |
|--------------------------|-----------------------------------------|--------------------------------------|
| NuGet package            | `Photino.NET`                           | `InfiniLore.InfiniFrame`             |
| C# namespace             | `Photino.NET`                           | `InfiniFrame`                        |
| Native DLL               | `Photino.Native`                        | `InfiniFrame.Native` (internal)      |
| C++ class                | `Photino`                               | `InfiniFrameWindow`                  |
| C++ init params          | `PhotinoInitParams`                     | `InfiniFrameInitParams`              |
| Exported function prefix | `Photino_`                              | `InfiniFrameNative_`                       |
| Default window title     | `"Photino"`                             | `"InfiniFrame"`                      |
| Default user agent       | `"Photino WebView"`                     | `"InfiniFrame WebView"`              |
| Default temp path        | `%LocalAppData%\Photino` (Windows only) | `%TEMP%\infiniframe` (all platforms) |

Every type that was prefixed `Photino` is now prefixed `InfiniFrame`. If you're calling the native DLL directly via P/Invoke, all exported symbol names need to change from `Photino_*` to `InfiniFrameNative_*`.

## Entry Point and Builder API

### Photino: direct construction

In Photino, you construct `PhotinoWindow` directly and configure it by calling methods on the object:

```csharp
var window = new PhotinoWindow()
    .SetTitle("My App")
    .SetDevToolsEnabled(true)
    .Load(new Uri("https://example.com"));
window.WaitForClose();
```

### InfiniFrame: explicit builder

InfiniFrame uses a dedicated builder. Configuration must be set before calling `Build()`:

```csharp
IInfiniFrameWindow window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .SetDevToolsEnabled(true)
    .SetStartUrl("https://example.com")
    .Build();
window.WaitForClose();
```

The builder optionally accepts an `IServiceProvider` for DI integration. The returned type is `IInfiniFrameWindow`, not a concrete class.

### Configuration from appsettings.json

InfiniFrame can pull window configuration from `IConfiguration` under an `"InfiniFrame"` key. This wasn't available in Photino:

```json
{
  "InfiniFrame": {
    "Title": "My App",
    "Width": 1280,
    "Height": 720
  }
}
```

### Type and class changes

| Photino                                           | InfiniFrame                                                       | Notes                                    |
|---------------------------------------------------|-------------------------------------------------------------------|------------------------------------------|
| `PhotinoWindow` (monolithic)                      | `IInfiniFrameWindow` (interface) + `InfiniFrameWindow` (concrete) | Users interact through the interface     |
| No builder                                        | `InfiniFrameWindowBuilder`                                        | Separate builder class                   |
| No configuration type                             | `InfiniFrameWindowConfiguration`                                  | Separates build-time config from runtime |
| `PhotinoWindow(PhotinoWindow parent)` constructor | `builder.SetParent(parentWindow)`                                 | Parent is now passed through the builder |

## Runtime Window API

### Method renames and removals

| Photino                                                                            | InfiniFrame                                                                         | Notes                                                                                             |
|------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------|
| `PhotinoWindow.Load(Uri)` / `Load(string)`                                         | `IInfiniFrameWindow.Load(Uri)` / `Load(string)`                                     | Available at runtime; initial URL can also be set via `SetStartUrl()` in the builder              |
| `PhotinoWindow.LoadRawString(string)`                                              | `IInfiniFrameWindow.LoadRawString(string)`                                          | Available at runtime; initial HTML can also be set via `SetStartString()` in the builder          |
| `PhotinoWindow.Center()`                                                           | `IInfiniFrameWindow.Center()` / `CenterOnCurrentMonitor()` / `CenterOnMonitor(int)` | Available at runtime                                                                              |
| `PhotinoWindow.MoveTo(Point, bool)` / `Offset(Point)`                              | `window.SetLocation(x, y)` / `window.Offset(x, y)`                                  |                                                                                                   |
| `PhotinoWindow.SetMinHeight(int)` / `SetMinWidth(int)`                             | Removed; use `SetMinSize(width, height)`                                            | Consolidated                                                                                      |
| `PhotinoWindow.SetMaxHeight(int)` / `SetMaxWidth(int)`                             | Removed; use `SetMaxSize(width, height)`                                            | Consolidated                                                                                      |
| `PhotinoWindow.SetLogVerbosity(int)`                                               | Removed; see [Logging](#logging)                                                    |                                                                                                   |
| `PhotinoWindow.Win32SetWebView2Path(string)`                                       | Internal; not on public interface                                                   |                                                                                                   |
| `PhotinoWindow.MacOsVersion` (static)                                              | Removed                                                                             |                                                                                                   |
| `PhotinoWindow.IsWindowsPlatform` / `IsMacOsPlatform` / `IsLinuxPlatform` (static) | Removed from public interface                                                       |                                                                                                   |
| `PhotinoWindow.WaitForClose()`                                                     | `IInfiniFrameWindow.WaitForClose()` and `WaitForCloseAsync()`                       | Async variant added                                                                               |
| `Monitor` struct                                                                   | `InfiniMonitor` record                                                              | Type renamed. Collection changed from `IReadOnlyList<Monitor>` to `ImmutableArray<InfiniMonitor>` |
| `PhotinoDialogButtons` / `PhotinoDialogResult` / `PhotinoDialogIcon`               | `InfiniFrameDialogButtons` / `InfiniFrameDialogResult` / `InfiniFrameDialogIcon`    | Renamed enums                                                                                     |
| `ShowSaveFile(title, defaultPath, filters, count)`                                 | `ShowSaveFile(title, defaultPath, filters, count, defaultFileName)`                 | Added `defaultFileName` parameter                                                                 |

### New APIs not in Photino

| API                                                                       | Description                                                                         |
|---------------------------------------------------------------------------|-------------------------------------------------------------------------------------|
| `IInfiniFrameWindow.Focused`                                              | Query or set keyboard focus state                                                   |
| `IInfiniFrameWindow.WaitForCloseAsync()`                                  | Async wait for window close                                                         |
| `IInfiniFrameWindow.ManagedThreadId`                                      | Thread ID of the window's message loop                                              |
| `IInfiniFrameWindow.AcquireNativeHandle()` / `NativeHandleLease.Handle`   | Lifetime-safe low-level native access; dispose the lease after the native call       |
| `IInfiniFrameWindow.CachedPreFullScreenBounds` / `CachedPreMaximizedBounds` | Saved geometry for restore                                                          |
| `RegisterCustomSchemeHandler()`                                           | Returns `IInfiniFrameWindow` (fluent); in Photino it returned void                  |
| `ZoomEnabled`                                                             | Separate bool controlling whether the user can zoom, distinct from the `Zoom` level |
| `PageNavigation.GetCurrentUri()` / `GetCurrentUrl()`              | First-class equivalent of Photino's `GetCurrentUrl()`; returns null after `LoadRawString` |
| `SetInstanceArbitrationMode(InstanceArbitrationMode)`            | Single-instance enforcement via named mutex; not available in Photino                    |
| `SetInstanceArbitrationMutexName(string)`                        | Configure the mutex name for instance arbitration                                        |
| `RegisterNavigationStartingHandler(handler)`                     | Inspect/cancel navigation requests before commit; not available in Photino               |

## Javascript Debugging

Photino users often configured JavaScript debugging through a mix of devtools flags and raw browser startup arguments.
InfiniFrame makes this explicit and deterministic.

### What changed

| Photino expectation | InfiniFrame behavior |
|---|---|
| `SetDevToolsEnabled(true)` also implies remote debug endpoint | `SetDevToolsEnabled(bool)` only controls local inspector/devtools UI |
| Remote debugging usually configured with raw Chromium args | Use `SetRemoteDebuggingPort(int? port)` (`1..65535`, `0/null` disables) |
| macOS inspector attachability was implicit via Safari Develop tools | Use explicit `SetWebInspectorEnabled(bool)` on macOS 13.3+ |
| Runtime mutation unclear | `Debug.RemoteDebuggingPort` is startup-only; runtime mutation throws `InvalidOperationException` |
| Platform behavior was implicit | Windows and Linux support remote debugging; macOS throws `PlatformNotSupportedException` when enabled |

### New API surface

- Builder: `SetRemoteDebuggingPort(int? port)`
- Builder: `SetWebInspectorEnabled(bool)` (macOS 13.3+)
- Runtime properties: `window.Debug.SupportsRemoteDebugging`, `window.Debug.RemoteDebuggingPort`
- Endpoint lookup: `window.Debug.TryGetRemoteDebuggingEndpoint(out Uri? endpoint)`
- Capability matrix: `window.Debug.Capabilities`
- Runtime diagnostics snapshot: `window.Debug.GetDiagnostics()`
- Endpoint probe helper: `window.Debug.TryProbeEndpoint(out Uri? endpoint, out string? reason)`
- Unified debug event stream: `window.Debug.Event`

### Precedence and security

- `SetRemoteDebuggingPort(...)` is authoritative over raw `SetBrowserControlInitParameters(...)` remote-debugging switches.
- InfiniFrame strips raw remote-debugging flags and applies loopback-only binding (`127.0.0.1`) for the explicit API.
- Startup fails with actionable `InvalidOperationException` when the requested port is unavailable (for example port-in-use).
- Linux path uses WebKitGTK inspector server configuration (`WEBKIT_INSPECTOR_SERVER` / `WEBKIT_INSPECTOR_HTTP_SERVER`).
- Linux inspector endpoint configuration is process-scoped in WebKitGTK.
- Remote endpoint parity is intentionally not universal. Always check capabilities before assuming endpoint or event support.

## Event System

Photino uses standard .NET `EventHandler<T>` delegates. Assigning a new handler replaces the previous one:

```csharp
// Photino: last assignment wins
window.RegisterWindowClosingHandler((sender, args) => { ... });
```

InfiniFrame uses `InfiniFrameOrderedEvent<T>`, an ordered multi-subscriber system with deterministic execution order. Handlers are added via `.Add()` and run in the order they were registered:

```csharp
// InfiniFrame: all handlers run in order
window.Events.WindowClosing.Add((window, args) => { ... });
window.Events.WindowClosing.Add((window, args) => { ... }); // also runs
```

### Closing event split

Photino has a single `RegisterWindowClosingHandler`. InfiniFrame splits this into two:

| Event                    | Type                             | Description                                                                 |
|--------------------------|----------------------------------|-----------------------------------------------------------------------------|
| `WindowClosingRequested` | `InfiniFrameOrderedClosingEvent` | Fired when close is requested; returning `true` from any handler cancels it |
| `WindowClosing`          | `InfiniFrameOrderedEvent`        | Fired when the window is definitively closing and cannot be cancelled       |

### Event handler DI injection

When an `IServiceProvider` is passed to `Build()`, InfiniFrame event handlers support DI-resolved parameters. This isn't available in Photino:

```csharp
window.Events.WindowClosing.Add((MyService svc, IInfiniFrameWindow w) => { ... });
```

## Web Messaging and Message Routing

### Photino: single raw handler

```csharp
window.RegisterWebMessageReceivedHandler((sender, message) => {
    // message is the full raw string from JS
});
```

One handler, full message string, no routing.

### InfiniFrame: typed message routing

InfiniFrame uses a versioned JSON envelope protocol. Messages sent from JavaScript are dispatched to the handler registered for their `id`:

```csharp
window.MessageHandlers.RegisterMessageHandler("myEvent", (window, payload) => {
    // payload is "some data"
});
```

The JavaScript side must use the envelope format:

```js
window.infiniframe.host.postData({ id: "myEvent", command: "Post", data: "some data", version: 2 });
```

`RegisterWebMessageReceivedHandler` is still available for raw message handling, but the legacy `messageId;payload` string format is gone. All messages must use the JSON envelope.

## Web Security, CORS, and Trusted Origins

This is probably the most impactful behavioral change if you're coming from a typical Photino setup.

In Photino, projects often just disabled web security to get around CORS and module-loading problems:

```csharp
// Common Photino workaround
windowBuilder.SetWebSecurityEnabled(false);
```

In InfiniFrame, browser web security and URI origin trust are separate concerns. `SetWebSecurityEnabled(...)` still controls browser engine security toggles, but origin trust is managed through `InfiniFrameUriSecurityPolicy` via builder APIs like `AddTrustedOrigin`, `SetTrustedOrigins`, and `SetTrustAllOrigins`.

For BlazorWebView, InfiniFrame serves content from `app://localhost/` and validates requests and messages against trusted origins. On Windows this relies on WebView2 custom scheme support (`ICoreWebView2EnvironmentOptions4`). Linux and macOS use WebKit and aren't affected by WebView2 runtime availability.

The preferred migration pattern is to keep web security on and explicitly trust only the origins you need:

```csharp
var app = InfiniFrameBlazorAppBuilder.CreateDefault(windowBuilder: wb => {
    wb.AddTrustedOrigin("https://xyz");
    wb.AddTrustedOrigin("https://cdn.jsdelivr.net");
    wb.AddTrustedOrigin("https://unpkg.com");
});
```

If you need broad compatibility during migration, you can opt in to trusting all origins:

```csharp
var app = InfiniFrameBlazorAppBuilder.CreateDefault(windowBuilder: wb => {
    wb.SetTrustAllOrigins(true);
});
```

`SetTrustAllOrigins(true)` is intentionally high-risk and should be treated as a temporary dev-time switch, not a production default.

### New API surface compared to Photino

| Purpose                             | InfiniFrame API                                       |
|-------------------------------------|-------------------------------------------------------|
| Add one trusted origin              | `builder.AddTrustedOrigin("https://xyz")`             |
| Replace trusted origin list         | `builder.SetTrustedOrigins("https://a", "https://b")` |
| Trust all origins (explicit opt-in) | `builder.SetTrustAllOrigins(true)`                    |
| Browser engine security toggle      | `builder.SetWebSecurityEnabled(bool)`                 |

## Logging

### Photino: integer verbosity

```csharp
window.SetLogVerbosity(2); // 0 = silent, higher = more verbose
```

Logs went to `Console.Out`. There was also a known bug ([#257](https://github.com/tryphotino/photino.NET/issues/257)) where setting verbosity to 0 would still emit a log message.

### InfiniFrame: ILogger

The integer verbosity system is gone entirely. InfiniFrame integrates with `Microsoft.Extensions.Logging`:

```csharp
var window = InfiniFrameWindowBuilder.Create()
    .Build(serviceProvider); // ILogger<IInfiniFrameWindow> resolved from DI
```

Log output respects your configured logging provider and level filters.

## Native C++ Interface

This section is only relevant if you're extending InfiniFrame at the native level or calling the native DLL directly.

### Pimpl architecture

Photino's `Photino` class exposes platform-specific fields (`_hWnd`, `GtkWidget*`, `NSWindow*`) directly in its class declaration, which means platform headers have to be included everywhere the class is used.

InfiniFrame uses the [Pimpl idiom](https://en.cppreference.com/w/cpp/language/pimpl.html) throughout: a `struct Impl` held by `std::unique_ptr<Impl>` is defined per-platform in the `.cpp`/`.mm` file. The `InfiniFrameWindow.h` header is entirely platform-agnostic.

### Build system and dependencies

| Aspect             | Photino                                | InfiniFrame                       |
|--------------------|----------------------------------------|-----------------------------------|
| Build system       | Visual Studio `.vcxproj` + Makefile    | CMake 4.0                         |
| C++ standard       | C++17                                  | C++23                             |
| JSON library       | `json.hpp` (nlohmann, bundled)         | `simdjson` via CMake FetchContent |
| String conversion  | Custom `ToWide`/`ToUTF8String` methods | `simdutf` via CMake FetchContent  |
| Formatting         | None                                   | `std::format` (C++23 standard)    |
| Sanitizers (Debug) | None                                   | ASan / UBSan / LeakSan enabled    |

### Platform file layout

| Photino                                  | InfiniFrame                                                                                                                     |
|------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------|
| `Photino.Windows.cpp` (all Windows code) | `Platform/Windows/Window.cpp`                                                                                                   |
| `Photino.Linux.cpp` (all Linux code)     | `Platform/Linux/Window.cpp`                                                                                                     |
| `Photino.Mac.mm` (all macOS code)        | `Platform/Mac/Window.mm` + separate delegate files                                                                              |
| macOS delegates inline in `.mm`          | `AppDelegate`, `UiDelegate`, `WindowDelegate`, `NavigationDelegate`, `UrlSchemeHandler`, `NSWindowBorderless` in separate files |

### P/Invoke generation

Photino uses manual `[DllImport]` attributes. InfiniFrame uses source-generated `[LibraryImport]` (requires .NET 7+):

```csharp
// Photino
[DllImport("Photino.Native", ...)]
static extern void Photino_SetTitle(IntPtr instance, string title);

// InfiniFrame
[LibraryImport("InfiniFrame.Native", ...)]
static partial void InfiniFrameNative_SetTitle(IntPtr instance, ...);
```

### String ownership

Photino has no explicit API for freeing native-allocated strings, which causes memory leaks in long-running applications.

InfiniFrame exports explicit free functions that must be called on any string returned from the native layer:

```csharp
InfiniFrameNative_FreeString(ptr);
InfiniFrameNative_FreeStringArray(ptr, count);
```

The managed wrapper calls these automatically. If you're calling native exports directly, you're responsible for invoking them yourself.

### SaveFileDialog signature change

The native `SaveFileDialog` export gained a `defaultFileName` parameter:

```csharp
// Photino
Photino_ShowSaveFile(title, defaultPath, filters, count)

// InfiniFrame
InfiniFrameNative_ShowSaveFile(title, defaultPath, filters, count, defaultFileName)
```

## Known Photino Issues Addressed

| Photino Issue                                                                      | Description                                                                              | How InfiniFrame addresses it                                                                              |
|------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------|
| [photino.native #173/174](https://github.com/tryphotino/photino.native/issues/173) | Custom scheme handlers completely broken on Windows (`WebResourceRequested` never fires) | Rewritten registration path; scheme handlers tested end-to-end in examples                                |
| [photino.native #165](https://github.com/tryphotino/photino.native/issues/165)     | Memory leak in `SendWebMessage` on Windows                                               | Explicit `InfiniFrameNative_FreeString` ownership model; Pimpl isolates per-window native allocations           |
| [photino.native #158](https://github.com/tryphotino/photino.native/issues/158)     | No way to programmatically focus a window                                                | `InfiniFrameNative_SetFocused` / `InfiniFrameNative_GetFocused` exported and exposed via `IInfiniFrameWindow.Focused` |
| [photino.native #163](https://github.com/tryphotino/photino.native/issues/163)     | UTF encoding bug in `SetWebView2RuntimePath` silently corrupts non-ASCII paths           | `simdutf` used for all UTF-8/UTF-16 conversions on Windows                                                |
| [photino.native #141](https://github.com/tryphotino/photino.native/issues/141)     | Stack overflow in `WaitForExit` on Linux                                                 | Per-window independent message loops; no shared global `MessageLoopState` lock                            |
| [photino.NET #75](https://github.com/tryphotino/photino.NET/issues/75)             | `RegisterWindowClosingHandler` does not fire on Linux                                    | Closing handler rewritten using the GTK `delete-event` signal correctly                                   |
| [photino.NET #257](https://github.com/tryphotino/photino.NET/issues/257)           | `SetLogVerbosity(0)` still logs a message                                                | Integer verbosity removed entirely and replaced by `ILogger<IInfiniFrameWindow>`                          |
| [photino.NET #232](https://github.com/tryphotino/photino.NET/issues/232)           | Custom scheme handlers break `fetch`/`XHR` (CORS interference)                           | Scheme handler registration refactored; CORS headers handled correctly per platform                       |
| [photino.native #175](https://github.com/tryphotino/photino.native/issues/175)     | `SetTopmost` uses wrong Win32 style; `null` crash on Linux                               | Fixed Win32 `HWND_TOPMOST`/`HWND_NOTOPMOST` usage; `null` guards added on Linux                           |

## Removed or Replaced Features

| Feature                                                                       | Notes                                                     |
|-------------------------------------------------------------------------------|-----------------------------------------------------------|
| `SetMinHeight` / `SetMinWidth` / `SetMaxHeight` / `SetMaxWidth`               | Consolidated into `SetMinSize(w, h)` / `SetMaxSize(w, h)` |
| `LogVerbosity` integer system                                                 | Replaced by `ILogger<IInfiniFrameWindow>`                 |
| `MacOsVersion` static property                                                | Removed                                                   |
| `IsWindowsPlatform` / `IsMacOsPlatform` / `IsLinuxPlatform` static properties | Internal; not on public interface                         |
| `UseOsDefaultLocation` / `UseOsDefaultSize` runtime properties                | Builder / config time only                                |
| `BrowserControlInitParameters` runtime property                               | Builder / config time only                                |
| `nlohmann/json.hpp` bundled JSON header                                       | Replaced by `simdjson`                                    |
| `fmt` formatting library                                                      | Replaced by `std::format`                                 |
