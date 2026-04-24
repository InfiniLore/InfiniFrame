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
    InfiniFrameOrderedEvent<Point> WindowLocationChanged { get; }
    InfiniFrameOrderedEvent<Size> WindowSizeChanged { get; }
    InfiniFrameOrderedEvent WindowFocusIn { get; }
    InfiniFrameOrderedEvent WindowMaximized { get; }
    InfiniFrameOrderedEvent WindowRestored { get; }
    InfiniFrameOrderedEvent WindowFocusOut { get; }
    InfiniFrameOrderedEvent WindowMinimized { get; }
    InfiniFrameOrderedEvent<string> WebMessageReceived { get; }
    InfiniFrameOrderedEvent WindowClosingRequested { get; }
    InfiniFrameOrderedClosingEvent WindowClosing { get; }
    InfiniFrameOrderedEvent WindowCreating { get; }
    InfiniFrameOrderedEvent WindowCreated { get; }

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
