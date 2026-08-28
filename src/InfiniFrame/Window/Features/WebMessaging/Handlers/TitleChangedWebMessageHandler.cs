// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides extension methods to register a JavaScript handler that updates the window title when changed from
///     web content.
/// </summary>
public static class TitleChangedWebMessageHandler {
    /// <summary>
    ///     Registers a JavaScript handler that updates the window title when changed from web content.
    /// </summary>
    /// <typeparam name="T">The builder type.</typeparam>
    /// <param name="builder">The window builder.</param>
    /// <returns>The builder for chaining.</returns>
    public static T RegisterTitleChangedWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessagePostHandler(JsHandlerNames.TitleChanged, HandleWebMessage);
        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, JsHandlerNames.RegisterTitleChange);
        return builder;
    }

    private static void HandleWebMessage(IInfiniFrameWindow window, string? payload) {
        if (string.IsNullOrWhiteSpace(payload)) return;

        window.Features.Decorations.SetTitle(payload);
    }
}
