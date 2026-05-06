// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniFrameTests.Utilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IconFileUtilityTests {
    [Test]
    public async Task TryResolveIconFilePath_UsesBaseDirectoryForRelativePath() {
        // Arrange
        string baseDirectory = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(baseDirectory);

        const string relativePath = "favicon.ico";
        string expectedAbsolutePath = Path.GetFullPath(relativePath, baseDirectory);
        await File.WriteAllTextAsync(expectedAbsolutePath, "icon");

        bool found;
        string? resolved;

        // Act
        try {
            found = IconFileUtility.TryResolveIconFilePath(relativePath, out resolved, baseDirectory);
        }
        finally {
            Directory.Delete(baseDirectory, true);
        }

        // Assert
        await Assert.That(found).IsTrue();
        await Assert.That(resolved).IsEqualTo(expectedAbsolutePath);
    }

    [Test]
    public async Task TryResolveIconFilePath_ReturnsNullForMissingPath() {
        // Arrange
        string baseDirectory = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(baseDirectory);

        // Act
        bool found;
        string? resolved;
        try {
            found = IconFileUtility.TryResolveIconFilePath("missing.ico", out resolved, baseDirectory);
        }
        finally {
            Directory.Delete(baseDirectory, true);
        }

        // Assert
        await Assert.That(found).IsFalse();
        await Assert.That(resolved).IsNull();
    }
}
