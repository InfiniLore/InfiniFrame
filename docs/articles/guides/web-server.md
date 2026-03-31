# Web Server Guide

`InfiniLore.InfiniFrame.WebServer` runs a standard ASP.NET Core web application in a background thread while opening a native window pointed at it
This approach gives you the full ASP.NET Core pipeline — middleware, controllers, SignalR, minimal APIs, Blazor Server — without any browser overhead

## Contents

- [How It Works](#how-it-works)
- [Installation](#installation)
- [Minimal Setup](#minimal-setup)
- [Builder API](#builder-api)
- [Start URL](#start-url)
- [Accessing the Window from ASP.NET Core](#accessing-the-window-from-aspnet-core)
- [Graceful Shutdown](#graceful-shutdown)
- [Example: Blazor Server](#example-blazor-server)
- [Static Web Assets](#static-web-assets)
- [Thread Model](#thread-model)

## How It Works

- The ASP.NET Core server starts on a background thread
- A native window opens and navigates to the server's URL
- Both shut down together when the window is closed (with `UseAutoServerClose()`)

## Installation

```bash
dotnet add package InfiniLore.InfiniFrame.WebServer
```

## Minimal Setup

```csharp
using InfiniFrame.WebServer;

var app = InfiniFrameWebApplication.CreateBuilder(args)
    .Build()
    .UseAutoServerClose();

app.WebApp.MapGet("/", () => "Hello from InfiniFrame");

app.Run();
```

`app.Run()` starts the web server in the background, opens the window, and blocks until the window is closed

## Builder API

`InfiniFrameWebApplication.CreateBuilder(args)` returns an `InfiniFrameWebApplicationBuilder` with two properties:

| Property | Type                       | Description                                                           |
|----------|----------------------------|-----------------------------------------------------------------------|
| `WebApp` | `WebApplicationBuilder`    | Standard ASP.NET Core builder — add services, configure Kestrel, etc. |
| `Window` | `InfiniFrameWindowBuilder` | Fluent window configuration                                           |

### Configuring the window

```csharp
var builder = InfiniFrameWebApplication.CreateBuilder(args);

builder.Window
    .SetTitle("My Desktop App")
    .SetSize(1280, 720)
    .Center()
    .SetDevToolsEnabled(true);

builder.WebApp.Services.AddControllers();
builder.WebApp.Services.AddSignalR();

var app = builder.Build().UseAutoServerClose();
app.WebApp.MapControllers();
app.WebApp.MapHub<MyHub>("/hub");

app.Run();
```

## Start URL

The window's start URL is automatically resolved from configuration in this priority order:

1. `ASPNETCORE_URLS` environment variable
2. `urls` configuration key (e.g. in `appsettings.json`)
3. Manual override via `builder.Window.SetStartUrl(...)`

```json
// appsettings.json
{
  "urls": "http://localhost:5200"
}
```

If multiple URLs are configured (e.g. `"http://localhost:5200;https://localhost:7200"`), the first one is used as the window's start URL

## Accessing the Window from ASP.NET Core

`IInfiniFrameWindow` and `IInfiniFrameWindowBuilder` are registered in the web app's DI container:

```csharp
app.WebApp.MapGet("/close", (IInfiniFrameWindow window) => {
    window.Close();
    return Results.Ok();
});
```

```csharp
public class MyController(IInfiniFrameWindow window) : ControllerBase {
    [HttpGet("minimize")]
    public IActionResult Minimize() {
        window.Invoke(() => { /* minimize logic */ });
        return Ok();
    }
}
```

> **Note:** Window operations that affect the native UI must be marshalled to the window thread using `window.Invoke(...)`

## Graceful Shutdown

### UseAutoServerClose

Automatically stops the web server when the window is closed or a close is requested:

```csharp
var app = builder.Build().UseAutoServerClose();
```

Internally this registers handlers on both `WindowClosing` and `WindowClosingRequested` that call `WebApp.StopAsync()` in a background task, so the UI thread is never blocked

### Manual shutdown

```csharp
app.Stop(); // Stops the web server and closes the window
```

```csharp
await app.WebApp.StopAsync(); // Stop server only
app.Window.Close();           // Then close window
```

## Example: Blazor Server

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

`UseStaticWebAssets()` is called automatically during builder initialization, so static files from Razor class libraries are served correctly without additional configuration

`UseDefaultFiles()` is also applied during `Build()`, which causes requests to `/` to serve `wwwroot/index.html` if it exists

## Thread Model

| Thread            | Runs                      |
|-------------------|---------------------------|
| Main thread       | Native window (UI thread) |
| Background thread | ASP.NET Core / Kestrel    |

All window API calls from ASP.NET Core handlers must use `window.Invoke(...)` to marshal to the window thread
Web server calls from window event handlers can be made directly since ASP.NET Core is thread-safe

> **Windows:** The main thread must be STA. Add `[STAThread]` to your `Main` method and use an explicit `static void Main()` — top-level statements and `async Task Main` do not support STA correctly

---

## Examples

- `InfiniFrameExample.WebApp.Blazor` (`examples/InfiniFrameExample.WebApp.Blazor`) - Blazor Server with InteractiveServerComponents, HttpClient factory, and InfiniFrameJs
- `InfiniFrameExample.WebApp.React` (`examples/InfiniFrameExample.WebApp.React`) - React frontend with custom scheme handler and two-way messaging
- `InfiniFrameExample.WebApp.Vue` (`examples/InfiniFrameExample.WebApp.Vue`) - Vue.js frontend with all built-in JS message handlers
