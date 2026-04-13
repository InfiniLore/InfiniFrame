// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class SingleThreadSyncContext(BlockingCollection<Func<Task>> queue) : SynchronizationContext {
    public override void Post(SendOrPostCallback callback, object? state) {
        queue.Add(() => {
            callback(state);
            return Task.CompletedTask;
        });
    }

    public override void Send(SendOrPostCallback callback, object? state) {
        callback(state);
    }
}