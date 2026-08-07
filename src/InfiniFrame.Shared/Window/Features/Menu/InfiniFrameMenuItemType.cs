// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Defines the type of a menu item in a native menu bar.
/// </summary>
public enum InfiniFrameMenuItemType {
    /// <summary>
    ///     A standard clickable menu item.
    /// </summary>
    Normal = 0,

    /// <summary>
    ///     A visual separator between menu items.
    /// </summary>
    Separator = 1,

    /// <summary>
    ///     A menu item that contains child items.
    /// </summary>
    Submenu = 2
}
