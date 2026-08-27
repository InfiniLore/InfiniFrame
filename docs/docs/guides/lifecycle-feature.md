# Lifecycle Feature

The Lifecycle feature manages the window's life cycle: creation, readiness, closing, teardown, and cross-thread invocation. It also covers the event system for responding to window state changes.

## Contents

- [Window States](#window-states)
- [Closing the Window](#closing-the-window)
- [Waiting for Close](#waiting-for-close)
- [STA Requirement (Windows)](#sta-requirement-windows)
- [Cross-Thread Invocation](#cross-thread-invocation)
- [Events](#events)
- [Intercepting Window Close](#intercepting-window-close)

## Window States

A window goes through a deterministic state machine:

```
Created → Creating → Ready → CloseRequested → NativeClosed
  → TeardownPending → TeardownComplete → NativeHandleReleased → Disposed
```

You can check the current state:

```csharp
InfiniFrameWindowLifecycleState state = window.Features.Lifecycle.State;
bool isClosed = window.Features.Lifecycle.IsClosedOrClosing();
```

## Closing the Window

Close the window programmatically:

```csharp
window.Close();                          // Synchronous
await window.CloseAsync();              // Async
```

:::note
`Close()` initiates the close sequence but does not immediately destroy the window. If a `WindowClosingRequested` handler returns `false`, the close is rejected and the window stays open.
:::

## Waiting for Close

Block or await until the window is destroyed:

```csharp
window.WaitForClose();                              // Blocks calling thread
await window.WaitForCloseAsync();                   // Async
await window.WaitForClosedCallbacksAsync();          // Wait for close callbacks
await window.WaitForTeardownAsync();                 // Wait for full teardown
```

`WaitForClose()` is the most common pattern it blocks until the native window is fully destroyed and is typically the last call in your entry point:

```csharp
var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .SetStartPageUrl("https://example.com")
    .Build();

window.WaitForClose(); // Blocks here until the user closes the window
```

### Waiting for ready

Wait until the window is fully initialized and ready for interaction:

```csharp
await window.WaitForReadyAsync();
```

## STA Requirement (Windows)

WebView2 is COM-based and requires the thread that calls `Build()` to be STA. Without `[STAThread]`, the window opens but the browser control renders as a black screen, and `Build()` now throws `InvalidOperationException` to surface this early.

```csharp
internal class Program {
    [STAThread]
    static void Main(string[] args) {
        var window = InfiniFrameWindowBuilder.Create()
            .SetTitle("My App")
            .SetStartPageUrl("https://example.com")
            .Build();

        window.WaitForClose();
    }
}
```

Top-level statements cannot carry `[STAThread]` so use an explicit `static void Main()` as shown above.

:::warning
`[STAThread]` is silently ignored on `async Task Main`. The async continuation runs on thread pool threads (MTA). Never use `async Task Main` as the entry point for an InfiniFrame application.
:::

**Linux does not have this restriction** because GTK has no COM apartment model. The native constructor calls `gtk_init()` itself and implicitly claims whichever thread calls `Build()` as the GTK main thread.

## Cross-Thread Invocation

All UI operations must run on the window's thread. Use `Invoke` to marshal work from a background thread:

```csharp
Task.Run(() => {
    // Background thread
    window.Invoke(() => {
        // Runs on the window thread
        window.Close();
    });
});
```

### Async dispatch

For non-blocking dispatch with timeout and cancellation support, use `DispatchAsync`:

```csharp
InfiniFrameDispatchResult result = await window.DispatchAsync(() => {
    window.SetTitle("Updated from background");
}, timeout: TimeSpan.FromSeconds(5), cancellationToken: ct);

switch (result) {
    case InfiniFrameDispatchResult.Completed:
        Console.WriteLine("Dispatch completed");
        break;
    case InfiniFrameDispatchResult.TimedOut:
        Console.WriteLine("Dispatch timed out");
        break;
    case InfiniFrameDispatchResult.WindowClosed:
        Console.WriteLine("Window was closed before dispatch");
        break;
}
```

See the [Invoke feature guide](invoke-feature.md) for full details.

## Events

Events are available through `IInfiniFrameWindowEvents`, accessible via `IInfiniFrameWindowBuilder.Events`.

```csharp
var builder = InfiniFrameWindowBuilder.Create();

builder.Events.WindowCreated.Add(() => Console.WriteLine("Window opened"));
builder.Events.WindowSizeChanged.Add(size => Console.WriteLine($"Resized to {size}"));
builder.Events.WindowLocationChanged.Add(loc => Console.WriteLine($"Moved to {loc}"));
builder.Events.WindowFocusIn.Add(() => Console.WriteLine("Focus gained"));
builder.Events.WindowFocusOut.Add(() => Console.WriteLine("Focus lost"));
builder.Events.WindowMaximized.Add(() => Console.WriteLine("Maximized"));
builder.Events.WindowMinimized.Add(() => Console.WriteLine("Minimized"));
builder.Events.WindowRestored.Add(() => Console.WriteLine("Restored"));
builder.Events.WebMessageReceived.Add(msg => Console.WriteLine($"Message: {msg}"));

var window = builder.Build();
window.WaitForClose();
```

## Intercepting Window Close

Use `WindowClosingRequested` to cancel or intercept a close:

```csharp
builder.Events.WindowClosingRequested.Add(() => {
    // Return true to allow closing, false to cancel
    return AskUserToConfirm();
});
```

Use `WindowClosing` to run cleanup before the window is destroyed:

```csharp
builder.Events.WindowClosing.Add((window, cancel) => {
    SaveAppState();
    return false; // returning false here does not cancel; use WindowClosingRequested for that
});
```

## See Also

- [Invoke Feature](invoke-feature.md) Cross-thread dispatch to the window's native thread
- [Window Features Architecture](window-features-architecture.md) How the feature system works
- [Core Window Guide](core-window.md) Builder API and feature overview
