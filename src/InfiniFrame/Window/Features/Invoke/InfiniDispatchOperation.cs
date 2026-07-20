// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class InfiniDispatchOperation {
    private const int Pending = 0;
    private const int Completed = 1;
    private const int TimedOut = 2;
    private const int Cancelled = 3;
    private const int WindowClosed = 4;
    private const int Failed = 5;

    private readonly IInfiniFrameWindow _window;
    private readonly ILogger _logger;
    private readonly Action _callback;
    private readonly TaskCompletionSource<InfiniFrameDispatchResult> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Timer _timeoutTimer;
    private readonly CancellationTokenRegistration _cancellationRegistration;
    private int _state;

    public Task<InfiniFrameDispatchResult> Task => _completion.Task;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public InfiniDispatchOperation(IInfiniFrameWindow window, ILogger logger, Action callback, TimeSpan timeout, CancellationToken cancellationToken) {
        _window = window;
        _logger = logger;
        _callback = callback;
        _timeoutTimer = new Timer(callback: _ => Finish(TimedOut), null, timeout, Timeout.InfiniteTimeSpan);
        _cancellationRegistration = cancellationToken.Register(callback: static state => ((InfiniDispatchOperation)state!).Finish(Cancelled), this);
        _ = _completion.Task.ContinueWith(continuationAction: static (_, state) => ((InfiniDispatchOperation)state!).Dispose(), this,
            CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        if (Volatile.Read(ref _state) != Pending) Dispose();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Start() => _ = System.Threading.Tasks.Task.Run(Execute);

    private void Execute() {
        if (Volatile.Read(ref _state) != Pending) return;

        if (_window.Features.Lifecycle.IsClosedOrClosing()) {
            Finish(WindowClosed);
            return;
        }

        try {
            NativeInvoke.InvokeSyncWithValidation(_logger, _window, _window.ManagedThreadId, callback: () => {
                // Native dispatch can dequeue after a timeout/cancellation. Never run user code then.
                if (Volatile.Read(ref _state) != Pending) return;

                if (_window.Features.Lifecycle.IsClosedOrClosing()) {
                    Finish(WindowClosed);
                    return;
                }

                _callback();
            });
            Finish(_window.Features.Lifecycle.IsClosedOrClosing() ? WindowClosed : Completed);
        }
        catch (ObjectDisposedException) when (_window.Features.Lifecycle.IsClosedOrClosing()) {
            Finish(WindowClosed);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogError(exception, "Asynchronous native-window dispatch failed. WindowId={WindowId}", _window.Id);
            Finish(_window.Features.Lifecycle.IsClosedOrClosing() ? WindowClosed : Failed);
        }
    }

    private void Finish(int state) {
        if (Interlocked.CompareExchange(ref _state, state, Pending) != Pending) return;

        var result = (InfiniFrameDispatchResult)(state - 1);

        if (result is not InfiniFrameDispatchResult.Completed) _logger.LogWarning("Native-window dispatch ended with {DispatchResult}. WindowId={WindowId}", result, _window.Id);

        _completion.TrySetResult(result);
    }

    private void Dispose() {
        _timeoutTimer.Dispose();
        _cancellationRegistration.Dispose();
    }
}
