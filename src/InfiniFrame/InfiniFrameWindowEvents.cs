// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowEvents : IInfiniFrameWindowEvents {
    private IInfiniFrameWindow Sender { get; set; } = null!;

    public event Action<IInfiniFrameWindow, Point>? WindowLocationChanged;
    public event Action<IInfiniFrameWindow, Size>? WindowSizeChanged;
    public event Action<IInfiniFrameWindow>? WindowFocusIn;
    public event Action<IInfiniFrameWindow>? WindowMaximized;
    public event Action<IInfiniFrameWindow>? WindowRestored;
    public event Action<IInfiniFrameWindow>? WindowFocusOut;
    public event Action<IInfiniFrameWindow>? WindowMinimized;
    public event Action<IInfiniFrameWindow, string>? WebMessageReceived;
    public event NetClosingDelegate? WindowClosing;
    public event Action<IInfiniFrameWindow>? WindowClosingRequested;
    public event Action<IInfiniFrameWindow>? WindowCreating;
    public event Action<IInfiniFrameWindow>? WindowCreated;

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
        WindowLocationChanged?.Invoke(Sender, location);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's size changes.
    /// </summary>
    public void OnSizeChanged(int width, int height) {
        var size = new Size(width, height);
        WindowSizeChanged?.Invoke(Sender, size);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses in.
    /// </summary>
    public void OnFocusIn() {
        WindowFocusIn?.Invoke(Sender);
    }


    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is maximized.
    /// </summary>
    public void OnMaximized() {
        WindowMaximized?.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is restored.
    /// </summary>
    public void OnRestored() {
        WindowRestored?.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses out.
    /// </summary>
    public void OnFocusOut() {
        WindowFocusOut?.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is minimized.
    /// </summary>
    public void OnMinimized() {
        WindowMinimized?.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window sends a message.
    /// </summary>
    public void OnWebMessageReceived(string message) {
        WebMessageReceived?.Invoke(Sender, message);
    }

    public void OnWindowClosingRequested() {
        WindowClosingRequested?.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is about to close.
    /// </summary>
    public byte OnWindowClosing() {
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
    public void OnWindowCreating() {
        WindowCreating?.Invoke(Sender);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods after the native window is created.
    /// </summary>
    public void OnWindowCreated() {
        WindowCreated?.Invoke(Sender);
    }

}
