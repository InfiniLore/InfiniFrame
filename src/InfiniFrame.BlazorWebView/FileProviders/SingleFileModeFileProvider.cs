// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace InfiniFrame.BlazorWebView.FileProviders;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     A file provider for single-file packed deployments that serves static web assets from
///     embedded resources and falls back to a physical wwwroot directory.
/// </summary>
internal sealed class SingleFileModeFileProvider : IFileProvider {
    private readonly CompositeFileProvider _composite;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    internal SingleFileModeFileProvider(Assembly entryAssembly, string baseDirectory) {
        var providers = new List<IFileProvider> {
            // Embedded resources with "publish." prefix (StaticWebAsset items from NuGet packages)
            new EmbeddedFileProvider(entryAssembly, "publish")
        };

        // Embedded resources with "{assemblyName}.wwwroot." prefix (project wwwroot files)
        string? assemblyName = entryAssembly.GetName().Name;
        if (!string.IsNullOrEmpty(assemblyName)) {
            providers.Add(new EmbeddedFileProvider(entryAssembly, $"{assemblyName}.wwwroot"));
        }

        // Physical wwwroot directory fallback (framework assets like _framework/blazor.webview.js).
        // In single-file self-extracting mode, BaseDirectory points to the temp extraction dir
        // but the real wwwroot is alongside the exe. Check both locations.
        string? exeDir = Path.GetDirectoryName(Environment.ProcessPath);
        string[] searchPaths = exeDir is not null && exeDir != baseDirectory
            ? [Path.Join(baseDirectory, "wwwroot"), Path.Join(exeDir, "wwwroot")]
            : [Path.Join(baseDirectory, "wwwroot")];

        foreach (string wwwrootPath in searchPaths) {
            if (!Directory.Exists(wwwrootPath)) continue;

            providers.Add(new PhysicalFileProvider(wwwrootPath));
            break;
        }

        _composite = new CompositeFileProvider(providers);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IFileInfo GetFileInfo(string subpath) => _composite.GetFileInfo(subpath);

    public IDirectoryContents GetDirectoryContents(string subpath) => _composite.GetDirectoryContents(subpath);

    public IChangeToken Watch(string filter) => NullChangeToken.Singleton;

    /// <summary>
    ///     Creates a <see cref="SingleFileModeFileProvider" /> if the entry assembly contains embedded
    ///     resources with the "publish." prefix, indicating a packed deployment.
    /// </summary>
    public static bool TryCreate(string baseDirectory, [NotNullWhen(true)] out IFileProvider? fileProvider) {
        fileProvider = null;
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly is null) return false;

        string[] resourceNames;
        try {
            resourceNames = entryAssembly.GetManifestResourceNames();
        }
        catch {
            return false;
        }

        bool hasPublishResources = resourceNames.Any(r => r.StartsWith("publish.", StringComparison.Ordinal));
        if (!hasPublishResources) return false;

        fileProvider = new SingleFileModeFileProvider(entryAssembly, baseDirectory);
        return true;
    }
}
