// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.WebServer;
using InfiniTests.TestSupport.Attributes;
using InfiniTests.TestSupport.Utilities;
using Microsoft.AspNetCore.Hosting;

namespace InfiniTests.InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationWebServerTests {
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
}
