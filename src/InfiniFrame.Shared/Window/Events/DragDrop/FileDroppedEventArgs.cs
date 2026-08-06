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
    ///     Gets the file paths that were dropped.
    /// </summary>
    public IReadOnlyList<string> Files { get; }

    /// <summary>
    ///     Gets the screen coordinates where the drop occurred.
    /// </summary>
    public Point DropLocation { get; }

    public FileDroppedEventArgs(IReadOnlyList<string> files, Point dropLocation) {
        Files = files;
        DropLocation = dropLocation;
    }
}
