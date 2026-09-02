// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Extension methods for integrating Blazor WebView with <see cref="InfiniFrameApplication"/>.
/// </summary>
public static class InfiniFrameApplicationBlazorExtensions {
    /// <summary>
    ///     Adds a Blazor WebView to the application. The Blazor components are hosted inside
    ///     the native window's web view.
    /// </summary>
    /// <param name="app">The application instance.</param>
    /// <param name="configure">Optional callback to configure the <see cref="InfiniFrameBlazorAppBuilder"/>.</param>
    /// <returns>The application instance for chaining.</returns>
    public static InfiniFrameApplication WithBlazor(
        this InfiniFrameApplication app,
        Action<InfiniFrameBlazorAppBuilder>? configure = null
    ) {
        InfiniFrameBlazorAppBuilder blazorBuilder = InfiniFrameBlazorAppBuilder.CreateDefault();
        configure?.Invoke(blazorBuilder);

        // Register the window with the application.
        app.WithWindow(blazorBuilder.WindowBuilder);

        return app;
    }
}
