// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.BlazorWebView.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal class SynchronousTaskScheduler : TaskScheduler {
    public override int MaximumConcurrencyLevel => 1;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override void QueueTask(Task task) 
        => TryExecuteTask(task);

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) 
        => TryExecuteTask(task);

    protected override IEnumerable<Task> GetScheduledTasks() 
        => [];
}
