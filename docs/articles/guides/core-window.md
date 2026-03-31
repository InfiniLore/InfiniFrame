# Core Window Guide

This guide covers everything available through the `InfiniLore.InfiniFrame` package — the foundation of all InfiniFrame integrations

## Contents

- [Building a Window](#building-a-window)
- [Window Configuration](#window-configuration)
- [Browser Features](#browser-features)
- [Runtime Window Control](#runtime-window-control)
- [Events](#events)
- [Web Messaging](#web-messaging)
- [Custom URL Schemes](#custom-url-schemes)
- [Dialogs](#dialogs)
- [Monitor Information](#monitor-information)
- [DI Container Integration](#di-container-integration)

## Building a Window

All windows are created through `InfiniFrameWindowBuilder` using a fluent API

```csharp
using InfiniFrame;

var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .SetSize(1280, 720)
    .Center()
    .SetStartUrl("https://myapp.local")
    .Build();

window.WaitForClose();
```

`Build()` creates and displays the native window immediately on the calling thread
The returned `IInfiniFrameWindow` gives you full control over the window at runtime

## Window Configuration

All configuration methods are chainable and must be called before `Build()`

### Title and Icon

```csharp
builder
    .SetTitle("My Application")
    .SetIconFile("assets/icon.ico")  // Windows and Linux only; .ico on Windows, .png on Linux
```

### Size and Position

```csharp
builder
    .SetSize(1280, 720)         // Width × Height
    .SetMinSize(800, 600)
    .SetMaxSize(1920, 1080)
    .SetLocation(100, 100)      // Left, Top in screen coordinates
    .Center()                   // Center on the primary monitor
    .SetUseOsDefaultSize(true)  // Let the OS choose the initial size
    .SetUseOsDefaultLocation(true)
```

Calling `SetSize` or `SetLocation` disables the corresponding OS default and centering behavior

### Window State

```csharp
builder
    .SetMaximized(true)
    .SetMinimized(true)
    .SetFullScreen(true)
    .SetResizable(false)
    .SetTopMost(true)           // Always on top
    .SetChromeless(true)        // Remove the native title bar and borders
    .SetTransparent(true)       // Enable window transparency
```

On Windows, enabling `SetChromeless` automatically disables `UseOsDefaultLocation`, `UseOsDefaultSize`, and `Resizable` since they are incompatible

### Content

```csharp
builder
    .SetStartUrl("https://example.com")
    .SetStartUrl(new Uri("https://example.com"))
    .SetStartString("<html><body>Hello</body></html>")  // Render HTML directly
```

`SetStartUrl` and `SetStartString` are mutually exclusive — the last one set wins

## Browser Features

```csharp
builder
    .SetDevToolsEnabled(true)
    .SetContextMenuEnabled(false)
    .SetZoomEnabled(false)
    .SetZoom(150)                          // Zoom level (100 = default)
    .SetMediaAutoplayEnabled(true)
    .SetFileSystemAccessEnabled(true)
    .SetWebSecurityEnabled(false)          // Disable CORS (use with caution)
    .SetJavascriptClipboardAccessEnabled(true)
    .SetMediaStreamEnabled(true)           // Camera/microphone access
    .SetSmoothScrollingEnabled()
    .SetIgnoreCertificateErrorsEnabled()
    .SetUserAgent("MyApp/1.0")
```

### Notifications (Windows only)

```csharp
builder
    .SetNotificationsEnabled()
    .SetNotificationRegistrationId("com.myapp.notifications")  // Windows only
    .GrantBrowserPermissions()  // Auto-grant camera/mic permissions (Windows only)
```

### Platform-specific browser parameters

The `SetBrowserControlInitParameters` method passes raw flags to the underlying browser engine:

```csharp
// Windows — space-separated Chromium flags
builder.SetBrowserControlInitParameters("--disable-gpu --no-sandbox")

// Linux — JSON object matching WebKit2GTK settings
builder.SetBrowserControlInitParameters("{ \"enable_developer_extras\": true }")

// macOS — JSON object matching WKPreferences keys
builder.SetBrowserControlInitParameters("{ \"minimumFontSize\": 12 }")
```

## Runtime Window Control

Once a window is built, `IInfiniFrameWindow` provides methods to control it at runtime

### State and properties

```csharp
window.Size        // Current size (read-only)
window.Location    // Current position (read-only)
window.MaxSize     // Get or set the maximum size at runtime
window.MinSize     // Get or set the minimum size at runtime
window.Focused     // Whether the window currently has focus
window.Maximized   // (via events, not a direct property at runtime)
window.ScreenDpi   // Current DPI

window.Monitors    // ImmutableArray<InfiniMonitor> — all connected monitors
window.MainMonitor // The monitor the window is currently on
```

### Window operations

```csharp
window.Close()
window.WaitForClose()
await window.WaitForCloseAsync()
```

### STA requirement (Windows)

WebView2 is COM-based and requires the thread that calls `Build()` to be STA. Without `[STAThread]`, the window opens but the browser control renders as a black screen, and `Build()` now throws `InvalidOperationException` to surface this early

```csharp
// Required for all InfiniFrame apps on Windows
internal class Program {
    [STAThread]
    static void Main(string[] args) {
        var window = InfiniFrameWindowBuilder.Create()
            // ...
            .Build();

        window.WaitForClose();
    }
}
```

Top-level statements cannot carry `[STAThread]` so use an explicit `static void Main()` as shown above

> **Note:** `[STAThread]` is silently ignored on `async Task Main`. The async continuation runs on thread pool threads (MTA). Never use `async Task Main` as the entry point for an InfiniFrame application. **Linux does not have this restriction** because GTK has no COM apartment model. The native constructor calls `gtk_init()` itself and implicitly claims whichever thread calls `Build()` as the GTK main thread

### Cross-thread invocation

All UI operations must run on the window's thread — use `Invoke` to marshal work from a background thread:

```csharp
Task.Run(() => {
    // Background thread
    window.Invoke(() => {
        // Runs on the window thread
        window.Close();
    });
});
```

## Events

Events are available through `IInfiniFrameWindowEvents`, accessible via `IInfiniFrameWindowBuilder.Events`

```csharp
var builder = InfiniFrameWindowBuilder.Create();

builder.Events.WindowCreated.Add(() => Console.WriteLine("Window opened"));
builder.Events.WindowSizeChanged.Add(size => Console.WriteLine($"Resized to {size}"));
builder.Events.WindowLocationChanged.Add(loc => Console.WriteLine($"Moved to {loc}"));
builder.Events.WindowFocusIn.Add(() => Console.WriteLine("Focus gained"));
builder.Events.WindowFocusOut.Add(() => Console.WriteLine("Focus lost"));
builder.Events.WindowMaximized.Add(() => Console.WriteLine("Maximized"));
builder.Events.WindowMinimized.Add(() => Console.WriteLine("Minimized"));
builder.Events.WindowRestored.Add(() => Console.WriteLine("Restored"));
builder.Events.WebMessageReceived.Add(msg => Console.WriteLine($"Message: {msg}"));

var window = builder.Build();
window.WaitForClose();
```

### Intercepting window close

Use `WindowClosingRequested` to cancel or intercept a close:

```csharp
builder.Events.WindowClosingRequested.Add(() => {
    // Return true to allow closing, false to cancel
    return AskUserToConfirm();
});
```

Use `WindowClosing` to run cleanup before the window is destroyed:

```csharp
builder.Events.WindowClosing.Add((window, cancel) => {
    SaveAppState();
    return false; // returning false here does not cancel — use WindowClosingRequested for that
});
```

See the [Events Reference](../api/events.md) for the full event system documentation

## Web Messaging

InfiniFrame provides a two-way messaging channel between JavaScript running in the browser control and your C# code

### C# → JavaScript

```csharp
window.SendWebMessage("hello from C#");
await window.SendWebMessageAsync("async hello");
```

In JavaScript, listen with:

```js
window.external.receiveMessage(function(message) {
    console.log("Received:", message);
});
```

### JavaScript → C#

In JavaScript, send with:

```js
window.external.sendMessage("hello from JS");
```

In C#, handle with:

```csharp
builder.Events.WebMessageReceived.Add(message => {
    Console.WriteLine($"From JS: {message}");
});
```

Or register a named handler through `IInfiniFrameWindowMessageHandlers`:

```csharp
builder.MessageHandlers.RegisterMessageHandler("ping", (window, _) => {
    window.SendWebMessage("pong");
});
```

## Custom URL Schemes

You can intercept requests for custom URL schemes (e.g. `app://`) and serve content from C# code — useful for loading local assets or implementing a virtual file system

```csharp
builder.RegisterCustomSchemeHandler("app", (sender, scheme, url, out string? contentType) => {
    contentType = "text/html";
    var html = "<html><body>Hello from custom scheme</body></html>";
    return new MemoryStream(Encoding.UTF8.GetBytes(html));
});
```

- Up to 16 custom schemes can be registered before `Build()` is called
- Additional handlers can be added after `Build()` via `window.RegisterCustomSchemeHandler(...)`
- Scheme names are lowercased automatically

## Dialogs

InfiniFrame exposes the native OS dialog system

### Message box

```csharp
var result = window.ShowMessage(
    title: "Confirm",
    text: "Are you sure you want to quit?",
    buttons: InfiniFrameDialogButtons.YesNo,
    icon: InfiniFrameDialogIcon.Question
);

if (result == InfiniFrameDialogResult.Yes) {
    window.Close();
}
```

### File pickers

```csharp
// Open one or more files
string?[] files = window.ShowOpenFile(
    title: "Open File",
    defaultPath: null,
    multiSelect: true,
    filters: [("Images", ["png", "jpg", "gif"]), ("All Files", ["*"])]
);

// Open folder(s)
string?[] folders = window.ShowOpenFolder("Select Folder", multiSelect: false);

// Save file
string? path = window.ShowSaveFile(
    title: "Save As",
    defaultPath: null,
    filters: [("Text Files", ["txt"])]
);
```

All dialogs also have async overloads (`ShowOpenFileAsync`, `ShowSaveFileAsync`, etc.)

### Notifications (Windows only)

```csharp
window.SendNotification("Update available", "A new version is ready to install");
```

Requires `SetNotificationsEnabled()` and `SetNotificationRegistrationId(...)` to be set during configuration

## Monitor Information

```csharp
// All connected monitors
foreach (InfiniMonitor monitor in window.Monitors) {
    Console.WriteLine($"Monitor: {monitor.MonitorArea}, Work area: {monitor.WorkArea}, Scale: {monitor.Scale}");
}

// The monitor the window is currently on
InfiniMonitor main = window.MainMonitor;
```

## DI Container Integration

When building with a `ServiceProvider`, the builder reads configuration from the `InfiniFrame` section automatically:

```csharp
// appsettings.json
{
  "InfiniFrame": {
    "Title": "My App",
    "Width": 1280,
    "Height": 720
  }
}
```

Pass the provider to `Build`:

```csharp
var window = builder.Build(serviceProvider);
```

`IInfiniFrameWindow` will then be resolvable from the container if registered

## Examples

- `InfiniFrameExample.WebApp.React` (`examples/InfiniFrameExample.WebApp.React`) - custom URL scheme handler and web messaging with DI-resolved services
- `InfiniFrameExample.BlazorWebView` (`examples/InfiniFrameExample.BlazorWebView`) - window builder configuration with size, position, and icon
