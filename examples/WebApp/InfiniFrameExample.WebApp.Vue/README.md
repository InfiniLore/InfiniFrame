# Example: WebApp — Vue

Demonstrates a Vue.js frontend served by ASP.NET Core inside an InfiniFrame window, showcasing all built-in JavaScript message handlers from `InfiniFrame.Js`

## What it shows

- `RegisterFullScreenWebMessageHandler()` — toggles native fullscreen from JavaScript
- `RegisterOpenExternalTargetWebMessageHandler()` — opens `target="_blank"` links in the system browser
- `RegisterTitleChangedWebMessageHandler()` — updates the native window title from JavaScript
- `RegisterWindowManagementWebMessageHandler()` — minimize, maximize, close from JavaScript
- `RegisterWebMessageReceivedHandler(...)` — generic message handler for custom app messages
- `SetBrowserControlInitParameters("--remote-debugging-port=9222")` — enables Chrome DevTools remote debugging

## Run

> Requires the Vue frontend to be built first — see `Source/InfiniFrame.Vue/` for the npm project

```bash
# Build the frontend
cd examples/InfiniFrameExample.WebApp.Vue/Source/InfiniFrame.Vue
npm install && npm run vue.build

# Run the app
dotnet run --project examples/InfiniFrameExample.WebApp.Vue
```

## Key code

```csharp
builder.Window
    .SetTitle("InfiniLore InfiniFrame.NET VUE Sample")
    .SetSize(new Size(800, 600))
    .SetBrowserControlInitParameters("--remote-debugging-port=9222")
    .RegisterFullScreenWebMessageHandler()
    .RegisterOpenExternalTargetWebMessageHandler()
    .RegisterTitleChangedWebMessageHandler()
    .RegisterWindowManagementWebMessageHandler()
    .RegisterWebMessageReceivedHandler((_, message) => {
        // handle custom messages from Vue
    });
```

## Packages used

- `InfiniLore.InfiniFrame.WebServer`
- `InfiniLore.InfiniFrame`
- `InfiniLore.InfiniFrame.Js`

## Related documentation

- [Web Server Guide](../../docs/docs/guides/web-server.md)
- [JavaScript Interop Guide](../../docs/docs/guides/javascript-interop.md)
- [API Reference](../../docs/docs/api.md)
