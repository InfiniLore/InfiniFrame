using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using InfiniTests.TestSupport.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InfiniTests.InfiniFrame.BlazorWebView;

[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationBlazorWebViewTests {
    [Test]
    public async Task UseBlazorWebView_RejectsMultipleTargetWindows() {
        await Assert.That(() => InfiniFrameApplication.CreateBuilder()
                .UseBlazorWebView(static _ => { }, "main", "settings"))
            .Throws<NotSupportedException>()
            .WithMessageContaining("only one window");
    }

    [Test]
    public async Task UseBlazorWebView_UsesConfiguredAppBaseUriForHttpClient() {
        if (!OperatingSystem.IsWindows()) return;

        Uri customBaseUri = new("app://custom-host/");
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(static _ => { })
            .UseBlazorWebView(configuration => configuration.Configure(options => options.AppBaseUri = customBaseUri))
            .Build();

        HttpClient client = application.RootServiceProvider.GetRequiredService<HttpClient>();

        await Assert.That(client.BaseAddress).IsEqualTo(customBaseUri);
        await Assert.That(application.RootServiceProvider.GetRequiredService<IOptions<InfiniFrameBlazorAppConfiguration>>().Value.AppBaseUri)
            .IsEqualTo(customBaseUri);
    }

    [Test]
    public async Task WithBlazorWebView_ReturnsApplicationAndDefersWindowBuild(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(window => window.SetStartPageContent("<html><body>Blazor</body></html>"))
            .UseBlazorWebView(static _ => { })
            .Build();

        await Assert.That(application.Windows).IsEmpty();
        await Assert.That(application.RootServiceProvider.GetService<IInfiniFrameWindowBuilder>()).IsNull();
    }

    [Test]
    [Timeout(60_000)]
    public async Task UseBlazorWebView_CombinesSingleUnnamedWindowConfiguration(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(window => window.SetTitle("Combined Blazor window"))
            .UseBlazorWebView(static _ => { })
            .Build();

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100 && application.Windows.Count == 0; attempt++) {
                if (runTask.IsFaulted) await runTask;
                await Task.Delay(100, ct);
            }

            await Assert.That(application.Windows).Count().IsEqualTo(1);
            await Assert.That(application.Windows[0].Features.Decorations.Title).IsEqualTo("Combined Blazor window");
        }
        finally {
            application.Shutdown();
            foreach (IInfiniFrameWindow window in application.Windows) window.Close();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }
}
