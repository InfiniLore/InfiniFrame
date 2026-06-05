// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameWindowDebugBuilderExtensions {
    /// <summary>
    ///     Enables or disables the DevTools functionality for the browser control.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="enabled">
    ///     Indicates whether the DevTools should be enabled. Pass true to enable DevTools, or false to disable them.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetDevToolsEnabled<T>(this T builder, bool enabled) where T : IInfiniFrameWindowBuilder {
        builder.Debug.SetDevToolsEnabled(enabled);
        return builder;
    }

    /// <summary>
    ///     Configures the browser remote debugging port at startup.
    ///     A value in the range 1..65535 enables remote debugging.
    ///     A value of 0 or null disables remote debugging.
    /// </summary>
    /// <remarks>
    ///     This API is supported on Windows and Linux.
    ///     Any remote-debugging switches in <see cref="InfiniFrameWindowBuilderExtensions.SetBrowserControlInitParameters{T}" />
    ///     are ignored in favor of this API.
    /// </remarks>
    public static T SetRemoteDebuggingPort<T>(this T builder, int? port) where T : IInfiniFrameWindowBuilder {
        builder.Debug.SetRemoteDebuggingPort(port);
        return builder;
    }

    /// <summary>
    ///     Enables Safari Web Inspector attachability for WKWebView on macOS.
    /// </summary>
    /// <remarks>
    ///     This API is startup-only and only supported on macOS 13.3+.
    /// </remarks>
    public static T SetWebInspectorEnabled<T>(this T builder, bool enabled = true) where T : IInfiniFrameWindowBuilder {
        builder.Debug.SetWebInspectorEnabled(enabled);
        return builder;
    }
}
