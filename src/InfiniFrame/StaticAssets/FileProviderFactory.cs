// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class FileProviderFactory {
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
