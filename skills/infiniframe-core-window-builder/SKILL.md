---
name: infiniframe-core-window-builder
description: Creating and configuring native windows using InfiniLore.InfiniFrame. Window builder API, events, dialogs, custom schemes, and web messaging.
---
# InfiniFrame Core Window Builder

> Skill for creating and configuring native windows using `InfiniLore.InfiniFrame`.

## When to Use This Skill

- Creating desktop application windows
- Configuring window properties (size, position, state)
- Setting up browser features (dev tools, zoom, CORS)
- Handling window events (resize, focus, close)
- Implementing C#↔JS messaging
- Creating custom URL scheme handlers
- Showing native dialogs (file pickers, message boxes)
- Multi-monitor setups

## Package

```bash
dotnet add package InfiniLore.InfiniFrame
```

## Window Creation

### Basic Pattern

```csharp
using InfiniFrame;

var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .SetSize(1280, 720)
    .Center()
    .SetStartUrl("https://example.com")
    .Build();

window.WaitForClose();
```

### Windows STA Requirement (CRITICAL)

```csharp
// MUST use explicit Main with [STAThread]
internal class Program {
    [STAThread]
    static void Main(string[] args) {
        var window = InfiniFrameWindowBuilder.Create()
            .SetTitle("My App")
            .Build();
        window.WaitForClose();
    }
}
```

**NEVER use**:
- Top-level statements (cannot carry `[STAThread]`)
- `async Task Main` (STA ignored, runs on MTA thread pool)

Without STA, WebView2 renders as black screen and `Build()` throws `InvalidOperationException`.

**Linux**: No STA requirement — GTK has no COM apartment model.

## Configuration Methods (All Chainable, Pre-Build Only)

### Title and Icon

```csharp
builder
    .SetTitle("My Application")
    .SetIconFile("assets/icon.ico")  // .ico on Windows, .png on Linux
```

### Size and Position

```csharp
builder
    .SetSize(1280, 720)              // Width × Height
    .SetMinSize(800, 600)
    .SetMaxSize(1920, 1080)
    .SetLocation(100, 100)           // Left, Top (screen coordinates)
    .Center()                        // Center on primary monitor
    .SetUseOsDefaultSize(true)       // Let OS choose initial size
    .SetUseOsDefaultLocation(true)   // Let OS choose position
```

**Important**: Calling `SetSize` or `SetLocation` disables corresponding OS default and centering behavior.

### Window State

```csharp
builder
    .SetMaximized(true)
    .SetMinimized(true)
    .SetFullScreen(true)
    .SetResizable(false)
    .SetTopMost(true)          // Always on top
    .SetChromeless(true)       // Remove native title bar/borders
    .SetTransparent(true)      // Enable transparency
```

**Windows note**: `SetChromeless(true)` automatically disables `UseOsDefaultLocation`, `UseOsDefaultSize`, and `Resizable`.

### Content

```csharp
builder
    .SetStartUrl("https://example.com")
    .SetStartUrl(new Uri("https://example.com"))
    .SetStartString("<html><body>Hello</body></html>")  // Render HTML directly
```

`SetStartUrl` and `SetStartString` are mutually exclusive — last one set wins.

## Browser Features

```csharp
builder
    .SetDevToolsEnabled(true)
    .SetContextMenuEnabled(false)
    .SetZoomEnabled(false)
    .SetZoom(150)                              // Zoom level (100 = default)
    .SetMediaAutoplayEnabled(true)
    .SetFileSystemAccessEnabled(true)
    .SetWebSecurityEnabled(false)              // Disable CORS (caution!)
    .SetJavascriptClipboardAccessEnabled(true)
    .SetMediaStreamEnabled(true)               // Camera/mic access
    .SetSmoothScrollingEnabled()
    .SetIgnoreCertificateErrorsEnabled()
    .SetUserAgent("MyApp/1.0")
```

### Notifications (Windows Only)

```csharp
builder
    .SetNotificationsEnabled()
    .SetNotificationRegistrationId("com.myapp.notifications")
    .GrantBrowserPermissions()  // Auto-grant camera/mic (Windows only)
```

### Platform-Specific Browser Parameters

```csharp
// Windows — space-separated Chromium flags
builder.SetBrowserControlInitParameters("--disable-gpu --no-sandbox")

// Linux — JSON matching WebKit2GTK settings
builder.SetBrowserControlInitParameters("{ \"enable_developer_extras\": true }")

// macOS — JSON matching WKPreferences keys
builder.SetBrowserControlInitParameters("{ \"minimumFontSize\": 12 }")
```

## Runtime Window Control (Post-Build)

### Properties

```csharp
window.Size              // Current size (read-only)
window.Location          // Current position (read-only)
window.MaxSize           // Get/set maximum size at runtime
window.MinSize           // Get/set minimum size at runtime
window.Focused           // Query/set keyboard focus
window.ScreenDpi         // Current DPI
window.Monitors          // ImmutableArray<InfiniMonitor> — all monitors
window.MainMonitor       // Monitor window is currently on
window.ManagedThreadId   // Thread ID of window's message loop
window.InstanceHandle    // Low-level native handle
window.NativeType        // Native type access
```

### Operations

```csharp
window.Close()
window.WaitForClose()
await window.WaitForCloseAsync()
window.Center()
window.CenterOnCurrentMonitor()
window.CenterOnMonitor(monitorIndex)
window.SetLocation(x, y)
window.Offset(x, y)
window.Load("https://example.com")
window.Load(new Uri("https://example.com"))
window.LoadRawString("<html>...</html>")
```

## Cross-Thread Invocation

All UI operations MUST run on window thread:

```csharp
Task.Run(() => {
    // Background thread work
    window.Invoke(() => {
        // Runs on window thread
        window.Close();
    });
});
```

## Events System

InfiniFrame uses `InfiniFrameOrderedEvent<T>` — ordered multi-subscriber with deterministic execution order.

### Available Events

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
```

### Intercepting Close

```csharp
// Return true to allow closing, false to cancel
builder.Events.WindowClosingRequested.Add(() => {
    return AskUserToConfirm();
});

// Runs when window is definitively closing (cannot cancel)
builder.Events.WindowClosing.Add((window, cancel) => {
    SaveAppState();
    return false;  // returning false here does NOT cancel
});
```

### DI-Resolved Event Handlers

When `IServiceProvider` is passed to `Build()`:

```csharp
window.Events.WindowClosing.Add((MyService svc, IInfiniFrameWindow w) => {
    // Dependencies resolved from DI
});
```

## Web Messaging

### C# → JavaScript

```csharp
window.SendWebMessage("hello from C#");
await window.SendWebMessageAsync("async hello");
```

JavaScript listener:

```js
window.infiniframe.host.receiveMessage(function(message) {
    console.log("Received:", message);
});
```

### JavaScript → C#

JavaScript (MUST use versioned envelope):

```js
window.infiniframe.host.postMessage({ 
    id: "hello", 
    data: "from JS", 
    version: 1 
});
```

C# raw handler:

```csharp
builder.Events.WebMessageReceived.Add(message => {
    Console.WriteLine($"From JS: {message}");
});
```

C# named handler:

```csharp
builder.MessageHandlers.RegisterMessageHandler("ping", (window, _) => {
    window.SendWebMessage("pong");
});

builder.MessageHandlers.RegisterMessageHandler("set-title", (window, title) => {
    // handle title change
});
```

## Custom URL Schemes

Intercept custom scheme requests (e.g., `app://`) to serve content from C#:

```csharp
builder.RegisterCustomSchemeHandler("app", (sender, scheme, url, out string? contentType) => {
    contentType = "text/html";
    var html = "<html><body>Hello from custom scheme</body></html>";
    return new MemoryStream(Encoding.UTF8.GetBytes(html));
});
```

**Rules**:
- Up to 16 custom schemes before `Build()`
- Additional handlers via `window.RegisterCustomSchemeHandler(...)` after build
- Scheme names are lowercased automatically

## Native Dialogs

### Message Box

```csharp
var result = window.ShowMessage(
    title: "Confirm",
    text: "Are you sure?",
    buttons: InfiniFrameDialogButtons.YesNo,
    icon: InfiniFrameDialogIcon.Question
);

if (result == InfiniFrameDialogResult.Yes) {
    window.Close();
}
```

### File Pickers

```csharp
// Open files
string?[] files = window.ShowOpenFile(
    title: "Open File",
    defaultPath: null,
    multiSelect: true,
    filters: [("Images", ["png", "jpg", "gif"]), ("All Files", ["*"])]
);

// Open folders
string?[] folders = window.ShowOpenFolder("Select Folder", multiSelect: false);

// Save file
string? path = window.ShowSaveFile(
    title: "Save As",
    defaultPath: null,
    filters: [("Text Files", ["txt"])]
);
```

Async variants available: `ShowOpenFileAsync`, `ShowSaveFileAsync`, etc.

### Notifications (Windows)

```csharp
window.SendNotification("Update available", "A new version is ready");
```

Requires `SetNotificationsEnabled()` and `SetNotificationRegistrationId(...)`.

## Monitor Information

```csharp
foreach (InfiniMonitor monitor in window.Monitors) {
    Console.WriteLine($"Monitor: {monitor.MonitorArea}, Work: {monitor.WorkArea}, Scale: {monitor.Scale}");
}

InfiniMonitor main = window.MainMonitor;
```

## DI Container Integration

Builder reads `InfiniFrame` section from `IConfiguration`:

```json
{
  "InfiniFrame": {
    "Title": "My App",
    "Width": 1280,
    "Height": 720
  }
}
```

```csharp
var window = builder.Build(serviceProvider);
```

`IInfiniFrameWindow` resolvable from container if registered.

## Single-File Bootstrap

For packaged apps with embedded natives:

```csharp
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameSingleFileBootstrap.Initialize();
        
        var window = InfiniFrameWindowBuilder.Create()
            .SetTitle("My App")
            .Build();
        
        window.WaitForClose();
    }
}
```

`Initialize()` is idempotent. Only needed for packaged outputs — NOT for `dotnet run`.

## Common Patterns

### Minimal App

```csharp
using InfiniFrame;

InfiniFrameWindowBuilder.Create()
    .SetTitle("Hello")
    .SetSize(800, 600)
    .SetStartUrl("https://example.com")
    .Build()
    .WaitForClose();
```

### Chromeless Window

```csharp
InfiniFrameWindowBuilder.Create()
    .SetChromeless(true)
    .SetTransparent(true)
    .SetSize(1280, 720)
    .Build();
```

### Kiosk Mode

```csharp
InfiniFrameWindowBuilder.Create()
    .SetFullScreen(true)
    .SetResizable(false)
    .SetContextMenuEnabled(false)
    .SetDevToolsEnabled(false)
    .SetStartUrl("https://myapp.com")
    .Build();
```

## Anti-Patterns

❌ **Never use async Task Main**:
```csharp
// WRONG — STA ignored
async Task Main(string[] args) { ... }
```

✅ **Use explicit Main with [STAThread]**:
```csharp
[STAThread]
static void Main(string[] args) { ... }
```

❌ **Never configure after Build**:
```csharp
var window = builder.Build();
window.SetTitle("New Title");  // WRONG — SetTitle is builder-only
```

✅ **Use runtime API for post-build changes**:
```csharp
var window = builder.Build();
window.Load("https://new-url.com");  // Correct runtime API
```

❌ **Never call window ops from background thread without Invoke**:
```csharp
Task.Run(() => window.Close());  // WRONG — thread affinity violation
```

✅ **Use Invoke**:
```csharp
Task.Run(() => window.Invoke(() => window.Close()));  // Correct
```
