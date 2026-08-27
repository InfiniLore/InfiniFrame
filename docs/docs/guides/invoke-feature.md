# Invoke Feature

The Invoke feature dispatches work to the window's native thread. All UI operations in InfiniFrame must run on the thread that called `Build()`. This feature provides synchronous and asynchronous mechanisms to marshal work from background threads.

## Contents

- [Why Cross-Thread Dispatch Is Needed](#why-cross-thread-dispatch-is-needed)
- [Synchronous Invoke](#synchronous-invoke)
- [Asynchronous Dispatch](#asynchronous-dispatch)
- [Dispatch Result](#dispatch-result)

## Why Cross-Thread Dispatch Is Needed

InfiniFrame windows are single-threaded. The native window (WebView2 on Windows, WebKitGTK on Linux, WKWebView on macOS) runs on a specific thread. Any call that modifies the window — changing the title, resizing, navigating, closing — must happen on that thread.

If you're on a background thread (e.g., from `Task.Run` or an async continuation), use `Invoke` or `DispatchAsync` to marshal the work:

```csharp
Task.Run(() => {
    // Background thread — cannot call window methods directly
    window.Invoke(() => {
        // Now on the window thread — safe to call window methods
        window.SetTitle("Updated from background");
        window.Close();
    });
});
```

## Synchronous Invoke

`Invoke` blocks the calling thread until the callback completes on the window thread:

```csharp
window.Invoke(() => {
    window.SetTitle("New Title");
    window.Close();
});
```

The extension method form returns the window for chaining:

```csharp
window.Invoke(() => window.SetTitle("New Title"));
```

:::warning
`Invoke` is synchronous and blocks the calling thread. If the window thread is busy (e.g., handling a dialog), this will deadlock. Prefer `DispatchAsync` for background work.
:::

## Asynchronous Dispatch

`DispatchAsync` queues the callback on the window thread without blocking the caller. It supports timeout and cancellation:

```csharp
InfiniFrameDispatchResult result = await window.DispatchAsync(() => {
    window.SetTitle("Updated from background");
}, timeout: TimeSpan.FromSeconds(5), cancellationToken: ct);
```

`DispatchAsync` is non-blocking and returns immediately. The callback is executed on the window thread when it becomes available.

## Dispatch Result

`InfiniFrameDispatchResult` indicates the outcome of the dispatch:

| Value | Description |
|-------|-------------|
| `Completed` | The callback executed successfully |
| `TimedOut` | The timeout elapsed before the callback could execute |
| `Cancelled` | The cancellation token was triggered |
| `WindowClosed` | The window was closed before the callback could execute |
| `Failed` | The callback threw an exception |

```csharp
InfiniFrameDispatchResult result = await window.DispatchAsync(() => {
    // Work that may fail
}, timeout: TimeSpan.FromSeconds(10));

switch (result) {
    case InfiniFrameDispatchResult.Completed:
        Console.WriteLine("Done");
        break;
    case InfiniFrameDispatchResult.TimedOut:
        Console.WriteLine("Window thread busy, try again later");
        break;
    case InfiniFrameDispatchResult.WindowClosed:
        Console.WriteLine("Window already closed");
        break;
}
```

## See Also

- [Lifecycle Feature](lifecycle-feature.md) — Window close, ready wait, and teardown
- [Window Features Architecture](window-features-architecture.md) — How the feature system works
- [Core Window Guide](core-window.md) — Builder API and feature overview
