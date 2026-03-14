# Example: BlazorWebView

Demonstrates the minimal setup for hosting a Blazor application inside a native InfiniFrame window using `InfiniLore.InfiniFrame.BlazorWebView`

## What it shows

- `InfiniFrameBlazorAppBuilder.CreateDefault()` entry point
- Registering a root Blazor component (`<App>`) mapped to the `#app` selector
- Configuring the window: size, position, and icon file
- Integrating Serilog for structured logging via `Microsoft.Extensions.Logging`
- Standard Blazor pages: Counter, FetchData, Index

## Run

```bash
dotnet run --project examples/InfiniFrameExample.BlazorWebView
```

## Key code

```csharp
var appBuilder = InfiniFrameBlazorAppBuilder.CreateDefault(args);

appBuilder.RootComponents.Add<App>("app");

appBuilder.WithInfiniFrameWindowBuilder(builder => builder
    .SetIconFile("favicon.ico")
    .SetLocation(new Point(100, 100))
    .SetSize(new Size(800, 600))
);

appBuilder.Build().Run();
```

## Packages used

- `InfiniLore.InfiniFrame.BlazorWebView`
- `InfiniLore.InfiniFrame.Blazor`

## Related documentation

- [Blazor WebView Guide](../../docs/Guides/Blazor.md)
- [Builder API Reference](../../docs/Reference/BuilderApi.md)
