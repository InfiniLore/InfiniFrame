namespace InfiniLore.Photino.Blazor;
public class PhotinoSynchronizationWorkItem {
    public SendOrPostCallback? Callback;
    public ExecutionContext? ExecutionContext;
    public object? StateObject;
    public PhotinoSynchronizationContext? SynchronizationContext;
}
