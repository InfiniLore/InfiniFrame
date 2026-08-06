// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Runtime feature interface for managing native menu bars on a window.
/// </summary>
public interface IMenuInfiniFrameWindowFeature {
    /// <summary>
    ///     Gets the current menu bar configuration.
    /// </summary>
    InfiniFrameMenuBar MenuBar { get; }

    /// <summary>
    ///     Sets the menu bar for the window.
    /// </summary>
    /// <param name="menuBar">The menu bar to apply.</param>
    void SetMenuBar(InfiniFrameMenuBar menuBar);

    /// <summary>
    ///     Enables or disables a specific menu item by its identifier.
    /// </summary>
    /// <param name="menuItemId">The unique identifier of the menu item.</param>
    /// <param name="enabled">Whether the item should be enabled.</param>
    void SetMenuItemEnabled(string menuItemId, bool enabled);

    /// <summary>
    ///     Shows or hides a specific menu item by its identifier.
    /// </summary>
    /// <param name="menuItemId">The unique identifier of the menu item.</param>
    /// <param name="visible">Whether the item should be visible.</param>
    void SetMenuItemVisible(string menuItemId, bool visible);

    /// <summary>
    ///     Sends a click command for a specific menu item to the native layer.
    /// </summary>
    /// <param name="menuItemId">The unique identifier of the menu item to click.</param>
    void ClickMenuItem(string menuItemId);
}
