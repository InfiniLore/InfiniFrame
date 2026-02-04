// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowEvents {
    event Action<IInfiniFrameWindow, Point>? WindowLocationChanged;
    event Action<IInfiniFrameWindow, Size>? WindowSizeChanged;
    event Action<IInfiniFrameWindow>? WindowFocusIn;
    event Action<IInfiniFrameWindow>? WindowMaximized;
    event Action<IInfiniFrameWindow>? WindowRestored;
    event Action<IInfiniFrameWindow>? WindowFocusOut;
    event Action<IInfiniFrameWindow>? WindowMinimized;
    event Action<IInfiniFrameWindow, string>? WebMessageReceived;
    event Action<IInfiniFrameWindow>? WindowClosingRequested;
    event NetClosingDelegate? WindowClosing;
    event Action<IInfiniFrameWindow>? WindowCreating;
    event Action<IInfiniFrameWindow>? WindowCreated;

    void CompleteSetup(IInfiniFrameWindow sender);

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
