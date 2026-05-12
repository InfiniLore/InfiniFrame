// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;

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

        window.Logger.LogDebug("Closing child windows");
        foreach (IInfiniFrameWindow childWindow in childWindows) {
            // Child windows created via SetParentWindow are now natively parented on each platform.
            // Avoid issuing a second close request while native parent teardown is already in progress.
            if (ReferenceEquals(childWindow.Configuration.ParentWindow, window))
                continue;

            childWindow.Close();
        }
    }
}
