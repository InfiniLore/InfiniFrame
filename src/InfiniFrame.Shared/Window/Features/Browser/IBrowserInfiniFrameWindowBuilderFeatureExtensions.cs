// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IBrowserInfiniFrameWindowBuilderFeatureExtensions {
    /// <summary>
    ///     Enables or disables the context menu for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether the context menu should be enabled.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableContextMenu(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Browser.EnableContextMenu(enabled);
        return builder;
    }

    /// <summary>
    ///     Enables or disables media autoplay for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether media autoplay should be enabled.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableMediaAutoplay(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Browser.EnableMediaAutoplay(enabled);
        return builder;
    }

    /// <summary>
    ///     Sets the user agent string for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="userAgent">The user agent string to set.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetUserAgent(this IInfiniFrameWindowBuilder builder, string? userAgent) {
        builder.Features.Browser.SetUserAgent(userAgent);
        return builder;
    }

    /// <summary>
    ///     Enables or disables file system access for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether file system access should be enabled.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableFileSystemAccess(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Browser.EnableFileSystemAccess(enabled);
        return builder;
    }

    /// <summary>
    ///     Enables or disables web security for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether web security should be enabled.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableWebSecurity(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Browser.EnableWebSecurity(enabled);
        return builder;
    }

    /// <summary>
    ///     Enables or disables JavaScript clipboard access for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether JavaScript clipboard access should be enabled.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableJavascriptClipboardAccess(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Browser.EnableJavascriptClipboardAccess(enabled);
        return builder;
    }

    /// <summary>
    ///     Enables or disables media stream for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether media stream should be enabled.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableMediaStream(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Browser.EnableMediaStream(enabled);
        return builder;
    }

    /// <summary>
    ///     Enables or disables ignoring certificate errors for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether certificate errors should be ignored.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableIgnoreCertificateErrors(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Browser.EnableIgnoreCertificateErrors(enabled);
        return builder;
    }

    /// <summary>
    ///     Enables or disables browser permissions for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether browser permissions should be granted.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableBrowserPermissions(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Browser.EnableBrowserPermissions(enabled);
        return builder;
    }

    /// <summary>
    ///     Enables or disables smooth scrolling for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether smooth scrolling should be enabled.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableSmoothScrolling(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Browser.EnableSmoothScrolling(enabled);
        return builder;
    }

    /// <summary>
    ///     Sets the browser control initialization parameters for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="parameters">The initialization parameters.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetBrowserControlInitParameters(this IInfiniFrameWindowBuilder builder, string? parameters) {
        builder.Features.Browser.SetBrowserControlInitParameters(parameters);
        return builder;
    }

    /// <summary>
    ///     Sets the temporary files path for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="parameters">The temporary files path.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetTemporaryFilesPath(this IInfiniFrameWindowBuilder builder, string parameters) {
        builder.Features.Browser.SetTemporaryFilesPath(parameters);
        return builder;
    }

    /// <summary>
    ///     Sets the fixed-version WebView2 runtime path used when creating the window on Windows.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="path">The path to the extracted WebView2 runtime directory.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetWebView2RuntimePath(this IInfiniFrameWindowBuilder builder, string path) {
        builder.Features.Browser.SetWebView2RuntimePath(path);
        return builder;
    }
}