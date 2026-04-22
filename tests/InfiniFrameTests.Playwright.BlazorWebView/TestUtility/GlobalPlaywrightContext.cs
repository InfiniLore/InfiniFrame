// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrame.Js.Interop.MessageHandlers;
using InfiniFrameTests.Playwright.BlazorWebView.Components;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using System.Net;
using System.Net.Sockets;

namespace InfiniFrameTests.Playwright.BlazorWebView.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GlobalPlaywrightContext {
    private static InfiniFrameBlazorApp? BlazorApp { [UsedImplicitly] get; set; }
    private static IInfiniFrameWindow? WindowValue { get; set; }
    private static Thread? AppThread { get; set; }

    private static IPlaywright? Playwright { get; set; }
    private static IBrowser? Browser { get; set; }
    private static readonly SemaphoreSlim BrowserLock = new(1, 1);
    private static int _windowCloseRequestCount;
    private static int _suppressCloseRequests;

    public static IInfiniFrameWindow Window => WindowValue!;

    private static readonly int PlaywrightDevtoolsPort = GetAvailablePort();
    private static readonly string PlaywrightConnectionString = $"http://127.0.0.1:{PlaywrightDevtoolsPort}";
    private static readonly Uri PlaywrightConnectionUri = new(PlaywrightConnectionString);

    private static readonly TimeSpan PlaywrightConnectTimeout = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan PlaywrightConnectRetryWindow = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan PlaywrightConnectRetryInterval = TimeSpan.FromSeconds(2);

    public const string DefaultDocumentTitle = "InfiniFrame Playwright BlazorWebView";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static int GetAvailablePort() {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    [Before(Assembly)]
    public static void BeforeAll(AssemblyHookContext _) {
        using var startupCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        var ready = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        Thread thread = new(() => {
            try {
                var builder = InfiniFrameBlazorAppBuilder.CreateDefault(
                    windowBuilder: wb => wb
                        .SetTitle(DefaultDocumentTitle)
                        .SetBrowserControlInitParameters($"--remote-debugging-port={PlaywrightDevtoolsPort}")
                        .RegisterWindowManagementWebMessageHandler()
                        .RegisterFullScreenWebMessageHandler()
                        .RegisterOpenExternalTargetWebMessageHandler()
                        .RegisterTitleChangedWebMessageHandler()
                        .RegisterWindowClosingHandler(static (_, _) => {
                            Interlocked.Increment(ref _windowCloseRequestCount);
                            return Volatile.Read(ref _suppressCloseRequests) == 1;
                        })
                );
                
                builder.RootComponents.Add<App>("app");

                InfiniFrameBlazorApp app = builder.Build();
                var window = app.ServiceProvider.GetRequiredService<IInfiniFrameWindow>();

                BlazorApp = app;
                WindowValue = window;
                ready.SetResult(null);

                app.Run();
            }
            catch (Exception ex) {
                ready.TrySetException(ex);
            }
        }) {
            IsBackground = true,
            Name = "InfiniFrame Playwright BlazorWebView App Thread"
        };

        if (OperatingSystem.IsWindows()) {
            thread.SetApartmentState(ApartmentState.STA);
        }

        AppThread = thread;
        thread.Start();

        ready.Task.WaitAsync(startupCancellation.Token).GetAwaiter().GetResult();
    }

    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _) {
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

        try {
            WindowValue?.Close();
        }
        catch (ApplicationException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        if (AppThread is not null && !AppThread.Join(TimeSpan.FromSeconds(5))) {
            AppThread.Interrupt();
        }

        BlazorApp = null;
        WindowValue = null;
        AppThread = null;
    }

    public static async Task<IBrowser> GetOrCreateBrowserAsync(string relativeUrl = "/") {
        await BrowserLock.WaitAsync();
        try {
            if (Browser is { IsConnected: true }) return Browser;

            Playwright ??= await Microsoft.Playwright.Playwright.CreateAsync().WaitAsync(TimeSpan.FromSeconds(20));

            Uri url = new(PlaywrightConnectionUri, relativeUrl);
            Browser = await ConnectOverCdpWithRetryAsync(url);
            return Browser;
        }
        finally {
            BrowserLock.Release();
        }
    }

    private static async Task<IBrowser> ConnectOverCdpWithRetryAsync(Uri url) {
        using var retryWindowCancellation = new CancellationTokenSource(PlaywrightConnectRetryWindow);
        CancellationToken cancellationToken = retryWindowCancellation.Token;
        Exception? lastException = null;

        while (!cancellationToken.IsCancellationRequested) {
            try {
                return await Playwright!.Chromium
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

    public static void ResetWindowCloseRequestCount()
        => Volatile.Write(ref _windowCloseRequestCount, 0);

    public static int GetWindowCloseRequestCount()
        => Volatile.Read(ref _windowCloseRequestCount);

    public static void SuppressWindowCloseRequests(bool suppress)
        => Volatile.Write(ref _suppressCloseRequests, suppress ? 1 : 0);
}
