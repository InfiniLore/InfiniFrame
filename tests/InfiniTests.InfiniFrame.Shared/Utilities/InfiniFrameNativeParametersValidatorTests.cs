// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameNativeParametersValidatorTests {
    [Test]
    public async Task Validate_AcceptsRelativeIconPathFromAppBaseDirectory(CancellationToken ct = default) {
        // Arrange
        string fileName = $"icon-{Guid.NewGuid():N}.ico";
        string absolutePath = Path.Join(AppContext.BaseDirectory, fileName);
        await File.WriteAllTextAsync(absolutePath, "icon", ct);

        string temporaryCurrentDirectory = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temporaryCurrentDirectory);
        string originalCurrentDirectory = Environment.CurrentDirectory;


        var parameters = new InfiniFrameNativeParameters {
            StartUrl = "https://example.com",
            WindowIconFile = fileName
        };

        bool valid;

        // Act

        try {
            Environment.CurrentDirectory = temporaryCurrentDirectory;
            valid = InfiniFrameNativeParametersValidator.Validate(parameters, NullLogger.Instance);
        }
        finally {
            Environment.CurrentDirectory = originalCurrentDirectory;
            File.Delete(absolutePath);
            Directory.Delete(temporaryCurrentDirectory, true);
        }

        // Assert
        await Assert.That(valid).IsTrue();
    }

    [Test]
    public async Task Validate_CreatesAndAcceptsWritableTemporaryFilesPath(CancellationToken ct = default) {
        // Arrange
        string path = Path.Join(Path.GetTempPath(), "InfiniTests.InfiniFrame.Shared", $"validator-{Guid.NewGuid():N}");
        if (Directory.Exists(path))
            Directory.Delete(path, true);

        var parameters = new InfiniFrameNativeParameters {
            StartUrl = "https://example.com",
            TemporaryFilesPath = path
        };

        // Act
        bool valid = InfiniFrameNativeParametersValidator.Validate(parameters, NullLogger.Instance);

        // Assert
        await Assert.That(valid).IsTrue();
        await Assert.That(Directory.Exists(path)).IsTrue();

        Directory.Delete(path, true);
    }

    [Test]
    public async Task Validate_RejectsInvalidTemporaryFilesPath(CancellationToken ct = default) {
        // Arrange
        var parameters = new InfiniFrameNativeParameters {
            StartUrl = "https://example.com",
            TemporaryFilesPath = $"invalid-{Guid.NewGuid():N}\0path"
        };

        // Act
        bool valid = InfiniFrameNativeParametersValidator.Validate(parameters, NullLogger.Instance);

        // Assert
        await Assert.That(valid).IsFalse();
    }
}
