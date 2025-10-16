// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameSynchronizationWorkItem {
    public SendOrPostCallback? Callback;
    public ExecutionContext? ExecutionContext;
    public object? StateObject;
    public InfiniFrameSynchronizationContext? SynchronizationContext;
}
