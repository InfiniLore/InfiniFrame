// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameWindowBuilderFeatureBrowserExtensions {
    public static IInfiniFrameWindowBuilder EnableContextMenu(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Browser.EnableContextMenu(enabled);
        return builder;
    }

    public static IInfiniFrameWindowBuilder EnableMediaAutoplay(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Browser.EnableMediaAutoplay(enabled);
        return builder;
    }

    public static IInfiniFrameWindowBuilder SetUserAgent(this IInfiniFrameWindowBuilder builder, string? userAgent) {
        builder.Features.Browser.SetUserAgent(userAgent);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder EnableFileSystemAccess(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Browser.EnableFileSystemAccess(enabled);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder EnableWebSecurity(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Browser.EnableWebSecurity(enabled);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder EnableJavascriptClipboardAccess(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Browser.EnableJavascriptClipboardAccess(enabled);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder EnableMediaStream(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Browser.EnableMediaStream(enabled);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder EnableIgnoreCertificateErrors(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Browser.EnableIgnoreCertificateErrors(enabled);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder EnableBrowserPermissions(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Browser.EnableBrowserPermissions(enabled);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder EnableSmoothScrolling(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Browser.EnableSmoothScrolling(enabled);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetBrowserControlInitParameters(this IInfiniFrameWindowBuilder builder, string? parameters) {
        builder.Features.Browser.SetBrowserControlInitParameters(parameters);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetTemporaryFilesPath(this IInfiniFrameWindowBuilder builder, string parameters) {
        builder.Features.Browser.SetTemporaryFilesPath(parameters);
        return builder;
    }
}
