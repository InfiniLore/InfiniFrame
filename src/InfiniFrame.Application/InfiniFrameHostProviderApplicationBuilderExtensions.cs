// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Hosting;

namespace InfiniFrame.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Selects an external host/window provider for an application.</summary>
public static class InfiniFrameHostProviderApplicationBuilderExtensions {
    /// <summary>
    ///     Selects <paramref name="provider" /> to supply this application's native windows, native loop and browser
    ///     engine. Call once, before <see cref="InfiniFrameApplicationBuilder.Build" />. InfiniFrame still owns the window
    ///     facade, lifecycle, managed dispatch, the Blazor integration and resource dispatch. The default native backend
    ///     is not loaded for this application, and a process cannot mix it with a host provider.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     A provider is already selected, or default-engine settings (WebView2 runtime path, notification registration,
    ///     AppUserModelId, default notification icon) are set.
    /// </exception>
    public static InfiniFrameApplicationBuilder UseHostProvider(this InfiniFrameApplicationBuilder builder, IInfiniFrameHostProvider provider) {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(provider);
        builder.SetHostProvider(provider);
        return builder;
    }
}
