# Getting Started

This guide walks you through installing InfiniFrame and creating your first native desktop window.

## Contents

- [Prerequisites](#prerequisites)
- [Choose Your Integration](#choose-your-integration)
- [Option 1: Core Window](#option-1-core-window)
- [Option 2: Blazor WebView](#option-2-blazor-webview)
- [Option 3: Web Server](#option-3-web-server)
- [Next Steps](#next-steps)

## Prerequisites

- [.NET 11 SDK](https://dotnet.microsoft.com/download) or later (.NET 10 and 11 are supported)
- A console or class library project targeting `net10.0` or `net11.0`

### Platform-specific requirements

| Platform | Requirement                                                                                                                                                       |
|----------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Windows  | [WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/); pre-installed on Windows 11 and available as a redistributable for Windows 10 |
| Linux    | GTK 4 and WebKitGTK 6.0 development packages (`libgtk-4-dev`, `libwebkitgtk-6.0-dev`), `libnotify-dev`, X11/Wayland development headers, `pkg-config`, and an X11 or Wayland display for GUI tests — see [Linux notes](#linux-notes) |
| macOS    | macOS 10.15 Catalina or later (WKWebView is built into the OS)                                                                                                    |

### Linux notes

**Prerequisites (Debian/Ubuntu):**

```bash
sudo apt install libgtk-4-dev libwebkitgtk-6.0-dev libnotify-dev \
    libx11-dev libwayland-dev pkg-config
```

Verify the expected pkg-config modules resolve:

```bash
pkg-config --modversion gtk4 webkitgtk-6.0 libnotify
```

**Display servers:** both X11 and Wayland are supported. On Wayland the compositor owns
window placement: arbitrary positioning (`SetLeft`/`SetTop`/`SetLocation`), topmost state,
and maximum-size constraints cannot be enforced and are ignored or best-effort, matching
long-standing Linux behavior. On X11 these work through the window manager.

**Rendering:** hardware rendering is the default whenever a render device (`/dev/dri`) is
present. Set `INFINIFRAME_LINUX_FORCE_SOFTWARE_RENDERING=1` to force the software (llvmpipe)
path, or `=0` to disable the automatic software fallback. At startup InfiniFrame logs a
one-time diagnostics block with the GTK/WebKitGTK versions, display backend, renderer
environment, and the software/hardware decision and its reason — attach it to bug reports
for rendering issues. If a library is missing at startup, `ldd` on the native library and
the `pkg-config` check above identify the gap.

**Inspector / debugging:** enable DevTools through the window's debugging feature. Remote
debugging can be bound to a port before window creation; the underlying WebKitGTK remote
inspector is configured through the process environment (`WEBKIT_INSPECTOR_SERVER` /
`WEBKIT_INSPECTOR_HTTP_SERVER`) and is process-scoped on Linux — it must be configured
before the first window is created.

## Choose Your Integration

InfiniFrame supports three integration models depending on your use case:

| Use Case                                            | Package                                |
|-----------------------------------------------------|----------------------------------------|
| Load a URL or HTML string in a window               | `InfiniLore.InfiniFrame`               |
| Run a Blazor app inside a native window (no server) | `InfiniLore.InfiniFrame.BlazorWebView` |
| Run an ASP.NET Core web app with a native window    | `InfiniLore.InfiniFrame.WebServer`     |

## Option 1: Core Window

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
    .SetStartPageUrl("https://example.com")
    .Build();

// Block until the user closes the window
window.WaitForClose();
```

The window opens immediately when `Build()` is called and runs on the current thread.
`WaitForClose()` blocks until the native window is destroyed.

> **Windows:** WebView2 requires the calling thread to be STA (single-threaded apartment). Add `[STAThread]` to your `Main` method. Top-level statements do not support `[STAThread]` directly; use an explicit `static void Main()` instead. Calling `Build()` on a non-STA thread will throw an `InvalidOperationException`.

### Single-file/native packaging bootstrap

If you package your app with embedded native binaries (for example via `InfiniLore.InfiniFrame.SingleFile`), initialize the single-file bootstrap before building your first window:

```csharp
using InfiniFrame;

public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameSingleFile.Initialize();

        var window = InfiniFrameWindowBuilder.Create()
            .SetTitle("Hello, InfiniFrame")
            .SetSize(1280, 720)
            .Center()
            .SetStartPageUrl("https://example.com")
            .Build();

        window.WaitForClose();
    }
}
```

You only need this for packaged outputs that embed native runtime files as resources. Standard development runs (`dotnet run`) do not require it.

## Option 2: Blazor WebView

This integration runs a Blazor application entirely in-process with no server and no HTTP port.

### Install

```bash
dotnet add package InfiniLore.InfiniFrame.BlazorWebView
```

### Project setup

Your project must use `Microsoft.NET.Sdk.Razor` and have a `wwwroot/` folder with `index.html`.

A minimal `index.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <base href="/" />
</head>
<body>
    <div id="app">Loading...</div>
    <div id="blazor-error-ui">An error has occurred.</div>
    <script src="_framework/blazor.webview.js"></script>
</body>
</html>
```

### Program.cs

```csharp
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;

var builder = InfiniFrameApplication.CreateBuilder(args)
    .WithWindow(window => window
        .SetTitle("My Blazor App")
        .SetSize(1280, 720)
        .Center());

// Register your services
builder.Services.AddSingleton<MyService>();

// Register root Blazor components
builder.UseBlazorWebView(configuration => {
    configuration.RootComponents.Add<App>("app");
    configuration.RootComponents.Add<HeadOutlet>("head::after");
});

builder.Build().Run();
```

### Service access in components

`IInfiniFrameWindow` is automatically registered in the DI container so you can inject it directly into any Blazor component or service:

```csharp
@inject IInfiniFrameWindow Window

<button @onclick="() => Window.Close()">Exit</button>
```

## Option 3: Web Server

This integration starts an ASP.NET Core web server in a background thread and opens a native window pointing at it. It's ideal when you want the full ASP.NET Core pipeline (middleware, controllers, SignalR, etc.)

### Install

```bash
dotnet add package InfiniLore.InfiniFrame.WebServer
```

### Program.cs

```csharp
using InfiniFrame.Application;
using InfiniFrame.WebServer;

var app = InfiniFrameApplication.CreateBuilder(args)
    .WithWindow("web", static _ => { })
    .UseWebServer("web", web => web.ConfigureWebApplication(application =>
        application.MapGet("/", () => "Hello from InfiniFrame")))
    .Build();

app.Run();
```

The start URL is resolved from the server's bound address after startup, and the server shuts down with the InfiniFrame application.

## Next Steps

- [Core Window Guide](core-window.md): Builder API, feature overview, and quick reference
- [Window Features Architecture](window-features-architecture.md): How the feature system works
- [Blazor Guide](blazor-webview.md): DI, file providers, component configuration
- [Web Server Guide](web-server.md): ASP.NET Core pipeline, DI access, graceful shutdown
- [API Reference](../api.md): Central API navigation and language-specific guides
