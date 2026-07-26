// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text.Json;

namespace InfiniTests.InfiniFrame.Window.Features.Browser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class Win32SetWebView2PathTests {
    private static readonly HttpClient Client = new() {
        Timeout = TimeSpan.FromMilliseconds(500)
    };
    private const string FixedRuntimeVersion = "150.0.4078.99";

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
    [Timeout(45_000)]
    public async Task AtWindowStage_FixedRuntimePath_StartsTheConfiguredFixedVersionRuntime(CancellationToken ct) {
        if (!OperatingSystem.IsWindows()) {
            Skip.Test("This test is only run on Windows.");
            return;
        }

        string? runtimePath = Environment.GetEnvironmentVariable("INFINIFRAME_TEST_WEBVIEW2_RUNTIME_PATH");
        if (string.IsNullOrWhiteSpace(runtimePath)) {
            throw new InvalidOperationException(
                "Set INFINIFRAME_TEST_WEBVIEW2_RUNTIME_PATH by running tests/scripts/ensure-webview2-fixed-runtime.ps1 first."
            );
        }
        await Assert.That(File.Exists(Path.Combine(runtimePath, "msedgewebview2.exe"))).IsTrue();

        int port = GetAvailableLoopbackPort();
        using InfiniFrameTestWindow windowUtility = CreateWindowWithFixedRuntime(runtimePath, port, ct);

        string? browserVersion = await WaitForBrowserVersion(port, ct);

        await Assert.That(browserVersion).Contains(FixedRuntimeVersion);
    }

    [SupportedOSPlatform("windows")]
    private static InfiniFrameTestWindow CreateWindowWithFixedRuntime(string runtimePath, int port, CancellationToken ct)
        => InfiniFrameTestWindow.Create(builder => builder
                .SetWebView2RuntimePath(runtimePath)
                .SetRemoteDebuggingPort(port),
            ct
        );

    private static int GetAvailableLoopbackPort() {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

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
