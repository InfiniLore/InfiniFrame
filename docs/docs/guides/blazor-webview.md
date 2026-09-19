# Blazor WebView Guide

`InfiniLore.InfiniFrame.BlazorWebView` hosts a Blazor application in an InfiniFrame native window.

## Setup

```csharp
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;

var builder = InfiniFrameApplication.CreateBuilder(args)
    .WithWindow(window => window
        .SetTitle("My Blazor App")
        .SetSize(1280, 720));

builder.UseBlazorWebView(configuration => {
    configuration.RootComponents.Add<App>("app");
});

await using InfiniFrameApplication app = builder.Build();
await app.RunAsync();
```

Configure window features before registering the integration with `WithWindow` or `WithWindow(id, ...)`.
`ConfigureWindow` remains available for integration-specific configuration.

```csharp
builder.WithWindow(window => window.SetChromeless(true));
builder.UseBlazorWebView(configuration => {
    configuration.ConfigureWindow(window => window.SetDevToolsEnabled(true));
    configuration.RootComponents.Add<App>("app");
});
```

The current manager supports one target window per Blazor WebView integration. Use separate application integrations when independent window managers are required.

## Services And Components

Register application services on `builder.Services` and root components on the integration configuration:

```csharp
builder.Services.AddSingleton<MyDataService>();
builder.UseBlazorWebView(configuration => {
    configuration.RootComponents.Add<App>("app");
    configuration.RootComponents.Add<HeadOutlet>("head::after");
});
```

`IInfiniFrameWindow`, `IInfiniFrameJs`, `HttpClient`, and the Blazor dispatcher are registered by the integration where applicable.

## File Providers And Security

The integration serves static assets from the application `wwwroot` and generated static-web-assets locations. Use the configuration APIs for custom file providers and explicitly trust external origins when needed. Keep WebSecurity enabled unless disabling it is an intentional development-only decision.

## Lifecycle

`Run` or `RunAsync` starts the native application and the Blazor WebView. Closing the application disposes the WebView manager, message pump, windows, and registered services.
