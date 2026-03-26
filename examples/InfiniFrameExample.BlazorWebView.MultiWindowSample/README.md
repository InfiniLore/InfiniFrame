# Example: BlazorWebView — Multi-Window

Demonstrates how to open multiple independent native windows, each hosting a different Blazor component and HTML page

## What it shows

- Creating multiple windows sequentially from a single `InfiniFrameBlazorAppBuilder`
- Each window has its own root Blazor component (`Window1`, `Window2`) and its own `index.html`
- Opening the next window from the `WindowCreated` event of the previous one
- Coordinating close: when any window closes, all others are closed too
- Per-window HTML files (`window1.html`, `window2.html`) and CSS

## Run

```bash
dotnet run --project examples/InfiniFrameExample.BlazorWebView.MultiWindowSample
```

## Key code

```csharp
// Queue of windows to create
CreateWindows(appBuilder, new Queue<WindowCreationArgs>(new[] {
    new WindowCreationArgs(typeof(Window1), "Window 1", new Uri("window1.html", UriKind.Relative)),
    new WindowCreationArgs(typeof(Window2), "Window 2", new Uri("window2.html", UriKind.Relative))
}));

// Each window triggers the next via WindowCreated
InfiniFrameWindowBuilder.Create()
    .SetTitle(args.Title)
    .SetStartUrl(args.HtmlPath)
    .RegisterWindowCreatedHandler(_ => Task.Run(() => CreateWindows(appBuilder, windowsToCreate)))
    .RegisterWindowClosingHandler((_, _) => { CloseAllWindows(); return false; })
    .Build();
```

## Packages used

- `InfiniLore.InfiniFrame.BlazorWebView`
- `InfiniLore.InfiniFrame.Blazor`

## Related documentation

- [Blazor WebView Guide](../../docs/Guides/Blazor.md)
- [Events Reference](../../docs/Reference/Events.md)
