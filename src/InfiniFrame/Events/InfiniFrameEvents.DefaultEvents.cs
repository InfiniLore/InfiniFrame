// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameEvents {

    public void AssignDefaultEventCallbacks() {
        EventsStore.WindowClosingRequested.Add(CloseChildWindows);
    }
    
    private static void CloseChildWindows(IInfiniFrameWindow window) {
        if (window.InstanceHandle == IntPtr.Zero) return; // Window already closed

        IInfiniFrameWindow[] childWindows;
        lock (window.Configuration.ChildWindows) {
            if (window.Configuration.ChildWindows.Count <= 0) return; // No child windows to close
            childWindows = window.Configuration.ChildWindows.ToArray();
            window.Configuration.ChildWindows.Clear();
        }

        window.Logger.LogDebug("Lifecycle child windows");
        foreach (IInfiniFrameWindow childWindow in childWindows) {
            childWindow.Close();

            // Keep parent teardown ordered for cross-thread ownership scenarios.
            // If parent and child share a UI thread, do not block here to avoid deadlock.
            if (childWindow.ManagedThreadId == window.ManagedThreadId) continue;

            var timeout = Stopwatch.StartNew();
            while (!childWindow.IsClosed && timeout.Elapsed < TimeSpan.FromSeconds(5)) {
                Thread.Sleep(25);
            }

            if (!childWindow.IsClosed) {
                window.Logger.LogWarning(
                    "Timed out waiting for child window close. Parent={ParentWindowId}, Child={ChildWindowId}",
                    window.Id,
                    childWindow.Id);
            }
        }
    }
}
