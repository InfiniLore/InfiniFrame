// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Delegates;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameEvents : IInfiniFrameEvents {
    
    // ReSharper disable once CollectionNeverQueried.Local
    // Native callbacks can outlive normal managed scopes during teardown/recreation bursts.
    // Keep event instances rooted for process lifetime to prevent GC of delegate targets that
    // native code may still invoke.
    private static readonly ConcurrentDictionary<Guid, InfiniFrameEvents> NativeCallbackRoots = new();

    /// <inheritdoc cref="IHasInfiniFrameEventsStore.EventsStore"/>
    public IInfiniFrameEventsStore EventsStore { get; }
    private ILogger<InfiniFrameEvents> Logger { get; }
    private IInfiniFrameWindow? Sender { get; set; }

    // Keep callback delegates rooted for the native callback lifetime.
    private CppClosedDelegate ClosedHandler { get; }
    private CppClosingDelegate ClosingHandler { get; }
    private CppDebugEventDelegate DebugEventHandler { get; }
    private CppFocusInDelegate FocusInHandler { get; }
    private CppFocusOutDelegate FocusOutHandler { get; }
    private CppMaximizedDelegate MaximizedHandler { get; }
    private CppMinimizedDelegate MinimizedHandler { get; }
    private CppMovedDelegate MovedHandler { get; }
    private CppResizedDelegate ResizedHandler { get; }
    private CppRestoredDelegate RestoredHandler { get; }
    private CppWebMessageReceivedDelegate WebMessageReceivedHandler { get; }
    private CppWebResourceRequestedDelegate CustomSchemeHandler { get; }

    public InfiniFrameEvents(IInfiniFrameEventsStore eventsStore, ILogger<InfiniFrameEvents> logger) {
        EventsStore = eventsStore;
        Logger = logger;

        ClosedHandler = OnWindowClosed;
        ClosingHandler = OnWindowClosing;
        DebugEventHandler = OnDebugEvent;
        FocusInHandler = OnFocusIn;
        FocusOutHandler = OnFocusOut;
        MaximizedHandler = OnMaximized;
        MinimizedHandler = OnMinimized;
        MovedHandler = OnLocationChanged;
        ResizedHandler = OnSizeChanged;
        RestoredHandler = OnRestored;
        WebMessageReceivedHandler = OnWebMessageReceived;
        CustomSchemeHandler = OnCustomScheme;
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void AssignToWindow(IInfiniFrameWindow window) {
        ArgumentNullException.ThrowIfNull(window);
        Sender = window;
        NativeCallbackRoots.TryAdd(window.Id, this);
    }

    public void PopulateFromBuilderEventStore(IInfiniFrameEventsStore eventStore) {
        eventStore.CopyTo(EventsStore);
    }
    
    public void AssignToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        // Rebind callbacks to the per-window event instance that has Sender set via CompleteSetup.
        parameters.ClosedHandler = ClosedHandler;
        parameters.ClosingHandler = ClosingHandler;
        parameters.CustomSchemeHandler = CustomSchemeHandler;
        parameters.DebugEventHandler = DebugEventHandler;
        parameters.FocusInHandler = FocusInHandler;
        parameters.FocusOutHandler = FocusOutHandler;
        parameters.MaximizedHandler = MaximizedHandler;
        parameters.MinimizedHandler = MinimizedHandler;
        parameters.MovedHandler = MovedHandler;
        parameters.ResizedHandler = ResizedHandler;
        parameters.RestoredHandler = RestoredHandler;
        parameters.WebMessageReceivedHandler = WebMessageReceivedHandler;

        ApplyCustomSchemeNames(ref parameters);
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnLocationChanged"/>
    public void OnLocationChanged(int left, int top) {
        ArgumentNullException.ThrowIfNull(Sender);

        var location = new Point(left, top);
        EventsStore.WindowLocationChanged.Invoke(Sender, location);
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnSizeChanged"/>
    public void OnSizeChanged(int width, int height) {
        ArgumentNullException.ThrowIfNull(Sender);

        var size = new Size(width, height);
        EventsStore.WindowSizeChanged.Invoke(Sender, size);
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnFocusIn"/>
    public void OnFocusIn() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowFocusIn.Invoke(Sender);
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnMaximized"/>
    public void OnMaximized() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowMaximized.Invoke(Sender);
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnRestored"/>
    public void OnRestored() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowRestored.Invoke(Sender);
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnFocusOut"/>
    public void OnFocusOut() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowFocusOut.Invoke(Sender);
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnMinimized"/>
    public void OnMinimized() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowMinimized.Invoke(Sender);
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnWindowClosed"/>
    public void OnWindowClosed() {
        ArgumentNullException.ThrowIfNull(Sender);

        Sender.Features.Lifecycle.MarkAsClosed();
        EventsStore.WindowClosed.Invoke(Sender);
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnWindowClosingRequested"/>
    public void OnWindowClosingRequested() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowClosingRequested.Invoke(Sender);
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnWindowClosing"/>
    public byte OnWindowClosing() {
        ArgumentNullException.ThrowIfNull(Sender);

        //C++ handles bool values as a single byte, C# uses 4 bytes
        byte cancel = 0;
        WindowClosingResult[] doNotClose = EventsStore.Closing.Invoke(Sender, null);
        if (doNotClose.Any(r => r == WindowClosingResult.Cancel)) cancel = 1;

        return cancel;
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnWindowCreating"/>
    public void OnWindowCreating() {
        ArgumentNullException.ThrowIfNull(Sender);

        EventsStore.WindowCreating.Invoke(Sender);
    }

    /// <inheritdoc cref="IInfiniFrameEvents.OnWindowCreated"/>
    public void OnWindowCreated() {
        ArgumentNullException.ThrowIfNull(Sender);

        EventsStore.WindowCreated.Invoke(Sender);
    }
}
