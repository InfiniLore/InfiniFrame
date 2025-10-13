namespace InfiniLore.Photino.Blazor.Utils;
public sealed class CallbackTaskCompletionSource<TCallback, TResult>(TCallback callback) : TaskCompletionSource<TResult> {
    public TCallback Callback { get; } = callback;
}
