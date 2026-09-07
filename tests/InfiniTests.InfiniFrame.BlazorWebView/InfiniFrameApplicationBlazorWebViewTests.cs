using InfiniFrame;
using InfiniFrame.BlazorWebView;

namespace InfiniTests.InfiniFrame.BlazorWebView;

[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationBlazorWebViewTests {
    [Test]
    public async Task WithBlazorWebView_ReturnsApplicationAndDefersWindowBuild(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        using var application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(window => window.SetStartPageContent("<html><body>Blazor</body></html>"))
            .UseBlazorWebView(static _ => { })
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }
}
