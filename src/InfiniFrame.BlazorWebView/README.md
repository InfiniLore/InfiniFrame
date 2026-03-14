# InfiniLore.InfiniFrame.BlazorWebView

Hosts a full Blazor application inside a native InfiniFrame window — no localhost server, no HTTP port, the Blazor runtime runs entirely in-process

## What it does

- Registers an in-process custom scheme handler for the Blazor app
- Manages the Blazor root component lifecycle
- Integrates with .NET DI — `IInfiniFrameWindow` and `IInfiniFrameJs` are injectable from any component
- Provides an `HttpClient` scoped to the internal app origin for static asset requests

## Installation

```bash
dotnet add package InfiniLore.InfiniFrame.BlazorWebView
```

## Basic Usage

```csharp
using InfiniFrame.BlazorWebView;

var builder = InfiniFrameBlazorAppBuilder.CreateDefault(args, w => w
    .SetTitle("My Blazor App")
    .SetSize(1280, 720)
    .Center()
);

builder.Services.AddSingleton<MyService>();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Build().Run();
```

## Key Types

| Type | Description |
|------|-------------|
| `InfiniFrameBlazorAppBuilder` | Entry point — call `CreateDefault()` to start |
| `InfiniFrameBlazorApp` | Built application — call `Run()` to open the window and block |
| `RootComponentList` | Maps Blazor components to DOM element selectors |

## Links

- [Full Documentation](../../docs/Guides/Blazor.md)
- [NuGet](https://www.nuget.org/packages/InfiniLore.InfiniFrame.BlazorWebView)
