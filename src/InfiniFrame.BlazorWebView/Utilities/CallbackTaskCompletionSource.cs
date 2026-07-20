// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.BlazorWebView.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     A <see cref="TaskCompletionSource{TResult}" /> that also holds a callback delegate, used to complete the task
///     when the callback is executed.
/// </summary>
/// <typeparam name="TCallback">The type of the callback delegate.</typeparam>
/// <typeparam name="TResult">The type of the task result.</typeparam>
public sealed class CallbackTaskCompletionSource<TCallback, TResult>(TCallback callback)
    : TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously)
{
    /// <summary>Gets the callback delegate that produces the task result.</summary>
    public TCallback Callback { get; } = callback;
}
