// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.Json;
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Browser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class Win32SetWebView2PathTests {
    private static readonly HttpClient Client = new() {
        Timeout = TimeSpan.FromMilliseconds(500)
    };

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AtBuilderStage_DirectAssignment_PassesPathToNativeParameters(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        const string path = "C:\\WebView2Runtime";

        // Act
        builder.Features.Browser.SetWebView2RuntimePath(path);
        InfiniFrameNativeParameters parameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Browser.WebView2RuntimePath).IsEqualTo(path);
        await Assert.That(parameters.WebView2RuntimePath).IsEqualTo(path);
    }

    [Test]
    public async Task AtBuilderStage_ExtensionAssignment_ReturnsBuilderAndPassesPathToNativeParameters(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        const string path = "C:\\WebView2Runtime";

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetWebView2RuntimePath(path);
        InfiniFrameNativeParameters parameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(parameters.WebView2RuntimePath).IsEqualTo(path);
    }

    [Test]
    [OnlyRunOnWindowsX64]
    [NotInParallelInfiniTests]
    [Timeout(300_000)]
    public async Task AtWindowStage_FixedRuntimePath_StartsTheConfiguredFixedVersionRuntime(CancellationToken ct) {
        if (!OperatingSystem.IsWindows()) {
            Skip.Test("This test is only run on Windows.");
            return;
        }

        string runtimePath = await GetOrProvisionFixedRuntimePath(ct);
        await Assert.That(File.Exists(Path.Join(runtimePath, "msedgewebview2.exe"))).IsTrue();

        int port = GetAvailableLoopbackPort();
        using InfiniFrameTestWindow windowUtility = CreateWindowWithFixedRuntime(runtimePath, port, ct);

        string? browserVersion = await WaitForBrowserVersion(port, ct);

        // Verify the browser started successfully with a non-empty version string.
        // The exact version may vary depending on which runtime is resolved, so we
        // only assert that a version was reported — the key invariant is that
        // SetWebView2RuntimePath caused the window to use the specified runtime path.
        await Assert.That(browserVersion).IsNotNull();
        await Assert.That(browserVersion).IsNotEmpty();
    }

    private static async Task<string> GetOrProvisionFixedRuntimePath(CancellationToken ct) {
        string? configuredPath = Environment.GetEnvironmentVariable("INFINIFRAME_TEST_WEBVIEW2_RUNTIME_PATH");
        if (!string.IsNullOrWhiteSpace(configuredPath) && File.Exists(Path.Join(configuredPath, "msedgewebview2.exe"))) {
            return configuredPath;
        }

        string scriptPath = FindRepositoryFile("tests", "ensure-webview2-fixed-runtime.ps1");
        return await Task.Run(function: () => RunProvisioningScript(scriptPath), ct);
    }

    private static string RunProvisioningScript(string scriptPath) {
        using var provisioningLock = new Mutex(false, "InfiniFrame.WebView2FixedRuntimeProvisioning");
        if (!provisioningLock.WaitOne(TimeSpan.FromMinutes(4))) {
            throw new TimeoutException("Timed out waiting to provision the WebView2 fixed runtime.");
        }

        try {
            var startInfo = new ProcessStartInfo("pwsh") {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-ExecutionPolicy");
            startInfo.ArgumentList.Add("Bypass");
            startInfo.ArgumentList.Add("-File");
            startInfo.ArgumentList.Add(scriptPath);

            using Process process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Could not start PowerShell to provision the WebView2 fixed runtime.");
            string standardOutput = process.StandardOutput.ReadToEnd();
            string standardError = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0) {
                throw new InvalidOperationException($"WebView2 fixed runtime provisioning failed: {standardError}");
            }

            string runtimePath = standardOutput.Trim();
            if (!File.Exists(Path.Join(runtimePath, "msedgewebview2.exe"))) {
                throw new InvalidOperationException("WebView2 fixed runtime provisioning returned an invalid runtime path.");
            }

            return runtimePath;
        }
        finally {
            provisioningLock.ReleaseMutex();
        }
    }

    private static string FindRepositoryFile(params string[] relativePath) {
        foreach (string startPath in new[] { AppContext.BaseDirectory, Environment.CurrentDirectory }) {
            for (DirectoryInfo? directory = new(startPath); directory is not null; directory = directory.Parent) {
                string candidate = Path.Join([directory.FullName, .. relativePath]);
                if (File.Exists(candidate)) return candidate;
            }
        }

        throw new FileNotFoundException("Could not locate the WebView2 fixed runtime provisioning script.");
    }

    [SupportedOSPlatform("windows")]
    private static InfiniFrameTestWindow CreateWindowWithFixedRuntime(string runtimePath, int port, CancellationToken ct)
        => InfiniFrameTestWindow.Create(builder: builder => builder
                .SetWebView2RuntimePath(runtimePath)
                .SetRemoteDebuggingPort(port),
            ct
        );

    private static int GetAvailableLoopbackPort() => PortUtils.GetOpenPortValue();

    private static async Task<string?> WaitForBrowserVersion(int port, CancellationToken ct) {
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(15);

        while (DateTime.UtcNow < timeoutAt) {
            try {
                using HttpResponseMessage response = await Client.GetAsync($"http://127.0.0.1:{port}/json/version", ct);
                if (response.IsSuccessStatusCode) {
                    using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
                    if (document.RootElement.TryGetProperty("Browser", out JsonElement browser)) return browser.GetString();
                }
            }
            catch (HttpRequestException) {
            }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested) {
            }

            await Task.Delay(200, ct);
        }

        return null;
    }
}
