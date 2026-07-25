// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Compatibility registration entry point for the JavaScript-to-window feature bridge.</summary>
public static class GetWebMessageHandler {
    public static T RegisterGetWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder
        => WindowFeatureWebMessageHandler.Register(builder);
}
