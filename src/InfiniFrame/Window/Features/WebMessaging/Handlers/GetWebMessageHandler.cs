// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Compatibility registration entry point for the JavaScript-to-window feature bridge.</summary>
public static class GetWebMessageHandler {
    /// <summary>
    ///     Registers the JavaScript-to-window feature bridge handler for GET operations.
    /// </summary>
    /// <typeparam name="T">The builder type.</typeparam>
    /// <param name="builder">The window builder.</param>
    /// <returns>The builder for chaining.</returns>
    public static T RegisterGetWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder
        => WindowFeatureWebMessageHandler.Register(builder);
}
