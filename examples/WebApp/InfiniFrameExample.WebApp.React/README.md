# Example: WebApp — React

Demonstrates a React frontend served by ASP.NET Core inside an InfiniFrame window, with a custom URL scheme handler and a two-way web messaging channel

## What it shows

- `RegisterCustomSchemeHandler("app", ...)` — intercepts `app://` requests and returns dynamically generated JavaScript
- `RegisterWebMessageReceivedHandler(...)` — receives messages from JavaScript, increments a counter, and echoes a response back via `SendWebMessage`
- A singleton `WebMessageCounter` service accessed inside the message handler via DI
- `UseAutoServerClose()` — server stops when the window is closed

## Run

> Requires the React frontend to be built first — see `Source/` for the npm project

```bash
dotnet run --project examples/InfiniFrameExample.WebApp.React
```

## Key code

```csharp
builder.Window
    .SetTitle("InfiniLore InfiniFrame.NET REACT Sample")
    .SetSize(new Size(800, 600))
    .Center()
    .RegisterCustomSchemeHandler("app", (_, _, _, out contentType) => {
        contentType = "text/javascript";
        return new MemoryStream("(() => { alert('Dynamic JS'); })();"u8.ToArray());
    })
    .RegisterWebMessageReceivedHandler((WebMessageCounter counter, IInfiniFrameWindow window, string message) => {
        window.SendWebMessage($"[{counter.Increment()}] Received: \"{message}\"");
    });
```

## Packages used

- `InfiniLore.InfiniFrame.WebServer`
- `InfiniLore.InfiniFrame`

## Related documentation

- [Web Server Guide](../../docs/docs/guides/web-server.md)
- [Core Window Guide — Custom URL Schemes](../../docs/docs/guides/core-window.md#custom-url-schemes)
- [JavaScript Interop Guide](../../docs/docs/guides/javascript-interop.md)
