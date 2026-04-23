using Microsoft.Playwright;

namespace InfiniFrameAutomationTests;

public sealed class AutomationSessionManager(int playwrightDevtoolsPort) : IDisposable {
    private static readonly TimeSpan PlaywrightConnectTimeout = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan PlaywrightConnectRetryWindow = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan PlaywrightConnectRetryInterval = TimeSpan.FromSeconds(2);

    private readonly SemaphoreSlim _browserLock = new(1, 1);
    private readonly Uri _playwrightConnectionUri = new($"http://127.0.0.1:{playwrightDevtoolsPort}");

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private SeleniumWebDriverSession? _seleniumSession;
    private int _windowCloseRequestCount;
    private int _suppressCloseRequests;

    // ReSharper disable once MemberCanBePrivate.Global
    public int PlaywrightDevtoolsPort { get; } = playwrightDevtoolsPort;

    public static void EnableLinuxWebKitAutomationIfNeeded() {
        if (OperatingSystem.IsLinux()) {
            Environment.SetEnvironmentVariable("INFINIFRAME_WEBKIT_AUTOMATION", "1");
        }
    }

    public string GetWindowsRemoteDebuggingArgs()
        => $"--remote-debugging-port={PlaywrightDevtoolsPort}";

    public bool OnWindowClosingRequested() {
        Interlocked.Increment(ref _windowCloseRequestCount);
        return Volatile.Read(ref _suppressCloseRequests) == 1;
    }

    public void ResetWindowCloseRequestCount()
        => Volatile.Write(ref _windowCloseRequestCount, 0);

    public int GetWindowCloseRequestCount()
        => Volatile.Read(ref _windowCloseRequestCount);

    public void SuppressWindowCloseRequests(bool suppress)
        => Volatile.Write(ref _suppressCloseRequests, suppress ? 1 : 0);

    public async Task<IAutomationPage> GetOrCreatePageAsync(string relativeUrl = "/") {
        if (OperatingSystem.IsLinux()) {
            return await GetOrCreateLinuxPageAsync();
        }

        return await GetOrCreateWindowsPageAsync(relativeUrl);
    }

    public void DelayIfVisibleDebugEnabled() {
        TimeSpan delay = GetVisibleDebugDelay();
        if (delay <= TimeSpan.Zero) {
            return;
        }

        Console.WriteLine($"[PlaywrightDebug] Holding window open for {delay.TotalSeconds:0}s before teardown.");
        Thread.Sleep(delay);
    }

    private async Task<IAutomationPage> GetOrCreateWindowsPageAsync(string relativeUrl) {
        await _browserLock.WaitAsync();
        try {
            if (_browser is not { IsConnected: true }) {
                _playwright ??= await Playwright.CreateAsync().WaitAsync(TimeSpan.FromSeconds(20));
                Uri url = new(_playwrightConnectionUri, relativeUrl);
                _browser = await ConnectOverCdpWithRetryAsync(url);
            }

            IBrowserContext context = _browser.Contexts.FirstOrDefault() ?? await _browser.NewContextAsync();
            IPage page = context.Pages.FirstOrDefault() ?? await context.NewPageAsync();
            return new PlaywrightAutomationPage(page);
        }
        finally {
            _browserLock.Release();
        }
    }

    private async Task<IAutomationPage> GetOrCreateLinuxPageAsync() {
        await _browserLock.WaitAsync();
        try {
            _seleniumSession ??= await SeleniumWebDriverSession.StartAsync();
            return new SeleniumAutomationPage(_seleniumSession.Driver);
        }
        finally {
            _browserLock.Release();
        }
    }

    private async Task<IBrowser> ConnectOverCdpWithRetryAsync(Uri url) {
        using var retryWindowCancellation = new CancellationTokenSource(PlaywrightConnectRetryWindow);
        CancellationToken cancellationToken = retryWindowCancellation.Token;
        Exception? lastException = null;

        while (!cancellationToken.IsCancellationRequested) {
            try {
                return await _playwright!.Chromium
                    .ConnectOverCDPAsync(url.ToString())
                    .WaitAsync(PlaywrightConnectTimeout, cancellationToken);
            }
            catch (PlaywrightException ex) {
                lastException = ex;
            }
            catch (TimeoutException ex) {
                lastException = ex;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
                break;
            }

            try {
                await Task.Delay(PlaywrightConnectRetryInterval, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
                break;
            }
        }

        throw new TimeoutException(
            $"Timed out connecting Playwright over CDP at '{url}' within {PlaywrightConnectRetryWindow.TotalSeconds} seconds.",
            lastException
        );
    }

    private static TimeSpan GetVisibleDebugDelay() {
        string? debugEnabled = Environment.GetEnvironmentVariable("PLAYWRIGHT_VISIBLE_DEBUG");
        if (!string.Equals(debugEnabled, "1", StringComparison.Ordinal)) {
            return TimeSpan.Zero;
        }

        string? secondsValue = Environment.GetEnvironmentVariable("PLAYWRIGHT_VISIBLE_DEBUG_SECONDS");
        if (int.TryParse(secondsValue, out int seconds) && seconds > 0) {
            return TimeSpan.FromSeconds(seconds);
        }

        return TimeSpan.FromSeconds(8);
    }

    public void Dispose() {
        try {
            _browser?.CloseAsync().GetAwaiter().GetResult();
        }
        catch (PlaywrightException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        _browser = null;
        _playwright?.Dispose();
        _playwright = null;
        _seleniumSession?.Dispose();
        _seleniumSession = null;
    }
}
