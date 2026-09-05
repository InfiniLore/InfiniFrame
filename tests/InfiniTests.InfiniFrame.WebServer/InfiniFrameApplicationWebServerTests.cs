using InfiniFrame;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Hosting;

namespace InfiniTests.InfiniFrame.WebServer;

[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationWebServerTests {
    [Test]
    public async Task WithWebServer_ReturnsApplicationAndDefersWindowBuild(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        using var application = InfiniFrameApplication.Initialize()
            .WithWebServer(builder => builder.WebApp.WebHost.UseUrls("http://127.0.0.1:0"));

        await Assert.That(application.Windows).IsEmpty();
    }
}
