// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.Utilities;
using InfiniFrame.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

// Most UI platforms have a built-in SyncContext/Dispatcher, e.g., Windows Forms and WPF, which WebView
// can normally use directly. However, InfiniFrame currently doesn't.
//
// This is a duplicate of Microsoft.AspNetCore.Components.Rendering.RendererSynchronizationContextDispatcher,
// except that it also uses InfiniFrame's "Invoke" to ensure we're running on the correct thread to be able to
// interact with the unmanaged resources (the window and WebView).
//
// It might be that a simpler variant of this would work, for example, purely using InfiniFrame's "Invoke" and
// relying on that for single-threadedness. Maybe also in the future InfiniFrame could consider having its own
// built-in SyncContext/Dispatcher like other UI platforms.

// ReSharper disable once InvalidXmlDocComment
/// <summary>
///     Provides a <see cref="SynchronizationContext" /> for Blazor components running inside an InfiniFrame WebView.
///     It ensures work items are dispatched on the native window thread via <see cref="IInfiniFrameWindow.Invoke" />,
///     enabling safe interaction with unmanaged WebView resources.
/// </summary>
/// <param name="provider">The service provider used to resolve the <see cref="IInfiniFrameWindow" />.</param>
/// <param name="state">An optional shared synchronization state; if omitted a new instance is created.</param>
public class InfiniFrameSynchronizationContext(IServiceProvider provider, InfiniFrameSynchronizationState? state = null) : SynchronizationContext {
    // ReSharper disable once ConvertClosureToMethodGroup
    private Lazy<IInfiniFrameWindow> LazyWindow { get; } = new(() => provider.GetRequiredService<IInfiniFrameWindow>());

    private readonly InfiniFrameSynchronizationState _state = state ?? new InfiniFrameSynchronizationState();

    /// <summary>Raised when an unhandled exception occurs during work item execution.</summary>
    public event UnhandledExceptionEventHandler? UnhandledException;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Invokes the specified action on the synchronization context, executing synchronously if possible.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A task that completes when the action has been executed.</returns>
    public Task InvokeAsync(Action action) {
        var completion = new CallbackTaskCompletionSource<Action, object>(action);

        ExecuteSynchronouslyIfPossible(d: static state => {
            if (state is not CallbackTaskCompletionSource<Action, object> completion) return;

            try {
                completion.Callback();
                completion.SetResult(null!);
            }
            catch (OperationCanceledException exception) {
                completion.TrySetCanceled(exception.CancellationToken);
            }
            catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                completion.TrySetException(exception);
            }
        }, completion);

        return completion.Task;
    }

    /// <summary>
    ///     Invokes the specified asynchronous function on the synchronization context.
    /// </summary>
    /// <param name="asyncAction">The asynchronous function to execute.</param>
    /// <returns>A task that completes when the function has been executed.</returns>
    public Task InvokeAsync(Func<Task> asyncAction) {
        var completion = new CallbackTaskCompletionSource<Func<Task>, object>(asyncAction);

        ExecuteSynchronouslyIfPossible(d: static state => {
            if (state is CallbackTaskCompletionSource<Func<Task>, object> completion)
                _ = CompleteAsync(completion);
        }, completion);

        return completion.Task;
    }

    /// <summary>
    ///     Invokes the specified function on the synchronization context and returns its result.
    /// </summary>
    /// <param name="function">The function to execute.</param>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <returns>A task that yields the function result.</returns>
    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function) {
        var completion = new CallbackTaskCompletionSource<Func<TResult>, TResult>(function);

        ExecuteSynchronouslyIfPossible(d: static state => {
            if (state is not CallbackTaskCompletionSource<Func<TResult>, TResult> completion) return;

            try {
                TResult result = completion.Callback();
                completion.SetResult(result);
            }
            catch (OperationCanceledException exception) {
                completion.TrySetCanceled(exception.CancellationToken);
            }
            catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                completion.TrySetException(exception);
            }
        }, completion);

        return completion.Task;
    }

    /// <summary>
    ///     Invokes the specified asynchronous function on the synchronization context and returns its result.
    /// </summary>
    /// <param name="asyncFunction">The asynchronous function to execute.</param>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <returns>A task that yields the function result.</returns>
    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> asyncFunction) {
        var completion = new CallbackTaskCompletionSource<Func<Task<TResult>>, TResult>(asyncFunction);

        ExecuteSynchronouslyIfPossible(d: static state => {
            if (state is CallbackTaskCompletionSource<Func<Task<TResult>>, TResult> completion)
                _ = CompleteAsync(completion);
        }, completion);

        return completion.Task;
    }

    // asynchronously runs the callback
    //
    // NOTE: this must always run async. It's not legal here to execute the work item synchronously.
    /// <summary>Dispatches an asynchronous message to the synchronization context.</summary>
    /// <param name="d">The callback to invoke.</param>
    /// <param name="state">The state object passed to the callback.</param>
    public override void Post(SendOrPostCallback d, object? state) {
        lock (_state.Lock) {
            _state.Task = Enqueue(_state.Task, d, state, true);
        }
    }

    // synchronously runs the callback
    /// <summary>Dispatches a synchronous message to the synchronization context, blocking until complete.</summary>
    /// <param name="d">The callback to invoke.</param>
    /// <param name="state">The state object passed to the callback.</param>
    public override void Send(SendOrPostCallback d, object? state) {
        if (Environment.CurrentManagedThreadId == LazyWindow.Value.ManagedThreadId) {
            throw new InvalidOperationException(
                "InfiniFrameSynchronizationContext.Send cannot be called from the native UI thread " +
                "as it would cause a deadlock. Use Post or InvokeAsync instead.");
        }

        Task antecedent;
        var completion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

        lock (_state.Lock) {
            antecedent = _state.Task;
            _state.Task = completion.Task;
        }

        // We have to block. That's the contract of Send - we don't expect this to be used
        // in many scenarios in Components.
        //
        // Using GetAwaiter().GetResult() here to preserve the original exception type
        // instead of wrapping it in an AggregateException, consistent with all other
        // blocking patterns in this codebase. The antecedent task never throws.
        antecedent.GetAwaiter().GetResult();

        ExecuteSynchronously(completion, d, state);
    }

    // shallow copy
    /// <summary>Creates a shallow copy of this synchronization context sharing the same state.</summary>
    /// <returns>A new <see cref="InfiniFrameSynchronizationContext" /> instance sharing the synchronization state.</returns>
    public override SynchronizationContext CreateCopy() {
        lock (_state.Lock) {
            return new InfiniFrameSynchronizationContext(provider, _state);
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
            completion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            _state.Task = completion.Task;
        }

        ExecuteSynchronously(completion, d, state);
    }

    private static void ExecutionContextThunk(object? state) {
        if (state is not InfiniFrameSynchronizationWorkItem item) return;

        item.SynchronizationContext?.ExecuteSynchronously(null, item.Callback, item.StateObject);
    }

    private static void BackgroundWorkThunk(Task antecedent, object? state) {
        if (state is not InfiniFrameSynchronizationWorkItem item) return;

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
        return antecedent.ContinueWith(BackgroundWorkThunk, new InfiniFrameSynchronizationWorkItem {
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
        // Anything run on the sync context should actually be dispatched as far as InfiniFrame
        // is concerned, so that it's safe to interact with the native window/WebView.
        Exception? callbackException = null;

        void ExecuteCallback() {
            SynchronizationContext? original = Current;
            try {
                SetSynchronizationContext(this);
                d?.Invoke(state);
                completion?.TrySetResult(null!);
            }
            catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                callbackException = exception;
                if (completion is not null)
                    completion.TrySetException(exception);
                else
                    throw;
            }
            finally {
                SetSynchronizationContext(original);
            }
        }

        InfiniFrameDispatchResult result = LazyWindow.Value.Features.Invoke.Invoke(ExecuteCallback);
        if (result == InfiniFrameDispatchResult.WindowClosed) {
            // Renderer disposal is scheduled after the native window has closed. There is no UI thread left to
            // dispatch to, but the serialized callback must still run or Blazor's DisposeAsync never completes.
            ExecuteCallback();
            return;
        }

        if (result == InfiniFrameDispatchResult.Completed) return;

        Exception dispatchException = callbackException ?? new InvalidOperationException(
            $"Could not execute an InfiniFrame synchronization callback. Dispatch result: {result}."
        );
        if (completion is not null)
            completion.TrySetException(dispatchException);
        else
            DispatchException(dispatchException);
    }

    private void ExecuteBackground(InfiniFrameSynchronizationWorkItem item) {
        if (item.ExecutionContext is null) {
            try {
                ExecuteSynchronously(null, item.Callback, item.StateObject);
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                DispatchException(ex);
            }

            return;
        }

        // Perf - using a static thunk here to avoid a delegate allocation.
        try {
            ExecutionContext.Run(item.ExecutionContext, ExecutionContextThunk, item);
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            DispatchException(ex);
        }
    }

    private void DispatchException(Exception ex) {
        UnhandledExceptionEventHandler? handler = UnhandledException;
        handler?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
    }

    private static async Task CompleteAsync(CallbackTaskCompletionSource<Func<Task>, object> completion) {
        try {
            await completion.Callback().ConfigureAwait(false);
            completion.TrySetResult(null!);
        }
        catch (OperationCanceledException exception) {
            completion.TrySetCanceled(exception.CancellationToken);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            completion.TrySetException(exception);
        }
    }

    private static async Task CompleteAsync<TResult>(CallbackTaskCompletionSource<Func<Task<TResult>>, TResult> completion) {
        try {
            TResult result = await completion.Callback().ConfigureAwait(false);
            completion.TrySetResult(result);
        }
        catch (OperationCanceledException exception) {
            completion.TrySetCanceled(exception.CancellationToken);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            completion.TrySetException(exception);
        }
    }
}