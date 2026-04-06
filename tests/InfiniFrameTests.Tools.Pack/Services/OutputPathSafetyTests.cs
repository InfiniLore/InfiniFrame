// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;
using InfiniFrameTests.Tools.Pack.TestUtilities;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OutputPathSafetyTests {
    private TemporaryDirectory TemporaryDirectory { get; set; } = null!;

    [Before(Test)]
    public void Before() {
        TemporaryDirectory = TemporaryDirectory.Create();
    }

    [After(Test)]
    public void After() {
        TemporaryDirectory.Dispose();
        TemporaryDirectory = null!;
    }

    [Test]
    public async Task EnsureOutputCanBeDeleted_AllowsProjectBinPath() {
        string projectDirectory = Path.Join(TemporaryDirectory.Path, "app");
        string outputPath = Path.Join(projectDirectory, "bin", "Release", "net10.0", "win-x64", "publish");

        bool output = OutputPathSafety.EnsureOutputCanBeDeleted(outputPath, projectDirectory, forceCleanOutput: false);
        await Assert.That(output).IsTrue();
    }

    [Test]
    public async Task EnsureOutputCanBeDeleted_ThrowsForNonDefaultPath_WhenNotForced() {
        string projectDirectory = Path.Join(TemporaryDirectory.Path, "app");
        string outputPath = Path.Join(TemporaryDirectory.Path, "publish-output");

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => {
            OutputPathSafety.EnsureOutputCanBeDeleted(outputPath, projectDirectory, forceCleanOutput: false);
            return Task.CompletedTask;
        }) ?? throw new InvalidOperationException("Expected exception was not thrown.");

        await Assert.That(ex.Message).Contains("--force-clean-output");
    }

    [Test]
    public async Task EnsureOutputCanBeDeleted_AllowsNonDefaultPath_WhenForced() {
        string projectDirectory = Path.Join(TemporaryDirectory.Path, "app");
        string outputPath = Path.Join(TemporaryDirectory.Path, "publish-output");

        bool output = OutputPathSafety.EnsureOutputCanBeDeleted(outputPath, projectDirectory, forceCleanOutput: true);
        await Assert.That(output).IsTrue();
    }

    [Test]
    public async Task EnsureOutputCanBeDeleted_RejectsCaseMismatchForBinDirectory_OnCaseSensitivePlatforms() {
        string projectDirectory = Path.Join(TemporaryDirectory.Path, "app");
        string outputPath = Path.Join(projectDirectory, "BIN", "Release", "net10.0", "win-x64", "publish");

        if (OperatingSystem.IsWindows()) {
            bool output = OutputPathSafety.EnsureOutputCanBeDeleted(outputPath, projectDirectory, forceCleanOutput: false);
            await Assert.That(output).IsTrue();
            return;
        }

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => {
            OutputPathSafety.EnsureOutputCanBeDeleted(outputPath, projectDirectory, forceCleanOutput: false);
            return Task.CompletedTask;
        }) ?? throw new InvalidOperationException("Expected exception was not thrown.");

        await Assert.That(ex.Message).Contains("--force-clean-output");
    }
}
