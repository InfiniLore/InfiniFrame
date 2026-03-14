# InfiniLore.InfiniFrame

**The core package of InfiniFrame**. Creates and manages native OS windows backed by an embedded browser control

## What it does

- Opens a native window on Windows, Linux, or macOS
- Embeds a browser engine (WebView2, WebKit2GTK, or WKWebView) to render web content
- Provides a fluent builder API to configure everything before the window opens
- Exposes runtime window control: size, position, state, dialogs, file pickers, and web messaging

## Installation

```bash
dotnet add package InfiniLore.InfiniFrame
```

## Basic Usage

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

## Key Types

| Type | Description |
|------|-------------|
| `InfiniFrameWindowBuilder` | Entry point — call `Create()` to start |
| `IInfiniFrameWindow` | Runtime window interface returned by `Build()` |
| `IInfiniFrameWindowBuilder` | Fluent configuration interface |
| `IInfiniFrameWindowEvents` | Subscribe to window and browser events |

## Links

- [Full Documentation](../../docs/Guides/CoreWindow.md)
- [Builder API Reference](../../docs/Reference/BuilderApi.md)
- [Window API Reference](../../docs/Reference/WindowApi.md)
- [Events Reference](../../docs/Reference/Events.md)
- [NuGet](https://www.nuget.org/packages/InfiniLore.InfiniFrame)
