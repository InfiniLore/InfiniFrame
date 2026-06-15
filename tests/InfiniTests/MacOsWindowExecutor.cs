// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using TUnit.Core.Interfaces;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class MacOsWindowExecutor : ITestExecutor {
    private static int MainThreadId { get; set; }
    private static SynchronizationContext? MainContext { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async ValueTask ExecuteTest(
        TestContext context,
        Func<ValueTask> action
    ) {
        if (!OperatingSystem.IsMacOS() || MainContext is null || Environment.CurrentManagedThreadId == MainThreadId) {
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
}
