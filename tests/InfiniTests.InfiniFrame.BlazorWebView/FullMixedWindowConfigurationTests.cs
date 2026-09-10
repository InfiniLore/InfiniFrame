using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using InfiniFrame.WebServer;
using InfiniTests.TestSupport.Attributes;
using InfiniTests.TestSupport.Utilities;
using Microsoft.AspNetCore.Hosting;

namespace InfiniTests.InfiniFrame.BlazorWebView;

[NotInParallelInfiniTests]
public sealed class FullMixedWindowConfigurationTests {
    [Test]
    [Timeout(60_000)]
    public async Task Application_CanRunPlainServerAndBlazorWindows(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        int port = PortUtils.GetOpenPortValue();
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow("plain", builder => builder.SetStartPageContent("<html><body>Plain</body></html>"))
            .WithWindow("server", static _ => { })
            .WithWindow("blazor", builder => builder.SetTitle("Blazor"))
            .UseWebServer("server", server => server.WebHost.UseUrls($"http://127.0.0.1:{port}"))
            .UseBlazorWebView("blazor", static _ => { })
            .Build();

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100; attempt++) {
                if (runTask.IsFaulted) await runTask;

                Uri? serverUri = application.TryGetWindow("server")?.Features.PageNavigation.GetCurrentUri();
                Uri? blazorUri = application.TryGetWindow("blazor")?.Features.PageNavigation.GetCurrentUri();
                if (serverUri?.Port == port && blazorUri?.Scheme == "app") break;

                await Task.Delay(100, ct);
            }

            await Assert.That(application.Windows).Count().IsEqualTo(3);
            await Assert.That(application.GetWindow("server").Features.PageNavigation.GetCurrentUri()?.Port)
                .IsEqualTo(port);
            await Assert.That(application.GetWindow("blazor").Features.PageNavigation.GetCurrentUri()?.Scheme)
                .IsEqualTo("app");
            await Assert.That(application.GetWindow("plain")).IsNotNull();
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }
}
