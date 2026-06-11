// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame.Debugging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameEventsStore {
    OrderedEvent<Point> WindowLocationChanged { get; }
    OrderedEvent<Size> WindowSizeChanged { get; }
    OrderedEvent WindowFocusIn { get; }
    OrderedEvent WindowMaximized { get; }
    OrderedEvent WindowRestored { get; }
    OrderedEvent WindowFocusOut { get; }
    OrderedEvent WindowMinimized { get; }
    OrderedEvent<InfiniFrameWebMessageReceivedEvent> WebMessageReceived { get; }
    OrderedEvent<InfiniFrameDebugEventArgs> DebuggingEvent { get; }
    
    KeyedEvent<string, string?> WebMessagePostData { get; }
    KeyedResultEvent<string, string?, string?> WebMessageGetData { get; }
    
    OrderedEvent WindowClosingRequested { get; }
    OrderedResultEvent<EventArgs?, WindowClosingResult> Closing { get; }
    OrderedEvent WindowClosed { get; }
    OrderedEvent WindowCreating { get; }
    OrderedEvent WindowCreated { get; }
    KeyedResultEvent<string, string, (Stream? Data, string? ContentType)> CustomScheme { get; }
    
    void CopyTo(IInfiniFrameEventsStore eventsStore);
}
