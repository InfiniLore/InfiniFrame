// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Playwright;
using System.Net;
using System.Net.Sockets;

namespace InfiniFrameTests.Playwright.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class PlaywrightConnectionUtility {
    private static readonly TimeSpan DefaultPlaywrightCreateTimeout = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan DefaultPlaywrightConnectTimeout = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan DefaultPlaywrightConnectRetryWindow = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan DefaultPlaywrightConnectRetryInterval = TimeSpan.FromSeconds(2);

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static int GetAvailablePort() {
        using TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public static async Task<IPlaywright> CreatePlaywrightAsync(TimeSpan timeout = default) {
        if (timeout == TimeSpan.Zero)
            timeout = DefaultPlaywrightCreateTimeout;

        return await Microsoft.Playwright.Playwright.CreateAsync().WaitAsync(timeout);
    }

    public static async Task<IBrowser> ConnectOverCdpWithRetryAsync(
        IPlaywright playwright,
        Uri url,
        TimeSpan connectTimeout = default,
        TimeSpan retryWindow = default,
        TimeSpan retryInterval = default
    ) {
        if (connectTimeout == TimeSpan.Zero)
            connectTimeout = DefaultPlaywrightConnectTimeout;
        if (retryWindow == TimeSpan.Zero)
            retryWindow = DefaultPlaywrightConnectRetryWindow;
        if (retryInterval == TimeSpan.Zero)
            retryInterval = DefaultPlaywrightConnectRetryInterval;

        using var retryWindowCancellation = new CancellationTokenSource(retryWindow);
        CancellationToken cancellationToken = retryWindowCancellation.Token;
        Exception? lastException = null;
        int attempt = 0;

        while (!cancellationToken.IsCancellationRequested) {
            attempt++;
            try {
                Console.WriteLine($"[PlaywrightConnect] CDP attempt {attempt} to {url}");
                return await playwright.Chromium
                    .ConnectOverCDPAsync(url.ToString())
                    .WaitAsync(connectTimeout, cancellationToken);
            }
            catch (PlaywrightException ex) {
                lastException = ex;
                Console.WriteLine($"[PlaywrightConnect] CDP attempt {attempt} failed with PlaywrightException: {ex.Message}");
            }
            catch (TimeoutException ex) {
                lastException = ex;
                Console.WriteLine($"[PlaywrightConnect] CDP attempt {attempt} timed out after {connectTimeout.TotalSeconds}s.");
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
                break;
            }

            try {
                await Task.Delay(retryInterval, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
                break;
            }
        }

        throw new TimeoutException(
            $"Timed out connecting Playwright over CDP at '{url}' within {retryWindow.TotalSeconds} seconds.",
            lastException
        );
    }

    public static void CloseBrowserSafely(IBrowser? browser) {
        try {
            browser?.CloseAsync().GetAwaiter().GetResult();
        }
        catch (PlaywrightException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }
    }

    public static void DisposePlaywrightSafely(IPlaywright? playwright) {
        try {
            playwright?.Dispose();
        }
        catch (ObjectDisposedException) {
            // ignored
        }
    }

    public static TimeSpan GetVisibleDebugDelay() {
        string? debugEnabled = Environment.GetEnvironmentVariable("PLAYWRIGHT_VISIBLE_DEBUG");
        if (!string.Equals(debugEnabled, "1", StringComparison.Ordinal))
            return TimeSpan.Zero;

        string? secondsValue = Environment.GetEnvironmentVariable("PLAYWRIGHT_VISIBLE_DEBUG_SECONDS");
        if (int.TryParse(secondsValue, out int seconds) && seconds > 0)
            return TimeSpan.FromSeconds(seconds);

        return TimeSpan.FromSeconds(8);
    }

}
