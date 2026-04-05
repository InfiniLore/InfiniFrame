// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;
using InfiniFrameTests.Tools.Pack.TestUtilities;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PublishOutputCleanerTests {
    private TemporaryDirectory TemporaryDirectory { get; set; } = null!;

    // -----------------------------------------------------------------------------------------------------------------
    // Test Setup
    // -----------------------------------------------------------------------------------------------------------------
    [Before(Test)]
    public void Before() {
        TemporaryDirectory = TemporaryDirectory.Create();
    }

    [After(Test)]
    public void After() {
        TemporaryDirectory.Dispose();
        TemporaryDirectory = null!;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Cleanup_RemovesWwwrootAndNativeRuntimeFiles_WhenTheyExist() {
        // Arrange
        string output = TemporaryDirectory.Path;
        string wwwroot = Path.Join(output, "wwwroot");

        Directory.CreateDirectory(wwwroot);
        await File.WriteAllTextAsync(Path.Join(wwwroot, "index.html"), "<html></html>");
        foreach (string file in NativeRuntimeBuilder.NativeRuntimeFiles) {
            await File.WriteAllTextAsync(Path.Join(output, file), string.Empty);
        }

        // Act
        PublishOutputCleaner.Cleanup(output);

        // Assert
        await Assert.That(Directory.Exists(wwwroot)).IsFalse();
        foreach (string file in NativeRuntimeBuilder.NativeRuntimeFiles) {
            await Assert.That(File.Exists(Path.Join(output, file))).IsFalse();
        }
    }

    [Test]
    public async Task Cleanup_DoesNotThrow_WhenTargetFilesDoNotExist() {
        // Arrange
        string output = TemporaryDirectory.Path;

        // Act
        PublishOutputCleaner.Cleanup(output);

        // Assert
        await Assert.That(Directory.Exists(output)).IsTrue();
    }
}
