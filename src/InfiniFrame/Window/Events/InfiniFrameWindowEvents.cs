// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameWindowEvents(IInfiniFrameWindowEventsStore store) : IInfiniFrameWindowEvents {
    public IInfiniFrameWindowEventsStore EventsStore { get; } = store;
    
    private IInfiniFrameWindow? Sender { get; set; }
    [MemberNotNullWhen(true, nameof(Sender))] private bool SetupComplete { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // constructors
    // -----------------------------------------------------------------------------------------------------------------
    internal InfiniFrameWindowEvents() : this(new InfiniFrameWindowEventsStore()) {}
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void CompleteSetup(IInfiniFrameWindow sender, ref InfiniFrameNativeParameters parameters) {
        ArgumentNullException.ThrowIfNull(sender);
        Sender = sender;
        
        // Rebind callbacks to the per-window event instance that has Sender set via CompleteSetup.
        parameters.ClosingHandler = OnWindowClosing;
        parameters.CustomSchemeHandler = OnCustomScheme;
        
        parameters.ClosedHandler = OnWindowClosed;
        parameters.ResizedHandler = OnSizeChanged;
        parameters.MaximizedHandler = OnMaximized;
        parameters.RestoredHandler = OnRestored;
        parameters.MinimizedHandler = OnMinimized;
        parameters.MovedHandler = OnLocationChanged;
        parameters.FocusInHandler = OnFocusIn;
        parameters.FocusOutHandler = OnFocusOut;
        parameters.WebMessageReceivedHandler = OnWebMessageReceived;
        
        ApplyCustomSchemeNames(ref parameters);
        
        SetupComplete = true;
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's location changes.
    /// </summary>
    /// <param name="left">Position from left in pixels</param>
    /// <param name="top">Position from top in pixels</param>
    public void OnLocationChanged(int left, int top) {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        
        var location = new Point(left, top);
        EventsStore.WindowLocationChanged.Invoke(Sender, location);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's size changes.
    /// </summary>
    public void OnSizeChanged(int width, int height) {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        
        var size = new Size(width, height);
        EventsStore.WindowSizeChanged.Invoke(Sender, size);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses in.
    /// </summary>
    public void OnFocusIn() {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        EventsStore.WindowFocusIn.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is maximized.
    /// </summary>
    public void OnMaximized() {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        EventsStore.WindowMaximized.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is restored.
    /// </summary>
    public void OnRestored() {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        EventsStore.WindowRestored.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses out.
    /// </summary>
    public void OnFocusOut() {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        EventsStore.WindowFocusOut.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is minimized.
    /// </summary>
    public void OnMinimized() {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        EventsStore.WindowMinimized.Invoke(Sender);
    }
    
    public void OnWindowClosed() {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");

        Sender.MarkClosedFromNativeCallback();
        EventsStore.WindowClosed.Invoke(Sender);
    }

    public void OnWindowClosingRequested() {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        EventsStore.WindowClosingRequested.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is about to close.
    /// </summary>
    public byte OnWindowClosing() {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        
        //C++ handles bool values as a single byte, C# uses 4 bytes
        byte cancel = 0;
        WindowClosingResult[] doNotClose = EventsStore.WindowClosing.Invoke(Sender, null);
        if (doNotClose.Any(r => r == WindowClosingResult.Cancel)) {
            cancel = 1;
        }
        
        return cancel;
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods before the native window is created.
    /// </summary>
    public void OnWindowCreating() {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        
        EventsStore.WindowCreating.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods after the native window is created.
    /// </summary>
    public void OnWindowCreated() {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        
        EventsStore.WindowCreated.Invoke(Sender);
    }
}
