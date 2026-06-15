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
    
    public void Load(Uri uri)
        => TryLoadUri(uri);

    public void Load(string path)
        => TryLoadPath(path);

    public bool TryLoadUri(Uri uri) {
        if (uri.IsFile) return TryNavigate(uri.ToString());
        
        IInfiniFrameUriSecurityPolicy uriSecurityPolicy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window);
        
        // ReSharper disable once InvertIf
        if (!uriSecurityPolicy.TrustAllOrigins && !uriSecurityPolicy.IsTrustedOrigin(uri)) {
            logger.LogWarning("Uri {Uri} is not trusted", uri);
            return false;
        }

        return TryNavigate(uri.ToString());
    }
    
    public bool TryLoadPath(string path) {
        foreach (string attempt in EnumeratePathAttempts(path)) {
            if (Uri.TryCreate(attempt, UriKind.Absolute, out Uri? absoluteUri)) {
                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (absoluteUri.IsFile) return TryNavigate(absoluteUri.ToString());
                return TryLoadUri(absoluteUri);
            }

            if (TryResolveStaticAssetUri(attempt, out Uri? staticAssetUri)) return TryLoadUri(staticAssetUri);
            if (TryNavigate(attempt)) return true;
        }
        
        return false;
    }
    
    public void LoadRawString( string content) {
        if (window.IsClosedOrClosing()) {
            logger.LogDebug("Skipping navigation because window is closing");
            return;
        }
        
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.NavigateToString,
            content
        );

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

    private bool TryNavigate(string target) {
        logger.LogDebug("Navigating to url: {Target}", target);
        try {
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.NavigateToUrl,
                target
            );
            return true;
        }
        catch (Exception ex) when (!ExceptionsUtility.IsNonFatalException(ex)) {
            logger.LogWarning(ex, "Failed to navigate to {Target}", target);
            return false;
        }
    }

    private static IEnumerable<string> EnumeratePathAttempts(string path) {
        yield return path;

        string? absolutePath = null;
        try {
            absolutePath = Path.GetFullPath(path);
        }
        catch (Exception) {
            // ignored intentionally; invalid paths still get other attempts
        }

        if (!string.IsNullOrWhiteSpace(absolutePath) 
            && !string.Equals(absolutePath, path, StringComparison.OrdinalIgnoreCase)) {
            yield return absolutePath;
        }

        string baseDirectoryPath = Path.Join(AppContext.BaseDirectory, path);
        if (!string.Equals(baseDirectoryPath, path, StringComparison.OrdinalIgnoreCase) 
            && !string.Equals(baseDirectoryPath, absolutePath, StringComparison.OrdinalIgnoreCase)) {
            yield return baseDirectoryPath;
        }
    }
}
