// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowEvents : IInfiniFrameWindowEvents {
    private object Sender { get; set; } = null!;

    public event EventHandler<Point>? WindowLocationChanged;
    public event EventHandler<Size>? WindowSizeChanged;
    public event EventHandler? WindowFocusIn;
    public event EventHandler? WindowMaximized;
    public event EventHandler? WindowRestored;
    public event EventHandler? WindowFocusOut;
    public event EventHandler? WindowMinimized;
    public event EventHandler<string>? WebMessageReceived;
    public event NetClosingDelegate? WindowClosing;
    public event EventHandler? WindowClosingRequested;
    public event EventHandler? WindowCreating;
    public event EventHandler? WindowCreated;

    public IInfiniFrameWindowEvents DefineSender<T>(T sender) where T : class {
        ArgumentNullException.ThrowIfNull(sender);
        Sender = sender;
        return this;
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's location changes.
    /// </summary>
    /// <param name="left">Position from left in pixels</param>
    /// <param name="top">Position from top in pixels</param>
    public void InvokeOnLocationChanged(int left, int top) {
        var location = new Point(left, top);
        WindowLocationChanged?.Invoke(Sender, location);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's size changes.
    /// </summary>
    public void InvokeOnSizeChanged(int width, int height) {
        var size = new Size(width, height);
        WindowSizeChanged?.Invoke(Sender, size);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses in.
    /// </summary>
    public void InvokeOnFocusIn() {
        WindowFocusIn?.Invoke(Sender, EventArgs.Empty);
    }


    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is maximized.
    /// </summary>
    public void InvokeOnMaximized() {
        WindowMaximized?.Invoke(Sender, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is restored.
    /// </summary>
    public void InvokeOnRestored() {
        WindowRestored?.Invoke(Sender, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses out.
    /// </summary>
    public void InvokeOnFocusOut() {
        WindowFocusOut?.Invoke(Sender, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is minimized.
    /// </summary>
    public void InvokeOnMinimized() {
        WindowMinimized?.Invoke(Sender, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window sends a message.
    /// </summary>
    public void InvokeOnWebMessageReceived(string message) {
        WebMessageReceived?.Invoke(Sender, message);
    }

    public void InvokeOnWindowClosingRequested() {
        WindowClosingRequested?.Invoke(Sender, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is about to close.
    /// </summary>
    public byte InvokeOnWindowClosing() {
        //C++ handles bool values as a single byte, C# uses 4 bytes
        byte noClose = 0;
        bool? doNotClose = WindowClosing?.Invoke(Sender, null);
        if (doNotClose ?? false)
            noClose = 1;

        return noClose;
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods before the native window is created.
    /// </summary>
    public void InvokeOnWindowCreating() {
        WindowCreating?.Invoke(Sender, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods after the native window is created.
    /// </summary>
    public void InvokeOnWindowCreated() {
        WindowCreated?.Invoke(Sender, EventArgs.Empty);
    }

}
