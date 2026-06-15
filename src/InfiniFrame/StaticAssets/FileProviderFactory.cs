// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Creates <see cref="IFileProvider" /> instances that combine embedded and physical wwwroot file sources.
/// </summary>
public static class FileProviderFactory {
    /// <summary>
    ///     Creates a composite file provider that resolves files from embedded assembly resources and an optional physical
    ///     wwwroot directory.
    /// </summary>
    /// <param name="assembly">The assembly that contains embedded wwwroot resources. Defaults to the entry assembly.</param>
    /// <param name="physicalWwwrootPath">An optional physical wwwroot path. Defaults to <c>wwwroot</c> under the base directory.</param>
    /// <param name="includePhysicalFallback">Whether to include a physical file provider as a fallback when the directory exists.</param>
    /// <returns>A composite file provider that aggregates all available sources.</returns>
    public static IFileProvider CreateWwwrootProvider(
        Assembly? assembly = null,
        string? physicalWwwrootPath = null,
        bool includePhysicalFallback = true
    ) {
        assembly ??= Assembly.GetEntryAssembly() ?? typeof(FileProviderFactory).Assembly;
        string assemblyName = assembly.GetName().Name ?? throw new InvalidOperationException("Unable to determine assembly name for embedded wwwroot resolution.");

        // Preferred embedded naming: <AssemblyName>.wwwroot.<path>
        var embeddedProvider = new EmbeddedFileProvider(assembly, $"{assemblyName}.wwwroot");

        // Compatibility with resources embedded as "wwwroot.<path>".
        var legacyEmbeddedProvider = new EmbeddedFileProvider(assembly, "wwwroot");

        var providers = new List<IFileProvider> {
            embeddedProvider,
            legacyEmbeddedProvider
        };

        if (!includePhysicalFallback) return new CompositeFileProvider(providers);

        string fallbackPath = physicalWwwrootPath ?? Path.Join(AppContext.BaseDirectory, "wwwroot");

        if (!Directory.Exists(fallbackPath)) return new CompositeFileProvider(providers);

        var physicalProvider = new PhysicalFileProvider(fallbackPath);
        providers.Add(physicalProvider);

        return new CompositeFileProvider(providers);

    }
}
