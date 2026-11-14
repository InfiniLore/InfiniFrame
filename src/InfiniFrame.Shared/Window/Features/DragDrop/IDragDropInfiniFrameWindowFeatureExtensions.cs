// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.DragDrop;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IDragDropInfiniFrameWindowFeature"/> on <see cref="IInfiniFrameWindow"/>.
/// </summary>
public static class IDragDropInfiniFrameWindowFeatureExtensions {
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Enables drag and drop with default settings and returns the window for chaining.
        /// </summary>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow EnableDragDrop() {
            window.Features.DragDrop.SetEnabled(true);
            return window;
        }
        /// <summary>
        ///     Enables drag and drop with specific allowed extensions and returns the window for chaining.
        /// </summary>
        /// <param name="extensions">File extensions (e.g., ".txt", ".png").</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow EnableDragDrop(params string[] extensions) {
            window.Features.DragDrop.SetEnabled(true);
            window.Features.DragDrop.SetAllowedExtensions(extensions);
            return window;
        }
        /// <summary>
        ///     Disables drag and drop and returns the window for chaining.
        /// </summary>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow DisableDragDrop() {
            window.Features.DragDrop.SetEnabled(false);
            return window;
        }
        /// <summary>
        ///     Registers a handler for file drop events and returns the window for chaining.
        /// </summary>
        /// <param name="handler">The handler to invoke with the window and file drop arguments.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow OnFileDropped(
            Action<IInfiniFrameWindow, FileDroppedEventArgs> handler
        ) {
            window.Events.EventsStore.FileDropped.Add(handler);
            return window;
        }
    }

}
