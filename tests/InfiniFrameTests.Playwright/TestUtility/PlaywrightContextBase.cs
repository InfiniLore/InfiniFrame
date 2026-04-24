// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Playwright;

namespace InfiniFrameTests.Playwright.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class PlaywrightContextBase(string documentTitle) : IPlaywrightRuntimeContext {
    public abstract IInfiniFrameWindow Window { get; }
    public string DefaultDocumentTitle => documentTitle;
    
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private int _windowCloseRequestCount;
    private int _suppressCloseRequests;

    private readonly SemaphoreSlim _browserLock = new(1, 1);
    
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
            Fail.Test("Could not connect to Playwright.");
            return null!;
        }
    }
}
