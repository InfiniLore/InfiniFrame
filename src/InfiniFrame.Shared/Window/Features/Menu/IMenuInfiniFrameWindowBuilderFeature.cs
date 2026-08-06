// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Builder feature interface for configuring the native menu bar before window creation.
/// </summary>
public interface IMenuInfiniFrameWindowBuilderFeature : IInfiniFrameWindowBuilderFeature {
    /// <summary>
    ///     Gets the current menu bar configuration.
    /// </summary>
    InfiniFrameMenuBar MenuBar { get; }

    /// <summary>
    ///     Sets the menu bar for the window.
    /// </summary>
    /// <param name="menuBar">The menu bar to apply.</param>
    void SetMenuBar(InfiniFrameMenuBar menuBar);
}
