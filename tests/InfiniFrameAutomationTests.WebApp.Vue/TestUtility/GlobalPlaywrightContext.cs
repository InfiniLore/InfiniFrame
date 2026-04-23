// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;
using Microsoft.Playwright;
using System.Net;
using System.Net.Sockets;
using InfiniFrame.Js.Interop.MessageHandlers;

namespace InfiniFrameAutomationTests.WebApp.Vue.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GlobalAutomationContext {
    private static InfiniFrameServerTestUtility? Utility { get; set; }
    private static IPlaywright? Playwright { get; set; }
    private static IBrowser? Browser { get; set; }
    private static SeleniumWebDriverSession? SeleniumSession { get; set; }
    private static readonly SemaphoreSlim BrowserLock = new(1, 1);
    private static int _windowCloseRequestCount;
    private static int _suppressCloseRequests;

    public static IInfiniFrameWindow Window => Utility!.Window;
    public static WebApplication WebApplication => Utility!.WebApplication;

    private static readonly int ServerPort = GetAvailablePort();
    private static readonly int PlaywrightDevtoolsPort = GetAvailablePort();

    private static readonly string ServerUrl = $"http://127.0.0.1:{ServerPort}";
    private static readonly string PlaywrightConnectionString = $"http://127.0.0.1:{PlaywrightDevtoolsPort}";
    private static readonly Uri PlaywrightConnectionUri = new(PlaywrightConnectionString);

    private static readonly TimeSpan PlaywrightConnectTimeout = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan PlaywrightConnectRetryWindow = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan PlaywrightConnectRetryInterval = TimeSpan.FromSeconds(2);

    public const string DefaultDocumentTitle = "InfiniFrame Playwright Vue";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static int GetAvailablePort() {
        using TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext _) {
        if (OperatingSystem.IsLinux()) {
            Environment.SetEnvironmentVariable("INFINIFRAME_WEBKIT_AUTOMATION", "1");
        }

        Console.WriteLine(
            $"[PlaywrightSetup] Starting assembly setup. server={ServerUrl}, cdp={PlaywrightConnectionString}");

        using var startupCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(90));

        Utility = InfiniFrameServerTestUtility.Create(
            appBuilder: static serverBuilder => serverBuilder
                .WebHost.UseUrls(ServerUrl),
            windowBuilder: static windowBuilder => {
                windowBuilder
                    .SetStartUrl(ServerUrl)
                    .SetTitle(DefaultDocumentTitle)
                    .RegisterWindowManagementWebMessageHandler()
                    .RegisterFullScreenWebMessageHandler()
                    .RegisterOpenExternalTargetWebMessageHandler()
                    .RegisterTitleChangedWebMessageHandler()
                    .RegisterWindowClosingHandler(static (_, _) => {
                        Interlocked.Increment(ref _windowCloseRequestCount);
                        return Volatile.Read(ref _suppressCloseRequests) == 1;
                    });

                if (OperatingSystem.IsWindows()) {
                    windowBuilder.SetBrowserControlInitParameters($"--remote-debugging-port={PlaywrightDevtoolsPort}");
                }
            },
            cancellationToken: startupCancellation.Token
        );
        Console.WriteLine("[PlaywrightSetup] Assembly setup completed.");
    }

    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _) {
        TimeSpan debugDelay = GetVisibleDebugDelay();
        if (debugDelay > TimeSpan.Zero) {
            Console.WriteLine($"[PlaywrightDebug] Holding window open for {debugDelay.TotalSeconds:0}s before teardown.");
            Thread.Sleep(debugDelay);
        }

        try {
            Browser?.CloseAsync().GetAwaiter().GetResult();
        }
        catch (PlaywrightException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        Browser = null;
        Playwright?.Dispose();
        Playwright = null;
        SeleniumSession?.Dispose();
        SeleniumSession = null;

        Utility?.Dispose();
    }

    public static async Task<IAutomationPage> GetOrCreatePageAsync(string relativeUrl = "/") {
        if (OperatingSystem.IsLinux()) {
            return await GetOrCreateLinuxPageAsync();
        }

        return await GetOrCreateWindowsPageAsync(relativeUrl);
    }

    private static async Task<IAutomationPage> GetOrCreateWindowsPageAsync(string relativeUrl = "/") {
        Console.WriteLine($"[PlaywrightConnect] GetOrCreateBrowserAsync start relativeUrl={relativeUrl}");
        await BrowserLock.WaitAsync();
        try {
            if (Browser is not { IsConnected: true }) {
                if (Playwright is null) {
                    Console.WriteLine("[PlaywrightConnect] Creating Playwright instance.");
                    Playwright = await Microsoft.Playwright.Playwright.CreateAsync().WaitAsync(TimeSpan.FromSeconds(20));
                    Console.WriteLine("[PlaywrightConnect] Playwright instance created.");
                }

                var url = new Uri(PlaywrightConnectionUri, relativeUrl);
                Console.WriteLine($"[PlaywrightConnect] Connecting over CDP: {url}");
                Browser = await ConnectOverCdpWithRetryAsync(url);
                Console.WriteLine("[PlaywrightConnect] CDP connection established.");
            }

            IBrowserContext context = Browser.Contexts.FirstOrDefault() ?? await Browser.NewContextAsync();
            IPage page = context.Pages.FirstOrDefault() ?? await context.NewPageAsync();
            return new PlaywrightAutomationPage(page);
        }
        finally {
            BrowserLock.Release();
            Console.WriteLine("[PlaywrightConnect] GetOrCreateBrowserAsync end.");
        }
    }

    private static async Task<IAutomationPage> GetOrCreateLinuxPageAsync() {
        await BrowserLock.WaitAsync();
        try {
            SeleniumSession ??= await SeleniumWebDriverSession.StartAsync();
            return new SeleniumAutomationPage(SeleniumSession.Driver);
        }
        finally {
            BrowserLock.Release();
        }
    }

    private static async Task<IBrowser> ConnectOverCdpWithRetryAsync(Uri url) {
        using var retryWindowCancellation = new CancellationTokenSource(PlaywrightConnectRetryWindow);
        CancellationToken cancellationToken = retryWindowCancellation.Token;
        Exception? lastException = null;
        int attempt = 0;

        while (!cancellationToken.IsCancellationRequested) {
            attempt++;
            try {
                Console.WriteLine($"[PlaywrightConnect] CDP attempt {attempt} to {url}");
                return await Playwright!.Chromium
                    .ConnectOverCDPAsync(url.ToString())
                    .WaitAsync(PlaywrightConnectTimeout, cancellationToken);
            }
            catch (PlaywrightException ex) {
                lastException = ex;
                Console.WriteLine($"[PlaywrightConnect] CDP attempt {attempt} failed with PlaywrightException: {ex.Message}");
            }
            catch (TimeoutException ex) {
                lastException = ex;
                Console.WriteLine($"[PlaywrightConnect] CDP attempt {attempt} timed out after {PlaywrightConnectTimeout.TotalSeconds}s.");
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

    public static void ResetWindowCloseRequestCount()
        => Volatile.Write(ref _windowCloseRequestCount, 0);

    public static int GetWindowCloseRequestCount()
        => Volatile.Read(ref _windowCloseRequestCount);

    public static void SuppressWindowCloseRequests(bool suppress) {
        Volatile.Write(ref _suppressCloseRequests, suppress ? 1 : 0);
    }

    private static TimeSpan GetVisibleDebugDelay() {
        string? debugEnabled = Environment.GetEnvironmentVariable("PLAYWRIGHT_VISIBLE_DEBUG");
        if (!string.Equals(debugEnabled, "1", StringComparison.Ordinal))
            return TimeSpan.Zero;

        string? secondsValue = Environment.GetEnvironmentVariable("PLAYWRIGHT_VISIBLE_DEBUG_SECONDS");
        if (int.TryParse(secondsValue, out int seconds) && seconds > 0)
            return TimeSpan.FromSeconds(seconds);

        return TimeSpan.FromSeconds(8);
    }
}


