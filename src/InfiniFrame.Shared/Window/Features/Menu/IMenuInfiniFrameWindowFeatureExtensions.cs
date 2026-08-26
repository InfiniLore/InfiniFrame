// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for the menu feature on <see cref="IInfiniFrameWindow" />.
/// </summary>
public static class IMenuInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Sets the menu bar for the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="menuBar">The menu bar to apply.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetMenuBar(this IInfiniFrameWindow window, InfiniFrameMenuBar menuBar) {
        window.Features.Menu.SetMenuBar(menuBar);
        return window;
    }

    /// <summary>
    ///     Enables or disables a menu item and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="menuItemId">The unique identifier of the menu item.</param>
    /// <param name="enabled">Whether the item should be enabled.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetMenuItemEnabled(this IInfiniFrameWindow window, string menuItemId, bool enabled) {
        window.Features.Menu.SetMenuItemEnabled(menuItemId, enabled);
        return window;
    }

    /// <summary>
    ///     Shows or hides a menu item and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="menuItemId">The unique identifier of the menu item.</param>
    /// <param name="visible">Whether the item should be visible.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetMenuItemVisible(this IInfiniFrameWindow window, string menuItemId, bool visible) {
        window.Features.Menu.SetMenuItemVisible(menuItemId, visible);
        return window;
    }

    /// <summary>
    ///     Sends a click command for a menu item and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="menuItemId">The unique identifier of the menu item to click.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow ClickMenuItem(this IInfiniFrameWindow window, string menuItemId) {
        window.Features.Menu.ClickMenuItem(menuItemId);
        return window;
    }
}
