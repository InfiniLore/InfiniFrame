namespace InfiniLore.Photino.Blazor;
public sealed class PhotinoSynchronizationTaskCompletionSource<TCallback, TResult>(TCallback callback) : TaskCompletionSource<TResult> {
    public TCallback Callback { get; } = callback;
}
