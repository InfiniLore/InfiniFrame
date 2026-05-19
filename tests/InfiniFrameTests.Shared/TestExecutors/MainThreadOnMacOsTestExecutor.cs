// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using TUnit.Core.Interfaces;

namespace InfiniFrameTests.Shared.TestExecutors;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MainThreadOnMacOsTestExecutor : ITestExecutor {
    private static readonly Lazy<bool> IsMacOs = new(() =>
        OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst()
    );

    public ValueTask ExecuteTest(TestContext context, Func<ValueTask> action) {
        if (!IsMacOs.Value || NativeMacOsLibDispatch.IsMainThread()) {
            return action();
        }

        var tcs = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        NativeMacOsLibDispatch.MainThreadCallback? callbackRef = null;

        callbackRef = () => {
            try {
                // IMPORTANT:
                // Execute synchronously on the actual macOS main thread.
                // Do NOT use async continuations or SynchronizationContext here.
                action().GetAwaiter().GetResult();

                tcs.SetResult();
            }
            catch (Exception ex) {
                tcs.SetException(ex);
            }
            finally {
                callbackRef = null;
            }
        };

        NativeMacOsLibDispatch.DispatchOnMainThread(callbackRef);

        return new ValueTask(tcs.Task);
    }
}
