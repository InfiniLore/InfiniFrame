---
name: infiniframe-web-server
description: Hosting ASP.NET Core applications with native windows using InfiniLore.InfiniFrame.WebServer. Blazor Server, SignalR, and graceful shutdown patterns.
---
# InfiniFrame Web Server (ASP.NET Core)

> Skill for hosting ASP.NET Core applications with native windows using `InfiniLore.InfiniFrame.WebServer`.

## When to Use This Skill

- Running ASP.NET Core apps with native window frontend
- Blazor Server applications
- SignalR real-time applications
- Applications requiring full ASP.NET Core pipeline (middleware, controllers, etc.)
- Apps that need both server and native window

## Package

```bash
dotnet add package InfiniLore.InfiniFrame.WebServer
```

## How It Works

- ASP.NET Core server starts on **background thread**
- Native window opens and navigates to server URL
- Both shut down together when window closes (with `UseAutoServerClose()`)
- Full ASP.NET Core pipeline available (middleware, controllers, SignalR, minimal APIs, Blazor Server)

## Minimal Setup

```csharp
using InfiniFrame.WebServer;

var app = InfiniFrameWebApplication.CreateBuilder(args)
    .Build()
    .UseAutoServerClose();

app.WebApp.MapGet("/", () => "Hello from InfiniFrame");

app.Run();  // Starts server, opens window, blocks until window closes
```

## Builder API

`InfiniFrameWebApplication.CreateBuilder(args)` returns `InfiniFrameWebApplicationBuilder`:

| Property | Type | Description |
|----------|------|-------------|
| `WebApp` | `WebApplicationBuilder` | Standard ASP.NET Core builder |
| `Window` | `InfiniFrameWindowBuilder` | Fluent window configuration |

### Configuring Window and Server

```csharp
var builder = InfiniFrameWebApplication.CreateBuilder(args);

// Configure window
builder.Window
    .SetTitle("My Desktop App")
    .SetSize(1280, 720)
    .Center()
    .SetDevToolsEnabled(true);

// Configure ASP.NET Core
builder.WebApp.Services.AddControllers();
builder.WebApp.Services.AddSignalR();
builder.WebApp.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build().UseAutoServerClose();

app.WebApp.MapControllers();
app.WebApp.MapHub<MyHub>("/hub");
app.WebApp.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

## Start URL Configuration

URL resolved from configuration in this priority order:

1. `ASPNETCORE_URLS` environment variable
2. `urls` configuration key (appsettings.json)
3. Manual override via `builder.Window.SetStartUrl(...)`

### appsettings.json

```json
{
  "urls": "http://localhost:5200"
}
```

**Multiple URLs**: If multiple URLs configured, first one used as window's start URL:
```json
{
  "urls": "http://localhost:5200;https://localhost:7200"
}
```
Window opens with `http://localhost:5200`.

## Accessing Window from ASP.NET Core

`IInfiniFrameWindow` and `IInfiniFrameWindowBuilder` registered in DI container:

### Minimal API

```csharp
app.WebApp.MapGet("/close", (IInfiniFrameWindow window) => {
    window.Close();
    return Results.Ok();
});
```

### Controllers

```csharp
public class WindowController(IInfiniFrameWindow window) : ControllerBase {
    [HttpGet("api/window/minimize")]
    public IActionResult Minimize() {
        window.Invoke(() => { 
            // Minimize logic on UI thread
        });
        return Ok();
    }
}
```

### SignalR Hubs

```csharp
public class MyHub : Hub {
    private readonly IInfiniFrameWindow _window;
    
    public MyHub(IInfiniFrameWindow window) {
        _window = window;
    }
    
    public async Task CloseApp() {
        _window.Invoke(() => _window.Close());
    }
}
```

**Critical**: Window operations from ASP.NET Core handlers MUST use `window.Invoke()` to marshal to window thread.

## Graceful Shutdown

### UseAutoServerClose (Recommended)

```csharp
var app = builder.Build().UseAutoServerClose();
```

Automatically stops web server when window closed or close requested. Internally registers handlers on `WindowClosing` and `WindowClosingRequested` that call `WebApp.StopAsync()` in background task — UI thread never blocked.

### Manual Shutdown

```csharp
// Stop both server and window
app.Stop();

// Stop server only
await app.WebApp.StopAsync();

// Then close window
app.Window.Close();
```

## Blazor Server Example

```csharp
using InfiniFrame.WebServer;

var builder = InfiniFrameWebApplication.CreateBuilder(args);

builder.Window
    .SetTitle("Blazor Server App")
    .SetSize(1280, 720)
    .Center();

builder.WebApp.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build().UseAutoServerClose();

app.WebApp.UseStaticFiles();
app.WebApp.UseAntiforgery();
app.WebApp.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

## Static Web Assets

`UseStaticWebAssets()` called automatically during builder initialization — static files from Razor class libraries served correctly.

`UseDefaultFiles()` also applied during `Build()` — requests to `/` serve `wwwroot/index.html` if it exists.

## Thread Model

| Thread | Runs |
|--------|------|
| **Main thread** | Native window (UI thread) — MUST be STA on Windows |
| **Background thread** | ASP.NET Core / Kestrel |

### Thread Affinity Rules

**From ASP.NET Core handlers → Window**:
```csharp
// MUST use Invoke
window.Invoke(() => window.Close());
```

**From window event handlers → ASP.NET Core**:
```csharp
// Can call directly — ASP.NET Core is thread-safe
await app.WebApp.StopAsync();
```

## Common Patterns

### Minimal API with Controllers

```csharp
var builder = InfiniFrameWebApplication.CreateBuilder(args);

builder.Window.SetTitle("API App").SetSize(1024, 768);

builder.WebApp.Services.AddControllers();
builder.WebApp.Services.AddEndpointsApiExplorer();
builder.WebApp.Services.AddSwaggerGen();

var app = builder.Build().UseAutoServerClose();

app.WebApp.MapControllers();
app.WebApp.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
```

### SignalR Real-Time App

```csharp
var builder = InfiniFrameWebApplication.CreateBuilder(args);

builder.WebApp.Services.AddSignalR();

var app = builder.Build().UseAutoServerClose();

app.WebApp.MapHub<ChatHub>("/chat");
app.WebApp.UseStaticFiles();

app.Run();
```

### Custom Port Configuration

```csharp
// Via code
builder.WebApp.WebHost.ConfigureKestrel(options => {
    options.ListenLocalhost(5000);
});

// Or via configuration
builder.WebApp.Configuration["urls"] = "http://localhost:5000";
```

## Anti-Patterns

❌ **Forget UseAutoServerClose**:
```csharp
// WRONG — server keeps running after window closes
var app = builder.Build();
```

✅ **Always use UseAutoServerClose**:
```csharp
var app = builder.Build().UseAutoServerClose();
```

❌ **Call window ops directly from controller**:
```csharp
// WRONG — thread affinity violation
[HttpGet("close")]
public IActionResult Close(IInfiniFrameWindow window) {
    window.Close();  // Called from Kestrel thread
    return Ok();
}
```

✅ **Use Invoke**:
```csharp
[HttpGet("close")]
public IActionResult Close(IInfiniFrameWindow window) {
    window.Invoke(() => window.Close());
    return Ok();
}
```

❌ **Use async Task Main**:
```csharp
// WRONG — STA ignored on Windows
async Task Main(string[] args) { ... }
```

✅ **Use explicit Main with [STAThread]**:
```csharp
[STAThread]
static void Main(string[] args) { ... }
```

## Examples

- https://github.com/InfiniLore/InfiniFrame/tree/master/examples/InfiniFrameExample.WebApp.Blazor — Blazor Server with InteractiveServerComponents
- https://github.com/InfiniLore/InfiniFrame/tree/master/examples/InfiniFrameExample.WebApp.React — React frontend with custom scheme handler and messaging
- https://github.com/InfiniLore/InfiniFrame/tree/master/examples/InfiniFrameExample.WebApp.Vue — Vue.js frontend with all built-in JS message handlers
