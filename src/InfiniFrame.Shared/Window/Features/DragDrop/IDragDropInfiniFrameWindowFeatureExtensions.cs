// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IDragDropInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Enables drag and drop with default settings and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow EnableDragDrop(this IInfiniFrameWindow window) {
        window.Features.DragDrop.SetEnabled(true);
        return window;
    }

    /// <summary>
    ///     Enables drag and drop with specific allowed extensions and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="extensions">File extensions (e.g., ".txt", ".png").</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow EnableDragDrop(this IInfiniFrameWindow window, params string[] extensions) {
        window.Features.DragDrop.SetEnabled(true);
        window.Features.DragDrop.SetAllowedExtensions(extensions);
        return window;
    }

    /// <summary>
    ///     Disables drag and drop and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow DisableDragDrop(this IInfiniFrameWindow window) {
        window.Features.DragDrop.SetEnabled(false);
        return window;
    }

    /// <summary>
    ///     Registers a handler for file drop events and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="handler">The handler to invoke with the window and file drop arguments.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow OnFileDropped(this IInfiniFrameWindow window,
        Action<IInfiniFrameWindow, FileDroppedEventArgs> handler) {
        window.Events.EventsStore.FileDropped.Add(handler);
        return window;
    }
}
