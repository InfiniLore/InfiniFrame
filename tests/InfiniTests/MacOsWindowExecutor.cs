// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using TUnit.Core.Interfaces;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed partial class MacOsWindowExecutor : ITestExecutor {
    private const string NativeWindowTestNamespace = "InfiniTests.InfiniFrame.Window";
    private const string LibDispatch = "/usr/lib/system/libdispatch.dylib";
    private const string LibSystem = "/usr/lib/libSystem.dylib";

    private static readonly Lazy<IntPtr> MainQueue = new(ResolveMainQueue);
    private static readonly DispatchWorkCallback DispatchWork = InvokeDispatchWork;
    private static readonly IntPtr DispatchWorkPointer = Marshal.GetFunctionPointerForDelegate(DispatchWork);
    private static readonly TimeSpan MainQueueTimeout = TimeSpan.FromSeconds(30);
    
    // WKWebView has no API for awaiting the remote layer tree's final display callbacks.
    // On macOS 15 Intel, starting another view immediately after teardown can crash inside
    // WebKit's RemoteLayerTreeDrawingAreaProxyMac. Hold the exclusive test lease until the
    // preceding view's deferred native cleanup has settled.
    private static readonly TimeSpan WebKitTeardownSettleTime = TimeSpan.FromMilliseconds(150);

    // AppKit and the test host share one main queue. Keep a lease for the complete async test lifetime so
    // continuations from separate tests cannot interleave on that queue.
    private static readonly SemaphoreSlim MainQueueTestLease = new(1, 1);

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async ValueTask ExecuteTest(
        TestContext context,
        Func<ValueTask> action
    ) {
        if (!OperatingSystem.IsMacOS() || !RequiresMainQueue(context) || pthread_main_np() == 1) {
            await action();
            return;
        }

        CancellationToken cancellationToken = context.Execution.CancellationToken;
        await MainQueueTestLease.WaitAsync(cancellationToken);
        try {
            await DispatchToMainQueueAsync(action, cancellationToken);
        }
        finally {
            if (IsNativeWindowTest(context)) {
                // Cleanup must complete even when the test failed or its cancellation token
                // has been signaled; releasing the lease early would start another WKWebView
                // while WebKit can still deliver display work for the previous one.
                await Task.Delay(WebKitTeardownSettleTime, CancellationToken.None);
            }

            MainQueueTestLease.Release();
        }
    }

    [LibraryImport(LibDispatch)]
    private static partial void dispatch_async_f(IntPtr queue, IntPtr context, IntPtr work);

    [LibraryImport(LibSystem)]
    private static partial int pthread_main_np();

    private static bool RequiresMainQueue(TestContext context) {
        if (context.Metadata.TestDetails.HasAttribute<RunOnMacOsMainThreadAttribute>()) {
            return true;
        }

        return IsNativeWindowTest(context);
    }

    private static bool IsNativeWindowTest(TestContext context) {
        string? testNamespace = context.Metadata.TestDetails.Class.ClassType.Namespace;
        return testNamespace is not null && (
            testNamespace.Equals(NativeWindowTestNamespace, StringComparison.Ordinal) ||
            testNamespace.StartsWith(NativeWindowTestNamespace + ".", StringComparison.Ordinal)
        );
    }

    private static IntPtr ResolveMainQueue() {
        if (!NativeLibrary.TryLoad(LibDispatch, out IntPtr libDispatchHandle)) {
            throw new DllNotFoundException($"Unable to load '{LibDispatch}'.");
        }

        if (NativeLibrary.TryGetExport(libDispatchHandle, "dispatch_get_main_queue", out IntPtr queueGetterPtr)) {
            var queueGetter = Marshal.GetDelegateForFunctionPointer<DispatchGetMainQueueCallback>(queueGetterPtr);
            return queueGetter();
        }

        foreach (string queueSymbol in new[] { "_dispatch_main_q", "__dispatch_main_q" }) {
            if (NativeLibrary.TryGetExport(libDispatchHandle, queueSymbol, out IntPtr queuePtr)) {
                return queuePtr;
            }
        }

        throw new EntryPointNotFoundException(
            "Unable to resolve the macOS main dispatch queue symbol in libdispatch.");
    }

    private static async ValueTask DispatchToMainQueueAsync(
        Func<ValueTask> action,
        CancellationToken cancellationToken
    ) {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var state = new MainQueueTestWork(action, completion, cancellationToken);
        GCHandle handle = GCHandle.Alloc(state);

        dispatch_async_f(MainQueue.Value, GCHandle.ToIntPtr(handle), DispatchWorkPointer);

        Task completedTask = await Task.WhenAny(
            completion.Task,
            Task.Delay(MainQueueTimeout, cancellationToken)
        );
        if (completedTask != completion.Task) {
            cancellationToken.ThrowIfCancellationRequested();
            throw new TimeoutException(
                "Timed out while waiting for the macOS main queue to execute a window test. " +
                "The AppKit main thread is not pumping dispatch work in this test host.");
        }

        await completion.Task;
    }

    private static void InvokeDispatchWork(IntPtr context) {
        GCHandle handle = GCHandle.FromIntPtr(context);
        object target = handle.Target!;
        handle.Free();

        switch (target) {
            case MainQueueTestWork work:
                work.Start();
                break;
            case MainQueueCallback callback:
                callback.Invoke();
                break;
            default:
                throw new InvalidOperationException($"Unexpected macOS main queue work item type '{target.GetType()}'.");
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void DispatchWorkCallback(IntPtr context);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr DispatchGetMainQueueCallback();

    private sealed class MainQueueTestWork(
        Func<ValueTask> action,
        TaskCompletionSource completion,
        CancellationToken cancellationToken
    ) {
        public void Start() {
            // A timed-out test may still be queued while AppKit is finishing a slow WebKit
            // operation. Do not start that abandoned test when the main queue recovers.
            if (cancellationToken.IsCancellationRequested) {
                completion.TrySetCanceled(cancellationToken);
                return;
            }

            SynchronizationContext? previousContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(MacOsMainQueueSynchronizationContext.Instance);

            try {
                ValueTask actionTask = action();
                if (actionTask.IsCompletedSuccessfully) {
                    completion.SetResult();
                    return;
                }

                _ = CompleteAsync(actionTask);
            }
            catch (Exception ex) {
                completion.SetException(ex);
            }
            finally {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        private async Task CompleteAsync(ValueTask actionTask) {
            try {
                await actionTask;
                completion.SetResult();
            }
            catch (Exception ex) {
                completion.SetException(ex);
            }
        }
    }

    private sealed class MacOsMainQueueSynchronizationContext : SynchronizationContext {
        public static readonly MacOsMainQueueSynchronizationContext Instance = new();

        public override void Post(SendOrPostCallback d, object? state) {
            ArgumentNullException.ThrowIfNull(d);

            var callbackState = new MainQueueCallback(d, state);
            GCHandle handle = GCHandle.Alloc(callbackState);
            dispatch_async_f(MainQueue.Value, GCHandle.ToIntPtr(handle), DispatchWorkPointer);
        }

        public override void Send(SendOrPostCallback d, object? state) {
            ArgumentNullException.ThrowIfNull(d);

            if (pthread_main_np() == 1) {
                d(state);
                return;
            }

            using var completed = new ManualResetEventSlim();
            var sendState = new MainQueueSendState(d, state, completed);

            Post(
                d: static callbackState => {
                    var sendState = (MainQueueSendState)callbackState!;
                    try {
                        sendState.Callback(sendState.State);
                    }
                    catch (Exception ex) {
                        sendState.Exception = ex;
                    }
                    finally {
                        sendState.Completed.Set();
                    }
                },
                sendState);

            if (!completed.Wait(MainQueueTimeout)) {
                throw new TimeoutException(
                    "Timed out while waiting for the macOS main queue to execute a synchronous callback.");
            }

            if (sendState.Exception is not null) throw sendState.Exception;
        }
    }

    private sealed class MainQueueCallback(
        SendOrPostCallback callback,
        object? state
    ) {
        public void Invoke() {
            SynchronizationContext? previousContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(MacOsMainQueueSynchronizationContext.Instance);

            try {
                callback(state);
            }
            finally {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }
    }

    private sealed class MainQueueSendState(
        SendOrPostCallback callback,
        object? state,
        ManualResetEventSlim completed
    ) {
        public SendOrPostCallback Callback { get; } = callback;
        public object? State { get; } = state;
        public ManualResetEventSlim Completed { get; } = completed;
        public Exception? Exception { get; set; }
    }
}
