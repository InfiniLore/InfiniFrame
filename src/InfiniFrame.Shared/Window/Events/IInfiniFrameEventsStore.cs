// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame.Debugging;
using InfiniFrame.DragDrop;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Stores event handler collections for all window lifecycle and interaction events.
/// </summary>
public interface IInfiniFrameEventsStore {
    /// <summary>
    ///     Gets the event that fires when the window location changes.
    /// </summary>
    OrderedEvent<Point> WindowLocationChanged { get; }

    /// <summary>
    ///     Gets the event that fires when the window size changes.
    /// </summary>
    OrderedEvent<Size> WindowSizeChanged { get; }

    /// <summary>
    ///     Gets the event that fires when the window receives focus.
    /// </summary>
    OrderedEvent WindowFocusIn { get; }

    /// <summary>
    ///     Gets the event that fires when the window is maximized.
    /// </summary>
    OrderedEvent WindowMaximized { get; }

    /// <summary>
    ///     Gets the event that fires when the window is restored.
    /// </summary>
    OrderedEvent WindowRestored { get; }

    /// <summary>
    ///     Gets the event that fires when the window loses focus.
    /// </summary>
    OrderedEvent WindowFocusOut { get; }

    /// <summary>
    ///     Gets the event that fires when the window is minimized.
    /// </summary>
    OrderedEvent WindowMinimized { get; }

    /// <summary>
    ///     Gets the event that fires when a web message is received.
    /// </summary>
    OrderedEvent<InfiniFrameWebMessageReceivedEvent> WebMessageReceived { get; }

    /// <summary>
    ///     Gets the event that fires with debugging information.
    /// </summary>
    OrderedEvent<InfiniFrameDebugEventArgs> DebuggingEvent { get; }

    /// <summary>
    ///     Gets the keyed event for posting web message data.
    /// </summary>
    KeyedEvent<string, string?> WebMessagePostData { get; }

    /// <summary>
    ///     Gets the keyed result event for retrieving web message data.
    /// </summary>
    KeyedResultEvent<string, string?, string?> WebMessageGetData { get; }

    /// <summary>
    ///     Gets the event that fires when a window close is requested.
    /// </summary>
    OrderedEvent WindowClosingRequested { get; }

    /// <summary>
    ///     Gets the event that fires when the window is closing, allowing cancellation.
    /// </summary>
    OrderedResultEvent<EventArgs?, WindowClosingResult> Closing { get; }

    /// <summary>
    ///     Gets the event that fires after the window has closed.
    /// </summary>
    OrderedEvent WindowClosed { get; }

    /// <summary>
    ///     Gets the event that fires before the window is created.
    /// </summary>
    OrderedEvent WindowCreating { get; }

    /// <summary>
    ///     Gets the event that fires after the window is created.
    /// </summary>
    OrderedEvent WindowCreated { get; }

    /// <summary>
    ///     Gets the event that fires when navigation is starting, allowing cancellation.
    /// </summary>
    OrderedResultEvent<NavigationStartingEventArgs, NavigationStartingResult> NavigationStarting { get; }

    /// <summary>
    ///     Gets the event that fires when files are dropped onto the window.
    /// </summary>
    OrderedEvent<FileDroppedEventArgs> FileDropped { get; }

    /// <summary>
    ///     Gets the keyed result event for handling custom scheme requests.
    /// </summary>
    KeyedResultEvent<string, string, (Stream? Data, string? ContentType)> CustomScheme { get; }

    /// <summary>
    ///     Copies all event handlers from this store to the target store.
    /// </summary>
    /// <param name="eventsStore">The target event store to copy handlers into.</param>
    void CopyTo(IInfiniFrameEventsStore eventsStore);
}
