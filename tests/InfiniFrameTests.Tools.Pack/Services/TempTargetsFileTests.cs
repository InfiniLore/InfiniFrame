// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Tools.Pack.Services;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TempTargetsFileTests {
    [Test]
    public async Task Create_CreatesTargetsFileWithExpectedContents() {
        // Arrange

        // Act
        using var tempTargetsFile = TempTargetsFile.Create();

        // Assert
        await Assert.That(File.Exists(tempTargetsFile.Path)).IsTrue();
        string contents = await File.ReadAllTextAsync(tempTargetsFile.Path);
        await Assert.That(contents).Contains("InfiniFramePackCleanupPublishArtifacts");
        await Assert.That(contents).Contains("InfiniFramePackRemoveTransitiveNativeFiles");
        await Assert.That(contents).Contains("wwwroot\\\\**\\\\*");
        foreach (string nativeFileName in InfiniFrameNativeArtifactManifest.AllFileNames) {
            await Assert.That(contents).Contains(nativeFileName);
        }
    }

    [Test]
    public async Task Dispose_DeletesCreatedTargetsFile() {
        // Arrange
        var tempTargetsFile = TempTargetsFile.Create();
        string path = tempTargetsFile.Path;

        // Act
        tempTargetsFile.Dispose();

        // Assert
        await Assert.That(File.Exists(path)).IsFalse();
    }

    [Test]
    public async Task Dispose_DoesNotThrow_WhenFileWasDeletedExternally() {
        // Arrange
        var tempTargetsFile = TempTargetsFile.Create();
        string path = tempTargetsFile.Path;
        File.Delete(path);

        // Act
        tempTargetsFile.Dispose();

        // Assert
        await Assert.That(File.Exists(path)).IsFalse();
    }
}
