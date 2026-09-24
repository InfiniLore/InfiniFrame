# Web Server Guide

`InfiniLore.InfiniFrame.WebServer` hosts an ASP.NET Core application alongside native InfiniFrame windows.

## Installation

```bash
dotnet add package InfiniLore.InfiniFrame.WebServer
```

## Minimal Setup

```csharp
using InfiniFrame.Application;
using InfiniFrame.WebServer;

var app = InfiniFrameApplication.CreateBuilder(args)
    .WithWindow("web", window => window.SetTitle("My Desktop App"))
    .UseWebServer("web", web => web.ConfigureWebApplication(application =>
        application.MapGet("/", () => "Hello from InfiniFrame")))
    .Build();

app.Run();
```

`Run()` starts the server, opens the window, and stops the server during application shutdown.

## Configuration

`InfiniFrameWebServerConfiguration` exposes `WebHost` for Kestrel configuration and
`ConfigureWebApplication` for configuring the built `WebApplication`:

```csharp
var builder = InfiniFrameApplication.CreateBuilder(args)
    .WithWindow("web", window => window
        .SetTitle("Web App")
        .SetSize(1280, 720));

builder.UseWebServer("web", web => {
    web.WebHost.UseUrls("http://127.0.0.1:5055");
    web.ConfigureWebApplication(application => {
        application.UseRouting();
        application.MapGet("/health", () => Results.Ok());
    });
});

builder.Build().Run();
```

The server's bound address is resolved after startup. Wildcard bindings are converted to a loopback URL before navigation and origin trust are configured.

## Dependency Injection

Application services are copied into the ASP.NET Core service collection. Window APIs can therefore be injected into handlers:

```csharp
application.MapGet("/close", (IInfiniFrameWindow window) => {
    window.Close();
    return Results.Ok();
});
```

## Blazor Server

```csharp
using InfiniFrame.Application;
using InfiniFrame.WebServer;

var builder = InfiniFrameApplication.CreateBuilder(args)
    .WithWindow(window => window.SetTitle("Blazor Server"));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.UseWebServer(web => web.ConfigureWebApplication(application =>
    application.MapRazorComponents<App>().AddInteractiveServerRenderMode()));

builder.Build().Run();
```

For static assets, configure the appropriate ASP.NET Core static-file or static-asset middleware in `ConfigureWebApplication`.
