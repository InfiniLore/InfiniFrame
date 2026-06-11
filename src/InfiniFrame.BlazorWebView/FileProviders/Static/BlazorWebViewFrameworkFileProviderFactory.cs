// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Reflection;

namespace InfiniFrame.BlazorWebView.FileProviders.Static;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class BlazorWebViewFrameworkFileProviderFactory {
    private const string WebViewPackageId = "microsoft.aspnetcore.components.webview";
    private const string FrameworkPrefix = "_framework";
    private const string FrameworkScriptFile = "blazor.webview.js";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static IFileProvider? TryCreate(string? packageRootOverride = null) {
        string? staticWebAssetsDirectory = ResolveStaticWebAssetsDirectory(packageRootOverride);
        if (string.IsNullOrWhiteSpace(staticWebAssetsDirectory)) return null;

        return new FrameworkPathMappedFileProvider(staticWebAssetsDirectory);
    }

    private static string? ResolveStaticWebAssetsDirectory(string? packageRootOverride) {
        string? packagesRoot = packageRootOverride;
        if (string.IsNullOrWhiteSpace(packagesRoot)) {
            packagesRoot = Environment.GetEnvironmentVariable("NUGET_PACKAGES");
        }

        if (string.IsNullOrWhiteSpace(packagesRoot)) {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrWhiteSpace(userProfile)) {
                packagesRoot = Path.Join(userProfile, ".nuget", "packages");
            }
        }

        if (string.IsNullOrWhiteSpace(packagesRoot) || !Directory.Exists(packagesRoot)) return null;

        string packageRoot = Path.Join(packagesRoot, WebViewPackageId);
        if (!Directory.Exists(packageRoot)) return null;

        string? preferredVersion = GetPreferredPackageVersion();
        if (!string.IsNullOrWhiteSpace(preferredVersion)) {
            string preferredAssetsPath = Path.Join(packageRoot, preferredVersion, "staticwebassets");
            if (HasFrameworkAssets(preferredAssetsPath)) return preferredAssetsPath;
        }

        foreach (string versionDirectory in Directory.GetDirectories(packageRoot).OrderByDescending(static path => path, StringComparer.OrdinalIgnoreCase)) {
            string staticWebAssetsPath = Path.Join(versionDirectory, "staticwebassets");
            if (HasFrameworkAssets(staticWebAssetsPath)) return staticWebAssetsPath;
        }

        return null;
    }

    private static bool HasFrameworkAssets(string staticWebAssetsDirectory) {
        if (!Directory.Exists(staticWebAssetsDirectory)) return false;
        return File.Exists(Path.Join(staticWebAssetsDirectory, FrameworkScriptFile));
    }

    private static string? GetPreferredPackageVersion() {
        Assembly assembly = typeof(Microsoft.AspNetCore.Components.WebView.WebViewManager).Assembly;

        string? informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        if (!string.IsNullOrWhiteSpace(informationalVersion)) {
            string normalized = informationalVersion.Split('+')[0].Trim();
            if (!string.IsNullOrWhiteSpace(normalized)) return normalized;
        }

        Version? version = assembly.GetName().Version;
        return version is null ? null : $"{version.Major}.{version.Minor}.{version.Build}";
    }

    private sealed class FrameworkPathMappedFileProvider : IFileProvider {
        private readonly PhysicalFileProvider _innerProvider;

        public FrameworkPathMappedFileProvider(string staticWebAssetsDirectory) {
            _innerProvider = new PhysicalFileProvider(staticWebAssetsDirectory);
        }

        public IFileInfo GetFileInfo(string subpath) {
            if (!TryMapPath(subpath, out string mappedSubpath)) return new NotFoundFileInfo(subpath);
            return _innerProvider.GetFileInfo(mappedSubpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath) {
            if (string.IsNullOrWhiteSpace(subpath)
                || string.Equals(NormalizeSubpath(subpath), FrameworkPrefix, StringComparison.OrdinalIgnoreCase)) {
                return _innerProvider.GetDirectoryContents(string.Empty);
            }

            if (!TryMapPath(subpath, out string mappedSubpath)) return NotFoundDirectoryContents.Singleton;
            return _innerProvider.GetDirectoryContents(Path.GetDirectoryName(mappedSubpath)?.Replace('\\', '/') ?? string.Empty);
        }

        public IChangeToken Watch(string filter) {
            if (!TryMapPath(filter, out string mappedSubpath)) return NullChangeToken.Singleton;
            return _innerProvider.Watch(mappedSubpath);
        }

        private static bool TryMapPath(string? subpath, out string mappedSubpath) {
            mappedSubpath = string.Empty;
            string normalizedSubpath = NormalizeSubpath(subpath);
            if (string.IsNullOrWhiteSpace(normalizedSubpath)) return false;
            if (!normalizedSubpath.StartsWith($"{FrameworkPrefix}/", StringComparison.OrdinalIgnoreCase)) return false;

            mappedSubpath = normalizedSubpath[(FrameworkPrefix.Length + 1)..];
            if (string.IsNullOrWhiteSpace(mappedSubpath)) return false;
            if (mappedSubpath.Split('/', StringSplitOptions.RemoveEmptyEntries).Any(static segment => segment == "..")) return false;
            return true;
        }

        private static string NormalizeSubpath(string? path) {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;
            return path.Trim().TrimStart('~').TrimStart('/', '\\').Replace('\\', '/');
        }
    }
}
