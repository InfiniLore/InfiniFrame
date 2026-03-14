# JavaScript Interop Guide

InfiniFrame provides two layers of JS interop:

## Contents

- [Web Messaging](#web-messaging)
- [InfiniFrame.Js](#infiniframejs)
- [Built-in JavaScript Message Handlers](#built-in-javascript-message-handlers)
- [Exchanging Structured Data](#exchanging-structured-data)

1. **Web messaging** — a simple string-based channel between C# and the page's JavaScript
2. **InfiniFrame.Js** — Blazor-specific utilities for pointer capture and built-in window management message handlers

## Web Messaging

The messaging channel works the same way regardless of whether you are using plain HTML, a Blazor app, or an ASP.NET Core server

### Sending from C# to JavaScript

```csharp
window.SendWebMessage("hello from C#");
await window.SendWebMessageAsync("async hello");
```

In the browser:

```js
window.external.receiveMessage(function(message) {
    console.log("Received from C#:", message);
});
```

### Sending from JavaScript to C#

```js
window.external.sendMessage(JSON.stringify({ type: "action", payload: 42 }));
```

In C#:

```csharp
builder.Events.WebMessageReceived.Add(raw => {
    var msg = JsonSerializer.Deserialize<MyMessage>(raw);
    // handle msg
});
```

### Named message handlers

Instead of parsing all messages in one handler, register named handlers through `IInfiniFrameWindowMessageHandlers`:

```csharp
// At build time, via the builder
var window = InfiniFrameWindowBuilder.Create()
    ...
    .Build();

window.MessageHandlers.Register("ping", _ => {
    window.SendWebMessage("pong");
});

window.MessageHandlers.Register("set-title", title => {
    // handle title change
});
```

```js
// JavaScript convention — send an object with a `type` field
window.external.sendMessage(JSON.stringify({ type: "ping" }));
window.external.sendMessage(JSON.stringify({ type: "set-title", value: "New Title" }));
```

> The built-in message handler routing in `InfiniFrame.Js` follows this `{ type, ... }` convention for its own handlers

## InfiniFrame.Js

`InfiniLore.InfiniFrame.Js` provides Blazor-specific interop and registers built-in message handlers for window management from JavaScript

### Installation

```bash
dotnet add package InfiniLore.InfiniFrame.Js
```

This package is automatically included by `InfiniLore.InfiniFrame.BlazorWebView`

### DI Registration

When using the core package directly (not BlazorWebView), register the service manually:

```csharp
services.AddInfiniFrameJs();
```

### IInfiniFrameJs

`IInfiniFrameJs` exposes pointer capture methods for Blazor components:

```csharp
public interface IInfiniFrameJs {
    Task SetPointerCaptureAsync(ElementReference element, long pointerId, CancellationToken ct);
    Task ReleasePointerCaptureAsync(ElementReference element, long pointerId, CancellationToken ct);
}
```

These wrap the browser's `element.setPointerCapture(pointerId)` / `element.releasePointerCapture(pointerId)` APIs, which are necessary for reliable drag interactions — the pointer capture keeps events flowing to the element even after the pointer leaves it

```razor
@inject IInfiniFrameJs InfiniJs

<div @ref="handle"
     @onpointerdown="StartDrag"
     @onpointerup="EndDrag">
    Drag me
</div>

@code {
    ElementReference handle;

    async Task StartDrag(PointerEventArgs e) {
        await InfiniJs.SetPointerCaptureAsync(handle, e.PointerId, default);
    }

    async Task EndDrag(PointerEventArgs e) {
        await InfiniJs.ReleasePointerCaptureAsync(handle, e.PointerId, default);
    }
}
```

## Built-in JavaScript Message Handlers

`InfiniFrame.Js` registers several message handlers that the client-side `InfiniFrame.js` script uses to control the native window from JavaScript

### Including the script

```html
<script src="_content/InfiniLore.InfiniFrame.Js/InfiniFrame.js"></script>
```

### Available handlers

| Handler name | Triggered by | What it does |
|-------------|--------------|--------------|
| `window_management` | `InfiniFrame.js` | Minimize, maximize, or close the window |
| `fullscreen` | `InfiniFrame.js` | Toggle fullscreen mode |
| `title_changed` | `InfiniFrame.js` | Update the native window title |
| `open_external_target` | `InfiniFrame.js` | Open links with `target="_blank"` in the default browser |

These are used internally by `InfiniFrameWindowDragArea`, `InfiniFrameWindowButton`, and related components — you do not need to call them manually unless you are building custom components

### Sending a window management message from custom JavaScript

```js
window.external.sendMessage(JSON.stringify({
    type: "window_management",
    action: "minimize"  // "minimize" | "maximize" | "close"
}));
```

```js
window.external.sendMessage(JSON.stringify({
    type: "title_changed",
    title: "New Window Title"
}));
```

```js
window.external.sendMessage(JSON.stringify({
    type: "fullscreen",
    enabled: true
}));
```

## Exchanging Structured Data

The message channel is string-only, so use JSON for structured communication:

**C# → JS:**

```csharp
var payload = JsonSerializer.Serialize(new { type = "update", count = 42 });
window.SendWebMessage(payload);
```

```js
window.external.receiveMessage(function(raw) {
    const msg = JSON.parse(raw);
    if (msg.type === "update") {
        updateUI(msg.count);
    }
});
```

**JS → C#:**

```js
window.external.sendMessage(JSON.stringify({ type: "log", message: "hello" }));
```

```csharp
builder.Events.WebMessageReceived.Add(raw => {
    using var doc = JsonDocument.Parse(raw);
    var type = doc.RootElement.GetProperty("type").GetString();
    // route by type
});
```

## Examples

- [InfiniFrameExample.WebApp.Vue](../../examples/InfiniFrameExample.WebApp.Vue/) — registers all four built-in message handlers (`fullscreen`, `open_external_target`, `title_changed`, `window_management`)
- [InfiniFrameExample.WebApp.React](../../examples/InfiniFrameExample.WebApp.React/) — custom scheme handler returning dynamic JavaScript, and a two-way messaging round-trip
