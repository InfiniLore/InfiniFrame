// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameEvents {

    /// <summary>
    ///     Registers default event callbacks, such as closing child windows when the parent window closing is requested.
    /// </summary>
    public void AssignDefaultEventCallbacks() {
        EventsStore.WindowClosingRequested.Add(CloseChildWindows);
    }

    private void CloseChildWindows(IInfiniFrameWindow window) {
        if (window.LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed) return;

        if (window.Configuration is not InfiniFrameWindowConfiguration config) return;

        IInfiniFrameWindow[] childWindows;
        lock (config.ChildWindowsLock) {
            if (config.ChildWindowsInternal.Count <= 0) return;// No child windows to close

            childWindows = config.ChildWindowsInternal.ToArray();
            config.ChildWindowsInternal.Clear();
        }

        Logger.LogDebug("Lifecycle child windows");
        foreach (IInfiniFrameWindow childWindow in childWindows) {
            childWindow.Close();

            // Keep parent teardown ordered for cross-thread ownership scenarios.
            // If parent and child share a UI thread, do not block here to avoid deadlock.
            if (childWindow.ManagedThreadId == window.ManagedThreadId) continue;

            var timeout = Stopwatch.StartNew();
            while (!childWindow.Features.Lifecycle.IsClosedOrClosing() && timeout.Elapsed < TimeSpan.FromSeconds(5)) {
                Thread.Sleep(25);
            }

            if (!childWindow.Features.Lifecycle.IsClosedOrClosing()) {
                Logger.LogWarning(
                    "Timed out waiting for child window close. Parent={ParentWindowId}, Child={ChildWindowId}",
                    window.Id,
                    childWindow.Id);
            }
        }
    }
}
