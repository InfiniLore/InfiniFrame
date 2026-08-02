// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using System.Drawing;
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record InfiniFrameEventsStore : IInfiniFrameEventsStore {
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowLocationChanged"/>
    public OrderedEvent<Point> WindowLocationChanged { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowSizeChanged"/>
    public OrderedEvent<Size> WindowSizeChanged { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowFocusIn"/>
    public OrderedEvent WindowFocusIn { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowMaximized"/>
    public OrderedEvent WindowMaximized { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowRestored"/>
    public OrderedEvent WindowRestored { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowFocusOut"/>
    public OrderedEvent WindowFocusOut { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowMinimized"/>
    public OrderedEvent WindowMinimized { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowClosingRequested"/>
    public OrderedEvent WindowClosingRequested { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.Closing"/>
    public OrderedResultEvent<EventArgs?, WindowClosingResult> Closing { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowClosed"/>
    public OrderedEvent WindowClosed { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowCreating"/>
    public OrderedEvent WindowCreating { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowCreated"/>
    public OrderedEvent WindowCreated { get; } = new();

    /// <inheritdoc cref="IInfiniFrameEventsStore.WebMessageReceived"/>
    public OrderedEvent<InfiniFrameWebMessageReceivedEvent> WebMessageReceived { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.DebuggingEvent"/>
    public OrderedEvent<InfiniFrameDebugEventArgs> DebuggingEvent { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WebMessagePostData"/>
    public KeyedEvent<string, string?> WebMessagePostData { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WebMessageGetData"/>
    public KeyedResultEvent<string, string?, string?> WebMessageGetData { get; } = new();

    /// <inheritdoc cref="IInfiniFrameEventsStore.CustomScheme"/>
    public KeyedResultEvent<string, string, (Stream? Data, string? ContentType)> CustomScheme { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.CopyTo"/>
    public void CopyTo(IInfiniFrameEventsStore target) {
        CopyHandlers(WebMessageReceived.Snapshot, target.WebMessageReceived.Add);
        CopyHandlers(DebuggingEvent.Snapshot, target.DebuggingEvent.Add);
        CopyHandlers(WebMessagePostData.Snapshot, static (t, item) => t.WebMessagePostData.Add(item.Key, item.Value), target);
        CopyHandlers(WebMessageGetData.Snapshot, static (t, item) => t.WebMessageGetData.Add(item.Key, item.Value), target);
        CopyHandlers(CustomScheme.Snapshot, static (t, item) => t.CustomScheme.Add(item.Key, item.Value), target);

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