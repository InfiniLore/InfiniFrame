# Blazor WebView Guide

`InfiniLore.InfiniFrame.BlazorWebView` integrates a full Blazor WebAssembly-style application into a native window with no HTTP server required. The Blazor runtime runs entirely in-process.

## Contents

- [How It Works](#how-it-works)
- [Project Setup](#project-setup)
- [Program.cs](#programcs)
- [Available Builder API](#available-builder-api)
- [Dependency Injection](#dependency-injection)
- [Custom File Provider](#custom-file-provider)
- [External JS Modules and Trusted Origins](#external-js-modules-and-trusted-origins)
- [Error Handling](#error-handling)
- [HttpClient](#httpclient)
- [Lifecycle](#lifecycle)
- [Custom Window Chrome](#custom-window-chrome)

## How It Works

InfiniFrame serves Blazor resources from an internal origin (`app://localhost/`) and handles requests inside the native host. Blazor component files, JavaScript, and CSS are served from an `IFileProvider` backed by your `wwwroot/` folder.
There is no external ASP.NET server required; all communication happens through the native browser bridge.

`app://localhost/` follows normal browser URL semantics. Query strings and fragments remain in the browser-visible URL, while InfiniFrame removes them only when looking up an embedded resource. For example, navigating to `app://localhost/index.html?mode=desktop#settings` serves `index.html`, leaves the full URL visible, and exposes `#settings` through `window.location.hash`.

Same-origin browser requests to application assets are supported with both `fetch()` and `XMLHttpRequest`. The native engines register `app` as a secure, authority-bearing scheme and allow the `app://localhost` origin. Cross-origin access is not enabled implicitly; add trusted origins through the URI security policy only when the application genuinely needs them.

Platform notes:
- Windows uses WebView2 and requires custom-scheme registration support (`ICoreWebView2EnvironmentOptions4`) to allow top-level `app://localhost/...` navigation.
- Linux and macOS use WebKit-based engines and do not depend on WebView2.
- On Windows, if the WebView2 runtime does not support custom-scheme registration, startup fails fast with a clear error asking for a WebView2 runtime update.

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
    <script src="_framework/blazor.webview.js"></script>
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

// Register services (same as a standard Blazor or ASP.NET Core app)
builder.Services.AddSingleton<MyDataService>();
builder.Services.AddScoped<IMyRepository, MyRepository>();

// Register root components (these map to elements in index.html)
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Build().Run();
```

`Run()` blocks until the window is closed and then disposes all services.

## Available Builder API

`InfiniFrameBlazorAppBuilder` exposes three properties for configuration:

| Property         | Type                        | Description                                                                  |
|------------------|-----------------------------|------------------------------------------------------------------------------|
| `WindowBuilder`  | `IInfiniFrameWindowBuilder` | Fluent window configuration; all options from the generated C# API reference |
| `Services`       | `IServiceCollection`        | Standard .NET DI container                                                   |
| `RootComponents` | `RootComponentList`         | Maps Blazor components to CSS selectors in index.html                        |

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

| Service              | Lifetime  | Description                           |
|----------------------|-----------|---------------------------------------|
| `IInfiniFrameWindow` | Singleton | The native window instance            |
| `IInfiniFrameJs`     | Scoped    | JavaScript interop utilities          |
| `HttpClient`         | Scoped    | Preconfigured for in-process requests |
| `Dispatcher`         | Singleton | Blazor's component dispatcher         |

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

By default, files are served from `{AppBaseDirectory}/wwwroot/`.
You can supply a custom `IFileProvider` for embedded resources, encrypted assets, or virtual file systems:

```csharp
using Microsoft.Extensions.FileProviders;

var embeddedProvider = new EmbeddedFileProvider(typeof(Program).Assembly, "MyApp.wwwroot");

var builder = InfiniFrameBlazorAppBuilder.CreateDefault(
    fileProvider: embeddedProvider,
    args: args
);
```

## External JS Modules and Trusted Origins

If your app imports scripts from external origins (for example `import ... from "https://cdn.example/..."`), keep `WebSecurity` enabled and explicitly trust those origins.

```csharp
var app = InfiniFrameBlazorAppBuilder.CreateDefault(windowBuilder: wb => {
    wb.AddTrustedOrigin("https://xyz");
    // add redirects too if needed (e.g. cdn.jsdelivr.net, unpkg.com, etc.)
});
```

For multiple hosts:

```csharp
var app = InfiniFrameBlazorAppBuilder.CreateDefault(windowBuilder: wb => {
    wb.AddTrustedOrigin("https://xyz");
    wb.AddTrustedOrigin("https://cdn.jsdelivr.net");
    wb.AddTrustedOrigin("https://unpkg.com");
});
```

To disable origin checks entirely (not recommended for production), opt in explicitly:

```csharp
var app = InfiniFrameBlazorAppBuilder.CreateDefault(windowBuilder: wb => {
    wb.SetTrustAllOrigins(true);
});
```

Do not use `.SetWebSecurityEnabled(false)` as a workaround for this scenario.

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

An `HttpClient` is registered automatically with `BaseAddress` set to the internal app base URI.
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

## Debugging workflow

Use devtools and remote debugging separately:

```csharp
var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetDevToolsEnabled(true)
    .SetWebInspectorEnabled(true)  // macOS 13.3+ Safari Web Inspector attachability
    .SetRemoteDebuggingPort(9222) // Windows and Linux, startup-only
);

var app = builder.Build();

if (app.Window.Debug.TryGetRemoteDebuggingEndpoint(out Uri? endpoint))
    Console.WriteLine($"Remote debug endpoint: {endpoint}");

app.Run();
```

- `SetDevToolsEnabled(true)` controls local inspector UI.
- `SetWebInspectorEnabled(true)` controls WKWebView Safari Web Inspector attachability on macOS 13.3+.
- `SetRemoteDebuggingPort(int? port)` controls TCP endpoint availability (`1..65535`, `0/null` disables).
- Linux inspector endpoint uses WebKitGTK inspector server (`http://127.0.0.1:<port>/`).
- On Linux, WebKit requires developer extras for remote inspector and keeps them enabled while remote debugging is active.
- On Linux, inspector server configuration is process-scoped (shared across windows in the same process).
- On unsupported platforms (macOS), enabling remote debugging throws `PlatformNotSupportedException`.
- On unsupported platforms (Windows/Linux, or macOS below 13.3), enabling web inspector mode throws `PlatformNotSupportedException`.

## Custom Window Chrome

Combine with `InfiniLore.InfiniFrame.Blazor` for a fully custom title bar.

See the [Custom Window Chrome Guide](custom-window-chrome.md) for details.

## Examples

- `InfiniFrameExample.BlazorWebView` (`examples/InfiniFrameExample.BlazorWebView`) - minimal Blazor app with window configuration and Serilog
- `InfiniFrameExample.BlazorWebView.MultiWindowSample` (`examples/InfiniFrameExample.BlazorWebView.MultiWindowSample`) - multiple windows each hosting a different Blazor component
