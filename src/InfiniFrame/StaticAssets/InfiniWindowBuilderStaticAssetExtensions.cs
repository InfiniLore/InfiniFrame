// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.StaticAssets;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniWindowBuilderStaticAssetExtensions {
    public static T UseEmbeddedWwwrootAssets<T>(
        this T builder,
        Assembly? assembly = null,
        string scheme = "app",
        string host = "localhost",
        string defaultDocument = "index.html",
        string? physicalWwwrootPath = null,
        bool includePhysicalFallback = true,
        bool setStartUrl = true
    ) where T : IInfiniFrameWindowBuilder {
        string normalizedScheme = scheme.Trim().ToLowerInvariant();
        string normalizedHost = host.Trim();
        string normalizedDefaultDocument = defaultDocument.TrimStart('/').Replace('\\', '/');
        string baseUri = $"{normalizedScheme}://{normalizedHost}/";

        IFileProvider provider = FileProviderFactory.CreateWwwrootProvider(
            assembly,
            physicalWwwrootPath,
            includePhysicalFallback);

        IFileInfo defaultDoc = provider.GetFileInfo(normalizedDefaultDocument);
        if (!defaultDoc.Exists || defaultDoc.IsDirectory) {
            string assemblyName = (assembly ?? Assembly.GetEntryAssembly() ?? typeof(InfiniWindowBuilderStaticAssetExtensions).Assembly)
                .GetName().Name ?? "<unknown>";
            throw new InvalidOperationException(
                $"Static asset '{normalizedDefaultDocument}' was not found via configured providers. " +
                $"Expected embedded naming like '{assemblyName}.wwwroot.{normalizedDefaultDocument.Replace('/', '.')}'.");
        }

        builder.StaticAssets = new InfiniFrameStaticAssets {
            FileProvider = provider,
            BaseUri = baseUri,
            DefaultDocument = normalizedDefaultDocument
        };

        builder.RegisterCustomSchemeHandler(
            normalizedScheme,
            StaticAssetSchemeHandler.Create(provider, normalizedDefaultDocument));

        if (setStartUrl) {
            builder.SetStartPageUrl($"{baseUri}{normalizedDefaultDocument}");
        }

        return builder;
    }
}
