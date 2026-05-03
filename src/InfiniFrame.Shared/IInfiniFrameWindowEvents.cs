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
    OrderedEvent WindowCreating { get; }
    OrderedEvent WindowCreated { get; }

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
