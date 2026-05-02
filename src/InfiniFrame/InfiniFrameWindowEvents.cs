// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BuilderSnapshots;
using InfiniFrame.Utilities;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowEvents : IInfiniFrameWindowEvents {

    private IInfiniFrameWindow Sender { get; set; } = null!;
    public OrderedEvent<Point> WindowLocationChanged { get; } = new();
    public OrderedEvent<Size> WindowSizeChanged { get; } = new();
    public OrderedEvent WindowFocusIn { get; } = new();
    public OrderedEvent WindowMaximized { get; } = new();
    public OrderedEvent WindowRestored { get; } = new();
    public OrderedEvent WindowFocusOut { get; } = new();
    public OrderedEvent WindowMinimized { get; } = new();
    public OrderedEvent<string> WebMessageReceived { get; } = new();
    public OrderedEvent WindowClosingRequested { get; } = new();
    public OrderedResultEvent<EventArgs?, bool> WindowClosing { get; } = new();
    public OrderedEvent WindowCreating { get; } = new();
    public OrderedEvent WindowCreated { get; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void CompleteSetup(IInfiniFrameWindow sender) {
        ArgumentNullException.ThrowIfNull(sender);
        Sender = sender;
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's location changes.
    /// </summary>
    /// <param name="left">Position from left in pixels</param>
    /// <param name="top">Position from top in pixels</param>
    public void OnLocationChanged(int left, int top) {
        var location = new Point(left, top);
        WindowLocationChanged.Invoke(Sender, location);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's size changes.
    /// </summary>
    public void OnSizeChanged(int width, int height) {
        var size = new Size(width, height);
        WindowSizeChanged.Invoke(Sender, size);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses in.
    /// </summary>
    public void OnFocusIn() {
        WindowFocusIn.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is maximized.
    /// </summary>
    public void OnMaximized() {
        WindowMaximized.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is restored.
    /// </summary>
    public void OnRestored() {
        WindowRestored.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses out.
    /// </summary>
    public void OnFocusOut() {
        WindowFocusOut.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is minimized.
    /// </summary>
    public void OnMinimized() {
        WindowMinimized.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window sends a message.
    /// </summary>
    public void OnWebMessageReceived(string message) {
        OnWebMessageReceived(message, null);
    }

    public void OnWindowClosingRequested() {
        WindowClosingRequested.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is about to close.
    /// </summary>
    public byte OnWindowClosing() {
        //C++ handles bool values as a single byte, C# uses 4 bytes
        byte noClose = 0;
        bool[] doNotClose = WindowClosing.Invoke(Sender, null);
        if (doNotClose.Any()) {
            noClose = 1;
        }
        
        return noClose;
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods before the native window is created.
    /// </summary>
    public void OnWindowCreating() {
        WindowCreating.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods after the native window is created.
    /// </summary>
    public void OnWindowCreated() {
        WindowCreated.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window sends a message.
    ///     This overload carries the native-reported message origin in an ambient context.
    /// </summary>
    public void OnWebMessageReceived(string message, string? origin) {
        using IDisposable _ = InfiniFrameWebMessageContext.Push(origin);
        WebMessageReceived.Invoke(Sender, message);
    }

    internal InfiniFrameWindowEventsSnapshot ToSnapshot()
        => new(
            WindowLocationChanged.Snapshot.ToArray(),
            WindowSizeChanged.Snapshot.ToArray(),
            WindowFocusIn.Snapshot.ToArray(),
            WindowMaximized.Snapshot.ToArray(),
            WindowRestored.Snapshot.ToArray(),
            WindowFocusOut.Snapshot.ToArray(),
            WindowMinimized.Snapshot.ToArray(),
            WebMessageReceived.Snapshot.ToArray(),
            WindowClosingRequested.Snapshot.ToArray(),
            WindowClosing.Snapshot.ToArray(),
            WindowCreating.Snapshot.ToArray(),
            WindowCreated.Snapshot.ToArray());

    internal static InfiniFrameWindowEvents FromSnapshot(InfiniFrameWindowEventsSnapshot snapshot) {
        var copy = new InfiniFrameWindowEvents();

        CopyHandlers(snapshot.WindowLocationChanged, copy.WindowLocationChanged.Add);
        CopyHandlers(snapshot.WindowSizeChanged, copy.WindowSizeChanged.Add);
        CopyHandlers(snapshot.WindowFocusIn, copy.WindowFocusIn.Add);
        CopyHandlers(snapshot.WindowMaximized, copy.WindowMaximized.Add);
        CopyHandlers(snapshot.WindowRestored, copy.WindowRestored.Add);
        CopyHandlers(snapshot.WindowFocusOut, copy.WindowFocusOut.Add);
        CopyHandlers(snapshot.WindowMinimized, copy.WindowMinimized.Add);
        CopyHandlers(snapshot.WebMessageReceived, copy.WebMessageReceived.Add);
        CopyHandlers(snapshot.WindowClosingRequested, copy.WindowClosingRequested.Add);
        CopyHandlers(snapshot.WindowClosing, copy.WindowClosing.Add);
        CopyHandlers(snapshot.WindowCreating, copy.WindowCreating.Add);
        CopyHandlers(snapshot.WindowCreated, copy.WindowCreated.Add);

        return copy;
    }

    private static void CopyHandlers<THandler>(IEnumerable<THandler> handlers, Action<THandler> addHandler) {
        foreach (THandler handler in handlers) {
            addHandler(handler);
        }
    }
}
