// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using InfiniFrame.Js.Interop.MessageHandlers;

namespace InfiniFrameTests.Playwright.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GlobalPlaywrightContext {
    private static InfiniFrameServerTestUtility? Utility { get; set; }
    private static Process? HostProcess { get; set; }
    private static HttpClient? HostClient { get; set; }
    private static IPlaywright? Playwright { get; set; }
    private static IBrowser? Browser { get; set; }
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
        Console.WriteLine(
            $"[PlaywrightSetup] Starting assembly setup. server={ServerUrl}, cdp={PlaywrightConnectionString}");

        if (OperatingSystem.IsMacOS()) {
            StartMacOsHostProcess();
        }
        else {
            using var startupCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(90));

            Utility = InfiniFrameServerTestUtility.Create(
                appBuilder: static serverBuilder => serverBuilder
                    .WebHost.UseUrls(ServerUrl),
                windowBuilder: static windowBuilder => windowBuilder
                    .SetStartUrl(ServerUrl)
                    .SetTitle(DefaultDocumentTitle)
                    .SetBrowserControlInitParameters($"--remote-debugging-port={PlaywrightDevtoolsPort}")
                    .RegisterWindowManagementWebMessageHandler()
                    .RegisterFullScreenWebMessageHandler()
                    .RegisterOpenExternalTargetWebMessageHandler()
                    .RegisterTitleChangedWebMessageHandler()
                    .RegisterWindowClosingHandler(static (_, _) => {
                        Interlocked.Increment(ref _windowCloseRequestCount);
                        return Volatile.Read(ref _suppressCloseRequests) == 1;
                    }),
                cancellationToken: startupCancellation.Token
            );
        }

        Console.WriteLine("[PlaywrightSetup] Assembly setup completed.");
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

        TryStopHostProcess();

        Utility?.Dispose();
    }

    public static async Task<IBrowser> GetOrCreateBrowserAsync(string relativeUrl = "/") {
        Console.WriteLine($"[PlaywrightConnect] GetOrCreateBrowserAsync start relativeUrl={relativeUrl}");
        await BrowserLock.WaitAsync();
        try {
            if (Browser is { IsConnected: true }) {
                Console.WriteLine("[PlaywrightConnect] Reusing connected browser.");
                return Browser;
            }

            if (Playwright is null) {
                Console.WriteLine("[PlaywrightConnect] Creating Playwright instance.");
                Playwright = await Microsoft.Playwright.Playwright.CreateAsync().WaitAsync(TimeSpan.FromSeconds(20));
                Console.WriteLine("[PlaywrightConnect] Playwright instance created.");
            }

            var url = new Uri(PlaywrightConnectionUri, relativeUrl);
            Console.WriteLine($"[PlaywrightConnect] Connecting over CDP: {url}");
            Browser = await ConnectOverCdpWithRetryAsync(url);
            Console.WriteLine("[PlaywrightConnect] CDP connection established.");
            return Browser;
        }
        finally {
            BrowserLock.Release();
            Console.WriteLine("[PlaywrightConnect] GetOrCreateBrowserAsync end.");
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
        => ResetWindowCloseRequestCountAsync().GetAwaiter().GetResult();

    public static int GetWindowCloseRequestCount()
        => GetWindowCloseRequestCountAsync().GetAwaiter().GetResult();

    public static void SuppressWindowCloseRequests(bool suppress) {
        SuppressWindowCloseRequestsAsync(suppress).GetAwaiter().GetResult();
    }

    public static async Task ResetWindowCloseRequestCountAsync() {
        if (OperatingSystem.IsMacOS()) {
            await HostClient!.PostAsync("/__host/window/close/reset", content: null);
            return;
        }

        Volatile.Write(ref _windowCloseRequestCount, 0);
    }

    public static async Task<int> GetWindowCloseRequestCountAsync() {
        // ReSharper disable once InvertIf
        if (OperatingSystem.IsMacOS()) {
            int? count = await HostClient!.GetFromJsonAsync<int>("/__host/window/close/count");
            return (int)count;
        }

        return Volatile.Read(ref _windowCloseRequestCount);
    }

    public static async Task SuppressWindowCloseRequestsAsync(bool suppress) {
        if (OperatingSystem.IsMacOS()) {
            await HostClient!.PostAsync($"/__host/window/close/suppress/{suppress.ToString().ToLowerInvariant()}", content: null);
            return;
        }

        Volatile.Write(ref _suppressCloseRequests, suppress ? 1 : 0);
    }

    public static async Task<string> GetWindowTitleAsync() {
        if (OperatingSystem.IsMacOS()) {
            return await HostClient!.GetStringAsync("/__host/window/title");
        }

        return Window.Title;
    }

    public static async Task SetWindowTitleAsync(string title) {
        if (OperatingSystem.IsMacOS()) {
            await HostClient!.PutAsJsonAsync("/__host/window/title", new SetTitleRequest(title));
            return;
        }

        Window.SetTitle(title);
    }

    public static async Task<bool> GetWindowFullscreenAsync() {
        // ReSharper disable once InvertIf
        if (OperatingSystem.IsMacOS()) {
            bool? fullScreen = await HostClient!.GetFromJsonAsync<bool>("/__host/window/fullscreen");
            return (bool)fullScreen;
        }

        return Window.FullScreen;
    }

    private static void StartMacOsHostProcess() {
        string repositoryRoot = ResolveRepositoryRoot();
        string hostProjectPath = Path.Join(repositoryRoot, "tests", "InfiniFrameTests.Playwright.Host", "InfiniFrameTests.Playwright.Host.csproj");
        string hostDllPath = Path.Join(
            repositoryRoot,
            "tests",
            "InfiniFrameTests.Playwright.Host",
            "bin",
            "Release",
            "net10.0",
            "InfiniFrameTests.Playwright.Host.dll"
        );
        string webRootPath = Path.Join(repositoryRoot, "tests", "InfiniFrameTests.Playwright", "wwwroot");

        EnsureMacOsHostBuilt(repositoryRoot, hostProjectPath, hostDllPath);

        var startInfo = new ProcessStartInfo {
            FileName = "dotnet",
            Arguments =
                $"\"{hostDllPath}\" --server-port {ServerPort} --cdp-port {PlaywrightDevtoolsPort} --webroot \"{webRootPath}\" --default-title \"{DefaultDocumentTitle}\"",
            WorkingDirectory = repositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        HostProcess = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start macOS Playwright host process.");

        var readySignal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var stderr = new List<string>();

        HostProcess.OutputDataReceived += (_, eventArgs) => {
            string? line = eventArgs.Data;
            if (line is null) return;

            Console.WriteLine($"[PlaywrightHost] {line}");
            if (line.StartsWith("READY|", StringComparison.Ordinal))
                readySignal.TrySetResult();
        };

        HostProcess.ErrorDataReceived += (_, eventArgs) => {
            string? line = eventArgs.Data;
            if (line is null) return;
            lock (stderr) {
                stderr.Add(line);
            }
            Console.WriteLine($"[PlaywrightHost:stderr] {line}");
        };

        HostProcess.BeginOutputReadLine();
        HostProcess.BeginErrorReadLine();

        bool ready = readySignal.Task.Wait(TimeSpan.FromSeconds(90));
        if (!ready || HostProcess.HasExited) {
            string errorText;
            lock (stderr) {
                errorText = string.Join(Environment.NewLine, stderr);
            }

            throw new InvalidOperationException(
                $"macOS Playwright host failed to start. exited={HostProcess.HasExited}. stderr:{Environment.NewLine}{errorText}"
            );
        }

        HostClient = new HttpClient {
            BaseAddress = new Uri(ServerUrl)
        };
    }

    private static void EnsureMacOsHostBuilt(string repositoryRoot, string hostProjectPath, string hostDllPath) {
        if (File.Exists(hostDllPath))
            return;

        var buildInfo = new ProcessStartInfo {
            FileName = "dotnet",
            Arguments = $"build \"{hostProjectPath}\" --configuration Release --framework net10.0 --no-restore",
            WorkingDirectory = repositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process buildProcess = Process.Start(buildInfo)
            ?? throw new InvalidOperationException("Failed to start macOS host build process.");
        string output = buildProcess.StandardOutput.ReadToEnd();
        string error = buildProcess.StandardError.ReadToEnd();
        buildProcess.WaitForExit();

        if (buildProcess.ExitCode != 0 || !File.Exists(hostDllPath)) {
            throw new InvalidOperationException(
                $"Failed to build macOS Playwright host. exit={buildProcess.ExitCode}{Environment.NewLine}{output}{Environment.NewLine}{error}"
            );
        }
    }

    private static void TryStopHostProcess() {
        if (HostClient is not null) {
            try {
                HostClient.PostAsync("/__host/shutdown", content: null).GetAwaiter().GetResult();
            }
            catch {
                // ignored
            }

            HostClient.Dispose();
            HostClient = null;
        }

        if (HostProcess is null)
            return;

        try {
            if (!HostProcess.HasExited && !HostProcess.WaitForExit(5000))
                HostProcess.Kill(entireProcessTree: true);
        }
        catch (InvalidOperationException) {
            // ignored
        }
        catch (System.ComponentModel.Win32Exception) {
            // ignored
        }
        catch (NotSupportedException) {
            // ignored
        }
        finally {
            HostProcess.Dispose();
            HostProcess = null;
        }
    }

    private static string ResolveRepositoryRoot() {
        string? current = Directory.GetCurrentDirectory();
        while (!string.IsNullOrWhiteSpace(current)) {
            if (File.Exists(Path.Join(current, "InfiniFrame.slnx")))
                return current;

            current = Directory.GetParent(current)?.FullName;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root (InfiniFrame.slnx).");
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private sealed record SetTitleRequest(string Title);
}
