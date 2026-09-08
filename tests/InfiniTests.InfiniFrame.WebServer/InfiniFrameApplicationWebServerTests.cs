using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Hosting;

namespace InfiniTests.InfiniFrame.WebServer;

[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationWebServerTests {
    [Test]
    public async Task WithWebServer_ReturnsApplicationAndDefersWindowBuild(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using var application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(window => window.SetStartPageContent("<html><body>Web</body></html>"))
            .UseWebServer(builder => builder.WebHost.UseUrls("http://127.0.0.1:0"))
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }
}
