// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Threading;
using TUnit.Core.Interfaces;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class MacOsWindowExecutor : ITestExecutor {
    private static int MainThreadId { get; set; } = -1;
    private static SynchronizationContext? MainContext { get; set; }
    private static readonly object SyncLock = new();

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

        EnsureMainContextCaptured();

        if (MainContext is null || Environment.CurrentManagedThreadId == MainThreadId) {
            await action();
            return;
        }

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        MainContext.Post(_ => {
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
    
    public static void CaptureMainThread(AssemblyHookContext context) {
        MainThreadId = Environment.CurrentManagedThreadId;
        MainContext = SynchronizationContext.Current;
    }

    private static void EnsureMainContextCaptured() {
        if (MainThreadId != -1) return;

        lock (SyncLock) {
            if (MainThreadId != -1) return;

            MainThreadId = Environment.CurrentManagedThreadId;
            var ctx = SynchronizationContext.Current;
            
            if (ctx != null && ctx.GetType() != typeof(SynchronizationContext)) {
                MainContext = ctx;
            }
        }
    }
}
