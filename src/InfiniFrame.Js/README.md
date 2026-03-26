# InfiniLore.InfiniFrame.Js

JavaScript and Blazor interop utilities for InfiniFrame — pointer capture helpers and built-in window management message handlers

## What it does

- Provides `IInfiniFrameJs` for calling `setPointerCapture` / `releasePointerCapture` from Blazor components
- Registers built-in message handlers for window management operations (minimize, maximize, close, fullscreen, title updates) that can be triggered from JavaScript via `window.external.sendMessage`
- Includes `InfiniFrame.js` — a client-side script that wires up the browser side of these operations

## Installation

```bash
dotnet add package InfiniLore.InfiniFrame.Js
```

This package is automatically included as a dependency of `InfiniLore.InfiniFrame.BlazorWebView` — install it explicitly only when using the core `InfiniLore.InfiniFrame` package directly

## DI Registration

```csharp
services.AddInfiniFrameJs();
```

## IInfiniFrameJs

```csharp
@inject IInfiniFrameJs InfiniJs

<div @ref="handle" @onpointerdown="StartDrag">...</div>

@code {
    ElementReference handle;

    async Task StartDrag(PointerEventArgs e) {
        await InfiniJs.SetPointerCaptureAsync(handle, e.PointerId, default);
    }
}
```

## Client Script

Include in your `index.html` to enable the built-in JS-side window management:

```html
<script src="_content/InfiniLore.InfiniFrame.Js/InfiniFrame.js"></script>
```

## Built-in Message Handlers

| Handler | Triggered by | Action |
|---------|-------------|--------|
| `window_management` | `InfiniFrame.js` | Minimize, maximize, or close the window |
| `fullscreen` | `InfiniFrame.js` | Toggle fullscreen mode |
| `title_changed` | `InfiniFrame.js` | Change the native window title |
| `open_external_target` | `InfiniFrame.js` | Open `target="_blank"` links in the default browser |

## Links

- [Full Documentation](../../docs/Guides/JsInterop.md)
- [NuGet](https://www.nuget.org/packages/InfiniLore.InfiniFrame.Js)
