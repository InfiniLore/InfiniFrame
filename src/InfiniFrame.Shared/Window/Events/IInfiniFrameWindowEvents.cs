// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowEvents : IHasInfiniFrameWindowEventsStore {
    void OnFocusIn();
    void OnFocusOut();
    void OnLocationChanged(int left, int top);
    void OnMaximized();
    void OnMinimized();
    void OnRestored();
    void OnSizeChanged(int width, int height);
    void OnWebMessageReceived(string message, string? origin = null);
    void OnWindowClosed();
    byte OnWindowClosing();
    void OnWindowClosingRequested();
    void OnWindowCreated();
    void OnWindowCreating();
    IntPtr OnCustomScheme(string url, out int numBytes, out string? contentType);
}
