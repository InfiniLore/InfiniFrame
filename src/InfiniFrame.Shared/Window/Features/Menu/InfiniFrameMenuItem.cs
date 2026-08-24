// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a single item in a native menu bar.
/// </summary>
/// <param name="Id">A unique identifier for this menu item, used for programmatic access and command routing.</param>
/// <param name="Label">
///     The display text for the menu item. Required for <see cref="InfiniFrameMenuItemType.Normal" /> and
///     <see cref="InfiniFrameMenuItemType.Submenu" /> items.
/// </param>
/// <param name="Type">The type of menu item.</param>
/// <param name="IsEnabled">Whether the menu item is enabled and can be interacted with.</param>
/// <param name="IsVisible">Whether the menu item is visible.</param>
/// <param name="KeyboardShortcut">An optional keyboard shortcut string (e.g., "Ctrl+S", "Cmd+Q").</param>
/// <param name="Children">Child items for submenu items. Ignored for other types.</param>
public sealed record InfiniFrameMenuItem(
    string Id,
    string? Label = null,
    InfiniFrameMenuItemType Type = InfiniFrameMenuItemType.Normal,
    bool IsEnabled = true,
    bool IsVisible = true,
    string? KeyboardShortcut = null,
    ImmutableArray<InfiniFrameMenuItem> Children = default
) {
    /// <summary>
    ///     Initializes a new instance of the <see cref="InfiniFrameMenuItem" /> record with default values.
    /// </summary>
    public InfiniFrameMenuItem() : this(string.Empty) {}

    /// <summary>
    ///     Gets the child items, returning an empty array if the default was not set.
    /// </summary>
    public ImmutableArray<InfiniFrameMenuItem> Children { get; init; } = Children.IsDefault ? ImmutableArray<InfiniFrameMenuItem>.Empty : Children;
}
