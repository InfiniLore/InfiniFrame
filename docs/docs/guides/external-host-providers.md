# External Host Providers

An external host provider supplies native windows and a browser engine through the public contract in
`InfiniFrame.Hosting`, in `InfiniLore.InfiniFrame.Shared`. An application selects it with `UseHostProvider` before
`Build()`. This lets a browser implementation ship independently of InfiniFrame while applications keep using
`IInfiniFrameWindow` and the Blazor integration.

InfiniFrame owns the window facade, lifecycle, managed dispatcher, document-aware messages, and resource dispatch.
The provider owns the native windows, browser, and native event loop or companion process. A provider implements
`IInfiniFrameHostProvider`, `IInfiniFrameHost`, and `IInfiniFrameWindowHost`; it does not implement `IInfiniFrameWindow`.

## Select a provider

Install the provider package and follow its window configuration and deployment requirements. This helper shows
where selection fits into application construction; the caller supplies the provider instance and its supported
window configuration:

```csharp
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.Hosting;

public static class HostedApplication {
    public static InfiniFrameApplication Build(
        IInfiniFrameHostProvider provider,
        Action<IInfiniFrameWindowBuilder> configureWindow) {
        var builder = InfiniFrameApplication.CreateBuilder();
        builder.UseHostProvider(provider);
        builder.WithWindow("main", configureWindow);
        return builder.Build();
    }
}
```

Run the returned application with `Run()` or `RunAsync()` and dispose it when finished. Hosted windows need an
absolute, trusted start URL. The provider may impose stricter browser policy or deployment requirements. Defaults
accepted by a built-in backend are not necessarily accepted by an external provider.

Select one provider per application builder. A process chooses either the default native backend or external
hosting before native initialization; mixing the modes or switching between them is unsupported. Default-engine
settings such as the WebView2 runtime path, notification registration, AppUserModelId, and default notification icon
cannot be combined with `UseHostProvider`.

## Capabilities and unsupported operations

Lifecycle, documents, messages, resources, navigation, and current-URL reporting are mandatory. Optional capability
flags declare bounds, window state, focus, title, icon, their notifications, and native-close cancellation. A provider
must implement the corresponding optional interfaces when it declares those operations.

InfiniFrame rejects configuration requiring absent capabilities. The provider must reject any remaining request it
cannot honor exactly with `NotSupportedException` before creating native resources for that window. Unsupported
configuration is never silently ignored. The hosted contract does not expose every built-in-backend feature; for
example, raw HTML start strings, parent windows, native menus, and drag-and-drop configuration are not supported by
version 1. Serve HTML through a registered custom scheme instead of a raw start string.

## Provider lifecycle and document rules

- `CreateHost` runs during `Build()` and must not start processes, threads, or windows. Native startup belongs in
  `StartAsync`, called by `Run()` or `RunAsync()` before windows are created.
- `CreateWindowAsync` returns a window host; the `Created` notification reports readiness separately. Notifications
  can arrive before creation returns. InfiniFrame queues application callbacks on its managed dispatcher.
- Every main-frame document has a nonzero, increasing ID within its window. The provider installs the bridge before
  page scripts and reports the browser's actual URL with `DocumentAttached`.
- The provider must validate that inbound messages came from the live main-frame document. `SendMessageAsync` must
  deliver only to its specified document, or throw `InfiniFrameHostDocumentRetiredException`; it must never redirect
  retired-document output to the next page.
- Resource responses retain their status and content type. The provider disposes each response, including its body
  stream, after consuming it.
- A window-level failure is reported through `Faulted`; a host-wide failure faults `IInfiniFrameHost.Completion` and
  is surfaced by the application run. Window disposal and host shutdown remain InfiniFrame's responsibility.

Provider operations must complete without waiting for InfiniFrame's dispatcher, because callers may be blocking
that dispatcher while awaiting an operation. Honor cancellation tokens and the ownership rules on each interface.
Window-host calls that ignore cancellation can overlap disposal after their deadline; providers must tolerate that
overlap. The XML documentation on the contract types specifies event ordering, terminal states, and threading.

Host shutdown is awaited for up to 30 seconds. If that wait expires, InfiniFrame requests cancellation and may
start host disposal while the shutdown operation is still running; providers must tolerate this specific teardown
overlap. Late shutdown, disposal, and cancellation-callback failures are observed and logged. Host disposal also has
a 30-second wait bound, so return from the runtime's disposal does not prove that a provider's late cleanup finished.
These bounds apply to returned asynchronous operations, not synchronous code that blocks before returning a task.

Providers should report the version they compiled against using the constants
`new Version(InfiniFrameHostContract.MajorVersion, InfiniFrameHostContract.MinorVersion)`. InfiniFrame rejects a
different major version or a newer minor version before creating a host.

## Hosted Blazor pages

Configure Blazor through `UseBlazorWebView` as usual. Each hosted browser document receives its own framework
`WebViewManager` when it first attaches. Output and replies remain bound to that document, so late work from a
replaced page cannot run JavaScript in, navigate, or consume replies from the next page.

Use the registered `IInfiniFrameWebViewManager` for root component changes and `TryDispatchAsync`. Its methods
coordinate the hosted document managers. The inherited concrete `WebViewManager` members act on the outer manager,
which hosts no page in hosted mode.

New JS interop calls on a retired page fail with `JSDisconnectedException`. Component disposal that calls JS should
handle that expected exception. Calls already pending when the page retires are neither completed nor cancelled by
the coordinator; use caller cancellation or a timeout when they must finish within a bound.

Retirement waits up to 10 seconds for asynchronous page disposal, then separately bounds fallback disposal of a
captured page scope by another 10 seconds. These waits do not cancel late disposal or bound synchronous code that
blocks the dispatcher. Late completion, fault, and cancellation are logged and counted. Disposal that resumes after
scope release can encounter already-disposed services.

The coordinator's shutdown wait is 15 seconds, but the window-closed handler starts manager disposal without
awaiting it. The application run can return and application services can be disposed while page retirement is still
running. A timeout does not prove that every continuation or allocation has been reclaimed.

## Contract tests and an example provider

`tests/InfiniTests.InfiniFrame.Hosting` contains a deterministic scripted provider and tests for lifecycle,
capabilities, dispatch, document isolation, resource handling, and teardown. It exercises the contract without a
real browser. With the repository's build prerequisites installed, run it with:

```sh
dotnet run --project tests/InfiniTests.InfiniFrame.Hosting -c Release -f net10.0
```

[InfiniFrame.CefHost](https://github.com/JoarEliasson/InfiniFrame.CefHost) is an independent preview implementation. It
runs CEF in a companion process and supplies its own native payload, packaging, deployment, and runnable samples. Its
README documents supported platforms and stricter browser policy. CEF and its payload are maintained in that
repository; InfiniFrame's hosting contract remains independent of any particular browser engine.
