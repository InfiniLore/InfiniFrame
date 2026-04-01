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
    extension<T>(T builder) where T : IInfiniFrameWindowBuilder {
        public T UseEmbeddedWwwrootAssets(
            Assembly? assembly = null,
            string scheme = "app",
            string host = "localhost",
            string defaultDocument = "index.html",
            string? physicalWwwrootPath = null,
            bool includePhysicalFallback = true,
            bool setStartUrl = true
        ) {
            if (builder is not InfiniFrameWindowBuilder concreteBuilder) {
                throw new NotSupportedException("UseEmbeddedWwwrootAssets currently requires InfiniFrameWindowBuilder.");
            }

            string normalizedScheme = scheme.Trim().ToLowerInvariant();
            string normalizedHost = host.Trim();
            string normalizedDefaultDocument = defaultDocument.TrimStart('/').Replace('\\', '/');
            string baseUri = $"{normalizedScheme}://{normalizedHost}/";

            IFileProvider provider = FileProviderFactory.CreateWwwrootProvider(
                assembly,
                physicalWwwrootPath,
                includePhysicalFallback);

            concreteBuilder.StaticAssets = new StaticAssetSettings {
                FileProvider = provider,
                BaseUri = baseUri,
                DefaultDocument = normalizedDefaultDocument
            };

            builder.RegisterCustomSchemeHandler(
                normalizedScheme,
                StaticAssetSchemeHandler.Create(provider, normalizedDefaultDocument));

            if (setStartUrl) {
                builder.SetStartUrl($"{baseUri}{normalizedDefaultDocument}");
            }

            return builder;
        }
    }
}
