# Example: WebApp Blazor Server

Demonstrates hosting a full ASP.NET Core Blazor Server application inside a native InfiniFrame window using `InfiniLore.InfiniFrame.WebServer`

## What it shows

- `InfiniFrameWebApplication.CreateBuilder()` entry point
- Blazor Server with `AddRazorComponents()` + `AddInteractiveServerComponents()`
- `HttpClient` factory configured to point at the local Kestrel server
- `AddInfiniFrameJs()` service registration for Blazor component interop
- `RegisterOpenExternalTargetWebMessageHandler()` links with `target="_blank"` open in the default browser
- `UseAutoServerClose()` server stops when the window is closed
- Serilog with async console sink

## Run

```bash
dotnet run --project examples/InfiniFrameExample.WebApp.Blazor
```

## Key code

```csharp
InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder(args);

builder.WebApp.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.WebApp.Services.AddInfiniFrameJs();

builder.Window
    .SetSize(new Size(800, 600))
    .RegisterOpenExternalTargetWebMessageHandler();

InfiniFrameWebApplication app = builder.Build();
app.UseAutoServerClose();
app.WebApp.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
```

## Packages used

- `InfiniLore.InfiniFrame.WebServer`
- `InfiniLore.InfiniFrame.Blazor`
- `InfiniLore.InfiniFrame.Js`

## Related documentation

- [Web Server Guide](../../docs/docs/guides/web-server.md)
- [JavaScript Interop Guide](../../docs/docs/guides/javascript-interop.md)
