// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Hosting;
using InfiniFrame.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace InfiniFrame.BlazorWebView.Hosted;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Hosted windows only (external host providers): one framework <c>WebViewManager</c> per browser document.
/// </summary>
/// <remarks>
///     <para>
///         Microsoft's <c>WebViewManager</c> shares one IPC sender between all page contexts it creates, and routes every
///         inbound message to its newest page context. A retired page's JS runtime and navigation manager therefore keep
///         sending through the same channel as the next page, and the next page's replies can complete the retired page's
///         calls. The framework gives <c>SendMessage</c> no producer identity, so no binding inside one manager can tell
///         the two pages apart.
///     </para>
///     <para>
///         This coordinator gives every browser document its own manager, created by that document's first
///         <c>AttachPage</c>. The manager's output is permanently bound to its document, and only that document's
///         messages reach it, validated again after waiting on the Blazor dispatcher. Each manager gets the root
///         components registered through <see cref="IInfiniFrameWebViewManager" /> and its own service scope per page,
///         while application singletons are shared. When the document's lifetime ends, the manager is retired and
///         disposed exactly once; its later output is refused and counted, never sent to another document.
///     </para>
///     <para>
///         A retired page's new JS interop calls are refused with <see cref="JSDisconnectedException" /> (seen at once by
///         callers on the Blazor dispatcher); its other output is dropped and counted. As on Blazor Server, JS in a
///         disposal path (for example a module's <c>DisposeAsync</c>) should catch that exception. If it does not, the
///         framework's page disposal stops before disposing the page's scope, and this coordinator then disposes that
///         scope itself.
///     </para>
///     <para>
///         Calls that its JS runtime still had pending when the document retired are not completed by this coordinator:
///         completing them would invent a JS result. They end when their caller's cancellation token or timeout fires.
///         A page disposal that waits on such a call is no longer tracked after a bound, and its scope is released.
///     </para>
/// </remarks>
internal sealed class HostedDocumentManagers : IAsyncDisposable {
    internal const string BlazorMessagePrefix = "__bwv:";
    private const string AttachPagePrefix = "__bwv:[\"AttachPage\"";
    private const string RenderBatchPrefix = "__bwv:[\"RenderBatch\"";
    private const string BeginInvokeJsPrefix = "__bwv:[\"BeginInvokeJS\"";
    private const string NavigatePrefix = "__bwv:[\"Navigate\"";
    private static readonly TimeSpan DisposeTimeout = TimeSpan.FromSeconds(15);
    // One retirement may not hold its page longer than this: a disposal can wait forever on a JS call that was already
    // pending when the document retired (WebView sets no default interop timeout, and nothing completes the call).
    private static readonly TimeSpan RetirementTimeout = TimeSpan.FromSeconds(10);

    private readonly InfiniFrameWebViewManager _owner;
    private readonly IServiceProvider _provider;
    private readonly Dispatcher _dispatcher;
    private readonly Uri _appBaseUri;
    private readonly IFileProvider _fileProvider;
    private readonly JSComponentConfigurationStore _jsComponents;
    private readonly string _hostPage;
    private readonly ILogger _logger;

    private readonly object _gate = new();
    private readonly Dictionary<ulong, HostedDocumentWebViewManager> _managers = new();
    private readonly List<RootRegistration> _roots = [];
    private readonly HashSet<Task> _retirements = [];
    private IInfiniFrameDocumentWindow? _window;
    private int _disposed;

    // Bounded diagnostics: fixed counters only.
    private long _created;
    private long _retired;
    private long _disposedManagers;
    private long _disposeFailures;
    private long _disposeTimeouts;
    private long _pageScopeFallbacks;
    private long _attachMessages;
    private long _repeatedAttaches;
    private long _refusedInbound;
    private long _refusedOutbound;
    private long _refusedRenderBatches;
    private long _refusedJsCalls;
    private long _refusedNavigations;
    private long _refusedNavigateCalls;
    private long _unboundOutbound;

    internal HostedDocumentManagers(
        InfiniFrameWebViewManager owner,
        IServiceProvider provider,
        Dispatcher dispatcher,
        Uri appBaseUri,
        IFileProvider fileProvider,
        JSComponentConfigurationStore jsComponents,
        string hostPage,
        ILogger logger
    ) {
        _owner = owner;
        _provider = provider;
        _dispatcher = dispatcher;
        _appBaseUri = appBaseUri;
        _fileProvider = fileProvider;
        _jsComponents = jsComponents;
        _hostPage = hostPage;
        _logger = logger;
    }

    /// <summary>True once a hosted window delivered a message, so the coordinator owns all Blazor pages.</summary>
    internal bool IsBound => Volatile.Read(ref _window) is not null;

    /// <summary>The manager of the window's current document, if its page attached and it is not retired.</summary>
    internal HostedDocumentWebViewManager? CurrentManager {
        get {
            if (Volatile.Read(ref _window) is not {} window) return null;
            ulong current = window.CurrentDocument;
            if (current == 0) return null;
            lock (_gate) {
                return _managers.TryGetValue(current, out HostedDocumentWebViewManager? manager) && !manager.IsRetired ? manager : null;
            }
        }
    }

    /// <summary>
    ///     Dispatches to the current document's page, or returns null when no hosted page exists. Retirement is checked
    ///     again on Blazor's dispatcher, where it runs inline before any queued disposal, so the work item never receives
    ///     a disposed page scope.
    /// </summary>
    internal Task<bool>? TryDispatchToCurrentAsync(Action<IServiceProvider> workItem) {
        if (CurrentManager is not {} current) return null;
        return _dispatcher.InvokeAsync(() => current.IsRetired ? Task.FromResult(false) : current.TryDispatchAsync(workItem));
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Root components (registered through IInfiniFrameWebViewManager, after the base manager accepted them)
    // -----------------------------------------------------------------------------------------------------------------
    internal Task OnRootAdded(Type componentType, string selector, ParameterView parameters) {
        HostedDocumentWebViewManager[] live;
        lock (_gate) {
            _roots.Add(new RootRegistration(selector, componentType, parameters));
            live = LiveManagersLocked();
        }
        if (live.Length == 0) return Task.CompletedTask;
        return _dispatcher.InvokeAsync(() => {
            var adds = new List<Task>(live.Length);
            foreach (HostedDocumentWebViewManager manager in live) {
                if (!manager.IsRetired) adds.Add(manager.AddRootComponentOnceAsync(componentType, selector, parameters));
            }
            return Task.WhenAll(adds);
        });
    }

    internal Task OnRootRemoved(string selector) {
        HostedDocumentWebViewManager[] live;
        lock (_gate) {
            int index = _roots.FindIndex(root => string.Equals(root.Selector, selector, StringComparison.Ordinal));
            if (index >= 0) _roots.RemoveAt(index);
            live = LiveManagersLocked();
        }
        if (live.Length == 0) return Task.CompletedTask;
        return _dispatcher.InvokeAsync(() => {
            var removals = new List<Task>(live.Length);
            foreach (HostedDocumentWebViewManager manager in live) {
                if (!manager.IsRetired) removals.Add(manager.RemoveRootComponentIfAddedAsync(selector));
            }
            return Task.WhenAll(removals);
        });
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Inbound (provider message callback thread; the document was the window's current document at that moment)
    // -----------------------------------------------------------------------------------------------------------------
    internal void Receive(IInfiniFrameDocumentWindow window, ulong document, Uri origin, string message) {
        IInfiniFrameDocumentWindow bound = Interlocked.CompareExchange(ref _window, window, null) ?? window;
        if (!ReferenceEquals(bound, window)) {
            RefuseInbound(document, "it came from a second window");
            return;
        }
        bool attach = message.StartsWith(AttachPagePrefix, StringComparison.Ordinal);
        if (attach) Interlocked.Increment(ref _attachMessages);
        // Every message waits on the Blazor dispatcher in arrival order, behind work already queued for older pages.
        Observe(_dispatcher.InvokeAsync(() => DeliverOnDispatcher(window, document, origin, message, attach)));
    }

    private void DeliverOnDispatcher(IInfiniFrameDocumentWindow window, ulong document, Uri origin, string message, bool attach) {
        try {
            if (Volatile.Read(ref _disposed) != 0) return;
            HostedDocumentWebViewManager? manager;
            lock (_gate) {
                if (!_managers.TryGetValue(document, out manager) || manager.IsRetired) manager = null;
            }
            // The document may have retired while this message waited.
            if (window.GetDocumentLifetime(document).IsCancellationRequested) {
                if (manager is not null) Retire(manager);
                RefuseInbound(document, "its document retired while it waited");
                return;
            }
            if (manager is not null) {
                // A repeated AttachPage from the same document stays inside that document's manager.
                if (attach) Interlocked.Increment(ref _repeatedAttaches);
            }
            else if (attach) {
                manager = Create(window, document);
                if (manager is null) return;
            }
            else {
                // Non-Blazor messages are not for the framework; Blazor messages need their document's page.
                if (message.StartsWith(BlazorMessagePrefix, StringComparison.Ordinal))
                    RefuseInbound(document, "no page of its document is attached");
                return;
            }
            manager.Deliver(origin, message);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogWarning(exception, "Delivering a message to the Blazor page of hosted document {Document} failed", document);
        }
    }

    private HostedDocumentWebViewManager? Create(IInfiniFrameDocumentWindow window, ulong document) {
        CancellationToken lifetime = window.GetDocumentLifetime(document);
        if (lifetime.IsCancellationRequested) {
            RefuseInbound(document, "its document retired before its page attached");
            return null;
        }
        if (Volatile.Read(ref _disposed) != 0) return null;
        var manager = new HostedDocumentWebViewManager(this, document, _provider, _dispatcher, _appBaseUri, _fileProvider, _jsComponents, _hostPage);
        RootRegistration[] roots = [];
        var superseded = new List<HostedDocumentWebViewManager>();
        bool shuttingDown;
        lock (_gate) {
            shuttingDown = Volatile.Read(ref _disposed) != 0;
            if (!shuttingDown) {
                foreach (KeyValuePair<ulong, HostedDocumentWebViewManager> existing in _managers) {
                    if (existing.Key < document) superseded.Add(existing.Value);
                }
                _managers[document] = manager;
                roots = [.. _roots];
            }
        }
        if (shuttingDown) {
            // The coordinator began disposing while this manager was built; it never had a page.
            manager.TryMarkRetired();
            Observe(manager.DisposeAsync().AsTask());
            return null;
        }
        Interlocked.Increment(ref _created);
        // Older documents cannot be current any more; their managers retire now even if their lifetime callback is late.
        foreach (HostedDocumentWebViewManager old in superseded) Retire(old);
        // No page is attached yet, so these only record the registrations; AttachPage adds them to the new page.
        foreach (RootRegistration root in roots) Observe(manager.AddRootComponentOnceAsync(root.ComponentType, root.Selector, root.Parameters));
        // Runs inline if the document retired meanwhile, which retires this manager before it attaches.
        manager.SetLifetimeRegistration(lifetime.UnsafeRegister(static state => {
            var retiring = (HostedDocumentWebViewManager)state!;
            retiring.Owner.Retire(retiring);
        }, manager));
        _logger.LogDebug("Created the Blazor manager of hosted document {Document}", document);
        return manager.IsRetired ? null : manager;
    }

    private void RefuseInbound(ulong document, string reason) {
        long count = Interlocked.Increment(ref _refusedInbound);
        if (count <= 8 || (count & 1023) == 0)
            _logger.LogDebug("Refused a Blazor message from hosted document {Document} because {Reason} (refused so far: {Count})", document, reason, count);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Outbound (from a document's own manager; the framework calls SendMessage on the Blazor dispatcher)
    // -----------------------------------------------------------------------------------------------------------------
    internal void SendFromDocument(HostedDocumentWebViewManager manager, string message) {
        IInfiniFrameDocumentWindow? window = Volatile.Read(ref _window);
        // The document may have retired before its lifetime callback ran: close the gate now.
        if (!manager.IsRetired && (window is null || window.GetDocumentLifetime(manager.Document).IsCancellationRequested)) Retire(manager);
        if (manager.IsRetired) {
            // A refused JS call fails as it would on a disconnected Blazor Server circuit, so framework and component
            // DisposeAsync code that already handles JSDisconnectedException completes. Callers on the Blazor dispatcher
            // observe it at once; the framework's IPC sender keeps a refusal from other threads to itself, so such a
            // call stays pending until its caller cancels it. Nothing is sent to any document either way.
            if (RefuseOutbound(manager, message))
                throw new JSDisconnectedException(
                    "The browser document of this Blazor page has retired, so its JavaScript interop calls cannot be delivered.");
            return;
        }
        _owner.QueueHostedMessage(window!, message, manager.Document);
    }

    internal void NavigateFromDocument(HostedDocumentWebViewManager manager, Uri absoluteUri) {
        if (manager.IsRetired) {
            long count = Interlocked.Increment(ref _refusedNavigateCalls);
            if (count <= 8) _logger.LogDebug("Refused a navigation requested by the retired Blazor manager of hosted document {Document}", manager.Document);
            return;
        }
        _owner.NavigateWindow(absoluteUri);
    }

    /// <summary>Output that no hosted document produced (the coordinator's own base manager never hosts a page).</summary>
    internal void RefuseUnbound() {
        long count = Interlocked.Increment(ref _unboundOutbound);
        if (count <= 8) _logger.LogWarning("Refused Blazor output of a hosted window that no browser document produced");
    }

    /// <summary>Counts a refused message and returns whether it is a JS interop call.</summary>
    private bool RefuseOutbound(HostedDocumentWebViewManager manager, string message) {
        Interlocked.Increment(ref _refusedOutbound);
        bool jsCall = false;
        if (message.StartsWith(RenderBatchPrefix, StringComparison.Ordinal)) Interlocked.Increment(ref _refusedRenderBatches);
        else if (message.StartsWith(BeginInvokeJsPrefix, StringComparison.Ordinal)) {
            Interlocked.Increment(ref _refusedJsCalls);
            jsCall = true;
        }
        else if (message.StartsWith(NavigatePrefix, StringComparison.Ordinal)) Interlocked.Increment(ref _refusedNavigations);
        if (manager.CountRefusedOutbound() == 1)
            _logger.LogDebug("Refused output of the retired Blazor page of hosted document {Document}; it is never sent to another document", manager.Document);
        return jsCall;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Retirement
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Closes the manager's gates at once and disposes it exactly once, later, on the Blazor dispatcher (never inline:
    ///     the caller may be inside the manager's own page work).
    /// </summary>
    internal void Retire(HostedDocumentWebViewManager manager) {
        if (!manager.TryMarkRetired()) return;
        Interlocked.Increment(ref _retired);
        lock (_gate) {
            // The disposal does not inherit the execution context of whatever retired the manager (for example the next
            // document's inbound callback).
            Task retirement;
            if (ExecutionContext.IsFlowSuppressed()) retirement = Task.Run(() => RetireCoreAsync(manager));
            else
                using (ExecutionContext.SuppressFlow())
                    retirement = Task.Run(() => RetireCoreAsync(manager));
            _retirements.Add(retirement);
            _ = retirement.ContinueWith(static (task, state) => {
                var self = (HostedDocumentManagers)state!;
                lock (self._gate) self._retirements.Remove(task);
            }, this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
    }

    private async Task RetireCoreAsync(HostedDocumentWebViewManager manager) {
        IServiceProvider? pageServices = null;
        Task disposal = _dispatcher.InvokeAsync(async () => {
            lock (_gate) {
                if (_managers.TryGetValue(manager.Document, out HostedDocumentWebViewManager? mapped) && ReferenceEquals(mapped, manager))
                    _managers.Remove(manager.Document);
            }
            manager.ReleaseLifetimeRegistration();
            // Capture the page's own scope first (public TryDispatchAsync): the framework's page disposal stops before
            // disposing that scope when a component's disposal fails, for example a JS call in DisposeAsync refused with
            // JSDisconnectedException and rethrown by the renderer.
            await manager.TryDispatchAsync(services => Volatile.Write(ref pageServices, services)).ConfigureAwait(false);
            await manager.DisposeAsync().ConfigureAwait(false);
        });
        try {
            await disposal.WaitAsync(RetirementTimeout).ConfigureAwait(false);
            Interlocked.Increment(ref _disposedManagers);
            _logger.LogDebug("Disposed the Blazor manager of retired hosted document {Document}", manager.Document);
            return;
        }
        catch (TimeoutException) {
            Interlocked.Increment(ref _disposeTimeouts);
            Observe(disposal);
            _logger.LogWarning(
                "The Blazor page of retired hosted document {Document} was still disposing after {Timeout} (it may wait for a JS call that can no longer complete); its page scope is released",
                manager.Document, RetirementTimeout);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            Interlocked.Increment(ref _disposeFailures);
            _logger.LogWarning(exception, "Disposing the Blazor page of retired hosted document {Document} failed; its page scope is released", manager.Document);
        }
        await ReleasePageScopeAsync(Volatile.Read(ref pageServices), manager.Document).ConfigureAwait(false);
    }

    /// <summary>
    ///     Fallback after a page's disposal failed or did not finish: disposes the page's own scope (captured through the
    ///     public <c>TryDispatchAsync</c>), never the application's root provider. InfiniFrame builds its services with
    ///     Microsoft.Extensions.DependencyInjection, whose scoped service provider is the scope itself; disposing it again
    ///     is a no-op, so this cannot dispose anything twice.
    /// </summary>
    private async Task ReleasePageScopeAsync(IServiceProvider? pageServices, ulong document) {
        if (pageServices is null || ReferenceEquals(pageServices, _provider)) return;
        Interlocked.Increment(ref _pageScopeFallbacks);
        try {
            await _dispatcher.InvokeAsync(async () => {
                if (pageServices is IAsyncDisposable asyncDisposable) await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                else if (pageServices is IDisposable disposable) disposable.Dispose();
            }).WaitAsync(RetirementTimeout).ConfigureAwait(false);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogWarning(exception, "Releasing the page scope of retired hosted document {Document} failed", document);
        }
    }

    /// <summary>Retires every manager and waits (bounded) until all are disposed.</summary>
    public async ValueTask DisposeAsync() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        HostedDocumentWebViewManager[] live;
        lock (_gate) live = [.. _managers.Values];
        foreach (HostedDocumentWebViewManager manager in live) Retire(manager);
        Task[] pending;
        lock (_gate) pending = [.. _retirements];
        if (pending.Length == 0) return;
        try {
            await Task.WhenAll(pending).WaitAsync(DisposeTimeout).ConfigureAwait(false);
        }
        catch (TimeoutException) {
            Interlocked.Increment(ref _disposeTimeouts);
            _logger.LogWarning("Hosted Blazor managers were still disposing after {Timeout}", DisposeTimeout);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Diagnostics
    // -----------------------------------------------------------------------------------------------------------------
    internal HostedDocumentDiagnostics GetDiagnostics() {
        int live;
        ulong latest = 0;
        lock (_gate) {
            live = 0;
            foreach (KeyValuePair<ulong, HostedDocumentWebViewManager> entry in _managers) {
                if (entry.Value.IsRetired) continue;
                live++;
                if (entry.Key > latest) latest = entry.Key;
            }
        }
        return new HostedDocumentDiagnostics(
            live, latest,
            Interlocked.Read(ref _created), Interlocked.Read(ref _retired), Interlocked.Read(ref _disposedManagers),
            Interlocked.Read(ref _disposeFailures), Interlocked.Read(ref _disposeTimeouts), Interlocked.Read(ref _pageScopeFallbacks),
            Interlocked.Read(ref _attachMessages), Interlocked.Read(ref _repeatedAttaches),
            Interlocked.Read(ref _refusedInbound), Interlocked.Read(ref _refusedOutbound),
            Interlocked.Read(ref _refusedRenderBatches), Interlocked.Read(ref _refusedJsCalls), Interlocked.Read(ref _refusedNavigations),
            Interlocked.Read(ref _refusedNavigateCalls), Interlocked.Read(ref _unboundOutbound));
    }

    private HostedDocumentWebViewManager[] LiveManagersLocked() {
        if (_managers.Count == 0) return [];
        var live = new List<HostedDocumentWebViewManager>(_managers.Count);
        foreach (HostedDocumentWebViewManager manager in _managers.Values) {
            if (!manager.IsRetired) live.Add(manager);
        }
        return [.. live];
    }

    private void Observe(Task task) {
        if (task.IsCompletedSuccessfully) return;
        _ = ObserveAsync(task);
    }

    private async Task ObserveAsync(Task task) {
        try {
            await task.ConfigureAwait(false);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogWarning(exception, "Hosted Blazor page work failed");
        }
    }

    private readonly record struct RootRegistration(string Selector, Type ComponentType, ParameterView Parameters);
}

/// <summary>Bounded counters describing the per-document Blazor managers of a hosted window.</summary>
internal readonly record struct HostedDocumentDiagnostics(
    int LiveManagers,
    ulong LatestLiveDocument,
    long ManagersCreated,
    long ManagersRetired,
    long ManagersDisposed,
    long DisposeFailures,
    long DisposeTimeouts,
    long PageScopeFallbacks,
    long AttachMessages,
    long RepeatedAttaches,
    long RefusedInbound,
    long RefusedOutbound,
    long RefusedRenderBatches,
    long RefusedJsCalls,
    long RefusedNavigations,
    long RefusedNavigateCalls,
    long UnboundOutbound,
    long SendsCancelledForRetiredDocument = 0
);
