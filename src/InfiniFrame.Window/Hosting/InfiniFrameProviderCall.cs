// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Window.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Thread hygiene at the boundary between core and an external provider.</summary>
internal static class InfiniFrameProviderCall {
    /// <summary>
    ///     Starts a provider operation with no ambient <see cref="SynchronizationContext" />, so the provider's own awaits
    ///     never post back to a context that may be blocked waiting for them (for example Blazor's renderer context on
    ///     the core dispatcher, or an application UI thread blocked in <c>Run()</c>).
    /// </summary>
    internal static TResult Start<TResult>(Func<TResult> call) {
        SynchronizationContext? previous = SynchronizationContext.Current;
        if (previous is null) return call();
        SynchronizationContext.SetSynchronizationContext(null);
        try {
            return call();
        }
        finally {
            SynchronizationContext.SetSynchronizationContext(previous);
        }
    }

    /// <summary>
    ///     Resumes on a thread-pool thread. Awaited after a provider-completed task, it keeps core and application
    ///     continuations off the provider's thread (and out of the provider's call stack) even when the provider completes
    ///     its tasks synchronously or on a thread with its own synchronization context.
    /// </summary>
    internal static ThreadPoolHop ToThreadPool() => default;

    /// <summary>Requests cancellation without running callbacks on the calling (possibly provider) thread.</summary>
    internal static void CancelInBackground(CancellationTokenSource source, ILogger logger) {
        try {
            Task cancellation = source.CancelAsync();
            if (!cancellation.IsCompleted) _ = ObserveAsync(cancellation, logger);
            else if (cancellation.IsFaulted) logger.LogWarning(cancellation.Exception, "A cancellation callback failed");
        }
        catch (ObjectDisposedException) {
            // Already disposed: nothing left to cancel.
        }
    }

    private static async Task ObserveAsync(Task task, ILogger logger) {
        try {
            await task.ConfigureAwait(false);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            logger.LogWarning(exception, "A cancellation callback failed");
        }
    }

    internal readonly struct ThreadPoolHop : ICriticalNotifyCompletion {
        public ThreadPoolHop GetAwaiter() => this;
        public bool IsCompleted => false;
        public void GetResult() {}
        public void OnCompleted(Action continuation) => ThreadPool.QueueUserWorkItem(static state => ((Action)state!)(), continuation);
        public void UnsafeOnCompleted(Action continuation) => ThreadPool.UnsafeQueueUserWorkItem(static state => ((Action)state!)(), continuation);
    }
}
