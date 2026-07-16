// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal class InfiniFrameDispatcher : Dispatcher {
    private readonly InfiniFrameSynchronizationContext _context;

    public InfiniFrameDispatcher(InfiniFrameSynchronizationContext context) {
        _context = context;
        _context.UnhandledException += (_, e) => OnUnhandledException(e);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public override bool CheckAccess() => SynchronizationContext.Current == _context;

    public override Task InvokeAsync(Action workItem) {
        if (!CheckAccess()) return _context.InvokeAsync(workItem);

        workItem();
        return Task.CompletedTask;
    }

    public override Task InvokeAsync(Func<Task> workItem) => CheckAccess() ? workItem() : _context.InvokeAsync(workItem);

    public override Task<TResult> InvokeAsync<TResult>(Func<TResult> workItem) => CheckAccess() ? Task.FromResult(workItem()) : _context.InvokeAsync(workItem);

    public override Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> workItem) => CheckAccess() ? workItem() : _context.InvokeAsync(workItem);
}
