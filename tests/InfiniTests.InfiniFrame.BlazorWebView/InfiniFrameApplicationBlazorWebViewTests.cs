using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using InfiniTests.TestSupport.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InfiniTests.InfiniFrame.BlazorWebView;

[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationBlazorWebViewTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task UseBlazorWebView_RejectsMultipleTargetWindows() {
        await Assert.That(() => InfiniFrameApplication.CreateBuilder()
                .UseBlazorWebView(static _ => { }, "main", "settings"))
            .Throws<NotSupportedException>()
            .WithMessageContaining("only one window");
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task UseBlazorWebView_RejectsSecondRegistration_WithNoWindows() {
        await Assert.That(() => InfiniFrameApplication.CreateBuilder()
                .UseBlazorWebView(static _ => { })
                .UseBlazorWebView(static _ => { }))
            .Throws<InvalidOperationException>()
            .WithMessageContaining("already been registered");
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task UseBlazorWebView_RejectsSecondRegistration_WithSameWindowId() {
        await Assert.That(() => InfiniFrameApplication.CreateBuilder()
                .WithWindow("main", static _ => { })
                .UseBlazorWebView("main", static _ => { })
                .UseBlazorWebView("main", static _ => { }))
            .Throws<InvalidOperationException>()
            .WithMessageContaining("already been registered");
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task UseBlazorWebView_RejectsSecondRegistration_WithDifferentWindowIds() {
        await Assert.That(() => InfiniFrameApplication.CreateBuilder()
                .WithWindow("a", static _ => { })
                .WithWindow("b", static _ => { })
                .UseBlazorWebView("a", static _ => { })
                .UseBlazorWebView("b", static _ => { }))
            .Throws<InvalidOperationException>()
            .WithMessageContaining("already been registered");
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task UseBlazorWebView_RejectsSecondRegistration_ViaWithBlazorWebView() {
        await Assert.That(() => InfiniFrameApplication.CreateBuilder()
                .UseBlazorWebView(static _ => { })
                .WithBlazorWebView(static _ => { }))
            .Throws<InvalidOperationException>()
            .WithMessageContaining("already been registered");
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task UseBlazorWebView_PreservesRootComponentsOnSingleRegistration() {
        if (!OperatingSystem.IsWindows()) return;

        InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder()
            .WithWindow(static _ => { });

        IInfiniFrameRootComponentList? capturedRootComponents = null;
        builder.UseBlazorWebView(config => {
            config.RootComponents.Add<StubComponent>("stub");
            capturedRootComponents = config.RootComponents;
        });

        await using InfiniFrameApplication application = builder.Build();

        IInfiniFrameRootComponentList resolved = application.RootServiceProvider.GetRequiredService<IInfiniFrameRootComponentList>();
        await Assert.That(resolved).IsSameReferenceAs(capturedRootComponents!);
        await Assert.That(resolved).Count().IsEqualTo(1);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task UseBlazorWebView_SingleRegistration_TargetsCorrectWindow() {
        if (!OperatingSystem.IsWindows()) return;

        bool windowResolved = false;
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow("blazor", builder => builder.SetTitle("Blazor Window"))
            .WithWindow("plain", builder => builder.SetStartPageContent("<html><body>Plain</body></html>"))
            .UseBlazorWebView("blazor", static _ => { })
            .Build();

        application.WindowCreated += window => {
            if (window.Features.Decorations.Title == "Blazor Window")
                windowResolved = true;
        };

        Task runTask = application.RunAsync(CancellationToken.None);
        try {
            for (int attempt = 0; attempt < 100 && !windowResolved; attempt++) {
                if (runTask.IsFaulted) await runTask;
                await Task.Delay(100, CancellationToken.None);
            }

            await Assert.That(windowResolved).IsTrue();
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task UseBlazorWebView_SingleRegistration_RegistersHandlers() {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(static _ => { })
            .UseBlazorWebView(static _ => { })
            .Build();

        await Assert.That(application.RootServiceProvider.GetRequiredService<IInfiniFrameWebViewManager>()).IsNotNull();
        await Assert.That(application.RootServiceProvider.GetRequiredService<InfiniFrameHttpHandler>()).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task UseBlazorWebView_SingleRegistration_ConfiguresStartupUrl() {
        if (!OperatingSystem.IsWindows()) return;

        Uri customBaseUri = new("app://my-app/");
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(static _ => { })
            .UseBlazorWebView(config => config.Configure(opts => {
                opts.AppBaseUri = customBaseUri;
                opts.HostPage = "custom.html";
            }))
            .Build();

        IOptions<InfiniFrameBlazorAppConfiguration> options =
            application.RootServiceProvider.GetRequiredService<IOptions<InfiniFrameBlazorAppConfiguration>>();
        await Assert.That(options.Value.AppBaseUri).IsEqualTo(customBaseUri);
        await Assert.That(options.Value.HostPage).IsEqualTo("custom.html");
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task UseBlazorWebView_SingleRegistration_SupportsDisposal() {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(static _ => { })
            .UseBlazorWebView(static _ => { })
            .Build();

        IInfiniFrameWebViewManager manager = application.RootServiceProvider.GetRequiredService<IInfiniFrameWebViewManager>();
        await Assert.That(manager).IsNotNull();
        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
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
    [NotInParallelInfiniTests]
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
    [NotInParallelInfiniTests]
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

    private sealed class StubComponent : IComponent {
        public void Attach(RenderHandle renderHandle) { }
        public Task SetParametersAsync(ParameterView parameters) => Task.CompletedTask;
    }
}
