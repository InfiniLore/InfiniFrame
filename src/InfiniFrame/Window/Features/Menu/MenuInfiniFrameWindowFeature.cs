// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Diagnostics;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Runtime feature implementation for managing native menu bars.
///     Stores the menu bar in memory and provides get/set/enable/disable/click operations.
///     When native P/Invoke is available, it will call the native layer.
/// </summary>
public sealed class MenuInfiniFrameWindowFeature : IMenuInfiniFrameWindowFeature {
    private readonly IInfiniFrameWindow _window;
    private readonly ILogger<MenuInfiniFrameWindowFeature> _logger;
    private InfiniFrameMenuBar _menuBar = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="MenuInfiniFrameWindowFeature"/> class.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="logger">The logger instance.</param>
    public MenuInfiniFrameWindowFeature(
        IInfiniFrameWindow window,
        ILogger<MenuInfiniFrameWindowFeature> logger
    ) {
        _window = window;
        _logger = logger;
    }

    /// <inheritdoc cref="IMenuInfiniFrameWindowFeature.MenuBar"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public InfiniFrameMenuBar MenuBar => _menuBar;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IMenuInfiniFrameWindowFeature.SetMenuBar"/>
    public void SetMenuBar(InfiniFrameMenuBar menuBar) {
        _logger.LogDebug(".SetMenuBar()");

        _menuBar = menuBar ?? new();

        // TODO: Call InfiniFrameNative.SetMenuBar when native P/Invoke is available.
    }

    /// <inheritdoc cref="IMenuInfiniFrameWindowFeature.SetMenuItemEnabled"/>
    public void SetMenuItemEnabled(string menuItemId, bool enabled) {
        _logger.LogDebug(".SetMenuItemEnabled({MenuItemId}, {Enabled})", menuItemId, enabled);

        _menuBar = UpdateMenuItemProperty(_menuBar, menuItemId, item => item with { IsEnabled = enabled });

        // TODO: Call InfiniFrameNative.SetMenuItemEnabled when native P/Invoke is available.
    }

    /// <inheritdoc cref="IMenuInfiniFrameWindowFeature.SetMenuItemVisible"/>
    public void SetMenuItemVisible(string menuItemId, bool visible) {
        _logger.LogDebug(".SetMenuItemVisible({MenuItemId}, {Visible})", menuItemId, visible);

        _menuBar = UpdateMenuItemProperty(_menuBar, menuItemId, item => item with { IsVisible = visible });

        // TODO: Call InfiniFrameNative.SetMenuItemVisible when native P/Invoke is available.
    }

    /// <inheritdoc cref="IMenuInfiniFrameWindowFeature.ClickMenuItem"/>
    public void ClickMenuItem(string menuItemId) {
        _logger.LogDebug(".ClickMenuItem({MenuItemId})", menuItemId);

        // TODO: Call InfiniFrameNative.ClickMenuItem when native P/Invoke is available.
    }

    private static InfiniFrameMenuBar UpdateMenuItemProperty(
        InfiniFrameMenuBar menuBar,
        string menuItemId,
        Func<InfiniFrameMenuItem, InfiniFrameMenuItem> updater
    ) {
        var updatedItems = UpdateItemsRecursive(menuBar.Items, menuItemId, updater);
        return menuBar with { Items = updatedItems };
    }

    private static ImmutableArray<InfiniFrameMenuItem> UpdateItemsRecursive(
        ImmutableArray<InfiniFrameMenuItem> items,
        string menuItemId,
        Func<InfiniFrameMenuItem, InfiniFrameMenuItem> updater
    ) {
        var builder = items.ToBuilder();

        for (int i = 0; i < builder.Count; i++) {
            if (builder[i].Id == menuItemId) {
                builder[i] = updater(builder[i]);
            } else if (!builder[i].Children.IsDefaultOrEmpty) {
                builder[i] = builder[i] with {
                    Children = UpdateItemsRecursive(builder[i].Children, menuItemId, updater)
                };
            }
        }

        return builder.MoveToImmutable();
    }
}
