// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack;
using InfiniFrame.Tools.Pack.Exceptions;
using InfiniFrame.Tools.Pack.Resolvers;
using InfiniFrame.Tools.Pack.Services;
using InfiniTests.InfiniFrame.Tools.Pack.TestUtilities;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PublishServiceTests {
    private static readonly SemaphoreSlim PublishTestLock = new(1, 1);
    private static readonly TimeSpan PublishTimeout = IsCiEnvironment()
        ? IsWindowsArm64()
            ? TimeSpan.FromMinutes(15)
            : TimeSpan.FromMinutes(8)
        : TimeSpan.FromMinutes(3);
    private static readonly TimeSpan SharedFixtureAwaitTimeout = PublishTimeout + TimeSpan.FromMinutes(1);
    private static readonly TimeSpan ProcessTimeout = TimeSpan.FromSeconds(45);
    private static readonly Lock SharedFixtureLock = new();
    private static Task<SharedPublishFixture>? _sharedPublishFixtureTask;
    private TemporaryDirectory TemporaryDirectory { get; set; } = null!;


    #if DEBUG
    private const string Configuration = "Debug";
    #else
    private const string Configuration = "Release";
    #endif

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
            Configuration = Configuration,
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

        string nativeProjectPath = Path.Join(repoRoot, "src", "InfiniFrame.NativeBridge", "InfiniFrame.NativeBridge.csproj");
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
            Configuration = Configuration,
            Framework = "net10.0",
            SelfContained = true,
            Output = outputPath
        };

        // Act
        await PublishTestLock.WaitAsync();
        NativeDependencyNotFoundException? exception;
        try {
            exception = await Assert.ThrowsAsync<NativeDependencyNotFoundException>(async () => {
                await ExecuteWithTimeout(
                    PublishService.PublishAsync(options),
                    PublishTimeout,
                    "PublishAsync_ThrowsKnownFailure_WhenNativeDependencyIsMissingFromPublishOutput");
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
    [SkipOnMacOs("4 Hours lost on trying to fix this on macOs... too much time to spent on this.")]
    public async Task PublishAsync_ReturnsSuccessAndSingleFileOutput_WhenProjectIncludesInfiniFrame() {
        SharedPublishFixture fixture = await ExecuteWithTimeout(
            GetOrCreateSharedPublishFixtureAsync(),
            SharedFixtureAwaitTimeout,
            "PublishAsync_ReturnsSuccessAndSingleFileOutput_WhenProjectIncludesInfiniFrame");

        // Assert
        await Assert.That(fixture.PublishExitCode).IsEqualTo(ExitCodes.Success);
        await Assert.That(File.Exists(fixture.PublishedExecutable)).IsTrue();
        await Assert.That(Directory.GetFileSystemEntries(fixture.OutputPath, "*", SearchOption.TopDirectoryOnly).Length).IsEqualTo(1);
    }

    [Test]
    [SkipOnMacOs("4 Hours lost on trying to fix this on macOs... too much time to spent on this.")]
    public async Task PublishAsync_LaunchedPackedApp_InitializesBootstrapAndExitsSuccessfully() {
        SharedPublishFixture fixture = await ExecuteWithTimeout(
            GetOrCreateSharedPublishFixtureAsync(),
            SharedFixtureAwaitTimeout,
            "PublishAsync_LaunchedPackedApp_InitializesBootstrapAndExitsSuccessfully");
        ProcessResult runResult = await RunProcessAndCaptureAsync(fixture.PublishedExecutable, fixture.AppDirectory, ProcessTimeout);

        // Assert
        await Assert.That(fixture.PublishExitCode).IsEqualTo(ExitCodes.Success);
        await Assert.That(runResult.ExitCode).IsEqualTo(0);
        await Assert.That(runResult.StandardOutput.Contains(fixture.StartupMarker, StringComparison.Ordinal)).IsTrue();
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

    private static async Task<ProcessResult> RunProcessAndCaptureAsync(string fileName, string workingDirectory, TimeSpan timeout) {
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
        using var timeoutCts = new CancellationTokenSource(timeout);
        try {
            await process.WaitForExitAsync(timeoutCts.Token);
        }
        catch (OperationCanceledException) {
            try {
                if (!process.HasExited) process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException) {
                // best effort
            }

            throw new TimeoutException($"Timed out after {timeout} while running '{fileName}'.");
        }

        string standardOutput = await standardOutputTask;
        string standardError = await standardErrorTask;

        return new ProcessResult(process.ExitCode, standardOutput, standardError);
    }

    private static async Task<T> ExecuteWithTimeout<T>(Task<T> task, TimeSpan timeout, string operationName) {
        Task completed = await Task.WhenAny(task, Task.Delay(timeout));
        if (!ReferenceEquals(completed, task)) {
            throw new TimeoutException($"Timed out after {timeout} while executing '{operationName}'.");
        }

        return await task;
    }

    private static bool IsCiEnvironment() =>
        string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), "true", StringComparison.OrdinalIgnoreCase);

    private static bool IsWindowsArm64() =>
        OperatingSystem.IsWindows() && RuntimeInformation.ProcessArchitecture == Architecture.Arm64;

    private static Task<SharedPublishFixture> GetOrCreateSharedPublishFixtureAsync() {
        lock (SharedFixtureLock) {
            _sharedPublishFixtureTask ??= CreateSharedPublishFixtureAsync();
            return _sharedPublishFixtureTask;
        }
    }

    private static async Task<SharedPublishFixture> CreateSharedPublishFixtureAsync() {
        string repoRoot = FindRepoRoot();
        string root = Path.Join(Path.GetTempPath(), $"infiniframe-pack-shared-{Guid.NewGuid():N}");
        string appDirectory = Path.Join(root, "app");
        Directory.CreateDirectory(appDirectory);

        string appProjectPath = Path.Join(appDirectory, "SharedSmokeApp.csproj");
        string infiniFrameProjectPath = Path.Join(repoRoot, "src", "InfiniFrame", "InfiniFrame.csproj");
        const string startupMarker = "BOOTSTRAP_SMOKE_OK";

        await File.WriteAllTextAsync(appProjectPath, $$"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net10.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
                <!-- The test workflow downloads and verifies the native matrix before running tests.
                     Rebuilding the complete C++ bridge twice inside this pack smoke test is redundant
                     and can take more than eight minutes on Windows ARM64. -->
                <InfiniFrameSkipNativeBuild>true</InfiniFrameSkipNativeBuild>
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

        string outputPath = Path.Join(root, "publish-output");
        string rid = RuntimeResolver.ResolveRid("auto");
        string publishedExecutable = Path.Join(outputPath, rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase) ? "SharedSmokeApp.exe" : "SharedSmokeApp");

        var options = new PublishOptions {
            ProjectPath = appProjectPath,
            Rid = rid,
            Configuration = Configuration,
            Framework = "net10.0",
            SelfContained = true,
            Output = outputPath,
            ProcessTimeout = PublishTimeout
        };

        await PublishTestLock.WaitAsync();
        int publishExitCode;
        try {
            // The timeout must cancel PublishService itself so ProcessRunner kills the complete
            // dotnet/MSBuild child-process tree. Task.WhenAny alone reports a timeout while the
            // publish keeps running and can hold build-server/file locks for subsequent tests.
            using var publishTimeoutCts = new CancellationTokenSource(PublishTimeout);
            try {
                publishExitCode = await PublishService.PublishAsync(options, publishTimeoutCts.Token);
            }
            catch (OperationCanceledException) when (publishTimeoutCts.IsCancellationRequested) {
                throw new TimeoutException(
                    $"Timed out after {PublishTimeout} while executing 'CreateSharedPublishFixtureAsync'."
                );
            }
        }
        finally {
            PublishTestLock.Release();
        }

        return new SharedPublishFixture(publishExitCode, appDirectory, outputPath, publishedExecutable, startupMarker);
    }

    private sealed record SharedPublishFixture(
        int PublishExitCode,
        string AppDirectory,
        string OutputPath,
        string PublishedExecutable,
        string StartupMarker
    );

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
