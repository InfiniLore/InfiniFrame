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
public class PageNavigationInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<PageNavigationInfiniFrameWindowFeature> logger,
    IInfiniFrameStaticAssets? staticAssets
) : IPageNavigationInfiniFrameWindowFeature {

    /// <inheritdoc cref="IPageNavigationInfiniFrameWindowFeature.Load(Uri)" />
    public void Load(Uri uri)
        => TryLoadUri(uri);

    public Task<NavigationResult> LoadAsync(Uri uri, CancellationToken ct = default) {
        ArgumentNullException.ThrowIfNull(uri);
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window);
        if (!uri.IsFile && !policy.TrustAllOrigins && !policy.IsTrustedOrigin(uri))
            return Task.FromResult(new NavigationResult(0, NavigationStatus.Failed, uri, FailureReason: "URI origin is not trusted."));

        var operation = new InfiniNavigationOperation(window, logger, uri.ToString(), uri, false, ct);
        _ = operation.StartAsync();
        return operation.Task;
    }

    /// <inheritdoc cref="IPageNavigationInfiniFrameWindowFeature.Load(string)" />
    public void Load(string path)
        => TryLoadPath(path);

    /// <inheritdoc cref="IPageNavigationInfiniFrameWindowFeature.TryLoadUri" />
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

    /// <inheritdoc cref="IPageNavigationInfiniFrameWindowFeature.TryLoadPath" />
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

    /// <inheritdoc cref="IPageNavigationInfiniFrameWindowFeature.LoadRawString" />
    public void LoadRawString(string content) {
        if (window.IsClosedOrClosing()) {
            logger.LogDebug("Skipping navigation because window is closing");
            return;
        }

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.NavigateToString,
            content
        );

    }

    public Task<NavigationResult> LoadRawStringAsync(string content, CancellationToken ct = default) {
        ArgumentNullException.ThrowIfNull(content);
        var operation = new InfiniNavigationOperation(window, logger, content, null, true, ct);
        _ = operation.StartAsync();
        return operation.Task;
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
                window,
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
        catch (ArgumentException) {
            // ignored intentionally; invalid paths still get other attempts
        }
        catch (NotSupportedException) {
            // ignored intentionally; invalid paths still get other attempts
        }
        catch (PathTooLongException) {
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