// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Hosting;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Window.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <remarks>
///     Lifecycle: Created -> Creating (StartAsync) -> Ready (provider Created() and the window host returned) ->
///     CloseRequested -> NativeClosed (provider Closed()/Faulted(), or a core-detected fault) -> TeardownPending ->
///     TeardownComplete (closed callbacks delivered once on the dispatcher, window host closed if needed and disposed
///     once, callback root released once) -> Disposed. Calling Close is never treated as completion: only the terminal
///     notification completes closing. Every wait on the provider during close, teardown and disposal is bounded.
/// </remarks>
internal sealed partial class InfiniFrameHostedWindow {
    private TimeSpan CloseTimeout => _runtime.CloseTimeout;

    private readonly TaskCompletionSource _ready = NewSignal();
    private readonly TaskCompletionSource _closed = NewSignal();
    private readonly TaskCompletionSource _closedCallbacks = NewSignal();
    private readonly TaskCompletionSource _teardown = NewSignal();
    private readonly TaskCompletionSource<IInfiniFrameWindowHost?> _windowHost = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly object _closeAttemptGate = new();
    private readonly SemaphoreSlim _disposeGate = new(1, 1);
    private TaskCompletionSource? _closeAttempt;
    private Exception? _fault;
    private int _state = (int)InfiniFrameWindowLifecycleState.Created;
    private int _terminal;
    private int _providerReportedClosed;
    private int _createdReported;
    private int _readyConditions;
    private int _startRequested;
    private int _teardownStarted;
    private int _hostDisposed;
    private int _callbackRootReleased;
    private int _disposeRequested;

    public InfiniFrameWindowLifecycleState LifecycleState => (InfiniFrameWindowLifecycleState)Volatile.Read(ref _state);
    public InfiniFrameWindowLifecycleState State => LifecycleState;

    /// <summary>The terminal fault, when the window ended by failure rather than an orderly close.</summary>
    internal Exception? Fault => Volatile.Read(ref _fault);

    private bool IsReadyReached => Volatile.Read(ref _readyConditions) >= 2;

    // -----------------------------------------------------------------------------------------------------------------
    // Creation (called by the hosted runtime after the application registered the window)
    // -----------------------------------------------------------------------------------------------------------------
    internal async Task StartAsync(CancellationToken cancellationToken) {
        if (Interlocked.Exchange(ref _startRequested, 1) != 0)
            throw new InvalidOperationException("The hosted window has already been started.");
        if (Interlocked.CompareExchange(ref _state, (int)InfiniFrameWindowLifecycleState.Creating,
                (int)InfiniFrameWindowLifecycleState.Created) != (int)InfiniFrameWindowLifecycleState.Created) {
            // Closed or disposed before creation: the provider is never asked for this window.
            _windowHost.TrySetResult(null);
            return;
        }

        IInfiniFrameWindowHost? host = null;
        try {
            await _runtime.Dispatcher.InvokeAsync(() => {
                _events.OnWindowCreating();
                return true;
            }).ConfigureAwait(false);
            host = await InfiniFrameProviderCall.Start(() => _runtime.Host.CreateWindowAsync(Request, _sink, cancellationToken)).ConfigureAwait(false)
                ?? throw new InvalidOperationException($"The host provider '{_runtime.DisplayName}' returned no window host.");
            await InfiniFrameProviderCall.ToThreadPool();
            ValidateDeclaredCapabilities(host);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            await AbandonCreationAsync(host, exception).ConfigureAwait(false);
            throw;
        }
        if (!_windowHost.TrySetResult(host)) {
            // Teardown gave up waiting for this window before the provider returned it: close and release it here.
            _logger.LogWarning("The host provider returned a window that had already ended. Window: {WindowId}", Id);
            await CloseAndReleaseHostAsync(host, requestClose: Volatile.Read(ref _providerReportedClosed) == 0).ConfigureAwait(false);
            return;
        }
        _logger.LogDebug("Hosted window accepted by provider {Provider}. Window: {WindowId}", _runtime.DisplayName, Id);
        OnReadyCondition();
    }

    private void ValidateDeclaredCapabilities(IInfiniFrameWindowHost host) {
        var missing = new List<string>();
        InfiniFrameHostCapabilities declared = _runtime.Capabilities;
        if (declared.HasFlag(InfiniFrameHostCapabilities.Bounds) && host is not IInfiniFrameWindowHostBounds) missing.Add(nameof(IInfiniFrameWindowHostBounds));
        if ((declared.HasFlag(InfiniFrameHostCapabilities.WindowState) || declared.HasFlag(InfiniFrameHostCapabilities.Focus))
            && host is not IInfiniFrameWindowHostState) missing.Add(nameof(IInfiniFrameWindowHostState));
        if (declared.HasFlag(InfiniFrameHostCapabilities.Title) && host is not IInfiniFrameWindowHostTitle) missing.Add(nameof(IInfiniFrameWindowHostTitle));
        if (declared.HasFlag(InfiniFrameHostCapabilities.Icon) && host is not IInfiniFrameWindowHostIcon) missing.Add(nameof(IInfiniFrameWindowHostIcon));
        if (missing.Count != 0)
            throw new InvalidOperationException(
                $"The host provider '{_runtime.DisplayName}' declares capabilities its window host does not implement: {string.Join(", ", missing)}.");
    }

    /// <summary>Rolls back a failed creation: no closed callbacks (the window never existed for the application).</summary>
    private async Task AbandonCreationAsync(IInfiniFrameWindowHost? host, Exception exception) {
        Interlocked.CompareExchange(ref _terminal, 2, 0);
        Volatile.Write(ref _fault, exception);
        _logger.LogError(exception, "Hosted window creation failed. Window: {WindowId}, Provider: {Provider}", Id, _runtime.DisplayName);
        RetireDocuments(windowClosing: true);
        if (_ready.TrySetException(exception)) _ = _ready.Task.Exception;
        _windowHost.TrySetResult(null);
        if (host is not null) await CloseAndReleaseHostAsync(host, requestClose: true).ConfigureAwait(false);
        ReleaseCallbackRootOnce();
        Volatile.Write(ref _state, (int)InfiniFrameWindowLifecycleState.Disposed);
        _closed.TrySetResult();
        _closedCallbacks.TrySetResult();
        _teardown.TrySetResult();
        lock (_closeAttemptGate) _closeAttempt?.TrySetResult();
    }

    /// <summary>Asks the provider to close a window host (unless it already did) and disposes it, within one deadline.</summary>
    private async Task CloseAndReleaseHostAsync(IInfiniFrameWindowHost host, bool requestClose) {
        using var deadline = new CancellationTokenSource(CloseTimeout);
        if (requestClose) {
            try {
                await InfiniFrameProviderCall.Start(() => host.RequestCloseAsync(deadline.Token)).AsTask().WaitAsync(deadline.Token).ConfigureAwait(false);
                await InfiniFrameProviderCall.ToThreadPool();
            }
            catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                _logger.LogDebug(exception, "Could not request native close for an ended hosted window. Window: {WindowId}", Id);
            }
        }
        await DrainSendsAsync(deadline.Token).ConfigureAwait(false);
        await DisposeHostOnceAsync(host, deadline.Token).ConfigureAwait(false);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Provider notifications
    // -----------------------------------------------------------------------------------------------------------------
    private void OnHostCreated() {
        if (Volatile.Read(ref _terminal) != 0) {
            _logger.LogDebug("Ignored Created() after the hosted window ended. Window: {WindowId}", Id);
            return;
        }
        if (Interlocked.Exchange(ref _createdReported, 1) != 0) {
            _logger.LogWarning("Ignored duplicate Created() from host provider {Provider}. Window: {WindowId}", _runtime.DisplayName, Id);
            return;
        }
        OnReadyCondition();
    }

    /// <summary>The window becomes ready once the provider reported Created() and returned its window host.</summary>
    private void OnReadyCondition() {
        if (Interlocked.Increment(ref _readyConditions) != 2 || Volatile.Read(ref _terminal) != 0) return;
        // A close requested while the window was being created is still pending; cancelling it then returns to Ready.
        Interlocked.CompareExchange(ref _state, (int)InfiniFrameWindowLifecycleState.Ready, (int)InfiniFrameWindowLifecycleState.Creating);
        _ready.TrySetResult();
        if (!_runtime.Dispatcher.PostControl(() => {
                if (Volatile.Read(ref _terminal) == 0) _events.OnWindowCreated();
            }))
            _logger.LogWarning("Could not queue WindowCreated callbacks because the dispatcher stopped. Window: {WindowId}", Id);
    }

    private void OnHostClosed() {
        Interlocked.Exchange(ref _providerReportedClosed, 1);
        CompleteTerminal(null, "provider reported Closed()");
    }

    /// <summary>Faults this window (provider Faulted(), a protocol violation, or a core delivery failure).</summary>
    public void FaultWindow(Exception exception) {
        ArgumentNullException.ThrowIfNull(exception);
        CompleteTerminal(exception, "fault");
    }

    /// <summary>The single terminal transition. The first caller wins; later or duplicate terminal calls are ignored.</summary>
    private void CompleteTerminal(Exception? fault, string reason) {
        if (Interlocked.CompareExchange(ref _terminal, fault is null ? 1 : 2, 0) != 0) {
            _logger.LogDebug("Ignored late terminal notification ({Reason}). Window: {WindowId}", reason, Id);
            return;
        }
        if (fault is not null) {
            Volatile.Write(ref _fault, fault);
            _logger.LogError(fault, "Hosted window faulted ({Reason}). Window: {WindowId}, Provider: {Provider}", reason, Id, _runtime.DisplayName);
        }
        else _logger.LogDebug("Hosted window closed ({Reason}). Window: {WindowId}", reason, Id);

        // Atomic with StartAsync's Created -> Creating transition: either the provider is never asked, or teardown
        // waits for the window host the provider returns.
        bool neverStarted = Interlocked.CompareExchange(ref _state, (int)InfiniFrameWindowLifecycleState.NativeClosed,
            (int)InfiniFrameWindowLifecycleState.Created) == (int)InfiniFrameWindowLifecycleState.Created;
        RetireDocuments(windowClosing: true);
        AdvanceState(InfiniFrameWindowLifecycleState.NativeClosed);
        AdvanceState(InfiniFrameWindowLifecycleState.TeardownPending);
        if (neverStarted) _windowHost.TrySetResult(null);
        if (_ready.TrySetException(fault ?? new ObjectDisposedException(nameof(InfiniFrameHostedWindow), "The window closed before becoming ready.")))
            _ = _ready.Task.Exception;
        _closed.TrySetResult();
        lock (_closeAttemptGate) _closeAttempt?.TrySetResult();

        if (!_runtime.Dispatcher.PostControl(DeliverClosedCallbacks)) {
            _logger.LogWarning("The dispatcher stopped before closed callbacks were queued; delivering them inline. Window: {WindowId}", Id);
            DeliverClosedCallbacks();
        }
    }

    /// <summary>Runs on the dispatcher, exactly once: WindowClosed handlers, then teardown.</summary>
    private void DeliverClosedCallbacks() {
        try {
            _events.OnWindowClosed();
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogError(exception, "A WindowClosed handler failed. Window: {WindowId}", Id);
        }
        finally {
            _closedCallbacks.TrySetResult();
            _ = TeardownAsync();
        }
    }

    private async Task TeardownAsync() {
        if (Interlocked.Exchange(ref _teardownStarted, 1) != 0) return;
        try {
            using var deadline = new CancellationTokenSource(CloseTimeout);
            IInfiniFrameWindowHost? host;
            try {
                host = await _windowHost.Task.WaitAsync(deadline.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) {
                // Claim the slot: a window host the provider returns later is closed and released by StartAsync.
                if (_windowHost.TrySetResult(null)) {
                    _logger.LogError("The host provider did not return a window that has already ended within {Timeout}. Window: {WindowId}", CloseTimeout, Id);
                    host = null;
                }
                else host = _windowHost.Task.Result;
            }
            if (host is not null) {
                // A window that ended by a fault (not a native close) is closed natively before its host is released.
                if (Volatile.Read(ref _providerReportedClosed) == 0) {
                    try {
                        await InfiniFrameProviderCall.Start(() => host.RequestCloseAsync(deadline.Token)).AsTask().WaitAsync(deadline.Token).ConfigureAwait(false);
                        await InfiniFrameProviderCall.ToThreadPool();
                    }
                    catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                        _logger.LogDebug(exception, "Could not request native close for a faulted hosted window. Window: {WindowId}", Id);
                    }
                }
                await DrainSendsAsync(deadline.Token).ConfigureAwait(false);
                await DisposeHostOnceAsync(host, deadline.Token).ConfigureAwait(false);
            }
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogError(exception, "Hosted window teardown failed. Window: {WindowId}", Id);
        }
        finally {
            ReleaseCallbackRootOnce();
            AdvanceState(InfiniFrameWindowLifecycleState.TeardownComplete);
            _teardown.TrySetResult();
            if (Volatile.Read(ref _disposeRequested) != 0) AdvanceState(InfiniFrameWindowLifecycleState.Disposed);
        }
    }

    /// <summary>Waits (bounded) for an in-flight send to leave the provider before its window host is disposed.</summary>
    private async Task DrainSendsAsync(CancellationToken deadline) {
        try {
            await _sendGate.WaitAsync(deadline).ConfigureAwait(false);
            _sendGate.Release();
        }
        catch (OperationCanceledException) {
            _logger.LogDebug("A send was still in the provider when its window host was released. Window: {WindowId}", Id);
        }
    }

    private async Task DisposeHostOnceAsync(IInfiniFrameWindowHost host, CancellationToken deadline) {
        if (Interlocked.Exchange(ref _hostDisposed, 1) != 0) return;
        try {
            await InfiniFrameProviderCall.Start(host.DisposeAsync).AsTask().WaitAsync(deadline).ConfigureAwait(false);
            await InfiniFrameProviderCall.ToThreadPool();
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogWarning(exception, "The host provider failed to dispose a window host. Window: {WindowId}", Id);
        }
    }

    private void ReleaseCallbackRootOnce() {
        if (Interlocked.Exchange(ref _callbackRootReleased, 1) != 0) return;
        ((IInfiniFrameEvents)_events).ReleaseNativeCallbackRoot();
    }

    private void AdvanceState(InfiniFrameWindowLifecycleState target) {
        while (true) {
            int current = Volatile.Read(ref _state);
            if (current >= (int)target) return;
            if (Interlocked.CompareExchange(ref _state, (int)target, current) == current) return;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Close
    // -----------------------------------------------------------------------------------------------------------------
    public bool IsClosedOrClosing() => LifecycleState >= InfiniFrameWindowLifecycleState.CloseRequested;

    public void Close() {
        // Like the default window, Close() requests closing and returns; it does not wait for completion. It never blocks
        // on window creation, so it is safe on the dispatcher thread (for example from a WindowCreating handler).
        try { CloseCoreAsync(waitForCompletion: false, CancellationToken.None).GetAwaiter().GetResult(); }
        catch (InfiniFrameCloseRejectedException) {}
    }

    public async ValueTask CloseAsync(CancellationToken ct = default) =>
        await CloseCoreAsync(waitForCompletion: true, ct).ConfigureAwait(false);

    private async Task CloseCoreAsync(bool waitForCompletion, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        if (LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed) return;
        if (LifecycleState == InfiniFrameWindowLifecycleState.Created) {
            CompleteTerminal(null, "closed before creation");
            return;
        }

        TaskCompletionSource? attempt;
        bool requested;
        lock (_closeAttemptGate) {
            requested = ((IInfiniFrameWindow)this).RequestClose();
            if (requested) _closeAttempt = NewSignal();
            attempt = _closeAttempt;
        }
        if (!requested) {
            if (waitForCompletion) await (attempt?.Task ?? _closed.Task).WaitAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        // Same order as the default window: closing-requested notification, cancellable Closing handlers, native close.
        bool cancelled = await RunClosingHandlersAsync(raiseClosingRequested: true, CancellationToken.None).ConfigureAwait(false);
        if (!cancelled) await DeliverHostCloseAsync(cancellationToken).ConfigureAwait(false);
        if (waitForCompletion && attempt is not null) await attempt.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Runs the closing handlers on the dispatcher. Like the default window, a failing handler does not cancel
    ///     closing. Returns <see langword="true" /> when a handler cancelled.
    /// </summary>
    private async Task<bool> RunClosingHandlersAsync(bool raiseClosingRequested, CancellationToken cancellationToken) =>
        await _runtime.Dispatcher.InvokeAsync(() => {
            if (raiseClosingRequested) {
                try { _events.OnWindowClosingRequested(); }
                catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                    _logger.LogError(exception, "A WindowClosingRequested handler failed. Window: {WindowId}", Id);
                }
            }
            try { return _events.OnWindowClosing() != 0; }
            catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                _logger.LogError(exception, "A window Closing handler failed; closing proceeds. Window: {WindowId}", Id);
                return false;
            }
        }).WaitAsync(cancellationToken).ConfigureAwait(false);

    /// <summary>
    ///     Delivers a committed close request. While the provider is still creating the window, delivery is chained to
    ///     creation instead of blocking (creation may need the dispatcher that is calling here).
    /// </summary>
    private async Task DeliverHostCloseAsync(CancellationToken cancellationToken) {
        Task<IInfiniFrameWindowHost?> pending = _windowHost.Task;
        if (!pending.IsCompleted) {
            _ = DeliverHostCloseWhenCreatedAsync(pending);
            return;
        }
        if (pending.Result is {} host) await RequestHostCloseAsync(host, cancellationToken).ConfigureAwait(false);
        else CompleteTerminal(null, "closed during creation");
    }

    private async Task DeliverHostCloseWhenCreatedAsync(Task<IInfiniFrameWindowHost?> pending) {
        try {
            IInfiniFrameWindowHost? host = await pending.ConfigureAwait(false);
            if (host is null) CompleteTerminal(null, "closed during creation");
            else await RequestHostCloseAsync(host, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogWarning(exception, "Deferred close delivery failed. Window: {WindowId}", Id);
        }
    }

    /// <summary>Delivers a close request within <see cref="CloseTimeout" />; a provider failure or timeout faults the window.</summary>
    private async Task RequestHostCloseAsync(IInfiniFrameWindowHost host, CancellationToken cancellationToken) {
        Task request;
        try {
            request = InfiniFrameProviderCall.Start(() => host.RequestCloseAsync(CancellationToken.None)).AsTask();
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            FaultWindow(new InfiniFrameHostOperationException("The host provider failed to close the window.", exception));
            return;
        }
        try {
            await request.WaitAsync(CloseTimeout, cancellationToken).ConfigureAwait(false);
            await InfiniFrameProviderCall.ToThreadPool();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            // A delivered close request cannot be recalled; observe its outcome without the caller.
            _ = ObserveCloseRequestAsync(request);
            throw;
        }
        catch (TimeoutException exception) {
            _ = ObserveCloseRequestAsync(request);
            FaultWindow(new InfiniFrameHostOperationException($"The host provider did not accept a close request within {CloseTimeout}.", exception));
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            FaultWindow(new InfiniFrameHostOperationException("The host provider failed to close the window.", exception));
        }
    }

    private async Task ObserveCloseRequestAsync(Task request) {
        try { await request.ConfigureAwait(false); }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            FaultWindow(new InfiniFrameHostOperationException("The host provider failed to close the window.", exception));
        }
    }

    /// <summary>Uncancellable, bounded close used by disposal and shutdown; it bypasses Closing handlers.</summary>
    private async Task ForceCloseAsync() {
        if (LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed) return;
        if (LifecycleState == InfiniFrameWindowLifecycleState.Created) {
            CompleteTerminal(null, "disposed before creation");
            return;
        }
        ((IInfiniFrameWindow)this).RequestClose();
        IInfiniFrameWindowHost? host;
        try {
            host = await _windowHost.Task.WaitAsync(CloseTimeout).ConfigureAwait(false);
        }
        catch (TimeoutException exception) {
            FaultWindow(new InfiniFrameHostOperationException("The host provider did not finish creating the window.", exception));
            return;
        }
        if (host is null) {
            CompleteTerminal(null, "disposed during creation");
            return;
        }
        await RequestHostCloseAsync(host, CancellationToken.None).ConfigureAwait(false);
        try {
            await _closed.Task.WaitAsync(CloseTimeout).ConfigureAwait(false);
        }
        catch (TimeoutException exception) {
            FaultWindow(new InfiniFrameHostOperationException($"The host provider did not report Closed() within {CloseTimeout}.", exception));
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Waits
    // -----------------------------------------------------------------------------------------------------------------
    public ValueTask WaitForReadyAsync(CancellationToken ct = default) => new(_ready.Task.WaitAsync(ct));

    public void WaitForClose() {
        // Native close may need the dispatcher (CloseRequestedAsync runs Closing handlers there), so blocking it is refused.
        if (_runtime.Dispatcher.CheckAccess && !_closed.Task.IsCompleted)
            throw new InvalidOperationException("WaitForClose would block the InfiniFrame dispatcher that closing needs. Use WaitForCloseAsync.");
        _closed.Task.GetAwaiter().GetResult();
    }

    public ValueTask WaitForCloseAsync(CancellationToken ct = default) => new(_closed.Task.WaitAsync(ct));
    public ValueTask WaitForClosedCallbacksAsync(CancellationToken ct = default) => new(_closedCallbacks.Task.WaitAsync(ct));

    public ValueTask WaitForTeardownAsync(CancellationToken ct = default) =>
        LifecycleState >= InfiniFrameWindowLifecycleState.TeardownComplete ? ValueTask.CompletedTask : new(_teardown.Task.WaitAsync(ct));

    // -----------------------------------------------------------------------------------------------------------------
    // Disposal
    // -----------------------------------------------------------------------------------------------------------------
    public void Dispose() {
        if (_runtime.Dispatcher.CheckAccess && !_teardown.Task.IsCompleted) {
            // Closed callbacks run on this thread, so waiting here would deadlock. Finish disposal after teardown.
            Volatile.Write(ref _disposeRequested, 1);
            _ = ObserveForceCloseAsync();
            return;
        }
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync() {
        await _disposeGate.WaitAsync().ConfigureAwait(false);
        try {
            if (LifecycleState == InfiniFrameWindowLifecycleState.Disposed) return;
            Volatile.Write(ref _disposeRequested, 1);
            await ForceCloseAsync().ConfigureAwait(false);
            try {
                // Teardown bounds each of its own provider waits by one CloseTimeout deadline.
                await _teardown.Task.WaitAsync(CloseTimeout + TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            catch (TimeoutException exception) {
                _logger.LogError(exception, "Hosted window teardown did not confirm completion. Window: {WindowId}", Id);
            }
            ReleaseCallbackRootOnce();
            // Disposal of the facade is complete; anything the provider still holds is released by host shutdown.
            Volatile.Write(ref _state, (int)InfiniFrameWindowLifecycleState.Disposed);
        }
        finally {
            _disposeGate.Release();
        }
    }

    private async Task ObserveForceCloseAsync() {
        try { await ForceCloseAsync().ConfigureAwait(false); }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogWarning(exception, "Deferred hosted window disposal failed. Window: {WindowId}", Id);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Internal lifecycle members of the public window surface. Only InfiniFrame.Window calls these; providers cannot.
    // -----------------------------------------------------------------------------------------------------------------
    void IInfiniFrameWindow.BeginInitialization() =>
        throw new InvalidOperationException("Hosted windows are created by InfiniFrame's host runtime, not by native initialization.");

    void IInfiniFrameWindow.AssignNativeHandle(IntPtr handle) => throw Unsupported("Assigning in-process native handles");

    void IInfiniFrameWindow.MarkReady() =>
        Interlocked.CompareExchange(ref _state, (int)InfiniFrameWindowLifecycleState.Ready, (int)InfiniFrameWindowLifecycleState.Creating);

    bool IInfiniFrameWindow.RequestClose() {
        while (true) {
            var state = (InfiniFrameWindowLifecycleState)Volatile.Read(ref _state);
            if (state is not (InfiniFrameWindowLifecycleState.Creating or InfiniFrameWindowLifecycleState.Ready)) return false;
            if (Interlocked.CompareExchange(ref _state, (int)InfiniFrameWindowLifecycleState.CloseRequested, (int)state) == (int)state) return true;
        }
    }

    void IInfiniFrameWindow.CancelCloseRequest() {
        // Return to Ready once creation completed (also when it completed while the close was pending), else to Creating.
        int returnState = IsReadyReached ? (int)InfiniFrameWindowLifecycleState.Ready : (int)InfiniFrameWindowLifecycleState.Creating;
        Interlocked.CompareExchange(ref _state, returnState, (int)InfiniFrameWindowLifecycleState.CloseRequested);
    }

    void IInfiniFrameWindow.MarkNativeClosed() => AdvanceState(InfiniFrameWindowLifecycleState.NativeClosed);
    void IInfiniFrameWindow.MarkTeardownPending() => AdvanceState(InfiniFrameWindowLifecycleState.TeardownPending);
    void IInfiniFrameWindow.MarkTeardownComplete() => AdvanceState(InfiniFrameWindowLifecycleState.TeardownComplete);
    void IInfiniFrameWindow.MarkNativeHandleReleased() => AdvanceState(InfiniFrameWindowLifecycleState.NativeHandleReleased);
    void IInfiniFrameWindow.MarkDisposed() => AdvanceState(InfiniFrameWindowLifecycleState.Disposed);
    void IInfiniFrameWindow.ReleaseNativeHandle() {}// No in-process native handle exists.

    void IInfiniFrameWindow.SetManagedThreadId(int managedThreadId) =>
        throw new InvalidOperationException("The core hosted dispatcher owns the managed thread of hosted windows.");

    void ILifecycleInfiniFrameWindowFeature.Initialize() =>
        throw new InvalidOperationException("Hosted windows are created by InfiniFrame's host runtime.");

    void ILifecycleInfiniFrameWindowFeature.MarkAsClosed() {
        // Reached from InfiniFrameEvents.OnWindowClosed after the terminal transition already ran.
        AdvanceState(InfiniFrameWindowLifecycleState.NativeClosed);
        _closed.TrySetResult();
    }

    void ILifecycleInfiniFrameWindowFeature.MarkClosedCallbacksDelivered() => _closedCallbacks.TrySetResult();

    void ILifecycleInfiniFrameWindowFeature.MarkCloseRejected() {
        TaskCompletionSource? attempt;
        lock (_closeAttemptGate) attempt = _closeAttempt;
        if (attempt?.TrySetException(new InfiniFrameCloseRejectedException()) == true) _ = attempt.Task.Exception;
    }

    void ILifecycleInfiniFrameWindowFeature.CompleteTeardownAfterNativeLoop() {}// Teardown is driven by the terminal notification.
    void ILifecycleInfiniFrameWindowFeature.CleanupNativeHandle() => ReleaseCallbackRootOnce();
    bool ILifecycleInfiniFrameWindowFeature.CanWaitForCloseDuringDispose() => !_runtime.Dispatcher.CheckAccess;
    bool ILifecycleInfiniFrameWindowFeature.CanWaitForTeardownDuringDispose() => !_runtime.Dispatcher.CheckAccess;
}
