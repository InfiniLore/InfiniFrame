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
    [DisplayName($"{nameof(PublishOutputCleanerTests)}.{nameof(Cleanup_RemovesWwwrootAndNativeRuntimeFiles_WhenTheyExist)}")]
    public async Task Cleanup_RemovesWwwrootAndNativeRuntimeFiles_WhenTheyExist() {
        // Arrange
        string output = TemporaryDirectory.Path;
        string wwwroot = Path.Combine(output, "wwwroot");

        Directory.CreateDirectory(wwwroot);
        await File.WriteAllTextAsync(Path.Combine(wwwroot, "index.html"), "<html></html>");
        foreach (string file in NativeRuntimeBuilder.NativeRuntimeFiles) {
            await File.WriteAllTextAsync(Path.Combine(output, file), string.Empty);
        }

        // Act
        PublishOutputCleaner.Cleanup(output);

        // Assert
        await Assert.That(Directory.Exists(wwwroot)).IsFalse();
        foreach (string file in NativeRuntimeBuilder.NativeRuntimeFiles) {
            await Assert.That(File.Exists(Path.Combine(output, file))).IsFalse();
        }
    }

    [Test]
    [DisplayName($"{nameof(PublishOutputCleanerTests)}.{nameof(Cleanup_DoesNotThrow_WhenTargetFilesDoNotExist)}")]
    public async Task Cleanup_DoesNotThrow_WhenTargetFilesDoNotExist() {
        // Arrange
        string output = TemporaryDirectory.Path;

        // Act
        PublishOutputCleaner.Cleanup(output);

        // Assert
        await Assert.That(Directory.Exists(output)).IsTrue();
    }
}
