using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using InfiniTests.TestSupport.Attributes;

namespace InfiniTests.InfiniFrame.BlazorWebView;

[NotInParallelInfiniTests]
public sealed class MixedWindowConfigurationTests {
    [Test]
    [Timeout(60_000)]
    public async Task BlazorWebView_CanTargetOneWindowAlongsidePlainHtml(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow("blazor", builder => builder.SetTitle("Blazor"))
            .WithWindow("plain", builder => builder.SetStartPageContent("<html><body>Plain</body></html>"))
            .UseBlazorWebView("blazor", static _ => { })
            .Build();

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100; attempt++) {
                if (runTask.IsFaulted) await runTask;

                Uri? blazorUri = application.TryGetWindow("blazor")?.Features.PageNavigation.GetCurrentUri();
                if (blazorUri?.Scheme == "app") break;

                await Task.Delay(100, ct);
            }

            await Assert.That(application.Windows).Count().IsEqualTo(2);
            await Assert.That(application.GetWindow("blazor").Features.Decorations.Title).IsEqualTo("Blazor");
            await Assert.That(application.GetWindow("plain").Features.Decorations.Title).IsNotEqualTo("Blazor");
            await Assert.That(application.GetWindow("blazor").Features.PageNavigation.GetCurrentUri()?.Scheme)
                .IsEqualTo("app");
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }
}
