// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Holds the shared synchronization state for an <see cref="InfiniFrameSynchronizationContext" />, including the lock
///     and the current task chain used to serialize work item execution.
/// </summary>
public class InfiniFrameSynchronizationState {
#if NET9_0_OR_GREATER
    /// <summary>Synchronization lock for thread-safe access to pending task state.</summary>
    public readonly Lock Lock = new();
#else
    /// <summary>Synchronization lock for thread-safe access to pending task state.</summary>
    public readonly object Lock = new();
#endif

    /// <summary>Gets or sets the tail of the task chain used to serialize work items.</summary>
    public Task Task { get; set; } = Task.CompletedTask;

    /// <summary>Returns a string representation of the current synchronization state.</summary>
    public override string ToString()
        => $"{{ Busy: {!Task.IsCompleted}, Pending Task: {Task.Id} }}";
}