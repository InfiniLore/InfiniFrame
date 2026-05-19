// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.ExceptionServices;

namespace InfiniFrameTests.Shared.TestExecutors;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// <summary>
/// SynchronizationContext that marshals continuations
/// onto the macOS main dispatch queue.
/// </summary>
internal sealed class MainThreadOnMacOsSynchronizationContext : SynchronizationContext {
    public override void Post(SendOrPostCallback callback, object? state) {
        NativeMacOsLibDispatch.DispatchOnMainThread(() => callback(state));
    }

    public override void Send(SendOrPostCallback callback, object? state) {
        if (NativeMacOsLibDispatch.IsMainThread()) {
            callback(state);
            return;
        }

        using var resetEvent = new ManualResetEventSlim(false);

        Exception? capturedException = null;

        NativeMacOsLibDispatch.DispatchOnMainThread(() => {
            try {
                callback(state);
            }
            catch (Exception ex) {
                capturedException = ex;
            }
            finally {
                // ReSharper disable once AccessToDisposedClosure
                resetEvent.Set();
            }
        });

        resetEvent.Wait();

        if (capturedException is not null) {
            ExceptionDispatchInfo.Capture(capturedException).Throw();
        }
    }
}
