// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniLore.Photino.NET;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PhotinoWindowEvents : IPhotinoWindowEvents {
    public event EventHandler<Point>? WindowLocationChanged;
    public event EventHandler<Size>? WindowSizeChanged;
    public event EventHandler? WindowFocusIn;
    public event EventHandler? WindowMaximized;
    public event EventHandler? WindowRestored;
    public event EventHandler? WindowFocusOut;
    public event EventHandler? WindowMinimized;
    public event EventHandler<string>? WebMessageReceived;
    public event NetClosingDelegate? WindowClosing;
    public event EventHandler? WindowCreating;
    public event EventHandler? WindowCreated;
    
    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's location changes.
    /// </summary>
    /// <param name="left">Position from left in pixels</param>
    /// <param name="top">Position from top in pixels</param>
    public void OnLocationChanged(int left, int top) {
        var location = new Point(left, top);
        WindowLocationChanged?.Invoke(this, location);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window's size changes.
    /// </summary>
    public void OnSizeChanged(int width, int height) {
        var size = new Size(width, height);
        WindowSizeChanged?.Invoke(this, size);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses in.
    /// </summary>
    public void OnFocusIn() {
        WindowFocusIn?.Invoke(this, EventArgs.Empty);
    }


    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is maximized.
    /// </summary>
    public void OnMaximized() {
        WindowMaximized?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is restored.
    /// </summary>
    public void OnRestored() {
        WindowRestored?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window focuses out.
    /// </summary>
    public void OnFocusOut() {
        WindowFocusOut?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is minimized.
    /// </summary>
    public void OnMinimized() {
        WindowMinimized?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window sends a message.
    /// </summary>
    public void OnWebMessageReceived(string message) {
        WebMessageReceived?.Invoke(this, message);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window is about to close.
    /// </summary>
    public byte OnWindowClosing() {
        //C++ handles bool values as a single byte, C# uses 4 bytes
        byte noClose = 0;
        bool? doNotClose = WindowClosing?.Invoke(this, null);
        if (doNotClose ?? false)
            noClose = 1;

        return noClose;
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods before the native window is created.
    /// </summary>
    public void OnWindowCreating() {
        WindowCreating?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods after the native window is created.
    /// </summary>
    public void OnWindowCreated() {
        WindowCreated?.Invoke(this, EventArgs.Empty);
    }
    
}
