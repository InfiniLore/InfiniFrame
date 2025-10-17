namespace InfiniLore.InfiniFrame.Blazor.Utils;
internal class SynchronousTaskScheduler : TaskScheduler {
    public override int MaximumConcurrencyLevel => 1;

    protected override void QueueTask(Task task) {
        TryExecuteTask(task);
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => TryExecuteTask(task);

    protected override IEnumerable<Task> GetScheduledTasks() => Enumerable.Empty<Task>();
}
