// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Runtime feature implementation for managing native menu bars.
///     Stores the menu bar in memory and provides get/set/enable/disable/click operations.
/// </summary>
public sealed class MenuInfiniFrameWindowFeature : IMenuInfiniFrameWindowFeature {
    private readonly IInfiniFrameWindow _window;
    private readonly ILogger<MenuInfiniFrameWindowFeature> _logger;
    private InfiniFrameMenuBar _menuBar;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MenuInfiniFrameWindowFeature"/> class.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="menuBar">The meny bar instance</param>
    public MenuInfiniFrameWindowFeature(
        IInfiniFrameWindow window,
        ILogger<MenuInfiniFrameWindowFeature> logger,
        InfiniFrameMenuBar? menuBar = null
    ) {
        _window = window;
        _logger = logger;
        _menuBar = menuBar ?? new InfiniFrameMenuBar();
    }

    /// <inheritdoc cref="IMenuInfiniFrameWindowFeature.MenuBar"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public InfiniFrameMenuBar MenuBar => _menuBar;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IMenuInfiniFrameWindowFeature.SetMenuBar"/>
    public void SetMenuBar(InfiniFrameMenuBar? menuBar) {
        _logger.LogDebug(".SetMenuBar()");

        _menuBar = menuBar ?? new InfiniFrameMenuBar();

        string? json = _menuBar.Items.IsEmpty
            ? null
            : JsonSerializer.Serialize(_menuBar, MenuJsonContext.Default.InfiniFrameMenuBar);

        NativeInvoke.InvokeSyncWithValidation(
            _logger,
            _window,
            _window.ManagedThreadId,
            InfiniFrameNative.SetMenuBar,
            json
        );
    }

    /// <inheritdoc cref="IMenuInfiniFrameWindowFeature.SetMenuItemEnabled"/>
    public void SetMenuItemEnabled(string menuItemId, bool enabled) {
        _logger.LogDebug(".SetMenuItemEnabled({MenuItemId}, {Enabled})", menuItemId, enabled);

        _menuBar = UpdateMenuItemProperty(_menuBar, menuItemId, item => item with { IsEnabled = enabled });

        NativeInvoke.InvokeSyncWithValidation(
            _logger,
            _window,
            _window.ManagedThreadId,
            InfiniFrameNative.SetMenuItemEnabled,
            menuItemId,
            enabled
        );
    }

    /// <inheritdoc cref="IMenuInfiniFrameWindowFeature.SetMenuItemVisible"/>
    public void SetMenuItemVisible(string menuItemId, bool visible) {
        _logger.LogDebug(".SetMenuItemVisible({MenuItemId}, {Visible})", menuItemId, visible);

        _menuBar = UpdateMenuItemProperty(_menuBar, menuItemId, item => item with { IsVisible = visible });

        NativeInvoke.InvokeSyncWithValidation(
            _logger,
            _window,
            _window.ManagedThreadId,
            InfiniFrameNative.SetMenuItemVisible,
            menuItemId,
            visible
        );
    }

    /// <inheritdoc cref="IMenuInfiniFrameWindowFeature.ClickMenuItem"/>
    public void ClickMenuItem(string menuItemId) {
        _logger.LogDebug(".ClickMenuItem({MenuItemId})", menuItemId);

        NativeInvoke.InvokeSyncWithValidation(
            _logger,
            _window,
            _window.ManagedThreadId,
            InfiniFrameNative.ClickMenuItem,
            menuItemId
        );
    }

    private static InfiniFrameMenuBar UpdateMenuItemProperty(
        InfiniFrameMenuBar menuBar,
        string menuItemId,
        Func<InfiniFrameMenuItem, InfiniFrameMenuItem> updater
    ) {
        ImmutableArray<InfiniFrameMenuItem> updatedItems = MenuItemTreeHelper.UpdateItem(menuBar.Items, menuItemId, updater);
        return menuBar with { Items = updatedItems };
    }
}
