// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using TUnit.Core.Interfaces;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class UiThreadExecutor : ITestExecutor {
    public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> action) {
        if (OperatingSystem.IsWindows()) {
            // STA requirement
            var sta = new STAThreadExecutor();
            await sta.ExecuteTest(context, action);
            return;
        }

        if (OperatingSystem.IsMacOS()) {
#if MACOS
            var tcs = new TaskCompletionSource();

            CoreFoundation.CFRunLoop.PerformBlock(
                CoreFoundation.CFRunLoop.Main,
                CoreFoundation.CFRunLoopMode.Default,
                async () => {
                    try {
                        await action();
                        tcs.SetResult();
                    }
                    catch (Exception ex) {
                        tcs.SetException(ex);
                    }
                });

            CoreFoundation.CFRunLoop.WakeUp(CoreFoundation.CFRunLoop.Main);

            await tcs.Task;
            return;
#else
            await action();
            return;
#endif
        }

        // Linux / fallback
        await action();
    }
}