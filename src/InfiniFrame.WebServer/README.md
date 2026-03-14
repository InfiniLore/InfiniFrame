# InfiniLore.InfiniFrame.WebServer

Combines an ASP.NET Core web application with a native InfiniFrame window — the server runs in the background, the window is the user interface

## What it does

- Starts Kestrel on a background thread
- Opens a native window navigating to the server's URL
- Registers `IInfiniFrameWindow` in the ASP.NET Core DI container
- Automatically reads the start URL from `ASPNETCORE_URLS` or `urls` configuration
- Provides optional auto-shutdown coordination between the server and the window

## Installation

```bash
dotnet add package InfiniLore.InfiniFrame.WebServer
```

## Basic Usage

```csharp
using InfiniFrame.WebServer;

var app = InfiniFrameWebApplication.CreateBuilder(args)
    .Build()
    .UseAutoServerClose();

app.WebApp.MapGet("/", () => "Hello from InfiniFrame");

app.Run();
```

## Key Types

| Type | Description |
|------|-------------|
| `InfiniFrameWebApplication` | Static entry point via `CreateBuilder(args)` |
| `InfiniFrameWebApplicationBuilder` | Exposes `WebApp` (ASP.NET Core) and `Window` (InfiniFrame) builders |
| `InfiniFrameWebApplication` | Built application — call `Run()` to start |

## Configuration

The window start URL is resolved automatically:

```json
// appsettings.json
{
  "urls": "http://localhost:5200"
}
```

## Links

- [Full Documentation](../../docs/Guides/WebServer.md)
- [NuGet](https://www.nuget.org/packages/InfiniLore.InfiniFrame.WebServer)
