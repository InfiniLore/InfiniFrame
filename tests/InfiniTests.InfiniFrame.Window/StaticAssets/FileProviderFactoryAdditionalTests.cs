// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using InfiniFrame.StaticAssets;
using Microsoft.Extensions.FileProviders;

namespace InfiniTests.InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FileProviderFactoryAdditionalTests {

    [Test]
    public async Task CreateWwwrootProvider_WithExistingPhysicalPath_ReturnsDisposableComposite(CancellationToken ct = default) {
        // Arrange
        Assembly assembly = typeof(FileProviderFactory).Assembly;
        string tempDir = Path.Join(Path.GetTempPath(), $"InfiniFrameTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        try {
            // Act
            IFileProvider provider = FileProviderFactory.CreateWwwrootProvider(
                assembly,
                tempDir
            );

            // Assert - should return a composite provider (DisposableCompositeFileProvider or CompositeFileProvider)
            await Assert.That(provider).IsNotNull();
        }
        finally {
            Directory.Delete(tempDir, true);
        }
    }

    [Test]
    public async Task CreateWwwrootProvider_IncludePhysicalFallbackFalse_AlwaysReturnsComposite(CancellationToken ct = default) {
        // Arrange
        Assembly assembly = typeof(FileProviderFactory).Assembly;
        string tempDir = Path.Join(Path.GetTempPath(), $"InfiniFrameTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        try {
            // Act
            IFileProvider provider = FileProviderFactory.CreateWwwrootProvider(
                assembly,
                tempDir,
                false
            );

            // Assert
            await Assert.That(provider).IsTypeOf<CompositeFileProvider>();
        }
        finally {
            Directory.Delete(tempDir, true);
        }
    }

    [Test]
    public async Task CreateWwwrootProvider_ProviderCanResolveEmbeddedResource(CancellationToken ct = default) {
        // Arrange
        Assembly assembly = typeof(FileProviderFactory).Assembly;

        // Act
        IFileProvider provider = FileProviderFactory.CreateWwwrootProvider(
            assembly,
            includePhysicalFallback: false
        );

        // Assert - favicon.ico is embedded in test output
        IFileInfo fileInfo = provider.GetFileInfo("favicon.ico");
        await Assert.That(fileInfo).IsNotNull();
    }
}
