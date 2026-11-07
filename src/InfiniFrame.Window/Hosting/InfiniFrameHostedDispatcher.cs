// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Window.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Core-owned managed dispatcher for hosted windows: one dedicated thread per hosted application, strictly FIFO.
///     Provider threads never run application code; provider notifications are only enqueued here.
/// </summary>
/// <remarks>
///     Three lanes share the FIFO but not their budgets. Provider data (inbound messages and window events) is bounded by
///     item count and payload bytes; an over-budget post is refused so the caller can fault the window instead of
///     dropping work. Application dispatches (<c>Invoke</c>/<c>DispatchAsync</c>) have their own item bound and report
///     <see cref="InfiniFrameDispatchResult.Failed" /> when it is exhausted, so neither side can starve the other.
///     Lifecycle control work is not counted: each window produces a small, fixed number of control items.
/// </remarks>
internal sealed class InfiniFrameHostedDispatcher : IDisposable {
    internal const int DefaultItemCapacity = 1024;
    internal const long DefaultByteCapacity = 64L * 1024 * 1024;
    internal const int DefaultApplicationItemCapacity = 4096;
    private static readonly TimeSpan DefaultDispatchTimeout = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan JoinTimeout = TimeSpan.FromSeconds(30);

    private readonly BlockingCollection<Work> _queue = new(new ConcurrentQueue<Work>());
    private readonly ILogger _logger;
    private readonly int _itemCapacity;
    private readonly long _byteCapacity;
    private readonly int _applicationItemCapacity;
    private readonly Thread _thread;
    private int _providerItems;
    private long _providerBytes;
    private int _applicationItems;
    private int _disposed;

    internal InfiniFrameHostedDispatcher(
        ILogger logger,
        int itemCapacity = DefaultItemCapacity,
        long byteCapacity = DefaultByteCapacity,
        int applicationItemCapacity = DefaultApplicationItemCapacity
    ) {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(itemCapacity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(byteCapacity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(applicationItemCapacity);
        _logger = logger;
        _itemCapacity = itemCapacity;
        _byteCapacity = byteCapacity;
        _applicationItemCapacity = applicationItemCapacity;
        _thread = new Thread(Run) { IsBackground = true, Name = "InfiniFrame hosted dispatcher" };
        _thread.Start();
    }

    private enum Lane {
        Control,
        Application,
        Provider
    }

    internal int ThreadId => _thread.ManagedThreadId;
    internal bool CheckAccess => Environment.CurrentManagedThreadId == _thread.ManagedThreadId;
    internal int QueuedProviderItems => Volatile.Read(ref _providerItems);
    internal long QueuedProviderBytes => Interlocked.Read(ref _providerBytes);
    internal int QueuedApplicationItems => Volatile.Read(ref _applicationItems);
    internal bool IsDisposed => Volatile.Read(ref _disposed) != 0;

    /// <summary>Runs <paramref name="callback" /> on the dispatcher thread and blocks until it has run.</summary>
    internal InfiniFrameDispatchResult Invoke(Action callback) {
        ArgumentNullException.ThrowIfNull(callback);
        if (CheckAccess) return RunInline(callback);
        var work = new Work(callback, Lane.Application, captureContext: true, withCompletion: true);
        if (!TryEnqueue(work, payloadBytes: 0)) return InfiniFrameDispatchResult.Failed;
        // Safe to block: this thread is not the dispatcher thread, and the work never waits for the caller.
        return work.Completion!.Task.GetAwaiter().GetResult();
    }

    /// <summary>Runs <paramref name="callback" /> on the dispatcher thread; inline when already on it.</summary>
    internal async ValueTask<InfiniFrameDispatchResult> DispatchAsync(Action callback, TimeSpan? timeout, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(callback);
        if (timeout is {} value && value <= TimeSpan.Zero) return InfiniFrameDispatchResult.TimedOut;
        if (cancellationToken.IsCancellationRequested) return InfiniFrameDispatchResult.Cancelled;
        if (CheckAccess) return RunInline(callback);
        var work = new Work(callback, Lane.Application, captureContext: true, withCompletion: true);
        if (!TryEnqueue(work, payloadBytes: 0)) return InfiniFrameDispatchResult.Failed;
        try {
            return await work.Completion!.Task.WaitAsync(timeout ?? DefaultDispatchTimeout, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            return work.TryAbandon() ? InfiniFrameDispatchResult.Cancelled : await work.Completion!.Task.ConfigureAwait(false);
        }
        catch (TimeoutException) {
            return work.TryAbandon() ? InfiniFrameDispatchResult.TimedOut : await work.Completion!.Task.ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Queues provider data work (for example, one inbound message). Returns <see langword="false" /> when the provider
    ///     item or byte budget is exhausted or the dispatcher is stopped; the work is then not queued.
    /// </summary>
    internal bool TryPost(Action callback, int payloadBytes) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfNegative(payloadBytes);
        return TryEnqueue(new Work(callback, Lane.Provider, captureContext: false, withCompletion: false), payloadBytes);
    }

    /// <summary>Queues lifecycle control work. Returns <see langword="false" /> only when the dispatcher is stopped.</summary>
    internal bool PostControl(Action callback) {
        ArgumentNullException.ThrowIfNull(callback);
        return TryEnqueue(new Work(callback, Lane.Control, captureContext: false, withCompletion: false), payloadBytes: 0);
    }

    /// <summary>Runs <paramref name="callback" /> on the dispatcher thread (control lane) and completes with its result.</summary>
    internal Task<T> InvokeAsync<T>(Func<T> callback) {
        ArgumentNullException.ThrowIfNull(callback);
        if (CheckAccess) {
            try { return Task.FromResult(callback()); }
            catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) { return Task.FromException<T>(exception); }
        }
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        var work = new Work(() => {
            try { completion.TrySetResult(callback()); }
            catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) { completion.TrySetException(exception); }
        }, Lane.Control, captureContext: false, withCompletion: false);
        if (!TryEnqueue(work, payloadBytes: 0))
            completion.TrySetException(new ObjectDisposedException(nameof(InfiniFrameHostedDispatcher)));
        return completion.Task;
    }

    private bool TryEnqueue(Work work, int payloadBytes) {
        if (Volatile.Read(ref _disposed) != 0) return false;
        if (!Reserve(work.Lane, payloadBytes)) return false;
        work.PayloadBytes = payloadBytes;
        try {
            if (_queue.TryAdd(work)) return true;
        }
        catch (Exception exception) when (exception is InvalidOperationException or ObjectDisposedException) {
            // The dispatcher stopped between the disposal check and the add.
        }
        Release(work.Lane, payloadBytes);
        return false;
    }

    private bool Reserve(Lane lane, int payloadBytes) {
        switch (lane) {
            case Lane.Provider: {
                int items = Interlocked.Increment(ref _providerItems);
                long bytes = Interlocked.Add(ref _providerBytes, payloadBytes);
                if (items <= _itemCapacity && bytes <= _byteCapacity) return true;
                Release(lane, payloadBytes);
                _logger.LogError("Hosted dispatcher provider budget exhausted. Items: {Items}/{ItemCapacity}, Bytes: {Bytes}/{ByteCapacity}",
                    items, _itemCapacity, bytes, _byteCapacity);
                return false;
            }
            case Lane.Application: {
                int items = Interlocked.Increment(ref _applicationItems);
                if (items <= _applicationItemCapacity) return true;
                Release(lane, payloadBytes);
                _logger.LogError("Hosted dispatcher application budget exhausted. Items: {Items}/{ItemCapacity}", items, _applicationItemCapacity);
                return false;
            }
            default:
                return true;
        }
    }

    private void Release(Lane lane, int payloadBytes) {
        switch (lane) {
            case Lane.Provider:
                Interlocked.Decrement(ref _providerItems);
                Interlocked.Add(ref _providerBytes, -payloadBytes);
                break;
            case Lane.Application:
                Interlocked.Decrement(ref _applicationItems);
                break;
        }
    }

    private InfiniFrameDispatchResult RunInline(Action callback) {
        try {
            callback();
            return InfiniFrameDispatchResult.Completed;
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogError(exception, "Hosted dispatcher callback failed");
            return InfiniFrameDispatchResult.Failed;
        }
    }

    private void Run() {
        foreach (Work work in _queue.GetConsumingEnumerable()) {
            try {
                work.Execute(_logger);
            }
            finally {
                Release(work.Lane, work.PayloadBytes);
            }
        }
    }

    /// <summary>Stops accepting work, lets queued work drain, and joins the thread unless called from it.</summary>
    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        _queue.CompleteAdding();
        if (CheckAccess) return;
        if (_thread.Join(JoinTimeout)) _queue.Dispose();
        else _logger.LogWarning("The hosted dispatcher thread did not finish draining within {Timeout}", JoinTimeout);
    }

    private sealed class Work(Action callback, Lane lane, bool captureContext, bool withCompletion) {
        private readonly ExecutionContext? _context = captureContext ? ExecutionContext.Capture() : null;
        private int _state;
        internal readonly TaskCompletionSource<InfiniFrameDispatchResult>? Completion = withCompletion
            ? new TaskCompletionSource<InfiniFrameDispatchResult>(TaskCreationOptions.RunContinuationsAsynchronously)
            : null;
        internal Lane Lane { get; } = lane;
        internal int PayloadBytes;

        internal bool TryAbandon() => Interlocked.CompareExchange(ref _state, 2, 0) == 0;

        internal void Execute(ILogger logger) {
            if (Interlocked.CompareExchange(ref _state, 1, 0) != 0) return;
            try {
                if (_context is not null) ExecutionContext.Run(_context, static state => ((Action)state!)(), callback);
                else callback();
                Completion?.TrySetResult(InfiniFrameDispatchResult.Completed);
            }
            catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                logger.LogError(exception, "Hosted dispatcher callback failed");
                Completion?.TrySetResult(InfiniFrameDispatchResult.Failed);
            }
        }
    }
}
