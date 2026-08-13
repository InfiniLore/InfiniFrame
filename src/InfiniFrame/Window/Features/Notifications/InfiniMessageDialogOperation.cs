// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Dialogs;
using InfiniFrame.NativeBridge.Handles;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class InfiniMessageDialogOperation {
    private static long _nextId;
    private static readonly InfiniFrameNative.OperationCompletedCallback CompletionCallback = Complete;

    private readonly IInfiniFrameWindow _window;
    private readonly ILogger _logger;
    private readonly string _title;
    private readonly string _text;
    private readonly InfiniFrameDialogButtons _buttons;
    private readonly InfiniFrameDialogIcon _icon;
    private readonly CancellationToken _cancellationToken;
    private readonly string? _diagnosticKey;
    private readonly TaskCompletionSource<InfiniFrameDialogResult> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private NativeHandleLease? _lease;
    private GCHandle _selfHandle;
    private CancellationTokenRegistration _cancellationRegistration;
    private int _nativeStarted;
    private int _cancellationRequested;
    private int _cancellationDispatchStarted;
    private int _completed;

    public ulong Id { get; } = unchecked((ulong)Interlocked.Increment(ref _nextId));
    public Task<InfiniFrameDialogResult> Task => _completion.Task;

    public InfiniMessageDialogOperation(
        IInfiniFrameWindow window, ILogger logger, string title, string text,
        InfiniFrameDialogButtons buttons, InfiniFrameDialogIcon icon,
        CancellationToken cancellationToken
    ) {
        _window = window;
        _logger = logger;
        _title = title;
        _text = text;
        _buttons = buttons;
        _icon = icon;
        _cancellationToken = cancellationToken;
        _diagnosticKey = (window as InfiniFrameWindow)?.BeginDiagnosticOperation("ShowMessage", Id);
    }

    public async Task StartAsync() {
        try {
            _cancellationRegistration = _cancellationToken.Register(
                static state => ((InfiniMessageDialogOperation)state!).OnCancellationRequested(), this
            );
            await _window.WaitForReadyAsync(_cancellationToken).ConfigureAwait(false);
            _lease = _window.AcquireNativeHandle();
            _selfHandle = GCHandle.Alloc(this);
            IntPtr context = GCHandle.ToIntPtr(_selfHandle);
            InfiniFrameDispatchResult dispatch = await _window.DispatchAsync(() => {
                InfiniFrameNativeInteropStatus status = InfiniFrameNative.BeginShowMessage(
                    _lease.Handle, Id, _title, _text, _buttons, _icon, CompletionCallback, context
                );
                if (status != InfiniFrameNativeInteropStatus.Success)
                    throw new InfiniFrameNativeInteropException(
                        InfiniFrameNative.GetLastErrorMessage() ?? "Could not show native message dialog."
                    );
            }).ConfigureAwait(false);

            if (dispatch != InfiniFrameDispatchResult.Completed) {
                Finish(InfiniFrameDialogResult.Cancel);
                return;
            }

            Volatile.Write(ref _nativeStarted, 1);
            if (Volatile.Read(ref _cancellationRequested) != 0)
                StartCancellationDispatch();
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogError(exception, "Asynchronous message dialog {OperationId} failed.", Id);
            Finish(InfiniFrameDialogResult.Cancel);
        }
    }

    private void OnCancellationRequested() {
        Volatile.Write(ref _cancellationRequested, 1);
        if (Volatile.Read(ref _nativeStarted) != 0)
            StartCancellationDispatch();
    }

    private void StartCancellationDispatch() {
        if (Volatile.Read(ref _completed) != 0) return;
        if (Interlocked.Exchange(ref _cancellationDispatchStarted, 1) == 0)
            _ = RequestCancellationAsync();
    }

    private async Task RequestCancellationAsync() {
        try {
            await _window.DispatchAsync(() => {
                if (_lease is null) return;
                InfiniFrameNativeInteropStatus status = InfiniFrameNative.CancelDialog(_lease.Handle, Id, out _);
                if (status != InfiniFrameNativeInteropStatus.Success)
                    throw new InfiniFrameNativeInteropException(
                        InfiniFrameNative.GetLastErrorMessage() ?? "Could not cancel native message dialog."
                    );
            }).ConfigureAwait(false);
        }
        catch (ObjectDisposedException) {
            // Window teardown requests cancellation for every registered dialog.
        }
        catch (Exception exception) {
            _logger.LogWarning(exception, "Native message dialog cancellation for {OperationId} failed.", Id);
        }
    }

    private static void Complete(
        IntPtr context, ulong operationId, int result, int nativeCode, IntPtr failureUtf8
    ) {
        if (!TryGet(context, out InfiniMessageDialogOperation? operation) || operation.Id != operationId)
            return;
        operation.Finish(result == 0
            ? (InfiniFrameDialogResult)nativeCode
            : InfiniFrameDialogResult.Cancel);
    }

    private void Finish(InfiniFrameDialogResult result) {
        if (Interlocked.Exchange(ref _completed, 1) != 0) return;
        (_window as InfiniFrameWindow)?.CompleteDiagnosticOperation(
            _diagnosticKey, _cancellationToken.IsCancellationRequested ? "Cancelled" : result.ToString()
        );
        _completion.TrySetResult(result);
        ThreadPool.QueueUserWorkItem(static state => state.Cleanup(), this, false);
    }

    private void Cleanup() {
        _cancellationRegistration.Dispose();
        if (_selfHandle.IsAllocated) _selfHandle.Free();
        Interlocked.Exchange(ref _lease, null)?.Dispose();
    }

    private static bool TryGet(IntPtr context, [NotNullWhen(true)] out InfiniMessageDialogOperation? operation) {
        operation = null;
        if (context == IntPtr.Zero) return false;
        try {
            operation = GCHandle.FromIntPtr(context).Target as InfiniMessageDialogOperation;
            return operation is not null;
        }
        catch (InvalidOperationException) {
            return false;
        }
    }
}