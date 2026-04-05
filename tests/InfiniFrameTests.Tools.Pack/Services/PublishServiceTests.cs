// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Exceptions;
using InfiniFrame.Tools.Pack.Services;
using InfiniFrameTests.Tools.Pack.TestUtilities;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PublishServiceTests {
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
    public async Task PublishAsync_Throws_WhenProjectFileDoesNotExist() {
        // Arrange
        var options = new PublishOptions {
            ProjectPath = Path.Join(Path.GetTempPath(), $"missing-project-{Guid.NewGuid():N}.csproj"),
            Rid = "auto",
            Configuration = "Release",
            Framework = "net10.0",
            SelfContained = true
        };

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(async () => {
            await PublishService.PublishAsync(options);
        });
    }

    [Test]
    public async Task PublishAsync_ThrowsKnownFailure_WhenNativeDependencyIsMissingFromPublishOutput() {
        // Arrange
        string repoRoot = TemporaryDirectory.Path;

        string nativeProjectPath = Path.Join(repoRoot, "src", "InfiniFrame.Native", "InfiniFrame.Native.proj");
        Directory.CreateDirectory(Path.GetDirectoryName(nativeProjectPath)!);
        await File.WriteAllTextAsync(nativeProjectPath, "<Project></Project>");

        string appDirectory = Path.Join(repoRoot, "samples", "app");
        Directory.CreateDirectory(appDirectory);
        string appProjectPath = Path.Join(appDirectory, "SampleApp.csproj");
        await File.WriteAllTextAsync(appProjectPath, """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
          </PropertyGroup>
        </Project>
        """);

        string outputPath = Path.Join(repoRoot, "publish-output");
        string rid = RuntimeResolver.ResolveRid("auto");

        var options = new PublishOptions {
            ProjectPath = appProjectPath,
            Rid = rid,
            Configuration = "Release",
            Framework = "net10.0",
            SelfContained = true,
            Output = outputPath
        };

        // Act
        var exception = await Assert.ThrowsAsync<NativeDependencyNotFoundException>(async () => {
            await PublishService.PublishAsync(options);
        });

        // Assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.Message.Contains("Could not resolve required InfiniFrame native artifacts from project publish output.", StringComparison.Ordinal)).IsTrue();
    }
}
