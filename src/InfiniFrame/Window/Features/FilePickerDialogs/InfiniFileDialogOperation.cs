// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal enum InfiniFileDialogKind { OpenFile, OpenFolder, SaveFile }

internal sealed class InfiniFileDialogOperation {
    private static long _nextId;
    private static readonly InfiniFrameNative.FileDialogCompletedCallback CompletionCallback = Complete;

    private readonly IInfiniFrameWindow _window;
    private readonly ILogger _logger;
    private readonly InfiniFileDialogKind _kind;
    private readonly string _title;
    private readonly string _defaultPath;
    private readonly bool _multiSelect;
    private readonly string[] _filters;
    private readonly CancellationToken _cancellationToken;
    private readonly string? _diagnosticKey;
    private readonly TaskCompletionSource<string?[]> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private NativeHandleLease? _lease;
    private GCHandle _selfHandle;
    private CancellationTokenRegistration _cancellationRegistration;
    private int _nativeStarted;
    private int _cancellationRequested;
    private int _cancellationDispatchStarted;
    private int _completed;

    public ulong Id { get; } = unchecked((ulong)Interlocked.Increment(ref _nextId));
    public Task<string?[]> Task => _completion.Task;

    public InfiniFileDialogOperation(
        IInfiniFrameWindow window,
        ILogger logger,
        InfiniFileDialogKind kind,
        string title,
        string defaultPath,
        bool multiSelect,
        string[] filters,
        CancellationToken cancellationToken
    ) {
        _window = window;
        _logger = logger;
        _kind = kind;
        _title = title;
        _defaultPath = defaultPath;
        _multiSelect = multiSelect;
        _filters = filters;
        _cancellationToken = cancellationToken;
        _diagnosticKey = (window as InfiniFrameWindow)?.BeginDiagnosticOperation(kind.ToString(), Id);
    }

    public async Task StartAsync() {
        try {
            _cancellationRegistration = _cancellationToken.Register(
                static state => ((InfiniFileDialogOperation)state!).OnCancellationRequested(), this
            );
            await _window.WaitForReadyAsync(_cancellationToken).ConfigureAwait(false);
            _lease = _window.AcquireNativeHandle();
            _selfHandle = GCHandle.Alloc(this);
            IntPtr context = GCHandle.ToIntPtr(_selfHandle);
            InfiniFrameDispatchResult dispatch = await _window.DispatchAsync(() => {
                InfiniFrameNativeInteropStatus status = _kind switch {
                    InfiniFileDialogKind.OpenFile => InfiniFrameNative.BeginShowOpenFile(
                        _lease.Handle, Id, _title, _defaultPath, _multiSelect,
                        _filters, _filters.Length, CompletionCallback, context),
                    InfiniFileDialogKind.OpenFolder => InfiniFrameNative.BeginShowOpenFolder(
                        _lease.Handle, Id, _title, _defaultPath, _multiSelect, CompletionCallback, context),
                    InfiniFileDialogKind.SaveFile => InfiniFrameNative.BeginShowSaveFile(
                        _lease.Handle, Id, _title, _defaultPath,
                        _filters, _filters.Length, string.Empty, CompletionCallback, context),
                    _ => InfiniFrameNativeInteropStatus.InvalidArgument
                };
                if (status != InfiniFrameNativeInteropStatus.Success)
                    throw new ApplicationException(InfiniFrameNative.GetLastErrorMessage() ?? "Could not show native dialog.");
            }).ConfigureAwait(false);

            if (dispatch != InfiniFrameDispatchResult.Completed) {
                Finish([]);
                return;
            }

            Volatile.Write(ref _nativeStarted, 1);
            if (Volatile.Read(ref _cancellationRequested) != 0)
                StartCancellationDispatch();
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogError(exception, "Asynchronous file dialog {OperationId} failed.", Id);
            Finish([]);
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
            InfiniFrameDispatchResult dispatched = await _window.DispatchAsync(() => {
                if (_lease is null) return;
                InfiniFrameNativeInteropStatus status = InfiniFrameNative.CancelDialog(_lease.Handle, Id, out _);
                if (status != InfiniFrameNativeInteropStatus.Success)
                    throw new ApplicationException(InfiniFrameNative.GetLastErrorMessage() ?? "Could not cancel native dialog.");
            }).ConfigureAwait(false);
            if (dispatched == InfiniFrameDispatchResult.Failed)
                _logger.LogWarning("Native dialog cancellation for operation {OperationId} could not be dispatched.", Id);
        }
        catch (ObjectDisposedException) {
            // Window teardown completes the registered native dialog operation.
        }
        catch (Exception exception) {
            _logger.LogWarning(exception, "Native dialog cancellation for operation {OperationId} failed.", Id);
        }
    }

    private static void Complete(IntPtr context, ulong operationId, int result, int valueCount, IntPtr values) {
        if (!TryGet(context, out InfiniFileDialogOperation? operation) || operation.Id != operationId)
            return;

        string?[] resultValues = [];
        if (result == 0 && values != IntPtr.Zero && valueCount > 0) {
            var pointers = new IntPtr[valueCount];
            Marshal.Copy(values, pointers, 0, valueCount);
            resultValues = pointers.Select(pointer => OperatingSystem.IsWindows()
                ? Marshal.PtrToStringUni(pointer)
                : Marshal.PtrToStringUTF8(pointer)).ToArray();
        }
        operation.Finish(resultValues);
    }

    private void Finish(string?[] result) {
        if (Interlocked.Exchange(ref _completed, 1) != 0) return;
        (_window as InfiniFrameWindow)?.CompleteDiagnosticOperation(
            _diagnosticKey, _cancellationToken.IsCancellationRequested ? "Cancelled" : "Completed"
        );
        _completion.TrySetResult(result);
        ThreadPool.QueueUserWorkItem(static state => state.Cleanup(), this, false);
    }

    private void Cleanup() {
        _cancellationRegistration.Dispose();
        if (_selfHandle.IsAllocated) _selfHandle.Free();
        Interlocked.Exchange(ref _lease, null)?.Dispose();
    }

    private static bool TryGet(IntPtr context, [NotNullWhen(true)] out InfiniFileDialogOperation? operation) {
        operation = null;
        if (context == IntPtr.Zero) return false;
        try {
            operation = GCHandle.FromIntPtr(context).Target as InfiniFileDialogOperation;
            return operation is not null;
        }
        catch (InvalidOperationException) {
            return false;
        }
    }
}