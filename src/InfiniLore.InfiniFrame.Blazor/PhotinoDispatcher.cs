// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;

namespace InfiniLore.InfiniFrame.Blazor;
internal class PhotinoDispatcher : Dispatcher {
    private readonly PhotinoSynchronizationContext _context;

    public PhotinoDispatcher(PhotinoSynchronizationContext context) {
        _context = context;
        _context.UnhandledException += (_, e) => OnUnhandledException(e);
    }

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
