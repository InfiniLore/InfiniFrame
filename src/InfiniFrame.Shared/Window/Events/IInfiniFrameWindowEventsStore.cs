// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowEventsStore {
    OrderedWindowEvent<Point> WindowLocationChanged { get; }
    OrderedWindowEvent<Size> WindowSizeChanged { get; }
    OrderedWindowEvent WindowFocusIn { get; }
    OrderedWindowEvent WindowMaximized { get; }
    OrderedWindowEvent WindowRestored { get; }
    OrderedWindowEvent WindowFocusOut { get; }
    OrderedWindowEvent WindowMinimized { get; }
    OrderedWindowEvent<string> WebMessageReceived { get; }
    
    KeyedWindowEvent<string, string?> WebMessagePostData { get; }
    KeyedWindowResultEvent<string, string?, string?> WebMessageGetData { get; }
    
    OrderedWindowEvent WindowClosingRequested { get; }
    OrderedWindowResultEvent<EventArgs?, WindowClosingResult> WindowClosing { get; }
    OrderedWindowEvent WindowClosed { get; }
    OrderedWindowEvent WindowCreating { get; }
    OrderedWindowEvent WindowCreated { get; }
    KeyedWindowResultEvent<string, string, (Stream? Data, string? ContentType)> CustomScheme { get; }

    IInfiniFrameWindowEventsStore DeepCopy();
}
