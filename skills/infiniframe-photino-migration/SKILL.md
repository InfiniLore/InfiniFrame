---
name: infiniframe-photino-migration
description: Migrating applications from Photino.NET/Photino.Native to InfiniFrame. API mapping, behavioral changes, event system migration, messaging protocol upgrade, and step-by-step migration guide.
---
# InfiniFrame Photino Migration

> Skill for migrating Photino.NET and Photino.Native applications to InfiniFrame. Covers all breaking changes, API mappings, and migration patterns.

## When to Use This Skill

- Migrating existing Photino apps to InfiniFrame
- Understanding API differences between Photino and InfiniFrame
- Resolving breaking changes after package update
- Converting event handlers from Photino to InfiniFrame
- Upgrading messaging protocol from raw strings to JSON envelope
- Updating native P/Invoke signatures

## Package Change

| Aspect | Photino | InfiniFrame |
|--------|---------|-------------|
| NuGet package | `Photino.NET` | `InfiniLore.InfiniFrame` |
| Namespace | `Photino.NET` | `InfiniFrame` |
| Native DLL | `Photino.Native` | `InfiniFrame.Native` |
| C++ class | `Photino` | `InfiniFrameWindow` |
| Init params | `PhotinoInitParams` | `InfiniFrameInitParams` |
| Export prefix | `Photino_` | `InfiniFrame_` |

## Entry Point Migration

### Photino Pattern (BEFORE)

```csharp
var window = new PhotinoWindow()
    .SetTitle("My App")
    .SetDevToolsEnabled(true)
    .Load(new Uri("https://example.com"));
window.WaitForClose();
```

### InfiniFrame Pattern (AFTER)

```csharp
var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .SetDevToolsEnabled(true)
    .SetStartUrl("https://example.com")
    .Build();
window.WaitForClose();
```

**Critical**: All configuration MUST happen before `Build()`. Runtime configuration is done through the window interface methods.

## API Mapping Table

### Window Construction

| Photino | InfiniFrame | Notes |
|---------|-------------|-------|
| `new PhotinoWindow()` | `InfiniFrameWindowBuilder.Create()` | Builder pattern |
| `new PhotinoWindow(parent)` | `builder.SetParent(parent).Build()` | Parent via builder |
| `.Load(url)` (initial) | `.SetStartUrl(url)` | Pre-build only |
| `.LoadRawString(html)` (initial) | `.SetStartString(html)` | Pre-build only |
| `.Center()` (initial) | `.Center()` | Pre-build only |

### Runtime Window API

| Photino | InfiniFrame | Notes |
|---------|-------------|-------|
| `.Load(url)` (runtime) | `.Load(url)` | Same, runtime available |
| `.LoadRawString(html)` (runtime) | `.LoadRawString(html)` | Same, runtime available |
| `.Center()` (runtime) | `.Center()` / `.CenterOnCurrentMonitor()` / `.CenterOnMonitor(int)` | Enhanced |
| `.MoveTo(Point, bool)` | `.SetLocation(x, y)` | Simplified |
| `.Offset(Point)` | `.Offset(x, y)` | Simplified |
| `.SetMinHeight(h)` / `.SetMinWidth(w)` | `.SetMinSize(w, h)` | Consolidated |
| `.SetMaxHeight(h)` / `.SetMaxWidth(w)` | `.SetMaxSize(w, h)` | Consolidated |
| `.SetLogVerbosity(int)` | **Removed** | Use `ILogger` via DI |
| `.Win32SetWebView2Path(string)` | **Internal** | Not public |
| `.WaitForClose()` | `.WaitForClose()` / `.WaitForCloseAsync()` | Async added |
| `Monitor` struct | `InfiniMonitor` record | Type renamed |
| `IReadOnlyList<Monitor>` | `ImmutableArray<InfiniMonitor>` | Collection type changed |
| `PhotinoDialogButtons` | `InfiniFrameDialogButtons` | Enum renamed |
| `PhotinoDialogResult` | `InfiniFrameDialogResult` | Enum renamed |
| `PhotinoDialogIcon` | `InfiniFrameDialogIcon` | Enum renamed |
| `ShowSaveFile(title, path, filters, count)` | `ShowSaveFile(title, path, filters, count, defaultFileName)` | Added parameter |

### Removed APIs (No Replacement)

| Photino API | Reason |
|-------------|--------|
| `MacOsVersion` (static) | Internal |
| `IsWindowsPlatform` / `IsMacOsPlatform` / `IsLinuxPlatform` | Internal |
| `UseOsDefaultLocation` / `UseOsDefaultSize` (runtime) | Builder/config time only |
| `BrowserControlInitParameters` (runtime) | Builder/config time only |

### New APIs (Not in Photino)

| API | Description |
|-----|-------------|
| `IInfiniFrameWindow.Focused` | Query/set keyboard focus |
| `IInfiniFrameWindow.WaitForCloseAsync()` | Async close wait |
| `IInfiniFrameWindow.ManagedThreadId` | Thread ID of message loop |
| `IInfiniFrameWindow.InstanceHandle` / `NativeType` | Low-level native access |
| `IInfiniFrameWindow.CachedPreFullScreenBounds` | Saved pre-fullscreen geometry |
| `IInfiniFrameWindow.CachedPreMaximizedBounds` | Saved pre-maximized geometry |
| `ZoomEnabled` | Separate bool for user zoom control |
| `RegisterCustomSchemeHandler()` returns window | Fluent interface (was void) |
| Configuration from `appsettings.json` | `"InfiniFrame"` section support |

## Event System Migration

### Photino: Last Assignment Wins

```csharp
// Photino — replacing handler
window.RegisterWindowClosingHandler((sender, args) => { ... });
window.RegisterWindowClosingHandler((sender, args) => { ... }); // REPLACES previous one
```

### InfiniFrame: Ordered Multi-Subscriber

```csharp
// InfiniFrame — both handlers run in order
window.Events.WindowClosing.Add((window, args) => { ... });
window.Events.WindowClosing.Add((window, args) => { ... }); // Also runs
```

### Closing Event Split

| Photino | InfiniFrame | Purpose |
|---------|-------------|---------|
| `RegisterWindowClosingHandler` | `WindowClosingRequested` | Can cancel close (return true to allow) |
| — | `WindowClosing` | Cannot cancel, runs when closing is definite |

```csharp
// Cancel close if unsaved changes
builder.Events.WindowClosingRequested.Add(() => {
    return HasUnsavedChanges() ? AskUserToConfirm() : true;
});

// Cleanup before close (cannot cancel)
builder.Events.WindowClosing.Add((window, cancel) => {
    SaveAppState();
});
```

### DI-Resolved Event Handlers

```csharp
window.Events.WindowClosing.Add((MyService svc, IInfiniFrameWindow w) => {
    svc.Cleanup();
});
```

Requires `IServiceProvider` passed to `Build()`.

## Web Messaging Protocol Upgrade

### Photino: Raw String Passthrough

```csharp
// Photino — single raw handler
window.RegisterWebMessageReceivedHandler((sender, message) => {
    // message is the full raw string from JS
});
```

### InfiniFrame: Versioned JSON Envelope

```json
{ "id": "event:name", "data": <any>, "version": 1 }
```

```csharp
// InfiniFrame — named handler with typed payload
window.MessageHandlers.RegisterMessageHandler("event:name", (window, payload) => {
    // payload is the "data" field from the envelope
});
```

```js
// JavaScript MUST use envelope format
window.infiniframe.host.postMessage({ id: "event:name", data: { value: 42 }, version: 1 });
```

**Legacy `messageId;payload` format is out of support.**

## Logging System Replacement

### Photino (Removed)

```csharp
window.SetLogVerbosity(2); // 0 = silent, higher = more verbose
// Bug: even verbosity 0 logged a message
```

### InfiniFrame: ILogger Integration

```csharp
var services = new ServiceCollection();
services.AddLogging(builder => {
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});
var serviceProvider = services.BuildServiceProvider();

var window = InfiniFrameWindowBuilder.Create()
    .Build(serviceProvider); // ILogger<IInfiniFrameWindow> resolved from DI
```

## Native C++ Interface Changes

### P/Invoke Signature Update

```csharp
// Photino — manual DllImport
[DllImport("Photino.Native", ...)]
static extern void Photino_SetTitle(IntPtr instance, string title);

// InfiniFrame — source-generated LibraryImport
[LibraryImport("InfiniFrame.Native", ...)]
static partial void InfiniFrame_SetTitle(IntPtr instance, ...);
```

### String Ownership

```csharp
// If calling native exports directly, you MUST free returned strings
IntPtr titlePtr = InfiniFrame_GetTitle(windowHandle);
string title = Marshal.PtrToStringAnsi(titlePtr);
InfiniFrame_FreeString(titlePtr);  // MANDATORY — prevents memory leaks
```

### SaveFileDialog Signature Change

```csharp
// Photino
Photino_ShowSaveFile(title, defaultPath, filters, count)

// InfiniFrame — added defaultFileName parameter
InfiniFrame_ShowSaveFile(title, defaultPath, filters, count, defaultFileName)
```

## Known Photino Issues Fixed in InfiniFrame

| Photino Issue | Description | How Fixed |
|---------------|-------------|-----------|
| [photino.native #173/174](https://github.com/tryphotino/photino.native/issues/173) | Custom scheme handlers broken on Windows | Rewritten registration path |
| [photino.native #165](https://github.com/tryphotino/photino.native/issues/165) | Memory leak in `SendWebMessage` | Explicit `InfiniFrame_FreeString` ownership |
| [photino.native #158](https://github.com/tryphotino/photino.native/issues/158) | No programmatic window focus | `InfiniFrame_SetFocused` / `GetFocused` exported |
| [photino.native #163](https://github.com/tryphotino/photino.native/issues/163) | UTF encoding bug corrupts non-ASCII paths | `simdutf` for all conversions |
| [photino.native #141](https://github.com/tryphotino/photino.native/issues/141) | Stack overflow in `WaitForExit` on Linux | Per-window independent message loops |
| [photino.NET #75](https://github.com/tryphotino/photino.NET/issues/75) | `RegisterWindowClosingHandler` doesn't fire on Linux | GTK `delete-event` signal correctly used |
| [photino.NET #257](https://github.com/tryphotino/photino.NET/issues/257) | `SetLogVerbosity(0)` still logs | Integer verbosity removed entirely |
| [photino.NET #232](https://github.com/tryphotino/photino.NET/issues/232) | Custom scheme handlers break `fetch`/`XHR` | CORS headers handled correctly |
| [photino.native #175](https://github.com/tryphotino/photino.native/issues/175) | `SetTopmost` wrong Win32 style, null crash on Linux | Fixed `HWND_TOPMOST`/`HWND_NOTOPMOST` |

## Step-by-Step Migration Checklist

1. **Update package references**: `Photino.NET` → `InfiniLore.InfiniFrame`
2. **Update namespaces**: `using Photino.NET;` → `using InfiniFrame;`
3. **Replace construction**: `new PhotinoWindow()` → `InfiniFrameWindowBuilder.Create()`
4. **Move configuration**: All config before `Build()`
5. **Update events**: `RegisterWindowClosingHandler` → `Events.WindowClosingRequested.Add()`
6. **Update messaging**: Raw handler → named handlers + JSON envelope
7. **Replace logging**: `SetLogVerbosity` → `ILogger` via DI
8. **Update P/Invoke**: `Photino_*` → `InfiniFrame_*`, `Photino.Native` → `InfiniFrame.Native`
9. **Consolidate size APIs**: `SetMinHeight/Width` → `SetMinSize(w, h)`
10. **Update dialog enums**: `PhotinoDialog*` → `InfiniFrameDialog*`
11. **Add bootstrap** (if packaging): `InfiniFrameSingleFileBootstrap.Initialize()`
12. **Verify STA**: `[STAThread]` on `Main()` for Windows

## Common Patterns

### Minimal Migration

```csharp
// BEFORE (Photino)
var window = new PhotinoWindow()
    .SetTitle("My App")
    .SetSize(1280, 720)
    .Load(new Uri("https://example.com"));
window.WaitForClose();

// AFTER (InfiniFrame)
var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .SetSize(1280, 720)
    .SetStartUrl("https://example.com")
    .Build();
window.WaitForClose();
```

### Event Handler Migration

```csharp
// BEFORE (Photino)
window.RegisterWindowClosingHandler((sender, args) => {
    if (unsaved) args.Cancel = true;
});

// AFTER (InfiniFrame)
builder.Events.WindowClosingRequested.Add(() => {
    return unsaved ? false : true; // false cancels close
});
```

### Messaging Migration

```csharp
// BEFORE (Photino)
window.RegisterWebMessageReceivedHandler((sender, message) => {
    if (message == "ping") window.SendWebMessage("pong");
});

// AFTER (InfiniFrame)
builder.MessageHandlers.RegisterMessageHandler("ping", (window, _) => {
    window.SendWebMessage(JsonSerializer.Serialize(new {
        id = "pong",
        data = null,
        version = 1
    }));
});
```

```js
// JavaScript MUST change from:
window.chrome.webview.postMessage("ping;payload");  // WRONG — out of support

// To:
window.infiniframe.host.postMessage({ id: "ping", data: "payload", version: 1 });
```

## Anti-Patterns

❌ **Keep Photino construction pattern**:
```csharp
// WRONG — Photino API doesn't exist in InfiniFrame
var window = new PhotinoWindow().SetTitle("My App");
```

✅ **Use builder pattern**:
```csharp
var window = InfiniFrameWindowBuilder.Create().SetTitle("My App").Build();
```

❌ **Configure after Build**:
```csharp
var window = builder.Build();
window.SetTitle("New Title");  // WRONG — SetTitle is builder-only
```

✅ **Configure before Build, use runtime API after**:
```csharp
var window = builder.SetTitle("My App").Build();
window.Load("https://new-url.com");  // Runtime API
```

❌ **Use legacy messaging format**:
```js
window.chrome.webview.postMessage("id;payload");  // WRONG — out of support
```

✅ **Use versioned envelope**:
```js
window.infiniframe.host.postMessage({ id: "id", data: "payload", version: 1 });
```

❌ **Use SetLogVerbosity**:
```csharp
window.SetLogVerbosity(2);  // WRONG — removed
```

✅ **Use ILogger**:
```csharp
services.AddLogging(builder => builder.AddConsole());
var window = builder.Build(serviceProvider);
```
