// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniFrameTests.Utilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IconFileUtilitiesTests {
    [Test]
    public async Task ResolveIconFilePath_UsesBaseDirectoryForRelativePath() {
        // Arrange
        string baseDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        string currentDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(baseDirectory);
        Directory.CreateDirectory(currentDirectory);

        const string relativePath = "favicon.ico";
        string expectedAbsolutePath = Path.GetFullPath(relativePath, baseDirectory);
        await File.WriteAllTextAsync(expectedAbsolutePath, "icon");

        string originalCurrentDirectory = Environment.CurrentDirectory;
        string? resolved;

        // Act
        try {
            Environment.CurrentDirectory = currentDirectory;

            resolved = IconFileUtilities.ResolveIconFilePath(relativePath, baseDirectory);
        }
        finally {
            Environment.CurrentDirectory = originalCurrentDirectory;
            Directory.Delete(baseDirectory, true);
            Directory.Delete(currentDirectory, true);
        }

        // Assert
        await Assert.That(resolved).IsEqualTo(expectedAbsolutePath);
    }

    [Test]
    public async Task ResolveIconFilePath_ReturnsNullForMissingPath() {
        // Arrange
        string baseDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(baseDirectory);

        // Act
        string? resolved;
        try {
            resolved = IconFileUtilities.ResolveIconFilePath("missing.ico", baseDirectory);
        }
        finally {
            Directory.Delete(baseDirectory, true);
        }

        // Assert
        await Assert.That(resolved).IsNull();
    }
}