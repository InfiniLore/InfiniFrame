# Breaking Changes vs Photino.NET

InfiniFrame is a complete, independent rework of [Photino.NET](https://github.com/tryphotino/photino.NET) and [photino.native](https://github.com/tryphotino/photino.native). It is not a drop-in replacement — this document covers every API, naming, and behavioral difference to assist with migration

## Table of Contents

- [Package and Namespace](#package-and-namespace)
- [Entry Point and Builder API](#entry-point-and-builder-api)
- [Runtime Window API](#runtime-window-api)
- [Event System](#event-system)
- [Web Messaging and Message Routing](#web-messaging-and-message-routing)
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
| Exported function prefix | `Photino_`                              | `InfiniFrame_`                       |
| Default window title     | `"Photino"`                             | `"InfiniFrame"`                      |
| Default user agent       | `"Photino WebView"`                     | `"InfiniFrame WebView"`              |
| Default temp path        | `%LocalAppData%\Photino` (Windows-only) | `%TEMP%\infiniframe` (all platforms) |

All type names that were prefixed `Photino` are now prefixed `InfiniFrame`. If you have any code targeting the native DLL directly via P/Invoke, all exported symbol names must be updated from `Photino_*` to `InfiniFrame_*`

## Entry Point and Builder API

### Photino — direct construction

```csharp
var window = new PhotinoWindow()
    .SetTitle("My App")
    .SetDevToolsEnabled(true)
    .Load(new Uri("https://example.com"));
window.WaitForClose();
```

`PhotinoWindow` is constructed directly. All configuration is done by calling methods on the object after construction. There is no separate builder class

### InfiniFrame — explicit builder

```csharp
IInfiniFrameWindow window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .SetDevToolsEnabled(true)
    .SetStartUrl("https://example.com")
    .Build();
window.WaitForClose();
```

Configuration must be set **before** calling `Build()`. The builder accepts an optional `IServiceProvider` for DI integration. The resulting type is `IInfiniFrameWindow`, not a concrete class

### Configuration from appsettings.json

InfiniFrame supports sourcing window configuration from `IConfiguration` under an `"InfiniFrame"` key — not available in Photino:

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

| Photino                                           | InfiniFrame                                                       | Notes                                          |
|---------------------------------------------------|-------------------------------------------------------------------|------------------------------------------------|
| `PhotinoWindow` (monolithic)                      | `IInfiniFrameWindow` (interface) + `InfiniFrameWindow` (concrete) | Users interact through the interface           |
| No builder                                        | `InfiniFrameWindowBuilder`                                        | Separate builder class                         |
| No configuration type                             | `InfiniFrameWindowConfiguration`                                  | Separates build-time config from runtime       |
| `PhotinoWindow(PhotinoWindow parent)` constructor | `builder.SetParent(parentWindow)`                                 | Parent passed through builder, not constructor |

## Runtime Window API

### Method renames and removals

| Photino                                                                            | InfiniFrame                                                                         | Notes                                                                                             |
|------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------|
| `PhotinoWindow.Load(Uri)` / `Load(string)`                                         | `IInfiniFrameWindow.Load(Uri)` / `Load(string)`                                     | Renamed and available at runtime; initial URL can also be set via `SetStartUrl()` in the builder  |
| `PhotinoWindow.LoadRawString(string)`                                              | `IInfiniFrameWindow.LoadRawString(string)`                                          | Available at runtime; initial HTML can also be set via `SetStartString()` in the builder          |
| `PhotinoWindow.Center()`                                                           | `IInfiniFrameWindow.Center()` / `CenterOnCurrentMonitor()` / `CenterOnMonitor(int)` | Available at runtime                                                                              |
| `PhotinoWindow.MoveTo(Point, bool)` / `Offset(Point)`                              | `window.SetLocation(x, y)` / `window.Offset(x, y)`                                  |                                                                                                   |
| `PhotinoWindow.SetMinHeight(int)` / `SetMinWidth(int)`                             | Removed — use `SetMinSize(width, height)`                                           | Consolidated                                                                                      |
| `PhotinoWindow.SetMaxHeight(int)` / `SetMaxWidth(int)`                             | Removed — use `SetMaxSize(width, height)`                                           | Consolidated                                                                                      |
| `PhotinoWindow.SetLogVerbosity(int)`                                               | Removed — see [Logging](#logging)                                                   |                                                                                                   |
| `PhotinoWindow.Win32SetWebView2Path(string)`                                       | Internal — not on public interface                                                  |                                                                                                   |
| `PhotinoWindow.MacOsVersion` (static)                                              | Removed                                                                             |                                                                                                   |
| `PhotinoWindow.IsWindowsPlatform` / `IsMacOsPlatform` / `IsLinuxPlatform` (static) | Removed from public interface                                                       |                                                                                                   |
| `PhotinoWindow.WaitForClose()`                                                     | `IInfiniFrameWindow.WaitForClose()` and `WaitForCloseAsync()`                       | Async variant added                                                                               |
| `Monitor` struct                                                                   | `InfiniMonitor` record                                                              | Type renamed. Collection changed from `IReadOnlyList<Monitor>` to `ImmutableArray<InfiniMonitor>` |
| `PhotinoDialogButtons` / `PhotinoDialogResult` / `PhotinoDialogIcon`               | `InfiniFrameDialogButtons` / `InfiniFrameDialogResult` / `InfiniFrameDialogIcon`    | Renamed enums                                                                                     |
| `ShowSaveFile(title, defaultPath, filters, count)`                                 | `ShowSaveFile(title, defaultPath, filters, count, defaultFileName)`                 | Added `defaultFileName` parameter                                                                 |

### New APIs not in Photino

| API                                                                         | Description                                                                 |
|-----------------------------------------------------------------------------|-----------------------------------------------------------------------------|
| `IInfiniFrameWindow.Focused`                                                | Query or set keyboard focus state                                           |
| `IInfiniFrameWindow.WaitForCloseAsync()`                                    | Async wait for window close                                                 |
| `IInfiniFrameWindow.ManagedThreadId`                                        | Thread ID of the window's message loop                                      |
| `IInfiniFrameWindow.InstanceHandle` / `NativeType`                          | Low-level native access                                                     |
| `IInfiniFrameWindow.CachedPreFullScreenBounds` / `CachedPreMaximizedBounds` | Saved geometry for restore                                                  |
| `RegisterCustomSchemeHandler()`                                             | Returns `IInfiniFrameWindow` (fluent) — in Photino it returned void         |
| `ZoomEnabled`                                                               | Separate bool controlling whether user can zoom, distinct from `Zoom` level |

## Event System

Photino uses standard .NET `EventHandler<T>` delegates. Assigning a new handler **replaces** the previous one:

```csharp
// Photino — last assignment wins
window.RegisterWindowClosingHandler((sender, args) => { ... });
```

InfiniFrame uses `InfiniFrameOrderedEvent<T>` — an ordered multi-subscriber system with deterministic execution order. Handlers are added via `.Add()` and execute in registration order:

```csharp
// InfiniFrame — all handlers run in order
window.Events.WindowClosing.Add((window, args) => { ... });
window.Events.WindowClosing.Add((window, args) => { ... }); // also runs
```

### Closing event split

Photino has a single `RegisterWindowClosingHandler`. InfiniFrame splits this into two events:

| Event                    | Type                             | Description                                                                           |
|--------------------------|----------------------------------|---------------------------------------------------------------------------------------|
| `WindowClosingRequested` | `InfiniFrameOrderedClosingEvent` | Fired when close is requested and returning `true` from any handler cancels the close |
| `WindowClosing`          | `InfiniFrameOrderedEvent`        | Fired when the window is definitively closing and cannot be cancelled                 |

### Event handler DI injection

InfiniFrame event handlers support DI-resolved parameters when a `IServiceProvider` is provided to `Build()`:

```csharp
window.Events.WindowClosing.Add((MyService svc, IInfiniFrameWindow w) => { ... });
```

This is not available in Photino.

## Web Messaging and Message Routing

### Photino — single raw handler

```csharp
window.RegisterWebMessageReceivedHandler((sender, message) => {
    // message is the full raw string from JS
});
```

One handler. The full message string is passed as-is.

### InfiniFrame — typed message routing

InfiniFrame uses a versioned JSON envelope protocol. Messages sent from JavaScript as `{"id":"myEvent","data":"some data","version":1}` are dispatched to the handler registered for `"myEvent"`:

```csharp
window.MessageHandlers.RegisterMessageHandler("myEvent", (window, payload) => {
    // payload is "some data"
});
```

A fallback `RegisterWebMessageReceivedHandler` is still available for raw unrouted messages, but the typed routing is the primary pattern.

The JavaScript side should use the envelope format:

```js
window.external.sendMessage(JSON.stringify({ id: "myEvent", data: "some data", version: 1 }));
```

Legacy `messageId;payload` is no longer supported.

## Logging

### Photino — integer verbosity

```csharp
window.SetLogVerbosity(2); // 0 = silent, higher = more verbose
```

Logs were written to `Console.Out`. Setting verbosity would itself log a message even when verbosity was 0 (known bug [#257](https://github.com/tryphotino/photino.NET/issues/257)).

### InfiniFrame — ILogger

The integer verbosity system is removed entirely. InfiniFrame integrates with `Microsoft.Extensions.Logging`:

```csharp
var window = InfiniFrameWindowBuilder.Create()
    .Build(serviceProvider); // ILogger<IInfiniFrameWindow> resolved from DI
```

Log output respects the configured logging provider and log-level filtering

## Native C++ Interface

This section is only relevant if you extend InfiniFrame at the native level or have code that calls the native DLL directly

### Pimpl architecture

Photino's `Photino` class exposes platform-specific fields (`_hWnd`, `GtkWidget*`, `NSWindow*`) directly in its class declaration, requiring platform headers to be included everywhere the class is used.

InfiniFrame uses the [Pimpl idiom](https://en.cppreference.com/w/cpp/language/pimpl.html) throughout: a `struct Impl` held by `std::unique_ptr<Impl>` is defined per-platform in the `.cpp`/`.mm` file. The `InfiniFrameWindow.h` header is entirely platform-agnostic

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
static partial void InfiniFrame_SetTitle(IntPtr instance, ...);
```

### String ownership

Photino has no explicit API for freeing native-allocated strings, which leads to memory leaks in long-running applications

InfiniFrame exports explicit free functions that must be called on any string returned from the native layer:

```csharp
InfiniFrame_FreeString(ptr);
InfiniFrame_FreeStringArray(ptr, count);
```

These are called automatically by the managed wrapper — but if you call native exports directly, you are responsible for invoking them

### `SaveFileDialog` signature change

The native `SaveFileDialog` export gained a `defaultFileName` parameter:

```csharp
// Photino
Photino_ShowSaveFile(title, defaultPath, filters, count)

// InfiniFrame
InfiniFrame_ShowSaveFile(title, defaultPath, filters, count, defaultFileName)
```

## Known Photino Issues Addressed

The following are open or previously reported issues in the Photino repositories that are resolved in InfiniFrame:

| Photino Issue                                                                      | Description                                                                            | How InfiniFrame Addresses It                                                                           |
|------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------|
| [photino.native #173/174](https://github.com/tryphotino/photino.native/issues/173) | Custom scheme handlers completely broken on Windows (WebResourceRequested never fires) | Rewritten registration path. Scheme handlers tested end-to-end in examples                             |
| [photino.native #165](https://github.com/tryphotino/photino.native/issues/165)     | Memory leak in `SendWebMessage` on Windows                                             | Explicit `InfiniFrame_FreeString` ownership model. Pimpl isolates per-window native allocations        |
| [photino.native #158](https://github.com/tryphotino/photino.native/issues/158)     | No way to programmatically focus a window                                              | `InfiniFrame_SetFocused` / `InfiniFrame_GetFocused` exported. Exposed via `IInfiniFrameWindow.Focused` |
| [photino.native #163](https://github.com/tryphotino/photino.native/issues/163)     | UTF encoding bug in `SetWebView2RuntimePath` silently corrupts non-ASCII paths         | `simdutf` used for all UTF-8 ↔ UTF-16 conversions on Windows                                           |
| [photino.native #141](https://github.com/tryphotino/photino.native/issues/141)     | Stack overflow in `WaitForExit` on Linux                                               | Per-window independent message loops. No shared global `MessageLoopState` lock                         |
| [photino.NET #75](https://github.com/tryphotino/photino.NET/issues/75)             | `RegisterWindowClosingHandler` does not fire on Linux                                  | Closing handler rewritten using GTK `delete-event` signal correctly                                    |
| [photino.NET #257](https://github.com/tryphotino/photino.NET/issues/257)           | `SetLogVerbosity(0)` still logs a message                                              | Integer verbosity removed entirely. Replaced by `ILogger<IInfiniFrameWindow>`                          |
| [photino.NET #232](https://github.com/tryphotino/photino.NET/issues/232)           | Custom scheme handlers break `fetch`/`XHR` (CORS interference)                         | Scheme handler registration refactored. CORS headers handled correctly per platform                    |
| [photino.native #175](https://github.com/tryphotino/photino.native/issues/175)     | `SetTopmost` uses wrong Win32 style. `null` crash on Linux                             | Fixed Win32 `HWND_TOPMOST`/`HWND_NOTOPMOST` usage. `null` guards added on Linux                        |

## Removed or Replaced Features

The following Photino features are not present in InfiniFrame:

| Feature                                                                            | Notes                                                     |
|------------------------------------------------------------------------------------|-----------------------------------------------------------|
| `SetMinHeight` / `SetMinWidth` / `SetMaxHeight` / `SetMaxWidth` individual methods | Consolidated into `SetMinSize(w, h)` / `SetMaxSize(w, h)` |
| `LogVerbosity` integer system                                                      | Replaced by `ILogger<IInfiniFrameWindow>`                 |
| `MacOsVersion` static property                                                     | Removed                                                   |
| `IsWindowsPlatform` / `IsMacOsPlatform` / `IsLinuxPlatform` static properties      | Internal. Not on public interface                         |
| `UseOsDefaultLocation` / `UseOsDefaultSize` runtime properties                     | Builder / config time only                                |
| `BrowserControlInitParameters` runtime property                                    | Builder / config time only                                |
| `nlohmann/json.hpp` bundled JSON header                                            | Replaced by `simdjson`                                    |
| `fmt` formatting library                                                           | Replaced by `std::format`                                 |
