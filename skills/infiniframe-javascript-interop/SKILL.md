---
name: infiniframe-javascript-interop
description: C# to JavaScript communication using InfiniLore.InfiniFrame.Js and web messaging. Versioned envelope, pointer capture, and built-in message handlers.
---
# InfiniFrame JavaScript Interop

> Skill for C#↔JavaScript communication using `InfiniLore.InfiniFrame.Js` and web messaging.

## When to Use This Skill

- Sending messages between C# and JavaScript
- Implementing custom window management from JS
- Pointer capture for drag operations
- Building custom interop components
- Exchanging structured data between layers

## Packages

### Core Messaging (included in all packages)
Web messaging is available in all InfiniFrame integrations — no additional package needed.

### InfiniFrame.Js (Blazor-specific)
```bash
dotnet add package InfiniLore.InfiniFrame.Js
```

Automatically included by `InfiniLore.InfiniFrame.BlazorWebView`.

## Web Messaging Protocol

### Message Envelope (MANDATORY)

All JavaScript→C# messages MUST use this JSON format:

```json
{
  "id": "<string>",
  "data": <any>,
  "version": 1,
  "channel": "<optional-string>"
}
```

**Required fields**: `id`, `version` (must be `1`)
**Optional fields**: `data`, `channel`

**Legacy format out of support**: `id;payload` is NOT supported.

## Sending Messages

### C# → JavaScript

```csharp
// Sync
window.SendWebMessage("hello from C#");

// Async
await window.SendWebMessageAsync("async hello");

// Structured data
window.SendWebMessage(JsonSerializer.Serialize(new {
    id = "update",
    data = new { count = 42 },
    version = 1
}));
```

JavaScript receiver:

```js
window.infiniframe.host.receiveMessage(function(message) {
    console.log("Received from C#:", message);
    
    // Parse if it's structured data
    const envelope = JSON.parse(message);
    if (envelope.id === "update") {
        updateUI(envelope.data.count);
    }
});
```

### JavaScript → C#

JavaScript (MUST use envelope):

```js
// Simple message
window.infiniframe.host.postMessage({ 
    id: "action", 
    data: 42, 
    version: 1 
});

// Structured data
window.infiniframe.host.postMessage({
    id: "log",
    data: { message: "hello from JS", level: "info" },
    version: 1
});
```

C# raw handler:

```csharp
builder.Events.WebMessageReceived.Add(raw => {
    var msg = JsonSerializer.Deserialize<MyMessage>(raw);
    Console.WriteLine($"From JS: {msg}");
});
```

C# named handler:

```csharp
builder.MessageHandlers.RegisterMessageHandler("action", (window, payload) => {
    Console.WriteLine($"Action triggered with: {payload}");
});

builder.MessageHandlers.RegisterMessageHandler("log", (window, payload) => {
    using var doc = JsonDocument.Parse(payload!);
    string? message = doc.RootElement.GetProperty("message").GetString();
    string? level = doc.RootElement.GetProperty("level").GetString();
    Console.WriteLine($"[{level}] {message}");
});
```

## Built-in JavaScript Message Handlers

`InfiniFrame.Js` registers handlers for window management from JavaScript.

### Including the Script

```html
<script src="_content/InfiniLore.InfiniFrame.Js/InfiniFrame.js"></script>
```

### Available Handlers

| Handler ID | What it does |
|------------|--------------|
| `__infiniframe:window:minimize` | Minimize window |
| `__infiniframe:window:maximize` | Maximize/restore window |
| `__infiniframe:window:close` | Close window |
| `__infiniframe:fullscreen:enter` | Enter fullscreen |
| `__infiniframe:fullscreen:exit` | Exit fullscreen |
| `__infiniframe:title:change` | Update native window title |
| `__infiniframe:open:external` | Open links in default browser |

### Using from Custom JavaScript

```js
// Window actions
window.infiniframe.host.postMessage({ 
    id: "__infiniframe:window:minimize", 
    data: null, 
    version: 1 
});

window.infiniframe.host.postMessage({ 
    id: "__infiniframe:window:close", 
    data: null, 
    version: 1 
});

// Title change (data = new title string)
window.infiniframe.host.postMessage({ 
    id: "__infiniframe:title:change", 
    data: "New Window Title", 
    version: 1 
});

// Fullscreen
window.infiniframe.host.postMessage({ 
    id: "__infiniframe:fullscreen:enter", 
    data: null, 
    version: 1 
});
```

### Using InfiniFrame.js API

If `InfiniFrame.js` script is included:

```js
window.infiniFrame.HostMessaging.sendMessageToHost("__infiniframe:window:minimize");
window.infiniFrame.HostMessaging.sendMessageToHost("__infiniframe:title:change", "New Title");
```

These are used internally by `InfiniFrameWindowDragArea`, `InfiniFrameWindowButton`, and related components.

## IInfiniFrameJs (Blazor-Specific)

### DI Registration

Auto-registered when using `BlazorWebView`. For core package only:

```csharp
services.AddInfiniFrameJs();
```

### API

```csharp
public interface IInfiniFrameJs {
    Task SetPointerCaptureAsync(ElementReference element, long pointerId, CancellationToken ct);
    Task ReleasePointerCaptureAsync(ElementReference element, long pointerId, CancellationToken ct);
}
```

Wraps browser's `element.setPointerCapture()` / `element.releasePointerCapture()` for reliable drag interactions.

### Usage in Blazor Components

```razor
@inject IInfiniFrameJs InfiniJs

<div @ref="handle"
     @onpointerdown="StartDrag"
     @onpointermove="OnDrag"
     @onpointerup="EndDrag">
    Drag me
</div>

@code {
    ElementReference handle;
    bool isDragging;

    async Task StartDrag(PointerEventArgs e) {
        await InfiniJs.SetPointerCaptureAsync(handle, e.PointerId, default);
        isDragging = true;
    }

    async Task OnDrag(PointerEventArgs e) {
        if (!isDragging) return;
        // Handle drag
    }

    async Task EndDrag(PointerEventArgs e) {
        await InfiniJs.ReleasePointerCaptureAsync(handle, e.PointerId, default);
        isDragging = false;
    }
}
```

**Why pointer capture is needed**: Keeps events flowing to element even after pointer leaves it — necessary for stable drag operations.

## Exchanging Structured Data

### C# → JS Pattern

C# sender:
```csharp
window.SendWebMessage(JsonSerializer.Serialize(new {
    id = "user:update",
    data = new { 
        name = "John", 
        email = "john@example.com",
        roles = new[] { "admin", "user" }
    },
    version = 1
}));
```

JavaScript receiver:
```js
window.infiniframe.host.receiveMessage(function(raw) {
    const envelope = JSON.parse(raw);
    
    if (envelope.id === "user:update") {
        const { name, email, roles } = envelope.data;
        updateUserDisplay(name, email, roles);
    }
});
```

### JS → C# Pattern

JavaScript sender:
```js
window.infiniframe.host.postMessage({
    id: "form:submit",
    data: {
        username: "john",
        action: "login"
    },
    version: 1
});
```

C# receiver (named handler):
```csharp
builder.MessageHandlers.RegisterMessageHandler("form:submit", (window, payload) => {
    using var doc = JsonDocument.Parse(payload!);
    string? username = doc.RootElement.GetProperty("username").GetString();
    string? action = doc.RootElement.GetProperty("action").GetString();
    
    // Handle form submission
    HandleLogin(username, action);
});
```

C# receiver (raw handler):
```csharp
builder.Events.WebMessageReceived.Add(raw => {
    using var doc = JsonDocument.Parse(raw);
    string? id = doc.RootElement.GetProperty("id").GetString();
    
    if (id == "form:submit") {
        var data = doc.RootElement.GetProperty("data");
        // Process data
    }
});
```

## Common Patterns

### Request-Response Pattern

C# registers handler:
```csharp
builder.MessageHandlers.RegisterMessageHandler("app:ping", (window, _) => {
    window.SendWebMessage(JsonSerializer.Serialize(new {
        id = "app:pong",
        data = new { timestamp = DateTime.UtcNow },
        version = 1
    }));
});
```

JavaScript sends and receives:
```js
window.infiniframe.host.receiveMessage(function(raw) {
    const envelope = JSON.parse(raw);
    if (envelope.id === "app:pong") {
        console.log("Pong received at:", envelope.data.timestamp);
    }
});

window.infiniframe.host.postMessage({
    id: "app:ping",
    data: null,
    version: 1
});
```

### Event Streaming Pattern

C# pushes updates:
```csharp
async Task StreamUpdates(IInfiniFrameWindow window, CancellationToken ct) {
    while (!ct.IsCancellationRequested) {
        var data = await FetchUpdateAsync();
        window.SendWebMessage(JsonSerializer.Serialize(new {
            id = "update:data",
            data = data,
            version = 1
        }));
        await Task.Delay(1000, ct);
    }
}
```

JavaScript consumes:
```js
window.infiniframe.host.receiveMessage(function(raw) {
    const envelope = JSON.parse(raw);
    if (envelope.id === "update:data") {
        renderChart(envelope.data);
    }
});
```

## Anti-Patterns

❌ **Send raw string without envelope**:
```js
// WRONG — C# won't parse it correctly
window.infiniframe.host.postMessage("hello");
```

✅ **Always use envelope**:
```js
window.infiniframe.host.postMessage({ id: "greeting", data: "hello", version: 1 });
```

❌ **Use wrong version**:
```js
// WRONG — version must be 1
window.infiniframe.host.postMessage({ id: "action", data: null, version: 2 });
```

✅ **Use version 1**:
```js
window.infiniframe.host.postMessage({ id: "action", data: null, version: 1 });
```

❌ **Forget pointer capture for drags**:
```razor
// WRONG — drag breaks when cursor leaves element
<div @onpointerdown="StartDrag" @onpointermove="OnDrag">
```

✅ **Use pointer capture**:
```razor
async Task StartDrag(PointerEventArgs e) {
    await InfiniJs.SetPointerCaptureAsync(handle, e.PointerId, default);
}
```

## Examples

- https://github.com/InfiniLore/InfiniFrame/tree/master/examples/InfiniFrameExample.WebApp.Vue — Registers all built-in message handlers
- https://github.com/InfiniLore/InfiniFrame/tree/master/examples/InfiniFrameExample.WebApp.React — Custom scheme handler returning dynamic JS, two-way messaging
