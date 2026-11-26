// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.WebServer;
using InfiniTests.Native;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace InfiniTests.InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationWebServerTests {
    private static readonly HttpClient TestClient = new();

    [Test]
    [NotInParallelInfiniTests]
    public async Task WithWebServer_ReturnsApplicationAndDefersWindowBuild(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(window => window.SetStartPageContent("<html><body>Web</body></html>"))
            .UseWebServer(builder => builder.WebHost.UseUrls("http://127.0.0.1:0"))
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task WithWebServer_RequiresExplicitIdsForMultipleWindows(CancellationToken ct = default) {
        await Assert.That(() => InfiniFrameApplication.CreateBuilder()
                .WithWindow("main", static _ => { })
                .WithWindow("settings", static _ => { })
                .UseWebServer(static _ => { })
                .Build())
            .Throws<InvalidOperationException>()
            .WithMessageContaining("explicit window IDs");
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task WithWebServer_RejectsMissingAndDuplicateIds(CancellationToken ct = default) {
        await Assert.That(() => InfiniFrameApplication.CreateBuilder()
                .WithWindow("main", static _ => { })
                .UseWebServer(static _ => { }, "missing")
                .Build())
            .Throws<InvalidOperationException>()
            .WithMessageContaining("missing");

        await Assert.That(() => InfiniFrameApplication.CreateBuilder()
                .WithWindow("main", static _ => { })
                .UseWebServer(static _ => { }, "main", "main")
                .Build())
            .Throws<ArgumentException>()
            .WithMessageContaining("duplicates");
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task WithWebServer_AcceptsSingleNamedWindow(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow("server", static _ => { })
            .UseWebServer("server", static _ => { })
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task WithWebServer_AcceptsMultipleNamedWindows(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow("first", static _ => { })
            .WithWindow("second", static _ => { })
            .UseWebServer(["first", "second"], static _ => { })
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task WithWebServer_BuildApplication_CanBeDisposedWithoutRunning(CancellationToken ct = default) {
        InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(static _ => { })
            .UseWebServer(static _ => { })
            .Build();

        await application.DisposeAsync();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task WithWebServer_ConfigureWebApplication_AcceptsCallback(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(static _ => { })
            .UseWebServer(configure => {
                configure.ConfigureWebApplication(static _ => { });
            })
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    public async Task WithWebServer_StartupAndShutdown_CompletesCleanly(CancellationToken ct = default) {
        if ((!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS()) || Environment.Version.Major < 8) return;

        int port = PortUtils.GetOpenPortValue();
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow("server", static _ => { })
            .UseWebServer("server", server => server.WebHost.UseUrls($"http://127.0.0.1:{port}"))
            .Build();

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100; attempt++) {
                if (runTask.IsFaulted) await runTask;

                Uri? currentUri = application.TryGetWindow("server")?.Features.PageNavigation.GetCurrentUri();
                if (currentUri?.Port == port) break;

                await Task.Delay(100, ct);
            }

            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), ct);
        }
        finally {
            application.Shutdown();
            try { await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None); }
            catch (OperationCanceledException) { }
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    [OnlyRunOnMacOs]
    public async Task WithWebServer_MacOsRunAsync_PresentsWindowAndServesContent(CancellationToken ct = default) {
        int port = PortUtils.GetOpenPortValue();
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow("server", static builder => builder.SetTitle("InfiniFrame macOS WebServer regression"))
            .UseWebServer("server", server => {
                server.WebHost.UseUrls($"http://127.0.0.1:{port}");
                server.ConfigureWebApplication(static webApplication =>
                    webApplication.MapGet("/", static () => Results.Content(
                        "<html><body>InfiniFrame loaded</body></html>",
                        "text/html"
                    )));
            })
            .Build();

        Task runTask = application.RunAsync(ct);
        try {
            string? responseBody = null;

            for (int attempt = 0; attempt < 300; attempt++) {
                if (runTask.IsFaulted) await runTask;

                try {
                    using HttpResponseMessage response = await TestClient.GetAsync(
                        $"http://127.0.0.1:{port}/",
                        ct
                    );
                    if (response.IsSuccessStatusCode)
                        responseBody = await response.Content.ReadAsStringAsync(ct);
                }
                catch (HttpRequestException) {
                    // Kestrel may not be listening during the first few startup turns.
                }

                if (responseBody?.Contains("InfiniFrame loaded", StringComparison.Ordinal) == true
                    && application.TryGetWindow("server") is not null
                    && MacOsNative.IsApplicationRegisteredWithAppKit()
                    && MacOsNative.GetVisibleApplicationWindowCount() > 0)
                    break;

                await Task.Delay(100, ct);
            }

            await Assert.That(responseBody).Contains("InfiniFrame loaded");
            await Assert.That(application.TryGetWindow("server")).IsNotNull();
            await Assert.That(application.GetWindow("server").Features.PageNavigation.GetCurrentUri()?.Port)
                .IsEqualTo(port);
            await Assert.That(MacOsNative.IsApplicationRegisteredWithAppKit()).IsTrue();
            await Assert.That(MacOsNative.GetVisibleApplicationWindowCount()).IsGreaterThan(0);
        }
        finally {
            application.Shutdown();
            try { await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None); }
            catch (OperationCanceledException) { }
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    [OnlyRunOnMacOs]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public async Task WithWebServer_MacOsRun_PresentsWindowAndServesContent(CancellationToken ct = default) {
        int port = PortUtils.GetOpenPortValue();
        using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow("server", static builder => builder.SetTitle("InfiniFrame macOS WebServer synchronous regression"))
            .UseWebServer("server", server => {
                server.WebHost.UseUrls($"http://127.0.0.1:{port}");
                server.ConfigureWebApplication(static webApplication =>
                    webApplication.MapGet("/", static () => Results.Content(
                        "<html><body>InfiniFrame loaded</body></html>",
                        "text/html"
                    )));
            })
            .Build();

        var observed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _ = Task.Run(async () => {
            try {
                for (int attempt = 0; attempt < 300; attempt++) {
                    try {
                        using HttpResponseMessage response = await TestClient.GetAsync(
                            $"http://127.0.0.1:{port}/",
                            ct
                        );
                        string body = await response.Content.ReadAsStringAsync(ct);
                        if (response.IsSuccessStatusCode
                            && body.Contains("InfiniFrame loaded", StringComparison.Ordinal)
                            && application.TryGetWindow("server") is not null
                            && MacOsNative.IsApplicationRegisteredWithAppKit()
                            && MacOsNative.GetVisibleApplicationWindowCount() > 0) {
                            observed.TrySetResult(true);
                            application.Shutdown();
                            return;
                        }
                    }
                    catch (HttpRequestException) {
                        // Kestrel may not be listening during the first startup turns.
                    }

                    await Task.Delay(100, ct);
                }

                observed.TrySetResult(false);
                application.Shutdown();
            }
            catch (Exception exception) {
                observed.TrySetException(exception);
                application.Shutdown();
            }
        }, ct);

        application.Run();

        await observed.Task;
        await Assert.That(observed.Task.Result).IsTrue();
    }
}
