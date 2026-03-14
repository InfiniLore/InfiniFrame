# Getting Started

This guide walks you through installing InfiniFrame and creating your first native desktop window

## Contents

- [Prerequisites](#prerequisites)
- [Choose Your Integration](#choose-your-integration)
- [Option 1 — Core Window](#option-1--core-window)
- [Option 2 — Blazor WebView](#option-2--blazor-webview)
- [Option 3 — Web Server](#option-3--web-server)
- [Next Steps](#next-steps)



## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or later (.NET 8, 9, and 10 are all supported)
- A console or class library project targeting `net8.0`, `net9.0`, or `net10.0`

### Platform-specific requirements

| Platform | Requirement |
|----------|-------------|
| Windows | [WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) — pre-installed on Windows 11; available as a redistributable for Windows 10 |
| Linux | `webkit2gtk-4.0` and `libgtk-3-dev` installed via your package manager |
| macOS | macOS 10.15 Catalina or later (WKWebView is built into the OS) |



## Choose Your Integration

InfiniFrame supports three integration models depending on your use case:

| Use Case | Package |
|----------|---------|
| Load a URL or HTML string in a window | `InfiniLore.InfiniFrame` |
| Run a Blazor app inside a native window (no server) | `InfiniLore.InfiniFrame.BlazorWebView` |
| Run an ASP.NET Core web app with a native window | `InfiniLore.InfiniFrame.WebServer` |



## Option 1 — Core Window

### Install

```bash
dotnet add package InfiniLore.InfiniFrame
```

### Create a window

```csharp
using InfiniFrame;

// Create and configure the window
var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("Hello, InfiniFrame")
    .SetSize(1280, 720)
    .Center()
    .SetStartUrl("https://example.com")
    .Build();

// Block until the user closes the window
window.WaitForClose();
```

The window opens immediately when `Build()` is called and runs on the current thread
`WaitForClose()` blocks until the native window is destroyed



## Option 2 — Blazor WebView

This integration runs a Blazor application entirely in-process — no server, no HTTP port

### Install

```bash
dotnet add package InfiniLore.InfiniFrame.BlazorWebView
```

### Project setup

Your project must use `Microsoft.NET.Sdk.Razor` and have a `wwwroot/` folder with `index.html`

A minimal `index.html`:

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <base href="/" />
</head>
<body>
    <div id="app">Loading...</div>
    <div id="blazor-error-ui">An error has occurred.</div>
    <script src="_framework/blazor.webview.js" autostart="false"></script>
</body>
</html>
```

### Program.cs

```csharp
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;

var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetTitle("My Blazor App")
    .SetSize(1280, 720)
    .Center()
);

// Register your services
builder.Services.AddSingleton<MyService>();

// Register root Blazor components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Build().Run();
```

### Service access in components

`IInfiniFrameWindow` is automatically registered in the DI container so you can inject it directly into any Blazor component or service:

```csharp
@inject IInfiniFrameWindow Window

<button @onclick="() => Window.Close()">Exit</button>
```

## Option 3 — Web Server

This integration starts an ASP.NET Core web server in a background thread and opens a native window pointing at it — ideal when you want the full ASP.NET Core pipeline (middleware, controllers, SignalR, etc.)

### Install

```bash
dotnet add package InfiniLore.InfiniFrame.WebServer
```

### Program.cs

```csharp
using InfiniFrame.WebServer;

var app = InfiniFrameWebApplication.CreateBuilder(args)
    .Build()
    .UseAutoServerClose();

// Configure the ASP.NET Core pipeline on app.WebApp
app.WebApp.UseRouting();
app.WebApp.MapGet("/", () => "Hello from InfiniFrame");

app.Run();
```

The start URL is automatically read from `ASPNETCORE_URLS` or the `urls` configuration key
`UseAutoServerClose()` ensures the server shuts down gracefully when the window is closed

## Next Steps

- [Core Window Guide](Guides/CoreWindow.md) — Window events, messaging, dialogs, custom schemes
- [Blazor Guide](Guides/Blazor.md) — DI, file providers, component configuration
- [Web Server Guide](Guides/WebServer.md) — ASP.NET Core pipeline, DI access, graceful shutdown
- [Builder API Reference](Reference/BuilderApi.md) — All window configuration options
