---
title: Awaitable window operations design
---

# Awaitable window operations design

Status: implemented. This document records the shipped contract and additive ABI used by the managed layer.

## Return-type convention

Fire-and-forget dispatch and lightweight submission APIs return `ValueTask` or `ValueTask<T>`. Operations that produce a computed result (navigation, dialogs) return `Task<T>`. This avoids allocating a `Task` for the common fast-path (dispatch completes synchronously on the native thread) while keeping the result-bearing APIs compatible with `await`.

## Scope and current inventory

This inventory covers the public .NET window-feature surface and the corresponding JavaScript window facade. It does not classify unrelated ASP.NET, Blazor, test, or tooling tasks.

| API | Current completion meaning | Classification |
| --- | --- | --- |
| `CloseAsync` | Native close callback reached `NativeClosed` | Real native completion; concurrent requests share the close attempt; cancellation affects only one waiter |
| `WaitForCloseAsync` | `NativeClosed` | Real native event observation |
| `DispatchAsync` | Registered callback ran on the owning native loop, or reached a terminal suppression state | Real native completion; cancellation/timeout removes or suppresses pending work so it cannot execute later |
| `SendWebMessageAsync` | The platform WebView accepted or locally queued the message | Local submission acknowledgement |
| `SendWebMessageWithAcknowledgementAsync` | JavaScript acknowledgement received after WebView accepted the message | Request/response with JS receipt confirmation |
| `ShowOpenFileAsync` | Native response/cancellation callback | Real native completion; cancellation closes the native dialog and terminal callback owns cleanup |
| `ShowOpenFolderAsync` | Native response/cancellation callback | Real native completion; same operation model as open-file |
| `ShowSaveFileAsync` | Native response/cancellation callback | Real native completion; same operation model as open-file |
| `.NET ShowMessageAsync` | Native response/cancellation callback | Real native completion; Windows uses a dedicated STA; macOS sheets and GTK response signals remain on their owning loops |
| JavaScript `tryLoad*Async` | A correlated browser-to-managed request receives its managed response | Real request/response, but `tryLoad*Async` means request acceptance rather than navigation completion |

There is currently no .NET async readiness or navigation-completion API. `InputDataProbe.tsx` is a generated React input component and has no lifecycle, navigation, or bridge-protocol coupling.

## Lifecycle contract

The common successful path is:

`Created -> Creating -> Ready -> CloseRequested -> NativeClosed -> TeardownPending -> TeardownComplete -> NativeHandleReleased -> Disposed`

The public enum `InfiniFrameWindowLifecycleState` uses those names. Existing `Created`, `Initializing`, `Running`, `ClosingRequested`, `NativeClosed`, and `Disposed` values are aliases or ordinal matches; code must not depend on enum ordinal ordering.

The observable milestones are distinct:

| Milestone | Exact meaning |
| --- | --- |
| `CloseRequested` | InfiniFrame accepted one shared close attempt and successfully scheduled it on the owning native loop |
| `NativeClosed` | The logical native window session can no longer receive window/browser work. On macOS this is the InfiniFrame session boundary; a pooled `NSWindow`/`WKWebView` may remain alive internally |
| `TeardownPending` | Backend resources are being released; managed callback routes and owned session resources are quiescent |
| `TeardownComplete` | InfiniFrame-owned signals, event tokens, pending operations, dialogs, and browser callback routes are disconnected or terminal, and backend resources are safe to release |
| `NativeHandleReleased` | Native ownership was relinquished and the session pointer is no longer usable |
| `Disposed` | The managed handle, operation sink, callback registrations, and GC roots were released exactly once |

`TeardownComplete` on macOS does not promise that private Apple WebKit display-link or process-pool work is fully drained. It promises only that InfiniFrame callback routes and owned session resources are quiescent and safe to release.

A close veto is the only exceptional branch. If an existing `Closing` handler rejects a close attempt, that attempt completes with `CloseRejectedException` and the window returns to `Ready`. Concurrent callers share that attempt but have independent cancellation. Cancellation only stops one caller waiting; it never retracts a close already accepted by native code.

Recommended lifecycle API:

```csharp
ValueTask WaitForReadyAsync(CancellationToken cancellationToken = default);
ValueTask CloseAsync(CancellationToken cancellationToken = default);
ValueTask WaitForCloseAsync(CancellationToken cancellationToken = default); // NativeClosed
ValueTask WaitForClosedCallbacksAsync(CancellationToken cancellationToken = default);
ValueTask WaitForTeardownAsync(CancellationToken cancellationToken = default);
ValueTask DisposeAsync();
```

`Close()` remains a non-blocking compatibility request. `WaitForClose()` and `Dispose()` remain explicitly blocking compatibility APIs and must reject or avoid waits that would deadlock the owning event loop. `InfiniFrameWindow` implements `IAsyncDisposable`.

Windows synchronous `Build()` remains caller-STA-owned. Because WebView2 cannot finish initialization until that STA pumps messages, add an opt-in `BuildAsync`/hosted-window path that creates the window on an InfiniFrame-owned STA and starts its message loop before awaiting readiness. `WaitForReadyAsync` is valid on an already-pumping caller-owned window; it does not secretly pump or move UI work to the thread pool. Linux continues to use its owning GLib context. macOS creation is scheduled on the process main run loop and requires the embedding host to run that loop.

`Ready` means the native window exists, the browser controller/view exists, native browser event handlers are installed, and the InfiniFrame bridge script/transport is installed so navigation and browser-to-managed messages can be initiated. It does not mean that a particular page navigation succeeded. A separate JavaScript acknowledgement proves document-side receipt.

## Reusable native-operation model

Each window owns a native operation registry. Managed code assigns monotonically increasing unsigned 64-bit IDs; zero is reserved for unsolicited lifecycle events. An operation record contains:

- ID, kind/name, start timestamp, and owning window/session;
- `Pending`, `Running`, or one terminal state;
- one atomic terminal transition;
- cancellation/timeout source state;
- native platform code, failure reason, and optional result payload;
- a native lifetime retention token valid through callback return.

Terminal states are `Succeeded`, `Failed`, `Superseded`, `Cancelled`, `TimedOut`, `WindowClosed`, `Disposed`, and `Rejected`. Exactly one path removes the record and emits completion. Completion callbacks are copied while locked and invoked only after releasing registry/platform locks. Closing the window atomically detaches the outstanding set, then completes each operation outside the lock.

Managed `WindowOperation<T>` records diagnostics, owns a `NativeHandleLease`, and uses a `TaskCompletionSource<T>` with asynchronous continuations. The lease and callback context are not released from inside the reverse P/Invoke. Native callback scopes retain the session until the reverse callback has returned; destruction requested during a callback is deferred to the owning loop. Begin failure, native completion, and disposal each release the managed callback context through the same exactly-once owner.

For dispatch, cancellation or timeout performs a native `Pending -> Cancelled/TimedOut` compare/exchange and removes/suppresses the queued source or message. If execution already won `Pending -> Running`, cancellation cannot pretend the callback did not run; completion follows the callback. A callback whose cancellation/timeout won is never invoked later.

## Shipped operation APIs

### Dispatch

```csharp
ValueTask<InfiniFrameDispatchResult> DispatchAsync(
    Action callback,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
```

`Completed` means the callback returned on the native UI thread. `Failed` includes callback/native scheduling failure. `Invoke` remains the only explicitly blocking dispatcher. Cancellation/timeout removes or suppresses pending work so it cannot execute later.

### Navigation

```csharp
Task<NavigationResult> LoadAsync(Uri uri, CancellationToken cancellationToken = default);
Task<NavigationResult> LoadRawStringAsync(string content, CancellationToken cancellationToken = default);

public enum NavigationStatus {
    Succeeded, Failed, Superseded, WindowClosed
}
```

`NavigationResult` contains the operation ID, status, final URI when available, native error code, and failure reason. Caller cancellation yields a canceled task and records the diagnostic final state as `Cancelled`. Starting a newer managed navigation completes the older one as `Superseded`; late native events are ignored by ID/generation.

Correlation signals:

- Windows: subscribe to `NavigationStarting` and `NavigationCompleted`, associate the InfiniFrame operation with WebView2's navigation ID, and use `IsSuccess`/`WebErrorStatus`.
- macOS: map the `WKNavigation*` returned by `loadRequest:`/`loadHTMLString:` to the operation; complete from `didFinishNavigation`, `didFailNavigation`, or `didFailProvisionalNavigation`.
- Linux: serialize replacement on the owning GLib context. Mark the old operation superseded, stop it, wait for/consume its terminal signal, and then start the replacement generation. Complete from `WEBKIT_LOAD_FINISHED` or `load-failed`; never correlate by URI alone.

Existing `Load`, `LoadRawString`, and `TryLoad*` methods remain request/acceptance compatibility APIs and are documented accordingly.

### Web messages

```csharp
ValueTask SendWebMessageAsync(string message, CancellationToken ct = default);
Task SendWebMessageWithAcknowledgementAsync(string message, CancellationToken ct = default);
```

`SendWebMessageAsync` is a lightweight local-submission acknowledgement. `SendWebMessageWithAcknowledgementAsync` adds a JavaScript acknowledgement envelope: the injected bridge sends `ack` after its receive handler accepts the envelope, and may later send `reply`. Managed code validates the current document/navigation generation before completing. Cancellation, timeout, navigation replacement, close, and disposal all terminate the request. Native dispatch or WebView submission never counts as JavaScript receipt.

The wire envelope adds a protocol-owned `operationId`, `kind` (`request`, `ack`, `reply`, `error`), and payload.

### Dialogs

The existing async method signatures remain genuinely event-backed. New structured-result overloads may distinguish user cancellation from owner close without breaking existing `null`/empty-array projections.

- macOS: `beginSheetModalForWindow:completionHandler:` for file panels and `NSAlert`; completion runs on the main run loop. Owner close calls `endSheet` and completes `WindowClosed`.
- Linux: create/show dialogs on the owning `GMainContext`, subscribe to `response`/`destroy`, and avoid `gtk_dialog_run`. Cancellation destroys the dialog on that context after disconnecting exactly once.
- Windows: use a dedicated, correctly initialized STA dialog coordinator. It calls `IFileDialog::Show(ownerHwnd)` with the real owner, reports completion to the window operation registry, and uses `IFileDialog::Close` for cancellation/owner close. The HWND thread is never blocked by `Show`, and the coordinator is not a thread-pool MTA. Message dialogs use the same owned-STA strategy or a callback-based platform API.

Synchronous dialog APIs remain modal/blocking compatibility methods with explicit documentation.

### Window state acknowledgements

Do not add async getters or async versions of immediate setters. After the priority operations are stable, add `WaitForStateAsync` and small convenience methods such as `SetFullScreenAsync` only where existing native events can confirm the transition. Maximize/minimize/restore, focus, move, and resize complete from their native event, not from setter return. A newer conflicting request supersedes the older one.

## Native ABI additions

All current exports and the size-equality-checked `InfiniFrameInitParams` layout remain unchanged. Additive operations use an opaque managed context, a 64-bit operation ID, and exactly-once completion callbacks. General operations report a terminal result, native code, and a borrowed UTF-8 failure string; file-dialog callbacks report borrowed native strings for the callback duration:

```cpp
using OperationCompletedCallback =
    void (*)(void* context, uint64_t operationId, int32_t result,
             int32_t nativeCode, const char* failureUtf8);

using FileDialogCompletedCallback =
    void (*)(void* context, uint64_t operationId, int32_t result,
             int32_t valueCount, AutoString* values);
```

Implemented additive exports (all existing exports and the constructor layout remain compatible):

- `InfiniFrameNative_SetReadyCallback` and `InfiniFrameNative_SetTeardownCallback`
- `InfiniFrameNative_BeginInvoke` and `InfiniFrameNative_CancelOperation`
- `InfiniFrameNative_BeginNavigateToUrl` and `InfiniFrameNative_BeginNavigateToString`
- `InfiniFrameNative_CancelNavigation`
- `InfiniFrameNative_BeginShowOpenFile`, `InfiniFrameNative_BeginShowOpenFolder`, `InfiniFrameNative_BeginShowSaveFile`, and `InfiniFrameNative_BeginShowMessage`
- `InfiniFrameNative_CancelDialog`

The managed diagnostics snapshot exposes the last lifecycle transition, all outstanding operations, and the most recent terminal operation with its name, ID, timestamps, final state, native code, and failure reason. Callback contexts and native-handle leases remain rooted until the terminal backend callback returns.

## Platform scheduling and teardown

- Windows posts operation records to the owning HWND/thread dispatcher. WebView2 handlers and COM references are removed on that STA. A final posted loop turn after `WM_NCDESTROY` completes teardown. Late WebView2 initialization callbacks retain a session token and can only terminally complete/close operations.
- macOS uses AppKit on the main thread. New async dispatch and deferred teardown use a `CFRunLoopSource` or `CFRunLoopPerformBlock` in common modes plus `CFRunLoopWakeUp`, not a semaphore wait or a main-queue callback that a nested/default-mode run loop may fail to service.
- Linux attaches idle sources and signals to the window's owning `GMainContext`. Close detaches sources/signals, stops loads, lets GTK finish its destroy cascade, and completes teardown from a later idle turn before releasing objects in dependency order.

No managed callback is invoked while a native mutex is held. No backend accesses `this` or another raw session pointer after invoking a callback unless a native session-retention scope is active.

## Tests and diagnostics

Cross-platform tests cover concurrent close coalescing, cancellation of one close waiter, close then async disposal, exact-once callback/root release, close during every operation kind, navigation success/failure/cancellation/replacement, and dispatch cancellation/timeout without late callback execution. They also assert operation IDs are monotonic and no outstanding operation survives teardown.

Platform tests cover WebView2 initialization/navigation IDs and late callbacks, AppKit nested-run-loop/common-mode scheduling, GTK owning-context signals/idle teardown, and async dialog response/owner-close behavior. macOS stress tests await `WaitForTeardownAsync`/`DisposeAsync` and contain no teardown sleeps or UI-thread blocking waits.

On timeout or crash diagnostics print the window ID, last lifecycle transition with timestamp/thread, and every outstanding operation's name, ID, age, state, and last failure. CI should collect these logs on all net8.0, net9.0, and net10.0 jobs for Windows, Linux, macOS x64, and macOS arm64.

## Implementation sequence

1. Add lifecycle/operation primitives, diagnostics, and additive ABI types.
2. Implement native async dispatch and lifecycle readiness/teardown/release.
3. Add correlated navigation.
4. Add async file/message dialogs and async web-message routing.
5. Add JavaScript acknowledgement envelopes and reply handling.
6. Add event-backed window-state waits where portable signals are reliable.

### Planned / future work

The following items are deferred and not yet implemented:

- `BuildAsync` / hosted-STA window construction path (Windows caller-STA only; see lifecycle contract section above).
- JavaScript reply envelope and `WebMessageReply` protocol (the acknowledgement envelope is shipped; JS-side reply routing is planned).
- State async waits (`WaitForStateAsync`, `SetFullScreenAsync`, etc.) where native events are not yet reliable.
- Native operation registry (`NativeOperationRegistry` abstraction) — currently the operation lifecycle is inline within each operation type.