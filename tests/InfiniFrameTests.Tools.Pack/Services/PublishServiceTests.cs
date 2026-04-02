// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
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
            ProjectPath = Path.Combine(Path.GetTempPath(), $"missing-project-{Guid.NewGuid():N}.csproj"),
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
    public async Task PublishAsync_Throws_WhenNativeBuildFails() {
        // Arrange
        string repoRoot = TemporaryDirectory.Path;

        string nativeProjectPath = Path.Combine(repoRoot, "src", "InfiniFrame.Native", "InfiniFrame.Native.proj");
        Directory.CreateDirectory(Path.GetDirectoryName(nativeProjectPath)!);
        await File.WriteAllTextAsync(nativeProjectPath, "<Project></Project>");

        string appDirectory = Path.Combine(repoRoot, "samples", "app");
        Directory.CreateDirectory(appDirectory);
        string appProjectPath = Path.Combine(appDirectory, "SampleApp.csproj");
        await File.WriteAllTextAsync(appProjectPath, """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
          </PropertyGroup>
        </Project>
        """);

        string outputPath = Path.Combine(repoRoot, "publish-output");
        string rid = RuntimeResolver.ResolveRid("auto");

        var options = new PublishOptions {
            ProjectPath = appProjectPath,
            Rid = rid,
            Configuration = "Release",
            Framework = "net10.0",
            SelfContained = true,
            Output = outputPath
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => {
                await PublishService.PublishAsync(options);
            })
            .WithMessage("Native build failed with exit code 1.");
    }
}
