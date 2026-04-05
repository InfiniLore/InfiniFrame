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

        IFileProvider provider = new CompositeFileProvider(embeddedProvider, legacyEmbeddedProvider);
        if (!includePhysicalFallback) return provider;

        string fallbackPath = physicalWwwrootPath
            ?? Path.Join(AppContext.BaseDirectory, "wwwroot");

        if (!Directory.Exists(fallbackPath)) return provider;

        var physicalProvider = new PhysicalFileProvider(fallbackPath);
        return new CompositeFileProvider(provider, physicalProvider);
    }
}
