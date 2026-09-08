// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Hosting;

namespace InfiniTests.InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationWebServerTests {
    [Test]
    public async Task WithWebServer_ReturnsApplicationAndDefersWindowBuild(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(window => window.SetStartPageContent("<html><body>Web</body></html>"))
            .UseWebServer(builder => builder.WebHost.UseUrls("http://127.0.0.1:0"))
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    public async Task WithWebServer_RequiresExplicitIdsForMultipleWindows(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await Assert.That(() => InfiniFrameApplication.CreateBuilder()
                .WithWindow("main", static _ => { })
                .WithWindow("settings", static _ => { })
                .UseWebServer(static _ => { })
                .Build())
            .Throws<InvalidOperationException>()
            .WithMessageContaining("explicit window IDs");
    }

    [Test]
    public async Task WithWebServer_RejectsMissingAndDuplicateIds(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

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
}
