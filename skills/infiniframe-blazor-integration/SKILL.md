---
name: infiniframe-blazor-integration
description: Building Blazor WebView apps with InfiniLore.InfiniFrame.BlazorWebView. Project setup, DI, file providers, lifecycle, and multi-window patterns.
---
# InfiniFrame Blazor Integration

> Skill for building Blazor WebView apps using `InfiniLore.InfiniFrame.BlazorWebView`.

## When to Use This Skill

- Running Blazor apps in native windows with no HTTP server
- Building desktop apps with Blazor UI
- Multi-window Blazor applications
- Custom file providers for embedded assets
- Custom window chrome with Blazor components

## Package

```bash
dotnet add package InfiniLore.InfiniFrame.BlazorWebView
```

`InfiniLore.InfiniFrame.Js` is automatically included.

## How It Works

- Registers custom URL scheme (`app://`)
- Blazor component files served from `IFileProvider` (wwwroot/)
- No localhost server — all communication through native browser bridge
- Blazor runtime runs entirely in-process

## Project Setup

### .csproj Configuration

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

**MUST use `Microsoft.NET.Sdk.Razor`** for Blazor compilation.

### wwwroot/index.html

Minimal host page:

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

**Critical**: `autostart="false"` is required — InfiniFrame controls Blazor startup.

## Program.cs Pattern

```csharp
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;

var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetTitle("My Blazor App")
    .SetSize(1280, 720)
    .Center()
    .SetChromeless(true)     // Optional: remove native title bar
);

// Register services (same as standard Blazor/ASP.NET Core)
builder.Services.AddSingleton<MyDataService>();
builder.Services.AddScoped<IMyRepository, MyRepository>();

// Register root components (map to CSS selectors in index.html)
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Build().Run();  // Blocks until window closes, then disposes services
```

## Builder API

`InfiniFrameBlazorAppBuilder` exposes three properties:

| Property | Type | Description |
|----------|------|-------------|
| `WindowBuilder` | `IInfiniFrameWindowBuilder` | Fluent window configuration |
| `Services` | `IServiceCollection` | Standard .NET DI container |
| `RootComponents` | `RootComponentList` | Maps Blazor components to CSS selectors |

### Configuring Window Separately

```csharp
var builder = InfiniFrameBlazorAppBuilder.CreateDefault();

builder.WithInfiniFrameWindowBuilder(w => w
    .SetTitle("Configured Later")
    .SetDevToolsEnabled(true)
    .SetSize(1280, 720)
    .Center()
);
```

## Dependency Injection

### Auto-Registered Services

| Service | Lifetime | Description |
|---------|----------|-------------|
| `IInfiniFrameWindow` | Singleton | The native window instance |
| `IInfiniFrameJs` | Scoped | JavaScript interop utilities |
| `HttpClient` | Scoped | Preconfigured for in-process requests |
| `Dispatcher` | Singleton | Blazor's component dispatcher |

### Injecting Window in Components

```razor
@inject IInfiniFrameWindow Window

<button @onclick="Minimize">Minimize</button>
<button @onclick="Close">Close</button>

@code {
    void Minimize() => Window.Invoke(() => { 
        // Window ops MUST run on UI thread
    });
    
    void Close() => Window.Close();
}
```

**Important**: All window operations affecting native UI MUST use `Window.Invoke()` from Blazor components.

## Custom File Provider

Default: files served from `{AppBaseDirectory}/wwwroot/`

### Embedded Resources

```csharp
using Microsoft.Extensions.FileProviders;

var embeddedProvider = new EmbeddedFileProvider(
    typeof(Program).Assembly, 
    "MyApp.wwwroot"
);

var builder = InfiniFrameBlazorAppBuilder.CreateDefault(
    fileProvider: embeddedProvider,
    args: args
);
```

### Use Cases

- Embedded resources (no external files)
- Encrypted assets
- Virtual file systems
- Dynamic content generation

## Error Handling

### Automatic

Unhandled exceptions caught automatically and shown as native message dialog:

```
Fatal exception
System.NullReferenceException: Object reference not set...
```

### Custom Error Handling

```csharp
AppDomain.CurrentDomain.UnhandledException += (_, e) => {
    // Custom logging or reporting
};
```

Register before `Build()`.

## HttpClient

Auto-registered with `BaseAddress` set to internal app base URI:

```razor
@inject HttpClient Http

@code {
    protected override async Task OnInitializedAsync() {
        var data = await Http.GetFromJsonAsync<MyData[]>("data/mydata.json");
    }
}
```

Can make in-process requests to static assets or call external APIs.

## Lifecycle

```
InfiniFrameBlazorAppBuilder.CreateDefault()
    ↓
Configure Services & RootComponents
    ↓
.Build()  ← Registers custom scheme, creates window
    ↓
.Run()    ← Starts Blazor runtime, blocks until window closes
    ↓
DisposeAsync()  ← Disposes all services
```

## Multi-Window Pattern

```csharp
// Window 1
var builder1 = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetTitle("Window 1")
    .SetSize(800, 600)
);
builder1.RootComponents.Add<Window1Component>("#app");

// Window 2
var builder2 = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetTitle("Window 2")
    .SetSize(800, 600)
);
builder2.RootComponents.Add<Window2Component>("#app");

// Run first window (blocks)
builder1.Build().Run();
```

See https://github.com/InfiniLore/InfiniFrame/tree/master/examples/InfiniFrameExample.BlazorWebView.MultiWindowSample for complete example.

## Custom Window Chrome

Combine with `InfiniLore.InfiniFrame.Blazor` for custom title bar:

```csharp
var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetChromeless(true)
    .SetTransparent(true)
    .SetSize(1280, 720)
    .Center()
);
```

Then use Blazor components:
- `InfiniFrameWindowDragArea` — draggable title bar
- `InfiniFrameWindowButton` — minimize/maximize/close buttons
- `InfiniFrameWindowResizeThumb` — resize handles
- `InfiniFrameWindowResizeThumbContainer` — all resize thumbs

## Common Patterns

### Minimal Blazor App

```csharp
using InfiniFrame.BlazorWebView;

var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetTitle("My Blazor App")
    .SetSize(1280, 720)
    .Center()
);

builder.RootComponents.Add<App>("#app");
builder.Build().Run();
```

### App with Services

```csharp
var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetTitle("My App")
    .SetDevToolsEnabled(true)
);

builder.Services.AddSingleton<IApiClient, ApiClient>();
builder.Services.AddScoped<IAppState, AppState>();
builder.Services.AddLogging();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Build().Run();
```

### Chromeless with Custom Chrome

```csharp
var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetChromeless(true)
    .SetTransparent(true)
);

builder.Services.AddInfiniFrameChromeComponents();
builder.RootComponents.Add<MainLayout>("#app");
builder.Build().Run();
```

## Anti-Patterns

❌ **Forget autostart="false"**:
```html
<!-- WRONG — Blazor will start twice -->
<script src="_framework/blazor.webview.js"></script>
```

✅ **Always disable autostart**:
```html
<script src="_framework/blazor.webview.js" autostart="false"></script>
```

❌ **Call window ops directly from background thread**:
```csharp
// WRONG — thread affinity violation
Task.Run(() => Window.Close());
```

✅ **Use Invoke**:
```csharp
Task.Run(() => Window.Invoke(() => Window.Close()));
```

❌ **Use standard WebAssembly hosting model**:
```csharp
// WRONG — InfiniFrame uses in-process Blazor, not WASM hosting
builder.RootComponents.AddForJavaScript<App>("#app");
```

✅ **Use direct component registration**:
```csharp
builder.RootComponents.Add<App>("#app");
```
