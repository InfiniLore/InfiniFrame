using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.WebServer;
using InfiniTests.TestSupport.Attributes;
using InfiniTests.TestSupport.Utilities;
using Microsoft.AspNetCore.Hosting;

namespace InfiniTests.InfiniFrame.WebServer;

[NotInParallelInfiniTests]
public sealed class MultiWindowWebServerTests {
    [Test]
    [Timeout(60_000)]
    public async Task UseWebServer_CanRunTwoIndependentIntegrations(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        int firstPort = PortUtils.GetOpenPortValue();
        int secondPort = PortUtils.GetOpenPortValue();
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow("first", static _ => { })
            .WithWindow("second", static _ => { })
            .UseWebServer("first", server => server.WebHost.UseUrls($"http://127.0.0.1:{firstPort}"))
            .UseWebServer("second", server => server.WebHost.UseUrls($"http://127.0.0.1:{secondPort}"))
            .Build();

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100; attempt++) {
                if (runTask.IsFaulted) await runTask;

                Uri? firstUri = application.TryGetWindow("first")?.Features.PageNavigation.GetCurrentUri();
                Uri? secondUri = application.TryGetWindow("second")?.Features.PageNavigation.GetCurrentUri();
                if (firstUri?.Port == firstPort && secondUri?.Port == secondPort) break;

                await Task.Delay(100, ct);
            }

            await Assert.That(application.GetWindow("first").Features.PageNavigation.GetCurrentUri()?.Port)
                .IsEqualTo(firstPort);
            await Assert.That(application.GetWindow("second").Features.PageNavigation.GetCurrentUri()?.Port)
                .IsEqualTo(secondPort);
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }

    [Test]
    [Timeout(60_000)]
    public async Task UseWebServer_UsesCommandLineUrls(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        int port = PortUtils.GetOpenPortValue();
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder([
                "--urls", $"http://127.0.0.1:{port}"
            ])
            .WithWindow("server", static _ => { })
            .UseWebServer("server", static _ => { })
            .Build();

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100; attempt++) {
                if (runTask.IsFaulted) await runTask;

                if (application.TryGetWindow("server")?.Features.PageNavigation.GetCurrentUri()?.Port == port)
                    break;

                await Task.Delay(100, ct);
            }

            await Assert.That(application.GetWindow("server").Features.PageNavigation.GetCurrentUri()?.Port)
                .IsEqualTo(port);
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }

    [Test]
    [Timeout(60_000)]
    public async Task UseWebServer_AttachesTheStartedServerToSelectedWindows(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow("main", builder => builder.SetTitle("Main"))
            .WithWindow("secondary", builder => builder.SetTitle("Secondary"))
            .WithWindow("plain", builder => builder.SetStartPageContent("<html><body>Plain</body></html>"))
            .UseWebServer(["main", "secondary"], server =>
                server.WebHost.UseUrls("http://127.0.0.1:0"))
            .Build();

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100; attempt++) {
                if (runTask.IsFaulted) await runTask;

                Uri? mainUri = application.TryGetWindow("main")?.Features.PageNavigation.GetCurrentUri();
                Uri? secondaryUri = application.TryGetWindow("secondary")?.Features.PageNavigation.GetCurrentUri();
                if (mainUri?.Scheme == Uri.UriSchemeHttp && secondaryUri?.Scheme == Uri.UriSchemeHttp)
                    break;

                await Task.Delay(100, ct);
            }

            Uri? mainResult = application.GetWindow("main").Features.PageNavigation.GetCurrentUri();
            Uri? secondaryResult = application.GetWindow("secondary").Features.PageNavigation.GetCurrentUri();

            await Assert.That(mainResult?.Scheme).IsEqualTo(Uri.UriSchemeHttp);
            await Assert.That(secondaryResult?.Scheme).IsEqualTo(Uri.UriSchemeHttp);
            await Assert.That(mainResult?.Port).IsEqualTo(secondaryResult?.Port);
            await Assert.That(application.GetWindow("plain")).IsNotNull();
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }
}
