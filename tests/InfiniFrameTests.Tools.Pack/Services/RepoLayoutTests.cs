// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;
using InfiniFrameTests.Tools.Pack.TestUtilities;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RepoLayoutTests {
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
    [DisplayName($"{nameof(RepoLayoutTests)}.{nameof(Resolve_ReturnsExpectedPaths_WhenRepositoryStructureIsValid)}")]
    public async Task Resolve_ReturnsExpectedPaths_WhenRepositoryStructureIsValid() {
        // Arrange
        string repoRoot = TemporaryDirectory.Path;
        string projectDirectory = Path.Combine(repoRoot, "samples", "app");
        string nativeProject = Path.Combine(repoRoot, "src", "InfiniFrame.Native", "InfiniFrame.Native.proj");

        Directory.CreateDirectory(projectDirectory);
        Directory.CreateDirectory(Path.GetDirectoryName(nativeProject)!);
        await File.WriteAllTextAsync(nativeProject, "<Project />");

        // Act
        RepoPaths paths = RepoLayout.Resolve(projectDirectory, "win-x64", "Release");

        // Assert
        await Assert.That(paths.RepoRoot).IsEqualTo(repoRoot);
        await Assert.That(paths.NativeProjectPath).IsEqualTo(nativeProject);
        await Assert.That(paths.NativePlatform).IsEqualTo("x64");
        await Assert.That(paths.NativeArtifactsDir)
            .IsEqualTo(Path.Combine(repoRoot, "artifacts", "native", "windows", "x64", "Release"));
    }

    [Test]
    [DisplayName($"{nameof(RepoLayoutTests)}.{nameof(Resolve_Throws_WhenRepositoryRootCannotBeFound)}")]
    public async Task Resolve_Throws_WhenRepositoryRootCannotBeFound() {
        // Arrange
        string projectDirectory = Path.Combine(TemporaryDirectory.Path, "samples", "app");
        Directory.CreateDirectory(projectDirectory);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                RepoLayout.Resolve(projectDirectory, "linux-arm64", "Debug");
                return Task.CompletedTask;
            })
            .WithMessage("Unable to locate repository root containing src/InfiniFrame.Native/InfiniFrame.Native.proj.");
    }
}
