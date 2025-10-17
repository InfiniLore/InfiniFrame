// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniLore.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowEvents {
    event EventHandler<Point>? WindowLocationChanged;
    event EventHandler<Size>? WindowSizeChanged;
    event EventHandler? WindowFocusIn;
    event EventHandler? WindowMaximized;
    event EventHandler? WindowRestored;
    event EventHandler? WindowFocusOut;
    event EventHandler? WindowMinimized;
    event EventHandler<string>? WebMessageReceived;
    event EventHandler? WindowClosingRequested;
    event NetClosingDelegate? WindowClosing;
    event EventHandler? WindowCreating;
    event EventHandler? WindowCreated;

    IInfiniFrameWindowEvents DefineSender<T>(T sender) where T : class;

    void OnLocationChanged(int left, int top);
    void OnSizeChanged(int width, int height);
    void OnFocusIn();
    void OnMaximized();
    void OnRestored();
    void OnFocusOut();
    void OnMinimized();
    void OnWebMessageReceived(string message);
    void OnWindowClosingRequested();
    byte OnWindowClosing();
    void OnWindowCreating();
    void OnWindowCreated();
}
