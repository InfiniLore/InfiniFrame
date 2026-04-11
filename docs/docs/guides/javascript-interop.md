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
Messages are validated against a versioned envelope contract:

```json
{ "id": "<string>", "data": <any>, "version": 1 }
```

`id` and `version` are required. `version` must be `1`. Non-envelope messages are rejected.

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

window.MessageHandlers.RegisterMessageHandler("ping", (window, _) => {
    window.SendWebMessage("pong");
});

window.MessageHandlers.RegisterMessageHandler("set-title", (window, title) => {
    // handle title change — title is the parsed envelope data
});
```

```js
// Messages use a JSON envelope: { id, data, version }
window.external.sendMessage(JSON.stringify({ id: "ping", data: null, version: 1 }));
window.external.sendMessage(JSON.stringify({ id: "set-title", data: "New Title", version: 1 }));
```

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

| Handler ID                       | Triggered by     | What it does                                             |
|----------------------------------|------------------|----------------------------------------------------------|
| `__infiniframe:window:minimize`  | `InfiniFrame.js` | Minimize the window                                      |
| `__infiniframe:window:maximize`  | `InfiniFrame.js` | Maximize the window                                      |
| `__infiniframe:window:close`     | `InfiniFrame.js` | Close the window                                         |
| `__infiniframe:fullscreen:enter` | `InfiniFrame.js` | Enter fullscreen                                         |
| `__infiniframe:fullscreen:exit`  | `InfiniFrame.js` | Exit fullscreen                                          |
| `__infiniframe:title:change`     | `InfiniFrame.js` | Update the native window title                           |
| `__infiniframe:open:external`    | `InfiniFrame.js` | Open links with `target="_blank"` in the default browser |

These are used internally by `InfiniFrameWindowDragArea`, `InfiniFrameWindowButton`, and related components — you do not need to call them manually unless you are building custom components

### Sending a window management message from custom JavaScript

All messages follow a versioned JSON envelope:

```js
window.external.sendMessage(JSON.stringify({ id: "__infiniframe:window:minimize", data: null, version: 1 }));
window.external.sendMessage(JSON.stringify({ id: "__infiniframe:window:maximize", data: null, version: 1 }));
window.external.sendMessage(JSON.stringify({ id: "__infiniframe:window:close", data: null, version: 1 }));
```

```js
// Title data is the new title string
window.external.sendMessage(JSON.stringify({ id: "__infiniframe:title:change", data: "New Window Title", version: 1 }));
```

```js
window.external.sendMessage(JSON.stringify({ id: "__infiniframe:fullscreen:enter", data: null, version: 1 }));
window.external.sendMessage(JSON.stringify({ id: "__infiniframe:fullscreen:exit", data: null, version: 1 }));
```

When using `InfiniFrame.js`, you can go through its API instead:

```js
window.infiniFrame.HostMessaging.sendMessageToHost("__infiniframe:window:minimize");
window.infiniFrame.HostMessaging.sendMessageToHost("__infiniframe:title:change", "New Title");
```

## Exchanging Structured Data

The message channel uses a JSON envelope, so structured data can be placed directly in `data`:

**C# → JS:**

```csharp
window.SendWebMessage(JsonSerializer.Serialize(new {
    id = "update",
    data = new { count = 42 },
    version = 1
}));
```

```js
window.external.receiveMessage(function(raw) {
    const envelope = JSON.parse(raw);
    if (envelope.id === "update") {
        updateUI(envelope.data.count);
    }
});
```

**JS → C#:**

```js
window.external.sendMessage(JSON.stringify({
    id: "log",
    data: { message: "hello" },
    version: 1
}));
```

```csharp
window.MessageHandlers.RegisterMessageHandler("log", (_, payload) => {
    using var doc = JsonDocument.Parse(payload!);
    string? message = doc.RootElement.GetProperty("message").GetString();
    Console.WriteLine(message);
});
```

## Examples

- `InfiniFrameExample.WebApp.Vue` (`examples/InfiniFrameExample.WebApp.Vue`) - registers all built-in message handlers for window management, fullscreen, title change, and external links
- `InfiniFrameExample.WebApp.React` (`examples/InfiniFrameExample.WebApp.React`) - custom scheme handler returning dynamic JavaScript, and a two-way messaging round-trip
