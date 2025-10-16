namespace InfiniLore.InfiniFrame.Blazor;
public class PhotinoSynchronizationState {
    public readonly Lock Lock = new();
    public Task Task { get; set; } = Task.CompletedTask;

    public override string ToString() => $"{{ Busy: {!Task.IsCompleted}, Pending Task: {Task} }}";
}
