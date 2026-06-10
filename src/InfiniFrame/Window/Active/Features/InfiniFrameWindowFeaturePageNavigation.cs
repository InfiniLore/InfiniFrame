// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Security;
using InfiniFrame.StaticAssets;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeaturePageNavigation(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeaturePageNavigation> logger,
    IInfiniFrameStaticAssets? staticAssets
) : IInfiniFrameWindowFeaturePageNavigation {
    
    public IInfiniFrameWindow Load(Uri uri) {
        if (TryLoadUri(uri) || TryLoadPath(uri.ToString())) {}
        return window;
    }

    public IInfiniFrameWindow Load(string path) {
        TryLoadPath(path);
        return window;
    }

    private bool TryLoadUri(Uri uri) {
        if (uri.IsFile) return TryLoadPath(uri.AbsolutePath);
        
        IInfiniFrameUriSecurityPolicy uriSecurityPolicy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window);
        if (!uriSecurityPolicy.TrustAllOrigins && !uriSecurityPolicy.IsTrustedOrigin(uri)) {
            logger.LogWarning("Uri {Uri} is not trusted", uri);
            return false;
        }
        
        logger.LogDebug("Navigating to url: {Uri}", uri);
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.NavigateToUrl,
            uri.ToString()
        );
        
        return true;
    }
    
    private bool TryLoadPath(string path) {
        ReadOnlySpan<string> attempts = [
            path,
            Path.GetFullPath(path),
            $"{AppContext.BaseDirectory}/{path}"
        ];
        
        foreach (string attempt in attempts) {
            if (Uri.TryCreate(attempt, UriKind.Absolute, out Uri? absoluteUri) && TryLoadUri(absoluteUri)) return true;
            if (TryResolveStaticAssetUri(attempt, out Uri? staticAssetUri)) return TryLoadUri(staticAssetUri);

            try {
                NativeInvoke.InvokeSyncWithValidation(
                    logger,
                    window.InstanceHandle,
                    window.ManagedThreadId,
                    InfiniFrameNative.NavigateToUrl,
                    attempt
                );
                return true;
            }
            catch (Exception ex) when (!ExceptionsUtility.IsNonFatalException(ex)) {
                logger.LogWarning(ex, "Failed to load static asset {Path}", attempt);
                return false;
            }
        }
        
        return false;
    }
    
    public IInfiniFrameWindow LoadRawString( string content) {
        if (window.IsClosedOrClosing()) {
            logger.LogDebug("Skipping navigation because window is closing");
            return window;
        }
        
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.NavigateToString,
            content
        );
        
        return window;
    }

    private bool TryResolveStaticAssetUri(string path, [NotNullWhen(true)] out Uri? uri) {
        uri = null!;
        if (staticAssets is null) return false;

        return StaticAssetSchemeHandler.TryResolveUri(
            staticAssets.FileProvider,
            path,
            staticAssets.BaseUri,
            staticAssets.DefaultDocument,
            out uri
        );
    }
}
