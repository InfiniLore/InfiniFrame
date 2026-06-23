// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a queued work item for the <see cref="InfiniFrameSynchronizationContext" />, including the callback,
///     optional execution context, and state.
/// </summary>
public class InfiniFrameSynchronizationWorkItem {
    /// <summary>The callback to invoke when the work item is executed.</summary>
    public SendOrPostCallback? Callback;
    /// <summary>The captured execution context to restore before invoking the callback.</summary>
    public ExecutionContext? ExecutionContext;
    /// <summary>The state object to pass to the callback.</summary>
    public object? StateObject;
    /// <summary>The synchronization context that owns this work item.</summary>
    public InfiniFrameSynchronizationContext? SynchronizationContext;
}
