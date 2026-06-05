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
    public OrderedEvent<InfiniFrameDebugEventArgs> DebugEvent { get; } = new();
    public KeyedEvent<string, string?> WebMessagePostData { get; } = new();
    public KeyedResultEvent<string, string?, string?> WebMessageGetData { get; } = new();
    
    public KeyedResultEvent<string, string, (Stream? Data, string? ContentType)> CustomScheme { get; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameEventsStore DeepCopy() {
        var copy = new InfiniFrameEventsStore();

        CopyHandlers(WebMessageReceived.Snapshot, copy.WebMessageReceived.Add);
        CopyHandlers(DebugEvent.Snapshot, copy.DebugEvent.Add);
        CopyHandlers(WebMessagePostData.Snapshot, static (target, item) => target.WebMessagePostData.Add(item.Key, item.Value), copy);
        CopyHandlers(WebMessageGetData.Handlers, static (target, item) => target.WebMessageGetData.Add(item.Key, item.Value), copy);
        CopyHandlers(CustomScheme.Handlers, static (target, item) => target.CustomScheme.Add(item.Key, item.Value), copy);
        
        CopyHandlers(WindowClosed.Snapshot, copy.WindowClosed.Add);
        CopyHandlers(Closing.Snapshot, copy.Closing.Add);
        CopyHandlers(WindowClosingRequested.Snapshot, copy.WindowClosingRequested.Add);
        CopyHandlers(WindowCreated.Snapshot, copy.WindowCreated.Add);
        CopyHandlers(WindowCreating.Snapshot, copy.WindowCreating.Add);
        CopyHandlers(WindowFocusIn.Snapshot, copy.WindowFocusIn.Add);
        CopyHandlers(WindowFocusOut.Snapshot, copy.WindowFocusOut.Add);
        CopyHandlers(WindowLocationChanged.Snapshot, copy.WindowLocationChanged.Add);
        CopyHandlers(WindowMaximized.Snapshot, copy.WindowMaximized.Add);
        CopyHandlers(WindowMinimized.Snapshot, copy.WindowMinimized.Add);
        CopyHandlers(WindowRestored.Snapshot, copy.WindowRestored.Add);
        CopyHandlers(WindowSizeChanged.Snapshot, copy.WindowSizeChanged.Add);

        return copy;
    }

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
