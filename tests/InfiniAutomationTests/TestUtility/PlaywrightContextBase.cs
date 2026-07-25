// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Playwright;

namespace InfiniAutomationTests.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class PlaywrightContextBase(string documentTitle) : IPlaywrightRuntimeContext {

    private readonly SemaphoreSlim _browserLock = new(1, 1);
    private IBrowser? _browser;

    private IPlaywright? _playwright;
    private int _suppressCloseRequests;
    private int _windowCloseRequestCount;
    public abstract IInfiniFrameWindow Window { get; }
    public string DefaultDocumentTitle => documentTitle;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task<IBrowser> GetOrCreateBrowserAsync(string relativeUrl = "/") {
        await _browserLock.WaitAsync();
        try {
            if (_browser is { IsConnected: true }) {
                return _browser;
            }

            _playwright ??= await PlaywrightConnectionUtility.CreatePlaywrightAsync();

            _browser = await ConnectAsync(relativeUrl);
            return _browser;
        }
        finally {
            _browserLock.Release();
        }
    }

    public void ResetWindowCloseRequestCount()
        => Volatile.Write(ref _windowCloseRequestCount, 0);

    public int GetWindowCloseRequestCount()
        => Volatile.Read(ref _windowCloseRequestCount);

    public virtual Task RestoreDefaultStateAsync()
        => Task.CompletedTask;

    public void SuppressWindowCloseRequests(bool suppress)
        => Volatile.Write(ref _suppressCloseRequests, suppress ? 1 : 0);

    protected void BeforeAssemblyTeardown() {
        TimeSpan debugDelay = PlaywrightConnectionUtility.GetVisibleDebugDelay();
        if (debugDelay > TimeSpan.Zero) {
            Thread.Sleep(debugDelay);
        }

        PlaywrightConnectionUtility.CloseBrowserSafely(_browser);
        _browser = null;

        PlaywrightConnectionUtility.DisposePlaywrightSafely(_playwright);
        _playwright = null;
    }

    protected bool OnWindowClosingRequested() {
        Interlocked.Increment(ref _windowCloseRequestCount);
        return Volatile.Read(ref _suppressCloseRequests) == 1;
    }

    protected abstract Uri CreatePlaywrightConnectionUri(string relativeUrl);

    private async Task<IBrowser> ConnectAsync(string relativeUrl) {
        Uri url = CreatePlaywrightConnectionUri(relativeUrl);

        try {
            IBrowser browser = await PlaywrightConnectionUtility.ConnectOverCdpWithRetryAsync(_playwright!, url);
            return browser;
        }
        catch (TimeoutException) {
            string os = OperatingSystem.IsWindows()
                ? "Windows"
                : OperatingSystem.IsLinux()
                    ? "Linux"
                    : "unknown";
            Fail.Test($"Could not connect to the CDP endpoint at '{url}' on {os}. " +
                "Verify the InfiniFrame native window started with remote debugging enabled.");
            return null!;
        }
    }
}
