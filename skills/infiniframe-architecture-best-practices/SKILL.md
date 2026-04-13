---
name: infiniframe-architecture-best-practices
description: InfiniFrame architecture patterns, project structure, DI patterns, threading models, and migration from Photino.NET.
---
# InfiniFrame Architecture & Best Practices

> Skill for understanding InfiniFrame architecture, project structure, design patterns, and migration from Photino.

## When to Use This Skill

- Setting up new InfiniFrame projects
- Choosing between integration paths
- Understanding project structure
- Implementing DI patterns
- Handling threading models
- Migrating from Photino.NET
- Following best practices

## Integration Path Selection

InfiniFrame supports three independent integration paths — choose ONE for your app:

| Use Case | Package | Description |
|----------|---------|-------------|
| Load URL/HTML in window | `InfiniLore.InfiniFrame` | Core window builder, lightest weight |
| Blazor app in native window | `InfiniLore.InfiniFrame.BlazorWebView` | No HTTP server, in-process Blazor |
| ASP.NET Core web app | `InfiniLore.InfiniFrame.WebServer` | Full ASP.NET Core pipeline with native window |

**They are mutually exclusive** — only one needed for a given application type.

## Project Structure

### Recommended Layout

```
MyApp/
├── src/
│   ├── MyApp.Core/           # Main application project
│   │   ├── Program.cs
│   │   ├── App.xaml.cs (if Blazor)
│   │   └── wwwroot/          # Static assets (Blazor/WebServer)
│   │       ├── index.html
│   │       ├── css/
│   │       └── js/
│   ├── MyApp.Components/     # Blazor components (optional)
│   │   ├── MainLayout.razor
│   │   ├── Pages/
│   │   └── Shared/
│   └── MyApp.Services/       # Business logic (optional)
│       ├── IMyService.cs
│       └── MyService.cs
├── tests/
│   └── MyApp.Tests/
└── assets/
    └── icon.ico               # Window icon
```

### Package Dependencies

**Core Window App**:
```xml
<PackageReference Include="InfiniLore.InfiniFrame" Version="..." />
```

**Blazor WebView App**:
```xml
<PackageReference Include="InfiniLore.InfiniFrame.BlazorWebView" Version="..." />
<!-- InfiniFrame.Js included automatically -->
```

**ASP.NET Core Web Server App**:
```xml
<PackageReference Include="InfiniLore.InfiniFrame.WebServer" Version="..." />
```

**Custom Window Chrome** (optional, with BlazorWebView or WebServer):
```xml
<PackageReference Include="InfiniLore.InfiniFrame.Blazor" Version="..." />
```

## DI Patterns

### Core Window with DI

```csharp
var services = new ServiceCollection();
services.AddSingleton<IMyService, MyService>();
services.AddLogging();

var serviceProvider = services.BuildServiceProvider();

var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .Build(serviceProvider);

// IInfiniFrameWindow resolvable from container
var appService = serviceProvider.GetRequiredService<IMyService>();
```

### Blazor WebView (DI Built-In)

```csharp
var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetTitle("My Blazor App")
);

// Services automatically available in Blazor components
builder.Services.AddSingleton<IMyService, MyService>();
builder.Services.AddScoped<IAppState, AppState>();

builder.RootComponents.Add<App>("#app");
builder.Build().Run();
```

### Web Server (DI Built-In)

```csharp
var builder = InfiniFrameWebApplication.CreateBuilder(args);

// Standard ASP.NET DI
builder.WebApp.Services.AddSingleton<IMyService, MyService>();
builder.WebApp.Services.AddControllers();

var app = builder.Build().UseAutoServerClose();
app.WebApp.MapControllers();  // Controllers get DI
app.Run();
```

## Threading Model

### Core Window

| Thread | Purpose |
|--------|---------|
| Main thread (STA on Windows) | Native window, all UI operations |
| Background threads | Any background work |

**Rule**: All window operations from background threads MUST use `window.Invoke()`.

### Web Server

| Thread | Purpose |
|--------|---------|
| Main thread (STA on Windows) | Native window |
| Background thread | ASP.NET Core / Kestrel |

**Rules**:
- Window ops from ASP.NET Core handlers → MUST use `window.Invoke()`
- Server ops from window event handlers → Can call directly (ASP.NET Core is thread-safe)

### Blazor WebView

| Thread | Purpose |
|--------|---------|
| Main thread (STA on Windows) | Native window + Blazor runtime |

**Rule**: All window operations from Blazor components MUST use `window.Invoke()`.

## Windows STA Requirement (CRITICAL)

```csharp
// CORRECT — explicit Main with [STAThread]
internal class Program {
    [STAThread]
    static void Main(string[] args) {
        // InfiniFrame code here
    }
}
```

**NOT supported**:
- Top-level statements (cannot carry `[STAThread]`)
- `async Task Main` (STA silently ignored, runs on MTA thread pool)

**Linux**: No STA requirement — GTK has no COM apartment model.

## Event System Design

InfiniFrame uses `InfiniFrameOrderedEvent<T>` — ordered multi-subscriber system:

```csharp
// Multiple handlers, all run in registration order
window.Events.WindowClosing.Add((window, args) => {
    Console.WriteLine("Handler 1");
});

window.Events.WindowClosing.Add((window, args) => {
    Console.WriteLine("Handler 2");  // Also runs
});
```

### Closing Events

| Event | Purpose | Can Cancel? |
|-------|---------|-------------|
| `WindowClosingRequested` | Close requested, can veto | Yes (return false) |
| `WindowClosing` | Window definitively closing | No |

```csharp
// Cancel close if unsaved changes
window.Events.WindowClosingRequested.Add(() => {
    if (HasUnsavedChanges()) {
        return AskUserToConfirm();
    }
    return true;
});

// Cleanup (cannot cancel)
window.Events.WindowClosing.Add((window, cancel) => {
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

## Messaging Patterns

### Versioned Envelope (MANDATORY)

```json
{
  "id": "event:name",
  "data": { ... },
  "version": 1
}
```

`id` and `version` are required. `version` MUST be `1`.

### Named Handlers (Preferred)

```csharp
// C# — register named handlers
window.MessageHandlers.RegisterMessageHandler("app:ping", (window, _) => {
    window.SendWebMessage(JsonSerializer.Serialize(new {
        id = "app:pong",
        data = new { time = DateTime.UtcNow },
        version = 1
    }));
});
```

```js
// JS — send with envelope
window.infiniframe.host.postMessage({
    id: "app:ping",
    data: null,
    version: 1
});
```

### Request-Response Pattern

See [JavaScript Interop Skill](javascript-interop/SKILL.md) for full examples.

## Migration from Photino

### Critical Differences

| Aspect | Photino | InfiniFrame |
|--------|---------|-------------|
| Package | `Photino.NET` | `InfiniLore.InfiniFrame` |
| Namespace | `Photino.NET` | `InfiniFrame` |
| Construction | `new PhotinoWindow()` | `InfiniFrameWindowBuilder.Create()` |
| Configuration | On constructed object | Builder only, before `Build()` |
| Events | Single handler, last assignment wins | Ordered multi-subscriber via `.Add()` |
| Messaging | Raw string, single handler | Versioned JSON envelope, named handlers |
| Logging | `SetLogVerbosity(int)` | `ILogger<IInfiniFrameWindow>` via DI |
| Return types | Often void | Fluent (returns builder/window) |

### API Mapping

| Photino | InfiniFrame |
|---------|-------------|
| `new PhotinoWindow()` | `InfiniFrameWindowBuilder.Create()` |
| `.Load(url)` (initial) | `.SetStartUrl(url)` in builder |
| `.Center()` (initial) | `.Center()` in builder |
| `SetMinHeight(h)` / `SetMinWidth(w)` | `SetMinSize(w, h)` |
| `SetMaxHeight(h)` / `SetMaxWidth(w)` | `SetMaxSize(w, h)` |
| `RegisterWebMessageReceivedHandler` | `Events.WebMessageReceived.Add()` OR `MessageHandlers.RegisterMessageHandler()` |
| `RegisterWindowClosingHandler` | `Events.WindowClosingRequested.Add()` |
| `SetLogVerbosity(2)` | Use `ILogger` via DI |
| `ShowSaveFile(title, path, filters, count)` | `ShowSaveFile(title, path, filters, count, defaultFileName)` |
| `Monitor` struct | `InfiniMonitor` record |
| `PhotinoDialogButtons` | `InfiniFrameDialogButtons` |

### Not Migrated (Removed)

| Photino Feature | Reason |
|-----------------|--------|
| `MacOsVersion` static property | Internal |
| `IsWindowsPlatform` / `IsMacOsPlatform` / `IsLinuxPlatform` | Internal, not on public interface |
| `UseOsDefaultLocation` / `UseOsDefaultSize` runtime properties | Builder/config time only |
| `BrowserControlInitParameters` runtime property | Builder/config time only |
| Individual min/max width/height methods | Consolidated into `SetMinSize`/`SetMaxSize` |

### Photino Issues Fixed

See [Migration Guide](docs/docs/migration/breaking-changes-from-photino.md) for complete list of fixed issues.

## Configuration from appsettings.json

InfiniFrame supports sourcing window config from `IConfiguration`:

```json
{
  "InfiniFrame": {
    "Title": "My App",
    "Width": 1280,
    "Height": 720,
    "DevToolsEnabled": true
  }
}
```

```csharp
var window = builder.Build(serviceProvider);  // Reads from IConfiguration in DI
```

## Logging

InfiniFrame integrates with `Microsoft.Extensions.Logging`:

```csharp
var services = new ServiceCollection();
services.AddLogging(builder => {
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});

var serviceProvider = services.BuildServiceProvider();
var window = InfiniFrameWindowBuilder.Create().Build(serviceProvider);

// Window operations logged via ILogger<IInfiniFrameWindow>
```

## Single-File Packaging

For packaged apps:

```csharp
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameSingleFileBootstrap.Initialize();  // Only for packaged apps
        
        var window = InfiniFrameWindowBuilder.Create()
            .SetTitle("My App")
            .Build();
        
        window.WaitForClose();
    }
}
```

`Initialize()` is idempotent — safe to call unconditionally.

## Platform-Specific Considerations

### Windows

- WebView2 Runtime required (pre-installed on Windows 11)
- STA thread mandatory
- `.ico` files for window icons
- Notifications require explicit enablement

### Linux

- `webkit2gtk-4.0` and `libgtk-3-dev` required
- No STA requirement
- `.png` files for window icons
- GTK main thread claimed implicitly

### macOS

- macOS 10.15 Catalina+ required
- WKWebView built into OS
- No STA requirement
- Platform-specific JSON for WKPreferences

## Code Style

### C# Code Style

- **K&R brace style** (enforced by `.editorconfig` at project root)
- Use `dotnet format` with repository settings

### C++ Code Style

- **K&R brace style** (enforced by `.clang-format` at project root)
- Use `clang-format` with repo configuration

## Testing Strategies

### UI Testing

For Playwright testing, see:
- https://github.com/InfiniLore/InfiniFrame/tree/master/tests
- `InfiniFrame.GitHubActions.Testing.Playwright.slnf` solution filter

### Integration Testing

Test window creation and lifecycle:

```csharp
[Fact]
public void Window_CanBeCreated_AndClosed() {
    var window = InfiniFrameWindowBuilder.Create()
        .SetTitle("Test")
        .SetSize(800, 600)
        .Build();
    
    Assert.NotNull(window);
    Assert.False(window.IsClosed);
    
    // Test close
    window.Close();
}
```

## Common Application Patterns

### Kiosk App

```csharp
InfiniFrameWindowBuilder.Create()
    .SetFullScreen(true)
    .SetResizable(false)
    .SetContextMenuEnabled(false)
    .SetDevToolsEnabled(false)
    .SetStartUrl("https://myapp.com")
    .Build()
    .WaitForClose();
```

### Multi-Window Blazor App

See https://github.com/InfiniLore/InfiniFrame/tree/master/examples/InfiniFrameExample.BlazorWebView.MultiWindowSample.

### Chromeless App with Custom Chrome

```csharp
var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetChromeless(true)
    .SetTransparent(true)
);

builder.Services.AddInfiniFrameChromeComponents();
builder.RootComponents.Add<MainLayout>("#app");
builder.Build().Run();
```

### App with Custom Scheme Handler

```csharp
var builder = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .SetStartUrl("app://index.html");

builder.RegisterCustomSchemeHandler("app", (sender, scheme, url, out contentType) => {
    contentType = "text/html";
    return new MemoryStream(Encoding.UTF8.GetBytes("<html>...</html>"));
});

builder.Build().WaitForClose();
```

## Anti-Patterns

❌ **Use multiple integration packages together**:
```xml
<!-- WRONG — pick one -->
<PackageReference Include="InfiniLore.InfiniFrame.BlazorWebView" />
<PackageReference Include="InfiniLore.InfiniFrame.WebServer" />
```

✅ **Use one integration path**:
```xml
<!-- Correct — choose based on use case -->
<PackageReference Include="InfiniLore.InfiniFrame.BlazorWebView" />
```

❌ **Configure window after Build**:
```csharp
var window = builder.Build();
window.SetTitle("New Title");  // WRONG — SetTitle is builder-only
```

✅ **Configure before Build, use runtime API after**:
```csharp
var window = builder.SetTitle("My App").Build();
window.Load("https://new-url.com");  // Correct runtime API
```

❌ **Forget STA on Windows**:
```csharp
// WRONG — top-level statements, no [STAThread]
using InfiniFrame;
InfiniFrameWindowBuilder.Create().Build();  // Throws InvalidOperationException
```

✅ **Use explicit Main**:
```csharp
internal class Program {
    [STAThread]
    static void Main(string[] args) {
        InfiniFrameWindowBuilder.Create().Build();
    }
}
```

❌ **Call window ops from background thread without Invoke**:
```csharp
Task.Run(() => window.Close());  // WRONG — thread affinity violation
```

✅ **Use Invoke**:
```csharp
Task.Run(() => window.Invoke(() => window.Close()));
```

❌ **Use legacy messaging format**:
```js
// WRONG — out of support
window.chrome.webview.postMessage("id;payload");
```

✅ **Use versioned envelope**:
```js
window.infiniframe.host.postMessage({ id: "action", data: "payload", version: 1 });
```
