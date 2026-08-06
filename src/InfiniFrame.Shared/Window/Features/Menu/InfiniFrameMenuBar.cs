// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents the native menu bar for a window, containing a collection of top-level menu items.
/// </summary>
/// <param name="Items">The top-level menu items in the menu bar.</param>
public sealed record InfiniFrameMenuBar(
    ImmutableArray<InfiniFrameMenuItem> Items = default
) {
    /// <summary>
    ///     Initializes a new instance of the <see cref="InfiniFrameMenuBar"/> record with an empty menu bar.
    /// </summary>
    public InfiniFrameMenuBar() : this(default(ImmutableArray<InfiniFrameMenuItem>)) { }

    /// <summary>
    ///     Gets the menu items, returning an empty array if the default was not set.
    /// </summary>
    public ImmutableArray<InfiniFrameMenuItem> Items { get; init; } = Items.IsDefault ? ImmutableArray<InfiniFrameMenuItem>.Empty : Items;
}
