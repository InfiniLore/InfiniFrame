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

        // Keep delegate alive until execution completes
        NativeMacOsLibDispatch.MainThreadCallback? callbackRef = null;

        callbackRef = () => {
            _ = ExecuteOnMainThreadAsync();
        };

        NativeMacOsLibDispatch.DispatchOnMainThread(callbackRef);

        return new ValueTask(tcs.Task);

        async Task ExecuteOnMainThreadAsync() {
            SynchronizationContext? previousContext = SynchronizationContext.Current;

            try {
                SynchronizationContext.SetSynchronizationContext(
                    new MainThreadOnMacOsSynchronizationContext()
                );

                // IMPORTANT:
                // Execute directly on the main thread.
                // Do NOT use Task.Run here.
                await action();

                tcs.SetResult();
            }
            catch (Exception ex) {
                tcs.SetException(ex);
            }
            finally {
                SynchronizationContext.SetSynchronizationContext(previousContext);

                // Release delegate reference
                callbackRef = null;
            }
        }
    }
}