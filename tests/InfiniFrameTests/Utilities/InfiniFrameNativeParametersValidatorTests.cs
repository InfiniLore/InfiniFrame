// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrameTests.Utilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameNativeParametersValidatorTests {
    [Test]
    public async Task Validate_AcceptsRelativeIconPathFromAppBaseDirectory() {
        // Arrange
        var fileName = $"icon-{Guid.NewGuid():N}.ico";
        string absolutePath = Path.Join(AppContext.BaseDirectory, fileName);
        await File.WriteAllTextAsync(absolutePath, "icon");

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
}