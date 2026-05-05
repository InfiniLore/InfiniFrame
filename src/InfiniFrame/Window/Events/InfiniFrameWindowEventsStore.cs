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
    
    public OrderedWindowEvent<string> WebMessageReceived { get; } = new();
    public KeyedWindowEvent<string, string?> WebMessagePostData { get; } = new();
    public KeyedWindowResultEvent<string, string?, string?> WebMessageGetData { get; } = new();
    
    public KeyedWindowResultEvent<string, string, (Stream? Data, string? ContentType)> CustomScheme { get; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindowEventsStore DeepCopy() {
        var copy = new InfiniFrameWindowEventsStore();

        CopyHandlers(WebMessageReceived.Snapshot, copy.WebMessageReceived.Add);
        
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
}
