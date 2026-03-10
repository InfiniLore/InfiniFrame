// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameSynchronizationState {

    #if NET9_0_OR_GREATER
    public readonly Lock Lock = new();
    #else
    public readonly object Lock = new();
    #endif

    public Task Task { get; set; } = Task.CompletedTask;

    public override string ToString() => $"{{ Busy: {!Task.IsCompleted}, Pending Task: {Task} }}";
}
