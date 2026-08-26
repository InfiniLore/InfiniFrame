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
internal sealed class InfiniFrameSynchronizationWorkItem {
    /// <summary>The callback to invoke when the work item is executed.</summary>
    internal SendOrPostCallback? Callback;
    /// <summary>The captured execution context to restore before invoking the callback.</summary>
    internal ExecutionContext? ExecutionContext;
    /// <summary>The state object to pass to the callback.</summary>
    internal object? StateObject;
    /// <summary>The synchronization context that owns this work item.</summary>
    internal InfiniFrameSynchronizationContext? SynchronizationContext;
}
