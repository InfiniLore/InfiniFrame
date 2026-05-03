// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowEvents {
    OrderedEvent<Point> WindowLocationChanged { get; }
    OrderedEvent<Size> WindowSizeChanged { get; }
    OrderedEvent WindowFocusIn { get; }
    OrderedEvent WindowMaximized { get; }
    OrderedEvent WindowRestored { get; }
    OrderedEvent WindowFocusOut { get; }
    OrderedEvent WindowMinimized { get; }
    OrderedEvent<string> WebMessageReceived { get; }
    OrderedEvent WindowClosingRequested { get; }
    OrderedResultEvent<EventArgs?, bool> WindowClosing { get; }
    OrderedEvent WindowClosed { get; }
    OrderedEvent WindowCreating { get; }
    OrderedEvent WindowCreated { get; }

    void CompleteSetup(IInfiniFrameWindow sender);

    void OnFocusIn();
    void OnFocusOut();
    void OnLocationChanged(int left, int top);
    void OnMaximized();
    void OnMinimized();
    void OnRestored();
    void OnSizeChanged(int width, int height);
    void OnWebMessageReceived(string message);
    void OnWindowClosed();
    byte OnWindowClosing();
    void OnWindowClosingRequested();
    void OnWindowCreated();
    void OnWindowCreating();
}
