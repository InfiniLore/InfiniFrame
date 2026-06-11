// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame.Debugging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record InfiniFrameEventsStore : IInfiniFrameEventsStore {
    public OrderedEvent<Point> WindowLocationChanged { get; } = new();
    public OrderedEvent<Size> WindowSizeChanged { get; } = new();
    public OrderedEvent WindowFocusIn { get; } = new();
    public OrderedEvent WindowMaximized { get; } = new();
    public OrderedEvent WindowRestored { get; } = new();
    public OrderedEvent WindowFocusOut { get; } = new();
    public OrderedEvent WindowMinimized { get; } = new();
    public OrderedEvent WindowClosingRequested { get; } = new();
    public OrderedResultEvent<EventArgs?, WindowClosingResult> Closing { get; } = new();
    public OrderedEvent WindowClosed { get; } = new();
    public OrderedEvent WindowCreating { get; } = new();
    public OrderedEvent WindowCreated { get; } = new();
    
    public OrderedEvent<InfiniFrameWebMessageReceivedEvent> WebMessageReceived { get; } = new();
    public OrderedEvent<InfiniFrameDebugEventArgs> DebuggingEvent { get; } = new();
    public KeyedEvent<string, string?> WebMessagePostData { get; } = new();
    public KeyedResultEvent<string, string?, string?> WebMessageGetData { get; } = new();
    
    public KeyedResultEvent<string, string, (Stream? Data, string? ContentType)> CustomScheme { get; } = new();
    public void CopyTo(IInfiniFrameEventsStore target) {
        CopyHandlers(WebMessageReceived.Snapshot, target.WebMessageReceived.Add);
        CopyHandlers(DebuggingEvent.Snapshot, target.DebuggingEvent.Add);
        CopyHandlers(WebMessagePostData.Snapshot, static (t, item) => t.WebMessagePostData.Add(item.Key, item.Value), target);
        CopyHandlers(WebMessageGetData.Handlers, static (t, item) => t.WebMessageGetData.Add(item.Key, item.Value), target);
        CopyHandlers(CustomScheme.Handlers, static (t, item) => t.CustomScheme.Add(item.Key, item.Value), target);
        
        CopyHandlers(WindowClosed.Snapshot, target.WindowClosed.Add);
        CopyHandlers(Closing.Snapshot, target.Closing.Add);
        CopyHandlers(WindowClosingRequested.Snapshot, target.WindowClosingRequested.Add);
        CopyHandlers(WindowCreated.Snapshot, target.WindowCreated.Add);
        CopyHandlers(WindowCreating.Snapshot, target.WindowCreating.Add);
        CopyHandlers(WindowFocusIn.Snapshot, target.WindowFocusIn.Add);
        CopyHandlers(WindowFocusOut.Snapshot, target.WindowFocusOut.Add);
        CopyHandlers(WindowLocationChanged.Snapshot, target.WindowLocationChanged.Add);
        CopyHandlers(WindowMaximized.Snapshot, target.WindowMaximized.Add);
        CopyHandlers(WindowMinimized.Snapshot, target.WindowMinimized.Add);
        CopyHandlers(WindowRestored.Snapshot, target.WindowRestored.Add);
        CopyHandlers(WindowSizeChanged.Snapshot, target.WindowSizeChanged.Add);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------


    private static void CopyHandlers<THandler>(IEnumerable<THandler> handlers, Action<THandler> addHandler) {
        foreach (THandler handler in handlers) {
            addHandler(handler);
        }
    }

    private static void CopyHandlers<THandler, TTarget>(IEnumerable<THandler> handlers, Action<TTarget, THandler> addHandler, TTarget target) {
        foreach (THandler handler in handlers) {
            addHandler(target, handler);
        }
    }
}
