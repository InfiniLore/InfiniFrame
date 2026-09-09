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
public class FileProviderFactoryTests {

    [Test]
    public async Task CreateWwwrootProvider_WithAssembly_ReturnsCompositeProvider(CancellationToken ct = default) {
        // Arrange
        Assembly assembly = typeof(FileProviderFactory).Assembly;

        // Act
        IFileProvider provider = FileProviderFactory.CreateWwwrootProvider(
            assembly,
            includePhysicalFallback: false
        );

        // Assert
        await Assert.That(provider).IsNotNull();
        await Assert.That(provider).IsTypeOf<CompositeFileProvider>();
    }

    [Test]
    public async Task CreateWwwrootProvider_WithoutPhysicalFallback_ReturnsCompositeProvider(CancellationToken ct = default) {
        // Arrange
        Assembly assembly = typeof(FileProviderFactory).Assembly;

        // Act
        IFileProvider provider = FileProviderFactory.CreateWwwrootProvider(
            assembly,
            includePhysicalFallback: false
        );

        // Assert
        await Assert.That(provider).IsTypeOf<CompositeFileProvider>();
    }

    [Test]
    public async Task CreateWwwrootProvider_NullAssembly_UsesDefaultAssembly(CancellationToken ct = default) {
        // Arrange & Act
        IFileProvider provider = FileProviderFactory.CreateWwwrootProvider(includePhysicalFallback: false);

        // Assert
        await Assert.That(provider).IsNotNull();
    }

    [Test]
    public async Task CreateWwwrootProvider_NonExistentPhysicalPath_ReturnsCompositeProvider(CancellationToken ct = default) {
        // Arrange
        Assembly assembly = typeof(FileProviderFactory).Assembly;
        string nonExistentPath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString(), "wwwroot");

        // Act
        IFileProvider provider = FileProviderFactory.CreateWwwrootProvider(
            assembly,
            nonExistentPath
        );

        // Assert
        await Assert.That(provider).IsTypeOf<CompositeFileProvider>();
    }
}
