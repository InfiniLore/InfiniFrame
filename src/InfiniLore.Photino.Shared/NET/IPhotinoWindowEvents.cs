// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniLore.Photino.NET;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IPhotinoWindowEvents {
    event EventHandler<Point>? WindowLocationChanged;
    event EventHandler<Size>? WindowSizeChanged;
    event EventHandler? WindowFocusIn;
    event EventHandler? WindowMaximized;
    event EventHandler? WindowRestored;
    event EventHandler? WindowFocusOut;
    event EventHandler? WindowMinimized;
    event EventHandler<string>? WebMessageReceived;
    event NetClosingDelegate? WindowClosing;
    event EventHandler? WindowCreating;
    event EventHandler? WindowCreated;
    
    void OnLocationChanged(int left, int top);
    void OnSizeChanged(int width, int height);
    void OnFocusIn();
    void OnMaximized();
    void OnRestored();
    void OnFocusOut();
    void OnMinimized();
    void OnWebMessageReceived(string message);
    byte OnWindowClosing();
    void OnWindowCreating();
    void OnWindowCreated();
}
