// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
    /// <returns>The <see cref="InfiniFrameBlazorApp"/> for further configuration.</returns>
    public static InfiniFrameBlazorApp WithBlazorWebView(
        this InfiniFrameApplication app,
        Action<InfiniFrameBlazorAppBuilder>? configure = null
    ) {
        InfiniFrameBlazorAppBuilder blazorBuilder = new InfiniFrameBlazorAppBuilder();
        configure?.Invoke(blazorBuilder);

        // Merge Blazor-specific services into the application's service collection.
        // This ensures all services (including IInfiniFrameApplication) resolve from
        // the same provider when windows are built.
        foreach (ServiceDescriptor descriptor in blazorBuilder.Services) {
            app.ServiceCollection.Add(descriptor);
        }

        // Register the window with the application.
        string windowId = $"blazor-{app.Id}";
        app.WithWindow(windowId, blazorBuilder.WindowBuilder);

        // Build the Blazor app using the application's shared service provider.
        IServiceProvider sharedProvider = app.ServiceProvider;
        return blazorBuilder.Build(sharedProvider);
    }
}
