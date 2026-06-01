// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;

namespace InfiniTests.InfiniFrame.Tools.Pack.Services;
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

    [Test]
    public async Task RunWithOutputAsync_CapturesStandardError_ForFailingCommand() {
        // Arrange
        const string fileName = "dotnet";
        string[] arguments = ["command-that-does-not-exist"];

        // Act
        ProcessRunner.ProcessRunResult result = await ProcessRunner.RunWithOutputAsync(fileName, arguments);

        // Assert
        await Assert.That(result.ExitCode).IsNotEqualTo(0);
        await Assert.That(string.IsNullOrWhiteSpace(result.StandardOutput) && string.IsNullOrWhiteSpace(result.StandardError)).IsFalse();
    }

    [Test]
    public async Task RunAsync_ThrowsTimeoutException_WhenProcessExceedsTimeout() {
        // Arrange
        (string fileName, string[] arguments) = BuildLongRunningCommand();

        // Act & Assert
        TimeoutException? ex = await Assert.ThrowsAsync<TimeoutException>(async () => {
            await ProcessRunner.RunAsync(fileName, arguments, timeout: TimeSpan.FromMilliseconds(250));
        });

        await Assert.That(ex).IsNotNull();
        await Assert.That(ex!.Message).Contains("Timed out after");
    }

    private static (string FileName, string[] Arguments) BuildLongRunningCommand() {
        if (OperatingSystem.IsWindows()) {
            return ("powershell", ["-NoProfile", "-Command", "Start-Sleep -Seconds 5"]);
        }

        return ("sh", ["-c", "sleep 5"]);
    }
}
