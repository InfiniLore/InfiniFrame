<img src="https://raw.githubusercontent.com/InfiniLore/InfiniFrame/refs/heads/core/assets/badge.png" alt="logo" height="76"/> 

*A modern and cross-platform native window framework for .NET that lets you build desktop applications using web
technologies — load any URL, render HTML strings, or embed a full Blazor application inside a native window*

Supports **Windows** (WebView2), **Linux** (WebKit2GTK), and **macOS** (WKWebView)

> **Note:** This project is a modern rework
> of [Photino.Net](https://github.com/tryphotino/photino.NET), [Photino.Net.Server](https://github.com/tryphotino/photino.NET.Server), [Photino.Blazor](https://github.com/tryphotino/Photino.Blazor)
> and [Photino.Native](https://github.com/tryphotino/photino.Native) and is not affiliated with or endorsed by the
> original Photino authors

[![CI: Platform Tests](https://github.com/InfiniLore/InfiniFrame/actions/workflows/ci-testing.yml/badge.svg)](https://github.com/InfiniLore/InfiniFrame/actions/workflows/ci-testing.yml)

## Packages

| Description                                          | Package                                                                                                                                                                                         |
|------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Core window builder and runtime                      | [![NuGet](https://img.shields.io/nuget/v/InfiniLore.InfiniFrame?label=InfiniLore.InfiniFrame)](https://www.nuget.org/packages/InfiniLore.InfiniFrame)                                           |
| Shared interfaces, types, enums, and delegates       | [![NuGet](https://img.shields.io/nuget/v/InfiniLore.InfiniFrame.Shared?label=InfiniLore.InfiniFrame.Shared)](https://www.nuget.org/packages/InfiniLore.InfiniFrame.Shared)                      |
| Pre-built Blazor components for custom window chrome | [![NuGet](https://img.shields.io/nuget/v/InfiniLore.InfiniFrame.Blazor?label=InfiniLore.InfiniFrame.Blazor)](https://www.nuget.org/packages/InfiniLore.InfiniFrame.Blazor)                      |
| Full Blazor app integration inside a native window   | [![NuGet](https://img.shields.io/nuget/v/InfiniLore.InfiniFrame.BlazorWebView?label=InfiniLore.InfiniFrame.BlazorWebView)](https://www.nuget.org/packages/InfiniLore.InfiniFrame.BlazorWebView) |
| ASP.NET Core web app running inside a native window  | [![NuGet](https://img.shields.io/nuget/v/InfiniLore.InfiniFrame.WebServer?label=InfiniLore.InfiniFrame.WebServer)](https://www.nuget.org/packages/InfiniLore.InfiniFrame.WebServer)             |
| JavaScript and Blazor interop utilities              | [![NuGet](https://img.shields.io/nuget/v/InfiniLore.InfiniFrame.Js?label=InfiniLore.InfiniFrame.Js)](https://www.nuget.org/packages/InfiniLore.InfiniFrame.Js)                                  |

## Quick Start

### Load a URL in a native window

Install: `dotnet add package InfiniLore.InfiniFrame`

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

### Embed a Blazor application

Install: `dotnet add package InfiniLore.InfiniFrame.BlazorWebView`

```csharp
using InfiniFrame.BlazorWebView;

var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetTitle("My Blazor App")
    .SetSize(1280, 720)
    .Center()
);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Build().Run();
```

### Host an ASP.NET Core web app

Install: `dotnet add package InfiniLore.InfiniFrame.WebServer`

```csharp
using InfiniFrame.WebServer;

var app = InfiniFrameWebApplication.CreateBuilder(args)
    .Build()
    .UseAutoServerClose();

app.Run();
```

## Architecture

```
Your Application
│
├── InfiniLore.InfiniFrame                ← Core: window builder & runtime
├── InfiniLore.InfiniFrame.BlazorWebView  ← Blazor app lifecycle
├── InfiniLore.InfiniFrame.WebServer      ← ASP.NET Core integration
│
├── InfiniLore.InfiniFrame.Blazor         ← Window chrome Razor components
├── InfiniLore.InfiniFrame.Js             ← JS/Blazor interop
│
└── InfiniLore.InfiniFrame.Shared         ← Interfaces, types, native interop
        └── InfiniFrame.Native (internal) ← C++ platform layer
```

Only one of `BlazorWebView`, `WebServer`, or the core `InfiniFrame` package is needed for a given application type —
they are independent integration paths

## Examples

| Example                                                                                         | What it demonstrates                                          |
|-------------------------------------------------------------------------------------------------|---------------------------------------------------------------|
| [BlazorWebView](examples/InfiniFrameExample.BlazorWebView/)                                     | Basic Blazor app in a native window                           |
| [BlazorWebView.MultiWindowSample](examples/InfiniFrameExample.BlazorWebView.MultiWindowSample/) | Multiple independent windows with different Blazor components |
| [WebApp.Blazor](examples/InfiniFrameExample.WebApp.Blazor/)                                     | Blazor Server hosted via ASP.NET Core                         |
| [WebApp.React](examples/InfiniFrameExample.WebApp.React/)                                       | React frontend with custom scheme handler and web messaging   |
| [WebApp.Vue](examples/InfiniFrameExample.WebApp.Vue/)                                           | Vue.js frontend with all built-in JS message handlers         |

## Single-File Executable Packing

Use the custom .NET tool `InfiniFrame-Pack` to package your app into a single executable with embedded native
dependencies.
See [`/docs`](docs/articles/guides/pack-tool.md) for full usage details and options.

Install the tool first before running packaging commands (including the `InfiniFrameExample.SingleFileExe` post-build
pack target):

```powershell
.\src\InfiniFrame.Tools.Pack\install-or-update-pack-tool.ps1
```

```bash
bash ./src/InfiniFrame.Tools.Pack/install-or-update-pack-tool.sh
```

Manual alternative:

```powershell
dotnet pack src/InfiniFrame.Tools.Pack/InfiniFrame.Tools.Pack.csproj -c Release
dotnet tool install --global --add-source .\src\InfiniFrame.Tools.Pack\bin\Release InfiniLore.InfiniFrame.Tools.Pack
```

```powershell
dotnet tool run infiniframe-pack publish src/MyApp/MyApp.csproj --rid win-x64
```

## Documentation

- [Docs Home](https://docs.infiniframe.dev/)
- [Getting Started](https://docs.infiniframe.dev/guides/getting-started) — Installation, first app,
  platform requirements

### Guides

- [Core Window](https://docs.infiniframe.dev/guides/core-window) — Builder pattern, configuration, events,
  messaging
- [Blazor WebView](https://docs.infiniframe.dev/guides/blazor-webview) — Hosting a full Blazor app in a
  native window
- [Web Server](https://docs.infiniframe.dev/guides/web-server) — ASP.NET Core + native window integration
- [Custom Window Chrome](https://docs.infiniframe.dev/guides/custom-window-chrome) — Chromeless windows
  with Blazor components
- [JavaScript Interop](https://docs.infiniframe.dev/guides/javascript-interop) — Communicating between JS
  and C#

### API Reference

- [Native C++ API Guide](https://docs.infiniframe.dev/cpp/native-cpp-api)
- [Native C++ API Reference (Generated)](https://docs.infiniframe.dev/api/cpp/native-cpp-reference.html)
- [Generated C# API Reference](https://docs.infiniframe.dev/api/cs/)

### Migration

- [Breaking Changes vs Photino.NET](https://docs.infiniframe.dev/concepts/breaking-changes-from-photino) —
  API, namespace, event system, and behavioral differences from the original Photino projects

### Build Docs Locally

Requirements:

- `Node.js`/`npm` (required)

Run:

```powershell
npm run docs:dev
```

Build static site:

```powershell
npm run docs:build
```

Legacy DocFX parity flow (temporary during migration):

```powershell
.\docs\scripts\serve-docs.ps1 -LegacyDocfx
```

## Platform Requirements

| Platform | Browser Engine      | Requirement                           |
|----------|---------------------|---------------------------------------|
| Windows  | WebView2 (Chromium) | Windows 10 or later, WebView2 Runtime |
| Linux    | WebKit2GTK          | GTK 3+                                |
| macOS    | WKWebView           | macOS 10.15 Catalina or later         |

## Repo History

This repo was originally forked from [Photino.NET](https://github.com/tryphotino/photino.NET) and then the history of
the [Photino.Blazor](https://github.com/tryphotino/Photino.Blazor)
and [Photino.Net.Server](https://github.com/tryphotino/photino.NET.Server) repositories were merged into this.
By merging the histories, it was possible to ease further development, especially whilst also preserving the original
commit history and attribution from the contributors of Photino

This was also done for the [Photino.Native](https://github.com/tryphotino/photino.Native) library, but given the
extensive work that had already been done, git was seemingly unable to fully merge the commit history without losing the
original commit history

## License

Unlike the other projects in the InfiniLore ecosystem, this repo follows the same [Apache 2.0 License](LICENSE) as the
original Photino projects
