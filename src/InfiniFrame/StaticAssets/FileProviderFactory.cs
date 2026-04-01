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

        var embeddedProvider = new EmbeddedFileProvider(assembly, "wwwroot");
        if (!includePhysicalFallback) return embeddedProvider;

        string fallbackPath = physicalWwwrootPath
            ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");

        if (!Directory.Exists(fallbackPath)) return embeddedProvider;

        var physicalProvider = new PhysicalFileProvider(fallbackPath);
        return new CompositeFileProvider(embeddedProvider, physicalProvider);
    }
}
