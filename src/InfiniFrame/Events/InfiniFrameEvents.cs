// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using InfiniFrame.Native.Delegates;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameEvents : IInfiniFrameEvents {
    public required IInfiniFrameEventsStore EventsStore { get; init; }
    private IInfiniFrameWindow? Sender { get; set; }
    
    // 
    private CppClosedDelegate ClosedHandler  { get; set; }
    private CppClosingDelegate ClosingHandler { get; set; }
    private CppFocusInDelegate FocusInHandler  { get; set; }
    private CppFocusOutDelegate FocusOutHandler  { get; set; }
    private CppMaximizedDelegate MaximizedHandler  { get; set; }
    private CppMinimizedDelegate MinimizedHandler  { get; set; }
    private CppMovedDelegate MovedHandler  { get; set; }
    private CppResizedDelegate ResizedHandler  { get; set; }
    private CppRestoredDelegate RestoredHandler  { get; set; }
    private CppWebMessageReceivedDelegate WebMessageReceivedHandler  { get; set; }
    private CppWebResourceRequestedDelegate CustomSchemeHandler  { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public InfiniFrameEvents(IInfiniFrameEventsStore store) {
        EventsStore = store;

        // Root stable delegate instances for native callback lifetime.
        ClosedHandler = OnWindowClosed;
        ClosingHandler = OnWindowClosing;
        CustomSchemeHandler = OnCustomScheme;
        FocusInHandler = OnFocusIn;
        FocusOutHandler = OnFocusOut;
        MaximizedHandler = OnMaximized;
        MinimizedHandler = OnMinimized;
        MovedHandler = OnLocationChanged;
        ResizedHandler = OnSizeChanged;
        RestoredHandler = OnRestored;
        WebMessageReceivedHandler = OnWebMessageReceived;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void AssignSender(IInfiniFrameWindow sender) {
        ArgumentNullException.ThrowIfNull(sender);
        Sender = sender;
    }
    
    public void AssignEventCallbacks(ref InfiniFrameNativeParameters parameters) {
        // Rebind callbacks to the per-window event instance that has Sender set via CompleteSetup.
        parameters.ClosedHandler = ClosedHandler;
        parameters.ClosingHandler = ClosingHandler;
        parameters.CustomSchemeHandler = CustomSchemeHandler;
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

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's location changes.
    /// </summary>
    /// <param name="left">Position from left in pixels</param>
    /// <param name="top">Position from top in pixels</param>
    public void OnLocationChanged(int left, int top) {
        ArgumentNullException.ThrowIfNull(Sender);
        
        var location = new Point(left, top);
        EventsStore.WindowLocationChanged.Invoke(Sender, location);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's size changes.
    /// </summary>
    public void OnSizeChanged(int width, int height) {
        ArgumentNullException.ThrowIfNull(Sender);
        
        var size = new Size(width, height);
        EventsStore.WindowSizeChanged.Invoke(Sender, size);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses in.
    /// </summary>
    public void OnFocusIn() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowFocusIn.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is maximized.
    /// </summary>
    public void OnMaximized() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowMaximized.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is restored.
    /// </summary>
    public void OnRestored() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowRestored.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses out.
    /// </summary>
    public void OnFocusOut() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowFocusOut.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is minimized.
    /// </summary>
    public void OnMinimized() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowMinimized.Invoke(Sender);
    }
    
    public void OnWindowClosed() {
        ArgumentNullException.ThrowIfNull(Sender);

        Sender.MarkClosedFromNativeCallback();
        EventsStore.WindowClosed.Invoke(Sender);
    }

    public void OnWindowClosingRequested() {
        ArgumentNullException.ThrowIfNull(Sender);
        EventsStore.WindowClosingRequested.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is about to close.
    /// </summary>
    public byte OnWindowClosing() {
        ArgumentNullException.ThrowIfNull(Sender);
        
        //C++ handles bool values as a single byte, C# uses 4 bytes
        byte cancel = 0;
        WindowClosingResult[] doNotClose = EventsStore.Closing.Invoke(Sender, null);
        if (doNotClose.Any(r => r == WindowClosingResult.Cancel)) {
            cancel = 1;
        }
        
        return cancel;
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods before the native window is created.
    /// </summary>
    public void OnWindowCreating() {
        ArgumentNullException.ThrowIfNull(Sender);
        
        EventsStore.WindowCreating.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods after the native window is created.
    /// </summary>
    public void OnWindowCreated() {
        ArgumentNullException.ThrowIfNull(Sender);
        
        EventsStore.WindowCreated.Invoke(Sender);
    }
}
