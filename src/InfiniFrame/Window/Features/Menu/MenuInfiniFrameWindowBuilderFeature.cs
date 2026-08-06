// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Builder feature implementation for native menu bar configuration.
///     Stores the menu bar and serializes it to JSON for the native layer.
/// </summary>
public class MenuInfiniFrameWindowBuilderFeature : IMenuInfiniFrameWindowBuilderFeature {
    /// <inheritdoc cref="IMenuInfiniFrameWindowBuilderFeature.MenuBar"/>
    public InfiniFrameMenuBar MenuBar { get; private set; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IMenuInfiniFrameWindowBuilderFeature.SetMenuBar"/>
    public void SetMenuBar(InfiniFrameMenuBar menuBar) {
        MenuBar = menuBar ?? new();
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeature.ApplyToNativeParameters"/>
    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.MenuBarJson = MenuBar.Items.IsEmpty
            ? null
            : System.Text.Json.JsonSerializer.Serialize(MenuBar, MenuJsonContext.Default.InfiniFrameMenuBar);
    }
}
