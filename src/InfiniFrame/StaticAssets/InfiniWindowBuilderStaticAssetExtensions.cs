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
/// <summary>
///     Provides extension methods for configuring embedded wwwroot static assets on an <see cref="IInfiniFrameWindowBuilder" />.
/// </summary>
public static class InfiniWindowBuilderStaticAssetExtensions {
    /// <summary>
    ///     Configures the window builder to serve static assets from embedded wwwroot resources and an optional physical
    ///     fallback directory.
    /// </summary>
    /// <param name="builder">The window builder.</param>
    /// <param name="assembly">The assembly containing embedded wwwroot resources. Defaults to the entry assembly.</param>
    /// <param name="scheme">The custom URI scheme to register (e.g., <c>app</c>).</param>
    /// <param name="host">The host to use in the base URI.</param>
    /// <param name="defaultDocument">The default document file name (e.g., <c>index.html</c>).</param>
    /// <param name="physicalWwwrootPath">Optional physical wwwroot path. Defaults to <c>wwwroot</c> under the base directory.</param>
    /// <param name="includePhysicalFallback">Whether to include a physical file provider fallback.</param>
    /// <param name="setStartUrl">Whether to set the start page URL to the resolved default document.</param>
    /// <typeparam name="T">The type of the window builder.</typeparam>
    /// <returns>The window builder for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the default document is not found in any configured provider.</exception>
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
