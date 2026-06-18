// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using TUnit.Core.Interfaces;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class MacOsWindowExecutor : ITestExecutor {
    private static int MainThreadId { get; set; } = -1;
    private static SynchronizationContext? MainContext { get; set; }
    private static bool HasValidContext { get; set; }
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

        if (!HasValidContext || Environment.CurrentManagedThreadId == MainThreadId) {
            await action();
            return;
        }

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        bool posted = false;

        MainContext!.Post(d: _ => {
            posted = true;
            try {
                action().GetAwaiter().GetResult();
                tcs.SetResult();
            }
            catch (Exception ex) {
                tcs.SetException(ex);
            }
        }, null);

        if (!posted) {
            await action();
            return;
        }

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        Task completedTask = await Task.WhenAny(tcs.Task, Task.Delay(Timeout.Infinite, cts.Token));

        if (completedTask == tcs.Task) {
            await tcs.Task;
        }
        else {
            await action();
        }
    }

    public static void CaptureMainThread(AssemblyHookContext context) {
        MainThreadId = Environment.CurrentManagedThreadId;
        SynchronizationContext? ctx = SynchronizationContext.Current;

        if (ctx != null && ctx.GetType() != typeof(SynchronizationContext)) {
            MainContext = ctx;
            HasValidContext = true;
        }
        else {
            MainContext = null;
            HasValidContext = false;
        }
    }

    private static void EnsureMainContextCaptured() {
        if (MainThreadId != -1) return;

        lock (SyncLock) {
            if (MainThreadId != -1) return;

            MainThreadId = Environment.CurrentManagedThreadId;
            SynchronizationContext? ctx = SynchronizationContext.Current;

            if (ctx != null && ctx.GetType() != typeof(SynchronizationContext)) {
                MainContext = ctx;
                HasValidContext = true;
            }
            else {
                MainContext = null;
                HasValidContext = false;
            }
        }
    }
}
