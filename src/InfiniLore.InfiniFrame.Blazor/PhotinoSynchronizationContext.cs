// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using InfiniLore.InfiniFrame.NET;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniLore.InfiniFrame.Blazor;
using InfiniLore.InfiniFrame.Blazor.Utils;

// Most UI platforms have a built-in SyncContext/Dispatcher, e.g., Windows Forms and WPF, which WebView
// can normally use directly. However, Photino currently doesn't.
//
// This is a duplicate of Microsoft.AspNetCore.Components.Rendering.RendererSynchronizationContextDispatcher,
// except that it also uses Photino's "Invoke" to ensure we're running on the correct thread to be able to
// interact with the unmanaged resources (the window and WebView).
//
// It might be that a simpler variant of this would work, for example, purely using Photino's "Invoke" and
// relying on that for single-threadedness. Maybe also in the future Photino could consider having its own
// built-in SyncContext/Dispatcher like other UI platforms.

public class PhotinoSynchronizationContext(IServiceProvider provider, PhotinoSynchronizationState? state = null) : SynchronizationContext {
    private readonly PhotinoSynchronizationState _state = state ?? new PhotinoSynchronizationState();
    private Lazy<IPhotinoWindow> LazyWindow { get; } = new(provider.GetRequiredService<IPhotinoWindow>);

    public event UnhandledExceptionEventHandler? UnhandledException;

    public Task InvokeAsync(Action action) {
        var completion = new CallbackTaskCompletionSource<Action, object>(action);
        ExecuteSynchronouslyIfPossible(d: static state => {
            if (state is not CallbackTaskCompletionSource<Action, object> completion) return;

            try {
                completion.Callback();
                completion.SetResult(null!);
            }
            catch (OperationCanceledException) {
                completion.SetCanceled();
            }
            catch (Exception exception) {
                completion.SetException(exception);
            }
        }, completion);

        return completion.Task;
    }

    public Task InvokeAsync(Func<Task> asyncAction) {
        var completion = new CallbackTaskCompletionSource<Func<Task>, object>(asyncAction);
        // ReSharper disable once AsyncVoidMethod
        ExecuteSynchronouslyIfPossible(d: static async void (state) => {
            if (state is not CallbackTaskCompletionSource<Func<Task>, object> completion) return;

            try {
                await completion.Callback();
                completion.SetResult(null!);
            }
            catch (OperationCanceledException) {
                completion.SetCanceled();
            }
            catch (Exception exception) {
                completion.SetException(exception);
            }
        }, completion);

        return completion.Task;
    }

    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function) {
        var completion = new CallbackTaskCompletionSource<Func<TResult>, TResult>(function);
        ExecuteSynchronouslyIfPossible(d: static state => {
            if (state is not CallbackTaskCompletionSource<Func<TResult>, TResult> completion) return;

            try {
                TResult result = completion.Callback();
                completion.SetResult(result);
            }
            catch (OperationCanceledException) {
                completion.SetCanceled();
            }
            catch (Exception exception) {
                completion.SetException(exception);
            }
        }, completion);

        return completion.Task;
    }

    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> asyncFunction) {
        var completion = new CallbackTaskCompletionSource<Func<Task<TResult>>, TResult>(asyncFunction);
        // ReSharper disable once AsyncVoidMethod
        ExecuteSynchronouslyIfPossible(d: static async void (state) => {
            if (state is not CallbackTaskCompletionSource<Func<Task<TResult>>, TResult> completion) return;

            try {
                TResult result = await completion.Callback();
                completion.SetResult(result);
            }
            catch (OperationCanceledException) {
                completion.SetCanceled();
            }
            catch (Exception exception) {
                completion.SetException(exception);
            }
        }, completion);

        return completion.Task;
    }

    // asynchronously runs the callback
    //
    // NOTE: this must always run async. It's not legal here to execute the work item synchronously.
    public override void Post(SendOrPostCallback d, object? state) {
        lock (_state.Lock) {
            _state.Task = Enqueue(_state.Task, d, state, true);
        }
    }

    // synchronously runs the callback
    public override void Send(SendOrPostCallback d, object? state) {
        Task antecedent;
        var completion = new TaskCompletionSource<object>();

        lock (_state.Lock) {
            antecedent = _state.Task;
            _state.Task = completion.Task;
        }

        // We have to block. That's the contract of Send - we don't expect this to be used
        // in many scenarios in Components.
        //
        // Using Wait here is ok because the antecedent task will never throw.
        antecedent.Wait();

        ExecuteSynchronously(completion, d, state);
    }

    // shallow copy
    public override SynchronizationContext CreateCopy() {
        lock (_state.Lock) {
            return new PhotinoSynchronizationContext(provider, _state);
        }
    }

    // Similar to Post, but it can run the work item synchronously if the context is not busy.
    //
    // This is the main code path used by components, we want to be able to run async work but only dispatch
    // if necessary.
    private void ExecuteSynchronouslyIfPossible(SendOrPostCallback d, object state) {
        TaskCompletionSource<object> completion;
        lock (_state.Lock) {
            if (!_state.Task.IsCompleted) {
                _state.Task = Enqueue(_state.Task, d, state);
                return;
            }

            // We can execute this synchronously because nothing is currently running
            // or queued.
            completion = new TaskCompletionSource<object>();
            _state.Task = completion.Task;
        }

        ExecuteSynchronously(completion, d, state);
    }

    private static void ExecutionContextThunk(object? state) {
        if (state is not PhotinoSynchronizationWorkItem item) return;

        item.SynchronizationContext?.ExecuteSynchronously(null, item.Callback, item.StateObject);
    }

    private static void BackgroundWorkThunk(Task antecedent, object? state) {
        if (state is not PhotinoSynchronizationWorkItem item) return;

        item.SynchronizationContext?.ExecuteBackground(item);
    }

    private Task Enqueue(Task antecedent, SendOrPostCallback d, object? state, bool forceAsync = false) {
        // If we get here, it means that a callback is being explicitly queued. Let's instead add it to the queue and yield.
        //
        // We use our own queue here to maintain the execution order of the callbacks scheduled here. Also,
        // we need a queue rather than just scheduling an item in the thread pool - those items would immediately
        // block and hurt scalability.
        //
        // We need to capture the execution context so we can restore it later. This code is similar to
        // the call path of ThreadPool.QueueUserWorkItem and System.Threading.QueueUserWorkItemCallback.
        ExecutionContext? executionContext = null;
        if (!ExecutionContext.IsFlowSuppressed()) {
            executionContext = ExecutionContext.Capture();
        }

        TaskContinuationOptions flags = forceAsync ? TaskContinuationOptions.RunContinuationsAsynchronously : TaskContinuationOptions.None;
        return antecedent.ContinueWith(BackgroundWorkThunk, new PhotinoSynchronizationWorkItem {
            SynchronizationContext = this,
            ExecutionContext = executionContext,
            Callback = d,
            StateObject = state
        }, CancellationToken.None, flags, TaskScheduler.Current);
    }

    private void ExecuteSynchronously(
        TaskCompletionSource<object>? completion,
        SendOrPostCallback? d,
        object? state
    ) {
        // Anything run on the sync context should actually be dispatched as far as Photino
        // is concerned, so that it's safe to interact with the native window/WebView.
        LazyWindow.Value.Invoke(() => {
            SynchronizationContext? original = Current;
            try {
                SetSynchronizationContext(this);
                d?.Invoke(state);
            }
            finally {
                SetSynchronizationContext(original);

                completion?.SetResult(null!);
            }
        });
    }

    private void ExecuteBackground(PhotinoSynchronizationWorkItem item) {
        if (item.ExecutionContext is null) {
            try {
                ExecuteSynchronously(null, item.Callback, item.StateObject);
            }
            catch (Exception ex) {
                DispatchException(ex);
            }

            return;
        }

        // Perf - using a static thunk here to avoid a delegate allocation.
        try {
            ExecutionContext.Run(item.ExecutionContext, ExecutionContextThunk, item);
        }
        catch (Exception ex) {
            DispatchException(ex);
        }
    }

    private void DispatchException(Exception ex) {
        UnhandledExceptionEventHandler? handler = UnhandledException;
        handler?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
    }
}
