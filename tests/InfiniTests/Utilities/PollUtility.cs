// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class PollUtility {
    public static TaskCompletionSource<bool> CreateSignal()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    public static Task WaitForSignalAsync(
        TaskCompletionSource<bool> signal,
        TimeSpan timeout,
        CancellationToken ct = default
    ) => signal.Task.WaitAsync(timeout, ct);

    /// <summary>
    ///     Polls <paramref name="getValue" /> every 50 ms until the returned value differs from
    ///     <paramref name="fromValue" />, then returns the new value.
    ///     Returns immediately if the value already differs at the time of the call (handles events
    ///     that fire synchronously during the act step).
    ///     Throws <see cref="TimeoutException" /> if no change is observed within <paramref name="timeout" />.
    /// </summary>
    public static async Task<T> WaitForChangeAsync<T>(
        Func<T> getValue,
        T fromValue,
        TimeSpan timeout,
        CancellationToken ct = default
    ) {
        DateTime deadline = DateTime.UtcNow + timeout;
        while (true) {
            T current = getValue();
            if (!EqualityComparer<T>.Default.Equals(current, fromValue))
                return current;

            if (DateTime.UtcNow >= deadline)
                throw new TimeoutException($"Value did not change from {fromValue} within {timeout}.");

            await Task.Delay(50, ct);
        }
    }
}
