// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ProcessRunnerTests {
    [Test]
    public async Task RunAsync_ReturnsZero_ForSuccessfulCommand() {
        // Arrange
        const string fileName = "dotnet";
        string[] arguments = ["--version"];

        // Act
        int exitCode = await ProcessRunner.RunAsync(fileName, arguments);

        // Assert
        await Assert.That(exitCode).IsEqualTo(0);
    }

    [Test]
    public async Task RunAsync_ReturnsNonZero_ForFailingCommand() {
        // Arrange
        const string fileName = "dotnet";
        string[] arguments = ["command-that-does-not-exist"];

        // Act
        int exitCode = await ProcessRunner.RunAsync(fileName, arguments);

        // Assert
        await Assert.That(exitCode).IsNotEqualTo(0);
    }

    [Test]
    public async Task RunAsync_Throws_WhenExecutableDoesNotExist() {
        // Arrange
        string fileName = $"definitely-not-a-real-executable-{Guid.NewGuid():N}";
        string[] arguments = [];

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => {
            await ProcessRunner.RunAsync(fileName, arguments);
        });
    }
}
