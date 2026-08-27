// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Net;
using System.Net.Sockets;
using Microsoft.Playwright;

namespace InfiniAutomationTests.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class PlaywrightConnectionUtility {
    private const string LoopbackAddress = "127.0.0.1";
    private static readonly TimeSpan DefaultPlaywrightCreateTimeout = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan DefaultPlaywrightConnectTimeout = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan DefaultPlaywrightConnectRetryWindow = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan DefaultPlaywrightConnectRetryInterval = TimeSpan.FromSeconds(2);

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static int GetAvailablePort() {
        using Mutex mutex = new(false, "InfiniFramePlaywrightPortReservation");

        try {
            mutex.WaitOne();
        }
        catch (AbandonedMutexException) {
            // Previous test process exited while reserving a port. The reservation file is still usable.
        }

        try {
            string reservationFile = Path.Join(Path.GetTempPath(), "infiniframe-playwright", "reserved-ports.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(reservationFile)!);
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset oldestValidReservation = now.AddHours(-6);

            List<(int Port, DateTimeOffset ReservedAt)> activeReservations = File.Exists(reservationFile)
                ? [
                    .. File.ReadLines(reservationFile)
                        .Select(ParsePortReservation)
                        .Where(reservation => reservation.Port > 0 && reservation.ReservedAt >= oldestValidReservation)
                ]
                : [];
            HashSet<int> reservedPorts = [.. activeReservations.Select(static reservation => reservation.Port)];

            for (int attempt = 0; attempt < 100; attempt++) {
                using TcpListener listener = new(IPAddress.Loopback, 0);
                listener.Start();
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;

                if (reservedPorts.Contains(port))
                    continue;

                activeReservations.Add((port, now));
                File.WriteAllLines(
                    reservationFile,
                    activeReservations.Select(static reservation => $"{reservation.Port}|{reservation.ReservedAt.UtcTicks}"));
                return port;
            }

            throw new InvalidOperationException("Could not reserve an available Playwright port.");
        }
        finally {
            mutex.ReleaseMutex();
        }
    }

    private static (int Port, DateTimeOffset ReservedAt) ParsePortReservation(string line) {
        string[] parts = line.Split('|', 2);

        if (!int.TryParse(parts[0], out int port))
            return (0, DateTimeOffset.MinValue);

        if (parts.Length < 2 || !long.TryParse(parts[1], out long ticks))
            return (port, DateTimeOffset.UtcNow);

        return (port, new DateTimeOffset(ticks, TimeSpan.Zero));
    }

    public static string CreateUniqueWebViewUserDataPath(string name) {
        string safeName = string.Concat(name.Select(static c => Path.GetInvalidFileNameChars().Contains(c) ? '-' : c));
        string path = Path.Join(
            Path.GetTempPath(),
            "infiniframe-playwright",
            $"{safeName}-{Environment.ProcessId}-{Guid.NewGuid():N}");

        Directory.CreateDirectory(path);
        return path;
    }

    public static void DeleteDirectorySafely(string? path) {
        if (string.IsNullOrWhiteSpace(path))
            return;

        try {
            if (Directory.Exists(path)) Directory.Delete(path, true);
        }
        catch (IOException) {
            // WebView2 can release files shortly after the window closes.
        }
        catch (UnauthorizedAccessException) {
            // Best-effort cleanup for test-only user data folders.
        }
    }

    /// <summary>
    ///     Creates the CDP connection URL for the given port.
    /// </summary>
    public static Uri CreateCdpConnectionUrl(int port)
        => new($"http://{LoopbackAddress}:{port}");

    public static async Task<IPlaywright> CreatePlaywrightAsync(TimeSpan timeout = default) {
        if (timeout == TimeSpan.Zero)
            timeout = DefaultPlaywrightCreateTimeout;

        return await Playwright.CreateAsync().WaitAsync(timeout);
    }

    /// <summary>
    ///     Connects to an InfiniFrame WebView via CDP with retry logic.
    ///     On Windows: connects to WebView2's Edge Chromium CDP endpoint.
    ///     On Linux:   connects to WebKitGTK 2.40+ CDP endpoint (via WEBKIT_INSPECTOR_HTTP_SERVER).
    /// </summary>
    public static async Task<IBrowser> ConnectOverCdpWithRetryAsync(
        IPlaywright playwright,
        Uri url,
        TimeSpan connectTimeout = default,
        TimeSpan retryWindow = default,
        TimeSpan retryInterval = default
    ) {
        if (connectTimeout == TimeSpan.Zero) connectTimeout = DefaultPlaywrightConnectTimeout;
        if (retryWindow == TimeSpan.Zero) retryWindow = DefaultPlaywrightConnectRetryWindow;
        if (retryInterval == TimeSpan.Zero) retryInterval = DefaultPlaywrightConnectRetryInterval;

        string engine = OperatingSystem.IsWindows()
            ? "WebView2 (Edge Chromium CDP)"
            : OperatingSystem.IsLinux()
                ? "WebKitGTK (WEBKIT_INSPECTOR_HTTP_SERVER CDP)"
                : "unknown";

        Console.WriteLine($"[PlaywrightConnect] Platform: {engine}, endpoint: {url}");

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

        string hint = OperatingSystem.IsLinux()
            ? " Ensure the container has DISPLAY set and WebKitGTK can initialize. Check WEBKIT_INSPECTOR_HTTP_SERVER is enabling the CDP endpoint."
            : "";

        throw new TimeoutException(
            $"Timed out connecting Playwright over CDP at '{url}' within {retryWindow.TotalSeconds} seconds.{hint}",
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
        if (!string.Equals(debugEnabled, "1", StringComparison.Ordinal)) return TimeSpan.Zero;

        string? secondsValue = Environment.GetEnvironmentVariable("PLAYWRIGHT_VISIBLE_DEBUG_SECONDS");
        if (int.TryParse(secondsValue, out int seconds) && seconds > 0) return TimeSpan.FromSeconds(seconds);

        return TimeSpan.FromSeconds(8);
    }
}
