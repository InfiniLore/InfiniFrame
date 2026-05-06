// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record InfiniFrameWindowEventsStore : IInfiniFrameWindowEventsStore {
    public OrderedWindowEvent<Point> WindowLocationChanged { get; } = new();
    public OrderedWindowEvent<Size> WindowSizeChanged { get; } = new();
    public OrderedWindowEvent WindowFocusIn { get; } = new();
    public OrderedWindowEvent WindowMaximized { get; } = new();
    public OrderedWindowEvent WindowRestored { get; } = new();
    public OrderedWindowEvent WindowFocusOut { get; } = new();
    public OrderedWindowEvent WindowMinimized { get; } = new();
    public OrderedWindowEvent WindowClosingRequested { get; } = new();
    public OrderedWindowResultEvent<EventArgs?, WindowClosingResult> WindowClosing { get; } = new();
    public OrderedWindowEvent WindowClosed { get; } = new();
    public OrderedWindowEvent WindowCreating { get; } = new();
    public OrderedWindowEvent WindowCreated { get; } = new();
    
    public OrderedWindowEvent<InfiniFrameWebMessageReceivedEvent> WebMessageReceived { get; } = new();
    public KeyedWindowEvent<string, string?> WebMessagePostData { get; } = new();
    public KeyedWindowResultEvent<string, string?, string?> WebMessageGetData { get; } = new();
    
    public KeyedWindowResultEvent<string, string, (Stream? Data, string? ContentType)> CustomScheme { get; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindowEventsStore DeepCopy() {
        var copy = new InfiniFrameWindowEventsStore();

        CopyHandlers(WebMessageReceived.Snapshot, copy.WebMessageReceived.Add);
        CopyHandlers(WebMessagePostData.Snapshot, static (target, item) => target.WebMessagePostData.Add(item.Key, item.Value), copy);
        CopyHandlers(WebMessageGetData.Handlers, static (target, item) => target.WebMessageGetData.Add(item.Key, item.Value), copy);
        CopyHandlers(CustomScheme.Handlers, static (target, item) => target.CustomScheme.Add(item.Key, item.Value), copy);
        
        CopyHandlers(WindowClosed.Snapshot, copy.WindowClosed.Add);
        CopyHandlers(WindowClosing.Snapshot, copy.WindowClosing.Add);
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
