# Events Reference

InfiniFrame uses a custom ordered event system — `InfiniFrameOrderedEvent<T>` — instead of standard C# events
This ensures handlers execute in the order they were registered and provides deterministic behavior for operations like cancellation

Events are accessed through `IInfiniFrameWindowEvents`, available on `IInfiniFrameWindowBuilder.Events`

## Contents

- [Registering Handlers](#registering-handlers)
- [Window Lifecycle Events](#window-lifecycle-events)
- [Window State Events](#window-state-events)
- [Geometry Events](#geometry-events)
- [Web Messaging Events](#web-messaging-events)
- [InfiniFrameOrderedEvent](#infiniframeorderedevent)
- [InfiniFrameOrderedClosingEvent](#infiniframeorderedclosingevent)
- [Event Handler Shortcuts on the Builder](#event-handler-shortcuts-on-the-builder)

## Registering Handlers

All events expose an `Add(handler)` method:

```csharp
var builder = InfiniFrameWindowBuilder.Create();

builder.Events.WindowCreated.Add(() => {
    Console.WriteLine("Window is ready");
});

var window = builder.Build();
```

Handlers are fired in registration order

## Window Lifecycle Events

### `WindowCreating`
`InfiniFrameOrderedEvent`

Fires just before the native window is created
Use this to perform last-minute setup before the window becomes visible

```csharp
builder.Events.WindowCreating.Add(() => {
    Console.WriteLine("Window about to be created");
});
```

### `WindowCreated`
`InfiniFrameOrderedEvent`

Fires after the native window has been created and is visible on screen

```csharp
builder.Events.WindowCreated.Add(() => {
    Console.WriteLine("Window is open");
});
```

### `WindowClosingRequested`
`InfiniFrameOrderedEvent` → handlers return `bool`

Fires when the user requests to close the window (e.g. clicking the X button)
Return `true` from any handler to **cancel** the close request — the window stays open
Return `false` (or don't return a value) to allow closing to proceed

```csharp
builder.Events.WindowClosingRequested.Add(() => {
    // Ask the user
    var result = window.ShowMessage(
        "Confirm",
        "Are you sure you want to quit?",
        InfiniFrameDialogButtons.YesNo,
        InfiniFrameDialogIcon.Question
    );
    return result != InfiniFrameDialogResult.Yes; // true = cancel
});
```

### `WindowClosing`
`InfiniFrameOrderedClosingEvent` → handlers receive `(IInfiniFrameWindow window, bool cancel)`

Fires after `WindowClosingRequested` completes without cancellation — the window **will** close
Use this for final cleanup (saving state, releasing resources, etc.)

```csharp
builder.Events.WindowClosing.Add((window, cancel) => {
    SaveAppState();
    return false;
});
```

The `cancel` parameter reflects whether any `WindowClosingRequested` handler attempted a cancellation (even if it was overridden); return value here does not cancel the close

## Window State Events

### `WindowMaximized`
`InfiniFrameOrderedEvent`

Fires when the window enters a maximized state

```csharp
builder.Events.WindowMaximized.Add(() => Console.WriteLine("Maximized"));
```

### `WindowMinimized`
`InfiniFrameOrderedEvent`

Fires when the window is minimized to the taskbar

### `WindowRestored`
`InfiniFrameOrderedEvent`

Fires when the window is restored from maximized or minimized state

### `WindowFocusIn`
`InfiniFrameOrderedEvent`

Fires when the window gains keyboard focus

### `WindowFocusOut`
`InfiniFrameOrderedEvent`

Fires when the window loses keyboard focus

## Geometry Events

### `WindowSizeChanged`
`InfiniFrameOrderedEvent<Size>`

Fires whenever the window is resized — the new size is passed to each handler

```csharp
builder.Events.WindowSizeChanged.Add(size => {
    Console.WriteLine($"New size: {size.Width}×{size.Height}");
});
```

### `WindowLocationChanged`
`InfiniFrameOrderedEvent<Point>`

Fires whenever the window is moved — the new screen position is passed to each handler

```csharp
builder.Events.WindowLocationChanged.Add(position => {
    Console.WriteLine($"Moved to {position.X}, {position.Y}");
});
```

## Web Messaging Events

### `WebMessageReceived`
`InfiniFrameOrderedEvent<string>`

Fires when JavaScript calls `window.external.sendMessage(...)` in the browser control
The raw string message is passed to each handler

```csharp
builder.Events.WebMessageReceived.Add(message => {
    Console.WriteLine($"From JS: {message}");
});
```

## InfiniFrameOrderedEvent

`InfiniFrameOrderedEvent` and `InfiniFrameOrderedEvent<T>` are the base event types

| Member | Description |
|--------|-------------|
| `Add(handler)` | Appends a handler — handlers fire in order added |
| `Remove(handler)` | Removes a previously added handler |

Handlers registered earlier always execute before handlers registered later — this is unlike standard C# multicast delegates which do not guarantee order

## InfiniFrameOrderedClosingEvent

A specialized event for `WindowClosing` that receives both the window and a cancellation flag

```csharp
// Handler signature
Func<IInfiniFrameWindow, bool, bool>
// Parameters: (window, wasCancelRequested) → return value is unused for close cancellation
```

## Event Handler Shortcuts on the Builder

For convenience, the builder also exposes direct registration methods:

```csharp
builder.RegisterWindowClosingHandler((window, cancel) => false);
builder.RegisterWindowClosingRequestedHandler(window => false);
```

These are equivalent to calling `builder.Events.WindowClosing.Add(...)` and `builder.Events.WindowClosingRequested.Add(...)` respectively
