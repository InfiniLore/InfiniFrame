// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

namespace InfiniFrameAutomationTests.Selenium;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class SeleniumWebDriverSession : IDisposable {
    private static readonly HttpClient Http = new() {
        Timeout = TimeSpan.FromSeconds(2)
    };
    public required IWebDriver Driver { get; init; }
    private Process? DriverProcess { get; init; }

    private int _disposed;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static async Task<SeleniumWebDriverSession> StartAsync(CancellationToken cancellationToken = default) {
        string? endpointOverride = Environment.GetEnvironmentVariable("INFINIFRAME_WEBKIT_WEBDRIVER_URL");
        if (!string.IsNullOrWhiteSpace(endpointOverride)) {
            var endpoint = new Uri(endpointOverride);
            IWebDriver driver = CreateDriver(endpoint);
            return new SeleniumWebDriverSession {
                Driver = driver,
                DriverProcess = null
            };
        }

        int port = GetAvailablePort();
        var endpointUri = new Uri($"http://127.0.0.1:{port}");
        Process process = StartDriverProcess(port);

        await WaitUntilServerHealthyAsync(endpointUri, process, TimeSpan.FromSeconds(20), cancellationToken);

        IWebDriver sessionDriver = CreateDriver(endpointUri);

        return new SeleniumWebDriverSession {
            Driver = sessionDriver,
            DriverProcess = process
        };
    }

    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        try {
            Driver.Quit();
        }
        catch {
            // ignored
        }

        try {
            Driver.Dispose();
        }
        catch {
            // ignored
        }

        if (DriverProcess is null) return;

        try {
            if (!DriverProcess.HasExited) {
                DriverProcess.Kill(entireProcessTree: true);
            }
        }
        catch {
            // ignored
        }
    }

    private static RemoteWebDriver CreateDriver(Uri endpointUri) {
        string[] browserNames = ["MiniBrowser", "WebKitGTK", "webkitgtk"];
        Exception? lastError = null;

        foreach (string browserName in browserNames) {
            try {
                var options = new SafariOptions {
                    AcceptInsecureCertificates = true
                };
                options.AddAdditionalOption("browserName", browserName);
                return new RemoteWebDriver(endpointUri, options.ToCapabilities(), TimeSpan.FromSeconds(60));
            }
            catch (Exception ex) {
                lastError = ex;
            }
        }

        throw new InvalidOperationException(
            $"Unable to start WebKit WebDriver session at {endpointUri}. Ensure WebKitWebDriver is available and automation is enabled.",
            lastError
        );
    }

    private static Process StartDriverProcess(int port) {
        var startInfo = new ProcessStartInfo {
            FileName = "WebKitWebDriver",
            Arguments = $"--port={port}",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process? process = Process.Start(startInfo);
        return process ?? throw new InvalidOperationException("Failed to start WebKitWebDriver process.");
    }

    private static async Task WaitUntilServerHealthyAsync(Uri endpointUri, Process process, TimeSpan timeout, CancellationToken cancellationToken) {

        DateTime deadline = DateTime.UtcNow.Add(timeout);
        Exception? lastError = null;

        while (DateTime.UtcNow < deadline) {
            cancellationToken.ThrowIfCancellationRequested();

            if (process.HasExited) {
                string stderr = process.StandardError.ReadToEnd();
                throw new InvalidOperationException(
                    $"WebKitWebDriver exited early with code {process.ExitCode}. stderr: {stderr}");
            }

            try {
                using HttpResponseMessage response = await Http.GetAsync(new Uri(endpointUri, "/status"), cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK) {
                    return;
                }
            }
            catch (Exception ex) {
                lastError = ex;
            }

            await Task.Delay(300, cancellationToken);
        }

        throw new TimeoutException($"Timed out waiting for WebKitWebDriver at {endpointUri}.", lastError);
    }

    private static int GetAvailablePort() {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}


