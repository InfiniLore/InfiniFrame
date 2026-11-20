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

    /// <summary>
    ///     Polls <paramref name="getValue" /> every 50 ms until the returned value equals
    ///     <paramref name="expected" />, then returns it.
    ///     Use this instead of <see cref="WaitForChangeAsync{T}" /> whenever the expected value is
    ///     known: a compositor can report an intermediate geometry while a resize is still being
    ///     applied, so a resize that is on its way shows up as a transient value first and
    ///     waiting only for "something changed" can observe that transient.
    ///     Returns the last observed value if <paramref name="timeout" /> expires, so the caller
    ///     can still assert on the value that was actually reached.
    /// </summary>
    public static async Task<T> WaitForValueAsync<T>(
        Func<T> getValue,
        T expected,
        TimeSpan timeout,
        CancellationToken ct = default
    ) {
        DateTime deadline = DateTime.UtcNow + timeout;
        T current = getValue();
        while (!EqualityComparer<T>.Default.Equals(current, expected)) {
            if (DateTime.UtcNow >= deadline)
                return current;

            await Task.Delay(50, ct);
            current = getValue();
        }

        return current;
    }
}
