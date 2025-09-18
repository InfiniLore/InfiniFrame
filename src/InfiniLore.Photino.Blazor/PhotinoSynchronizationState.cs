namespace InfiniLore.Photino.Blazor;

public class PhotinoSynchronizationState
{
    public readonly Lock Lock = new Lock();
    public Task Task { get; set; } = Task.CompletedTask;

    public override string ToString()
    {
        return $"{{ Busy: {!Task.IsCompleted}, Pending Task: {Task} }}";
    }
}
