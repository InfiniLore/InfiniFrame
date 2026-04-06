// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Exceptions;
using InfiniFrame.Tools.Pack.Services;
using InfiniFrameTests.Tools.Pack.TestUtilities;
using System.Diagnostics;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PublishServiceTests {
    private static readonly SemaphoreSlim PublishTestLock = new(1, 1);
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
        await PublishTestLock.WaitAsync();
        NativeDependencyNotFoundException? exception;
        try {
            exception = await Assert.ThrowsAsync<NativeDependencyNotFoundException>(async () => {
                await PublishService.PublishAsync(options);
            });
        }
        finally {
            PublishTestLock.Release();
        }

        // Assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.Message.Contains("Could not resolve required InfiniFrame native artifacts from project publish output.", StringComparison.Ordinal)).IsTrue();
    }

    [Test]
    public async Task PublishAsync_ReturnsSuccessAndSingleFileOutput_WhenProjectIncludesInfiniFrame() {
        // Arrange
        string repoRoot = FindRepoRoot();
        string appDirectory = Path.Join(TemporaryDirectory.Path, "minimal-app");
        Directory.CreateDirectory(appDirectory);

        string appProjectPath = Path.Join(appDirectory, "MinimalPublishApp.csproj");
        string infiniFrameProjectPath = Path.Join(repoRoot, "src", "InfiniFrame", "InfiniFrame.csproj");

        await File.WriteAllTextAsync(appProjectPath, $$"""
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net10.0</TargetFramework>
            <ImplicitUsings>enable</ImplicitUsings>
            <Nullable>enable</Nullable>
          </PropertyGroup>
          <ItemGroup>
            <ProjectReference Include="{{infiniFrameProjectPath}}" />
          </ItemGroup>
        </Project>
        """);

        await File.WriteAllTextAsync(Path.Join(appDirectory, "Program.cs"), """
        Console.WriteLine("InfiniFrame pack integration test");
        """);

        string outputPath = Path.Join(TemporaryDirectory.Path, "publish-output");
        string rid = RuntimeResolver.ResolveRid("auto");
        string expectedMainOutput = Path.Join(outputPath, rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase) ? "MinimalPublishApp.exe" : "MinimalPublishApp");

        var options = new PublishOptions {
            ProjectPath = appProjectPath,
            Rid = rid,
            Configuration = "Release",
            Framework = "net10.0",
            SelfContained = true,
            Output = outputPath
        };

        // Act
        await PublishTestLock.WaitAsync();
        int exitCode;
        try {
            exitCode = await PublishService.PublishAsync(options);
        }
        finally {
            PublishTestLock.Release();
        }

        // Assert
        await Assert.That(exitCode).IsEqualTo(ExitCodes.Success);
        await Assert.That(File.Exists(expectedMainOutput)).IsTrue();
        await Assert.That(Directory.GetFileSystemEntries(outputPath, "*", SearchOption.TopDirectoryOnly).Length).IsEqualTo(1);
    }

    [Test]
    public async Task PublishAsync_LaunchedPackedApp_InitializesBootstrapAndExitsSuccessfully() {
        // Arrange
        string repoRoot = FindRepoRoot();
        string appDirectory = Path.Join(TemporaryDirectory.Path, "launch-smoke-app");
        Directory.CreateDirectory(appDirectory);

        string appProjectPath = Path.Join(appDirectory, "LaunchSmokeApp.csproj");
        string infiniFrameProjectPath = Path.Join(repoRoot, "src", "InfiniFrame", "InfiniFrame.csproj");
        const string startupMarker = "BOOTSTRAP_SMOKE_OK";

        await File.WriteAllTextAsync(appProjectPath, $$"""
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net10.0</TargetFramework>
            <ImplicitUsings>enable</ImplicitUsings>
            <Nullable>enable</Nullable>
          </PropertyGroup>
          <ItemGroup>
            <ProjectReference Include="{{infiniFrameProjectPath}}" />
          </ItemGroup>
        </Project>
        """);

        await File.WriteAllTextAsync(Path.Join(appDirectory, "Program.cs"), $$"""
        using InfiniFrame;

        InfiniFrameSingleFileBootstrap.Initialize();
        Console.WriteLine("{{startupMarker}}");
        return 0;
        """);

        string outputPath = Path.Join(TemporaryDirectory.Path, "launch-smoke-publish-output");
        string rid = RuntimeResolver.ResolveRid("auto");
        string publishedExecutable = Path.Join(outputPath, rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase) ? "LaunchSmokeApp.exe" : "LaunchSmokeApp");

        var options = new PublishOptions {
            ProjectPath = appProjectPath,
            Rid = rid,
            Configuration = "Release",
            Framework = "net10.0",
            SelfContained = true,
            Output = outputPath
        };

        // Act
        await PublishTestLock.WaitAsync();
        int publishExitCode;
        try {
            publishExitCode = await PublishService.PublishAsync(options);
        }
        finally {
            PublishTestLock.Release();
        }
        ProcessResult runResult = await RunProcessAndCaptureAsync(publishedExecutable, appDirectory);

        // Assert
        await Assert.That(publishExitCode).IsEqualTo(ExitCodes.Success);
        await Assert.That(runResult.ExitCode).IsEqualTo(0);
        await Assert.That(runResult.StandardOutput.Contains(startupMarker, StringComparison.Ordinal)).IsTrue();
    }

    [Test]
    public async Task ValidateOutputShape_ReturnsUnexpectedEntries_WhenExtraPayloadFilesRemain() {
        // Arrange
        string output = TemporaryDirectory.Path;
        string expectedMainOutput = Path.Join(output, "SampleApp.exe");
        await File.WriteAllTextAsync(expectedMainOutput, "main");
        await File.WriteAllTextAsync(Path.Join(output, "leftover.payload"), "extra");
        Directory.CreateDirectory(Path.Join(output, "nested-assets"));

        // Act
        PublishService.OutputShapeValidation validation = PublishService.ValidateOutputShape(output, expectedMainOutput);

        // Assert
        await Assert.That(validation.FoundMainOutput).IsTrue();
        await Assert.That(validation.UnexpectedEntries).Contains("leftover.payload");
        await Assert.That(validation.UnexpectedEntries).Contains("nested-assets");
    }

    [Test]
    public async Task ValidateOutputShape_UsesPlatformPathCasingRules() {
        // Arrange
        string output = TemporaryDirectory.Path;
        string actualMainOutput = Path.Join(output, "SampleApp.exe");
        string expectedMainOutput = Path.Join(output, "sampleapp.exe");
        await File.WriteAllTextAsync(actualMainOutput, "main");

        // Act
        PublishService.OutputShapeValidation validation = PublishService.ValidateOutputShape(output, expectedMainOutput);

        // Assert
        if (OperatingSystem.IsWindows()) {
            await Assert.That(validation.FoundMainOutput).IsTrue();
            await Assert.That(validation.UnexpectedEntries.Length).IsEqualTo(0);
            return;
        }

        await Assert.That(validation.FoundMainOutput).IsFalse();
        await Assert.That(validation.UnexpectedEntries).Contains("SampleApp.exe");
    }

    private static string FindRepoRoot() {
        DirectoryInfo? current = new(AppContext.BaseDirectory);
        while (current is not null) {
            if (File.Exists(Path.Join(current.FullName, "InfiniFrame.slnx"))) return current.FullName;
            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root containing InfiniFrame.slnx.");
    }

    private static async Task<ProcessResult> RunProcessAndCaptureAsync(string fileName, string workingDirectory) {
        var startInfo = new ProcessStartInfo(fileName) {
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = new Process();
        process.StartInfo = startInfo;

        if (!process.Start()) throw new InvalidOperationException($"Failed to start process: {fileName}");

        Task<string> standardOutputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> standardErrorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        string standardOutput = await standardOutputTask;
        string standardError = await standardErrorTask;

        return new ProcessResult(process.ExitCode, standardOutput, standardError);
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
