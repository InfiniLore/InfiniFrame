// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for the menu builder feature on <see cref="IInfiniFrameWindowBuilder"/>.
/// </summary>
public static class IMenuInfiniFrameWindowBuilderFeatureExtensions {
    /// <summary>
    ///     Sets the menu bar for the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="menuBar">The menu bar to apply.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetMenuBar(this IInfiniFrameWindowBuilder builder, InfiniFrameMenuBar menuBar) {
        builder.Features.Menu.SetMenuBar(menuBar);
        return builder;
    }
}
