// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame.Debugging;
using InfiniFrame.DragDrop;

namespace InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Concrete implementation of <see cref="IInfiniFrameEventsStore"/> that holds ordered and keyed event handler
///     collections for all window events.
/// </summary>
public class InfiniFrameEventsStore : IInfiniFrameEventsStore {
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowLocationChanged" />
    public OrderedEvent<Point> WindowLocationChanged { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowSizeChanged" />
    public OrderedEvent<Size> WindowSizeChanged { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowFocusIn" />
    public OrderedEvent WindowFocusIn { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowMaximized" />
    public OrderedEvent WindowMaximized { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowRestored" />
    public OrderedEvent WindowRestored { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowFocusOut" />
    public OrderedEvent WindowFocusOut { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowMinimized" />
    public OrderedEvent WindowMinimized { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowClosingRequested" />
    public OrderedEvent WindowClosingRequested { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.Closing" />
    public OrderedResultEvent<EventArgs?, WindowClosingResult> Closing { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowClosed" />
    public OrderedEvent WindowClosed { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowCreating" />
    public OrderedEvent WindowCreating { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WindowCreated" />
    public OrderedEvent WindowCreated { get; } = new();

    /// <inheritdoc cref="IInfiniFrameEventsStore.WebMessageReceived" />
    public OrderedEvent<InfiniFrameWebMessageReceivedEvent> WebMessageReceived { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.DebuggingEvent" />
    public OrderedEvent<InfiniFrameDebugEventArgs> DebuggingEvent { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WebMessagePostData" />
    public KeyedEvent<string, string?> WebMessagePostData { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.WebMessageGetData" />
    public KeyedResultEvent<string, string?, string?> WebMessageGetData { get; } = new();

    /// <inheritdoc cref="IInfiniFrameEventsStore.FileDropped" />
    public OrderedEvent<FileDroppedEventArgs> FileDropped { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.CustomScheme" />
    public KeyedResultEvent<string, string, (Stream? Data, string? ContentType)> CustomScheme { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.NavigationStarting" />
    public OrderedResultEvent<NavigationStartingEvent, NavigationStartingResult> NavigationStarting { get; } = new();
    /// <inheritdoc cref="IInfiniFrameEventsStore.CopyTo" />
    public void CopyTo(IInfiniFrameEventsStore eventsStore) {
        CopyHandlers(WebMessageReceived.Snapshot, eventsStore.WebMessageReceived.Add);
        CopyHandlers(DebuggingEvent.Snapshot, eventsStore.DebuggingEvent.Add);
        CopyHandlers(WebMessagePostData.Snapshot, addHandler: static (t, item) => t.WebMessagePostData.Add(item.Key, item.Value), eventsStore);
        CopyHandlers(WebMessageGetData.Snapshot, addHandler: static (t, item) => t.WebMessageGetData.Add(item.Key, item.Value), eventsStore);
        CopyHandlers(CustomScheme.Snapshot, addHandler: static (t, item) => t.CustomScheme.Add(item.Key, item.Value), eventsStore);

        CopyHandlers(WindowClosed.Snapshot, eventsStore.WindowClosed.Add);
        CopyHandlers(Closing.Snapshot, eventsStore.Closing.Add);
        CopyHandlers(WindowClosingRequested.Snapshot, eventsStore.WindowClosingRequested.Add);
        CopyHandlers(WindowCreated.Snapshot, eventsStore.WindowCreated.Add);
        CopyHandlers(WindowCreating.Snapshot, eventsStore.WindowCreating.Add);
        CopyHandlers(WindowFocusIn.Snapshot, eventsStore.WindowFocusIn.Add);
        CopyHandlers(WindowFocusOut.Snapshot, eventsStore.WindowFocusOut.Add);
        CopyHandlers(WindowLocationChanged.Snapshot, eventsStore.WindowLocationChanged.Add);
        CopyHandlers(WindowMaximized.Snapshot, eventsStore.WindowMaximized.Add);
        CopyHandlers(WindowMinimized.Snapshot, eventsStore.WindowMinimized.Add);
        CopyHandlers(WindowRestored.Snapshot, eventsStore.WindowRestored.Add);
        CopyHandlers(WindowSizeChanged.Snapshot, eventsStore.WindowSizeChanged.Add);
        CopyHandlers(NavigationStarting.Snapshot, eventsStore.NavigationStarting.Add);
        CopyHandlers(FileDropped.Snapshot, eventsStore.FileDropped.Add);
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
