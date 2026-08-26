// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame.DragDrop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides data for file drop events.
/// </summary>
public sealed class FileDroppedEventArgs {

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileDroppedEventArgs"/> class.
    /// </summary>
    /// <param name="files">The file paths that were dropped.</param>
    /// <param name="dropLocation">The screen coordinates where the drop occurred.</param>
    public FileDroppedEventArgs(IReadOnlyList<string> files, Point dropLocation) {
        Files = files;
        DropLocation = dropLocation;
    }
    /// <summary>
    ///     Gets the file paths that were dropped.
    /// </summary>
    public IReadOnlyList<string> Files { get; }

    /// <summary>
    ///     Gets the screen coordinates where the drop occurred.
    /// </summary>
    public Point DropLocation { get; }
}
