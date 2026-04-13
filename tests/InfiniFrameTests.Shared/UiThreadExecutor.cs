// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using TUnit.Core.Interfaces;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class UiThreadExecutor : ITestExecutor {
    public ValueTask ExecuteTest(TestContext context, Func<ValueTask> action) {
        if (OperatingSystem.IsWindows()) {
            // STA requirement
            var sta = new STAThreadExecutor();
            return sta.ExecuteTest(context, action);
        }

        // ReSharper disable once InvertIf
        if (OperatingSystem.IsMacOS()) {
            var contextQueue = new BlockingCollection<Func<Task>>();
            var tcs = new TaskCompletionSource();

            var thread = new Thread(() => {
                SynchronizationContext.SetSynchronizationContext(new SingleThreadSyncContext(contextQueue));

                try {
                    Task task = action().AsTask();
                    task.ContinueWith(t => {
                        if (t.IsFaulted)
                            tcs.SetException(t.Exception!);
                        else
                            tcs.SetResult();
                    });

                    foreach (var work in contextQueue.GetConsumingEnumerable()) {
                        work().GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex) {
                    tcs.SetException(ex);
                }
            }) {
                IsBackground = true
            };

            thread.Start();

            return new ValueTask(tcs.Task);
        }
        
        // Linux / fallback
        return action();
    }
}