# Blazor WebView Guide

`InfiniLore.InfiniFrame.BlazorWebView` integrates a full Blazor WebAssembly-style application into a native window with no HTTP server required — the Blazor runtime runs entirely in-process

## Contents

- [How It Works](#how-it-works)
- [Project Setup](#project-setup)
- [Program.cs](#programcs)
- [Available Builder API](#available-builder-api)
- [Dependency Injection](#dependency-injection)
- [Custom File Provider](#custom-file-provider)
- [Error Handling](#error-handling)
- [HttpClient](#httpclient)
- [Lifecycle](#lifecycle)
- [Custom Window Chrome](#custom-window-chrome)

## How It Works

InfiniFrame registers a custom URL scheme (`app://`) and handles all requests from the WebView internally — Blazor component files, JavaScript, and CSS are served from an `IFileProvider` backed by your `wwwroot/` folder
There is no localhost server; all communication happens through the native browser bridge

## Project Setup

Your project must use the Razor SDK:

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="InfiniLore.InfiniFrame.BlazorWebView" Version="0.1.1" />
  </ItemGroup>
</Project>
```

### wwwroot/index.html

A minimal host page:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <link rel="stylesheet" href="app.css" />
</head>
<body>
    <div id="app">Loading...</div>
    <div id="blazor-error-ui" style="display:none">
        An unhandled error has occurred.
    </div>
    <script src="_framework/blazor.webview.js" autostart="false"></script>
</body>
</html>
```

## Program.cs

```csharp
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;

var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetTitle("My Blazor App")
    .SetSize(1280, 720)
    .Center()
    .SetChromeless(true)     // Optional: remove native title bar
);

// Register services — same as a standard Blazor or ASP.NET Core app
builder.Services.AddSingleton<MyDataService>();
builder.Services.AddScoped<IMyRepository, MyRepository>();

// Register root components — these map to elements in index.html
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Build().Run();
```

`Run()` blocks until the window is closed and then disposes all services

## Available Builder API

`InfiniFrameBlazorAppBuilder` exposes three properties for configuration:

| Property | Type | Description |
|----------|------|-------------|
| `WindowBuilder` | `IInfiniFrameWindowBuilder` | Fluent window configuration — all options from the [Builder API](../api/builder-api.md) |
| `Services` | `IServiceCollection` | Standard .NET DI container |
| `RootComponents` | `RootComponentList` | Maps Blazor components to CSS selectors in index.html |

### Configuring the window separately

```csharp
var builder = InfiniFrameBlazorAppBuilder.CreateDefault();

builder.WithInfiniFrameWindowBuilder(w => w
    .SetTitle("Configured Later")
    .SetDevToolsEnabled(true)
);
```

## Dependency Injection

The following services are automatically registered and available for injection:

| Service | Lifetime | Description |
|---------|----------|-------------|
| `IInfiniFrameWindow` | Singleton | The native window instance |
| `IInfiniFrameJs` | Scoped | JavaScript interop utilities |
| `HttpClient` | Scoped | Preconfigured for in-process requests |
| `Dispatcher` | Singleton | Blazor's component dispatcher |

### Injecting the window in a component

```razor
@inject IInfiniFrameWindow Window

<button @onclick="Minimize">Minimize</button>
<button @onclick="Close">Close</button>

@code {
    void Minimize() => Window.Invoke(() => { /* window ops must be on UI thread */ });
    void Close() => Window.Close();
}
```

## Custom File Provider

By default, files are served from `{AppBaseDirectory}/wwwroot/`
You can supply a custom `IFileProvider` for embedded resources, encrypted assets, or virtual file systems:

```csharp
using Microsoft.Extensions.FileProviders;

var embeddedProvider = new EmbeddedFileProvider(typeof(Program).Assembly, "MyApp.wwwroot");

var builder = InfiniFrameBlazorAppBuilder.CreateDefault(
    fileProvider: embeddedProvider,
    args: args
);
```

## Error Handling

Unhandled exceptions in the process are caught automatically and shown as a native message dialog:

```
Fatal exception
System.NullReferenceException: Object reference not set...
```

To customize error handling, register a handler before `Build()`:

```csharp
AppDomain.CurrentDomain.UnhandledException += (_, e) => {
    // Custom logging or reporting
};
```

## HttpClient

An `HttpClient` is registered automatically with `BaseAddress` set to the internal app base URI
This lets you make in-process requests to your static assets or call external APIs:

```razor
@inject HttpClient Http

@code {
    protected override async Task OnInitializedAsync() {
        var data = await Http.GetFromJsonAsync<MyData[]>("data/mydata.json");
    }
}
```

## Lifecycle

```
InfiniFrameBlazorAppBuilder.CreateDefault()
    ↓
Configure Services & RootComponents
    ↓
.Build()  ← Registers the custom scheme, creates the window
    ↓
.Run()    ← Starts the Blazor runtime, blocks until window closes
    ↓
DisposeAsync()  ← Disposes all services
```

## Custom Window Chrome

Combine with `InfiniLore.InfiniFrame.Blazor` for a fully custom title bar

See the [Custom Window Chrome Guide](custom-window-chrome.md) for details

## Examples

- `InfiniFrameExample.BlazorWebView` (`examples/InfiniFrameExample.BlazorWebView`) - minimal Blazor app with window configuration and Serilog
- `InfiniFrameExample.BlazorWebView.MultiWindowSample` (`examples/InfiniFrameExample.BlazorWebView.MultiWindowSample`) - multiple windows each hosting a different Blazor component
