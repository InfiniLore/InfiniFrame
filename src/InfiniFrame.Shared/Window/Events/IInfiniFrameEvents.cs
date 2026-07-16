// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Defines event handlers and lifecycle callbacks for an InfiniFrame window.
/// </summary>
public interface IInfiniFrameEvents : IHasInfiniFrameEventsStore {
    /// <summary>
    ///     Called when the window receives focus.
    /// </summary>
    void OnFocusIn();

    /// <summary>
    ///     Called when the window loses focus.
    /// </summary>
    void OnFocusOut();

    /// <summary>
    ///     Called when the window's location on screen changes.
    /// </summary>
    /// <param name="left">The new left position in pixels.</param>
    /// <param name="top">The new top position in pixels.</param>
    void OnLocationChanged(int left, int top);

    /// <summary>
    ///     Called when the window is maximized.
    /// </summary>
    void OnMaximized();

    /// <summary>
    ///     Called when the window is minimized.
    /// </summary>
    void OnMinimized();

    /// <summary>
    ///     Called when the window is restored from a minimized or maximized state.
    /// </summary>
    void OnRestored();

    /// <summary>
    ///     Called when the window's size changes.
    /// </summary>
    /// <param name="width">The new width in pixels.</param>
    /// <param name="height">The new height in pixels.</param>
    void OnSizeChanged(int width, int height);

    /// <summary>
    ///     Called when a web message is received from the web view.
    /// </summary>
    /// <param name="message">The message content received from the web view.</param>
    /// <param name="origin">The origin of the message, if available.</param>
    void OnWebMessageReceived(string message, string? origin = null);

    /// <summary>
    ///     Called after the window has been closed.
    /// </summary>
    void OnWindowClosed();

    /// <summary>
    ///     Called when the window is about to close, allowing cancellation of the close operation.
    /// </summary>
    /// <returns>A byte indicating whether the close should be cancelled (non-zero) or allowed (zero).</returns>
    byte OnWindowClosing();

    /// <summary>
    ///     Called when a close operation has been requested on the window.
    /// </summary>
    void OnWindowClosingRequested();

    /// <summary>
    ///     Called after the native window has been created.
    /// </summary>
    void OnWindowCreated();

    /// <summary>
    ///     Called before the native window is created.
    /// </summary>
    void OnWindowCreating();

    /// <summary>
    ///     Handles a custom scheme URL request and returns the response data.
    /// </summary>
    /// <param name="url">The URL being requested.</param>
    /// <param name="numBytes">The number of bytes in the response data.</param>
    /// <param name="contentType">The content type of the response.</param>
    /// <returns>A pointer to the response data buffer.</returns>
    IntPtr OnCustomScheme(string url, out int numBytes, out string? contentType);
    
    /// <summary>
    ///     Populates this event store from a builder's event store.
    /// </summary>
    /// <param name="builder">The builder event store to copy from.</param>
    internal void PopulateFromBuilderEventStore(IInfiniFrameEventsStore builder);

    /// <summary>
    ///     Assigns the event callbacks to the native parameters structure.
    /// </summary>
    /// <param name="nativeParameters">The native parameters to assign callbacks to.</param>
    internal void AssignToNativeParameters(ref InfiniFrameNativeParameters nativeParameters);

    /// <summary>
    ///     Assigns the window reference to this events instance.
    /// </summary>
    /// <param name="window">The window to assign.</param>
    internal void AssignToWindow(IInfiniFrameWindow window);

    /// <summary>
    ///     Assigns default event callbacks that handle basic window lifecycle.
    /// </summary>
    internal void AssignDefaultEventCallbacks();

    /// <summary>
    ///     Releases managed callback roots that are kept alive for native interop callback lifetime.
    /// </summary>
    internal void ReleaseNativeCallbackRoot();
}
