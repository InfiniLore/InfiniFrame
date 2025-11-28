// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
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

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    IInfiniFrameWindowEvents DefineSender<T>(T sender) where T : class;

    void InvokeOnLocationChanged(int left, int top);
    void InvokeOnSizeChanged(int width, int height);
    void InvokeOnFocusIn();
    void InvokeOnMaximized();
    void InvokeOnRestored();
    void InvokeOnFocusOut();
    void InvokeOnMinimized();
    void InvokeOnWebMessageReceived(string message);
    void InvokeOnWindowClosingRequested();
    byte InvokeOnWindowClosing();
    void InvokeOnWindowCreating();
    void InvokeOnWindowCreated();
}
