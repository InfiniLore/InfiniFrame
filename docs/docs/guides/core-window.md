# Core Window Guide

This guide covers everything available through the `InfiniLore.InfiniFrame` package, the foundation of all InfiniFrame integrations.

## Contents

- [Building a Window](#building-a-window)
- [Single-File Native Packaging](#single-file-native-packaging)
- [Window Configuration](#window-configuration)
- [Background Color](#background-color)
- [Browser Features](#browser-features)
- [DevTools and Remote Debugging](#devtools-and-remote-debugging)
- [Debug Tooling](#debug-tooling)
- [Runtime Window Control](#runtime-window-control)
- [Events](#events)
- [Web Messaging](#web-messaging)
- [Custom URL Schemes](#custom-url-schemes)
- [Dialogs](#dialogs)
- [Monitor Information](#monitor-information)
- [DI Container Integration](#di-container-integration)

## Building a Window

All windows are created through `InfiniFrameWindowBuilder` using a fluent API.

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

`Build()` creates and displays the native window immediately on the calling thread.
The returned `IInfiniFrameWindow` gives you full control over the window at runtime.

## Single-File Native Packaging

When your app is published as a single-file executable with embedded InfiniFrame native binaries, call `InfiniFrameSingleFileBootstrap.Initialize()` before creating any windows.

```csharp
using InfiniFrame;

public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameSingleFileBootstrap.Initialize();

        var window = InfiniFrameWindowBuilder.Create()
            .SetTitle("My App")
            .SetSize(1280, 720)
            .Center()
            .SetStartUrl("app://index.html")
            .Build();

        window.WaitForClose();
    }
}
```

`Initialize()` is idempotent and safe to call once at startup.
Use it for packaged deployments created by `InfiniLore.InfiniFrame.Tools.Pack` (or any equivalent flow that embeds native files as resources), not for standard development runs where native binaries are already present beside your app.

## Window Configuration

All configuration methods are chainable and must be called before `Build()`.

### Title and Icon

```csharp
builder
    .SetTitle("My Application")
    .SetIconFile("assets/icon.ico")  // Windows and Linux only; .ico on Windows, .png on Linux
    .SetWindowsAppUserModelId("MyCompany.MyApplication") // Windows taskbar identity
```

`SetWindowsAppUserModelId` assigns an explicit process identity before the first window is shown. Use one stable,
whitespace-free ID of at most 128 characters for every window in the process. For installed Windows applications,
configure shortcuts with the same AppUserModelID so pinned taskbar items group with the running application.

For a bundled fixed-version WebView2 runtime on Windows, set its extracted directory on the builder before `Build()`:

```csharp
builder.SetWebView2RuntimePath(Path.Combine(AppContext.BaseDirectory, "WebView2Runtime"));
```

The path applies only to that window. It is ignored on Linux and macOS.

The repository's Windows integration test provisions this pinned runtime automatically; its CI cache prevents repeat
downloads. You can optionally set `INFINIFRAME_TEST_WEBVIEW2_RUNTIME_PATH` to reuse an existing extracted runtime.

### Size and Position

```csharp
builder
    .SetSize(1280, 720)         // Width x Height
    .SetMinSize(800, 600)
    .SetMaxSize(1920, 1080)
    .SetLocation(100, 100)      // Left, Top in screen coordinates
    .Center()                   // Center on the primary monitor
    .SetUseOsDefaultSize(true)  // Let the OS choose the initial size
    .SetUseOsDefaultLocation(true)
```

Calling `SetSize` or `SetLocation` disables the corresponding OS default and centering behavior.

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

On Windows, enabling `SetChromeless` automatically disables `UseOsDefaultLocation`, `UseOsDefaultSize`, and `Resizable` since they are incompatible.

### Background Color

Set the native window background color using hex color strings:

```csharp
builder
    .SetBackgroundColor("#FF5733")    // Set to a specific color at builder stage
    .SetBackgroundColor("#AARRGGBB")  // With alpha channel
    .SetBackgroundColor("transparent") // Reset to platform default
    .SetBackgroundColor(null)          // Same as "transparent"
```

At runtime, the background color can be changed dynamically:

```csharp
window.SetBackgroundColor("#00FF00");
window.Features.Decorations.SetBackgroundColor(null); // Reset
string? currentColor = window.Features.Decorations.BackgroundColor;
```

| Platform         | Builder-time                                                              | Runtime                                                          | Notes                                                                |
|------------------|---------------------------------------------------------------------------|------------------------------------------------------------------|----------------------------------------------------------------------|
| Windows (WebView2) | Sets `DefaultBackgroundColor` at init; also applies if called before window creation | Sets `DefaultBackgroundColor` and reloads the webview | Color format: `#RRGGBB` or `#AARRGGBB`. Alpha=0 means transparent. |
| Linux (WebKitGTK)  | Sets WebKitGTK background color at init                                   | Sets WebKitGTK background color via `webkit_web_view_set_background_color` | Color format: `#RRGGBB`. GTK handles alpha via RGBA visual.         |
| macOS (WKWebView)  | Sets WKWebView `backgroundColor` at init                                  | Sets WKWebView `backgroundColor`                                | Color format: `#RRGGBB`. NSColor parsing from hex string.           |

- Pass `null` or `"transparent"` to reset to the platform default (no background color override).
- Invalid hex strings throw `ArgumentException` at runtime.

### Content

```csharp
builder
    .SetStartUrl("https://example.com")
    .SetStartUrl(new Uri("https://example.com"))
    .SetStartString("<html><body>Hello</body></html>")  // Render HTML directly
```

`SetStartUrl` and `SetStartString` are mutually exclusive; the last one set wins.

## Browser Features

```csharp
builder
    .SetDevToolsEnabled(true)
    .SetContextMenuEnabled(false)
    .SetZoomEnabled(false)
    .SetZoom(150)                          // Zoom level (100 = default)
    .SetMediaAutoplayEnabled(true)
    .SetFileSystemAccessEnabled(true)
    .SetWebSecurityEnabled(false)          // Browser-level web security toggle only (not a trusted-origin policy switch)
    .SetJavascriptClipboardAccessEnabled(true)
    .SetMediaStreamEnabled(true)           // Camera/microphone access
    .SetSmoothScrollingEnabled()
    .SetIgnoreCertificateErrorsEnabled()
    .SetUserAgent("MyApp/1.0")
```

## DevTools and Remote Debugging

`SetDevToolsEnabled(bool)` and remote debugging are separate controls:

- `SetDevToolsEnabled(bool)` controls local in-window inspector/devtools access.
- `SetRemoteDebuggingPort(int? port)` configures a loopback TCP debug endpoint at startup.
- `SetWebInspectorEnabled(bool)` enables Safari Web Inspector attachability on macOS 13.3+.

```csharp
var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("Debuggable App")
    .SetStartUrl("https://example.com")
    .SetDevToolsEnabled(true)          // local inspector
    .SetWebInspectorEnabled(true)      // macOS 13.3+ Safari Web Inspector attachability
    .SetRemoteDebuggingPort(9222)      // remote endpoint (Windows and Linux)
    .Build();

if (window.Debug.TryGetRemoteDebuggingEndpoint(out Uri? endpoint))
    Console.WriteLine(endpoint);
```

### Contract

- Port range: `1..65535`.
- `0` or `null`: disable remote debugging.
- Invalid ports throw `ArgumentOutOfRangeException`.
- Remote debugging is startup-only; configure it with `builder.SetRemoteDebuggingPort(...)` before `Build()`.
- Web inspector mode is startup-only; calling `window.Debug.SetWebInspectorEnabled(...)` after `Build()` throws `InvalidOperationException`.
- `window.Debug.RemoteDebuggingPort` remains stable after startup; after close, `window.Debug.TryGetRemoteDebuggingEndpoint(out _)` returns `false` with `null` endpoint.

### Platform behavior

| Platform           | `SetDevToolsEnabled` | `SetRemoteDebuggingPort`            |
|--------------------|----------------------|-------------------------------------|
| Windows (WebView2) | Supported            | Supported                           |
| Linux (WebKitGTK)  | Supported            | Supported                           |
| macOS (WKWebView)  | Supported            | Not supported (throws when enabled) |

| Platform           | `SetWebInspectorEnabled`            |
|--------------------|-------------------------------------|
| Windows (WebView2) | Not supported (throws when enabled) |
| Linux (WebKitGTK)  | Not supported (throws when enabled) |
| macOS (WKWebView)  | Supported on macOS 13.3+            |

- Use `window.Debug.SupportsRemoteDebugging` to query support.
- On unsupported platforms, `window.Debug.TryGetRemoteDebuggingEndpoint(out _)` throws `PlatformNotSupportedException`.

### Precedence with raw browser arguments

`SetRemoteDebuggingPort(...)` is authoritative.  
If `SetBrowserControlInitParameters(...)` contains `--remote-debugging-port=...` or `--remote-debugging-address=...`, those switches are stripped and replaced by the explicit API value.

### Security and networking

- InfiniFrame binds remote debugging to loopback (`127.0.0.1`) when enabled.
- It does not intentionally expose externally reachable debug endpoints.
- Startup validates port availability and throws actionable `InvalidOperationException` when the port is unavailable.
- Linux uses WebKitGTK inspector server environment variables (`WEBKIT_INSPECTOR_SERVER` and `WEBKIT_INSPECTOR_HTTP_SERVER`) at startup.
- Windows WebView2 and Linux inspector endpoints are exposed as `http://127.0.0.1:<port>/`.
- On Linux, WebKit requires developer extras for remote inspector; InfiniFrame keeps that capability active while remote debugging is enabled.
- Linux inspector server configuration is process-scoped (WebKitGTK environment-driven behavior), so all windows in the same process share the same remote-debugging endpoint configuration.

### Linux specifics (WebKitGTK)

- Remote debugging is configured before WebKit context/webview creation for deterministic startup behavior.
- Endpoint mechanism differs by platform:
  - Windows: WebView2 Chromium remote debugging flow.
  - Linux: WebKitGTK inspector server flow.
  - macOS: no remote endpoint support through `SetRemoteDebuggingPort(...)`.
- Limitation: WebKitGTK inspector depends on developer extras in the engine; local inspector UI and remote inspector capabilities are not fully decoupled while remote debugging is active.

## Debug Tooling

InfiniFrame exposes additive runtime diagnostics and debug events under `window.Debug`:

- `window.Debug.Capabilities` (what this platform/runtime supports)
- `window.Debug.GetDiagnostics()` (snapshot of enabled state + endpoint status + last init status/error)
- `window.Debug.Event` (best-effort event stream; capability-gated)
- `window.Debug.TryProbeEndpoint(out Uri? endpoint, out string? reason)` (endpoint probe where supported)

### Debug Tooling Matrix

| Capability                | Windows (WebView2)            | Linux (WebKitGTK) | macOS (WKWebView) |
|---------------------------|-------------------------------|-------------------|-------------------|
| Local DevTools toggle     | ✅                             | ✅                 | ✅                 |
| Remote debugging endpoint | ✅                             | ✅                 | ❌                 |
| Web Inspector attach mode | ❌                             | ❌                 | ✅ (macOS 13.3+)   |
| Script error forwarding   | ✅ (navigation failure mapped) | ✅                 | ✅                 |
| Navigation diagnostics    | ✅                             | ✅                 | ✅                 |

### Guarantees vs best effort

- Capability fields are deterministic and safe to branch on.
- Endpoint probing is bounded and loopback-only by design.
- Debug events are best effort and platform-dependent; InfiniFrame does not emulate missing native signals.
- Linux inspector endpoint is process-scoped (WebKitGTK behavior), not window-scoped.
- macOS inspector mode (`SetWebInspectorEnabled`) is Safari attachability, not a TCP remote debugging endpoint.

### Example

```csharp
InfiniFrameDebugCapabilities caps = window.Debug.Capabilities;
InfiniFrameDebugDiagnostics diag = window.Debug.GetDiagnostics();

if (caps.SupportsRemoteDebuggingEndpoint &&
    window.Debug.TryProbeEndpoint(out Uri? endpoint, out string? reason)) {
    Console.WriteLine($"Endpoint ready: {endpoint}");
}
else {
    Console.WriteLine($"Endpoint unavailable: {reason}");
}

window.Debug.Event += (_, e) => {
    Console.WriteLine($"[{e.TimestampUtc:O}] {e.Kind} {e.Level} {e.Message}");
};
```

### URI Security Policy (Trusted Origins)

InfiniFrame validates URI origins independently from browser `WebSecurity` toggles. For embedded apps (including BlazorWebView), trust external module/CDN origins explicitly:

```csharp
builder
    .AddTrustedOrigin("https://xyz")
    .AddTrustedOrigin("https://cdn.jsdelivr.net")
    .AddTrustedOrigin("https://unpkg.com");
```

To replace the trusted list entirely:

```csharp
builder.SetTrustedOrigins("https://xyz", "https://cdn.jsdelivr.net");
```

To trust all origins (high risk, not recommended in production):

```csharp
builder.SetTrustAllOrigins(true);
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
// Windows: space-separated Chromium flags
builder.SetBrowserControlInitParameters("--disable-gpu --no-sandbox")

// Linux: JSON object matching WebKit2GTK settings
builder.SetBrowserControlInitParameters("{ \"enable_developer_extras\": true }")

// macOS: JSON object matching WKPreferences keys
builder.SetBrowserControlInitParameters("{ \"minimumFontSize\": 12 }")
```

For remote debugging, prefer `SetRemoteDebuggingPort(...)` over raw flags.

## Runtime Window Control

Once a window is built, `IInfiniFrameWindow` provides methods to control it at runtime.

### State and properties

```csharp
window.Size        // Current size (read-only)
window.Location    // Current position (read-only)
window.MaxSize     // Get or set the maximum size at runtime
window.MinSize     // Get or set the minimum size at runtime
window.Focused     // Whether the window currently has focus
window.Maximized   // (via events, not a direct property at runtime)
window.ScreenDpi   // Current DPI

window.Monitors    // ImmutableArray<InfiniMonitor>; all connected monitors
window.MainMonitor // The monitor the window is currently on
```

### Page navigation properties

```csharp
string? url  = window.Features.PageNavigation.CurrentUrl;   // Current page URL (null after LoadRawString)
Uri? uri     = window.Features.PageNavigation.CurrentUri;   // Parsed Uri convenience property
string? url2 = window.GetCurrentUrl();                      // Extension method equivalent
```

`CurrentUrl` returns the active top-level URL after any redirects. It is `null` when the window has loaded raw HTML
via `LoadRawString` because there is no associated URL.

### Window operations

```csharp
window.Close()
window.WaitForClose()
await window.WaitForCloseAsync()
```

### STA requirement (Windows)

WebView2 is COM-based and requires the thread that calls `Build()` to be STA. Without `[STAThread]`, the window opens but the browser control renders as a black screen, and `Build()` now throws `InvalidOperationException` to surface this early.

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

Top-level statements cannot carry `[STAThread]` so use an explicit `static void Main()` as shown above.

> **Note:** `[STAThread]` is silently ignored on `async Task Main`. The async continuation runs on thread pool threads (MTA). Never use `async Task Main` as the entry point for an InfiniFrame application. **Linux does not have this restriction** because GTK has no COM apartment model. The native constructor calls `gtk_init()` itself and implicitly claims whichever thread calls `Build()` as the GTK main thread.

### Cross-thread invocation

All UI operations must run on the window's thread. Use `Invoke` to marshal work from a background thread:

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

Events are available through `IInfiniFrameWindowEvents`, accessible via `IInfiniFrameWindowBuilder.Events`.

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
    return false; // returning false here does not cancel; use WindowClosingRequested for that
});
```

See the generated C# API reference for the full event system documentation.

## Web Messaging

InfiniFrame provides a two-way messaging channel between JavaScript running in the browser control and your C# code.

### C# to JavaScript

```csharp
window.SendWebMessage("hello from C#");
await window.SendWebMessageAsync("async hello");
```

In JavaScript, listen with:

```js
window.infiniframe.host.receiveCallback(function(message) {
    console.log("Received:", message);
});
```

### JavaScript to C#

In JavaScript, send with:

```js
window.infiniframe.host.postData({ id: "hello", command: "Post", data: "from JS", version: 2 });
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

You can intercept requests for custom URL schemes (e.g. `app://`) and serve content from C# code. This is useful for loading local assets or implementing a virtual file system.

```csharp
builder.RegisterCustomSchemeHandler("app", (sender, scheme, url, out string? contentType) => {
    contentType = "text/html";
    var html = "<html><body>Hello from custom scheme</body></html>";
    return new MemoryStream(Encoding.UTF8.GetBytes(html));
});
```

- Up to 16 custom schemes can be registered before `Build()` is called.
- Additional handlers can be added after `Build()` via `window.RegisterCustomSchemeHandler(...)`.
- Scheme names are lowercased automatically.

## Dialogs

InfiniFrame exposes the native OS dialog system.

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

Requires `SetNotificationsEnabled()` and `SetNotificationRegistrationId(...)` to be set during configuration.

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

`IInfiniFrameWindow` will then be resolvable from the container if registered.

## Examples

- `InfiniFrameExample.WebApp.React` (`examples/InfiniFrameExample.WebApp.React`) - custom URL scheme handler and web messaging with DI-resolved services
- `InfiniFrameExample.BlazorWebView` (`examples/InfiniFrameExample.BlazorWebView`) - window builder configuration with size, position, and icon
- `InfiniFrameExample.SingleFileExe` (`examples/InfiniFrameExample.SingleFileExe`) - embedded static assets and single-file native bootstrap
