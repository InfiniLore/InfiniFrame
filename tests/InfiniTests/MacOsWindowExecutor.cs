// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using TUnit.Core.Interfaces;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class MacOsWindowExecutor : ITestExecutor {
    private static int _mainThreadId;
    private static SynchronizationContext? _mainContext;

    // macOS GCD dispatch
    [DllImport("/usr/lib/libSystem.dylib")]
    private static extern IntPtr dispatch_get_main_queue();

    [DllImport("/usr/lib/libSystem.dylib")]
    private static extern void dispatch_sync_f(IntPtr queue, IntPtr context, IntPtr work);

    private static readonly IntPtr MainQueue = dispatch_get_main_queue();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void GcdWorkCallback(IntPtr context);

    private static readonly GcdWorkCallback GcdWork = OnGcdWork;
    private static readonly IntPtr GcdWorkPtr = Marshal.GetFunctionPointerForDelegate(GcdWork);

    private static void OnGcdWork(IntPtr context) {
        var gch = GCHandle.FromIntPtr(context);
        var action = (Action)gch.Target!;
        gch.Free();
        action();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async ValueTask ExecuteTest(
        TestContext context,
        Func<ValueTask> action
    ) {
        if (!OperatingSystem.IsMacOS()) {
            await action();
            return;
        }

        // If we have a sync context and we're not on the main thread, dispatch via context
        if (_mainContext is not null && Environment.CurrentManagedThreadId != _mainThreadId) {
            await DispatchViaContext(action);
            return;
        }

        // If already on the captured main thread, run inline
        if (Environment.CurrentManagedThreadId == _mainThreadId) {
            await action();
            return;
        }

        // No sync context available and not on the main thread — dispatch via GCD
        await DispatchViaGcd(action);
    }

    private static async Task DispatchViaContext(Func<ValueTask> action) {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        _mainContext!.Post(_ => {
            try {
                action().GetAwaiter().GetResult();
                tcs.SetResult();
            }
            catch (Exception ex) {
                tcs.SetException(ex);
            }
        }, null);

        await tcs.Task;
    }

    private static async Task DispatchViaGcd(Func<ValueTask> action) {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var gch = GCHandle.Alloc((Action)(() => {
            try {
                action().GetAwaiter().GetResult();
                tcs.SetResult();
            }
            catch (Exception ex) {
                tcs.SetException(ex);
            }
        }));

        dispatch_sync_f(MainQueue, GCHandle.ToIntPtr(gch), GcdWorkPtr);

        await tcs.Task;
    }

    public static void CaptureMainThread(AssemblyHookContext context) {
        _mainThreadId = Environment.CurrentManagedThreadId;
        _mainContext = SynchronizationContext.Current;
    }
}
