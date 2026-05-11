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
        if (window.InstanceHandle == IntPtr.Zero) return;
        if (window.Configuration.ChildWindows.Count <= 0) return;

        window.Logger.LogDebug("Closing child windows");
        foreach (IInfiniFrameWindow childWindow in window.Configuration.ChildWindows) {
            childWindow.Close();
        }
        window.Configuration.ChildWindows.Clear();
        
    }
}
