// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Threading.Channels;
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using InfiniFrame.Hosting;
using InfiniFrame.Window.Builder;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// External host providers at application level: UseHostProvider, the run sequence, partial start-up failure, host
// failure, shutdown, and selection neutrality. The selection latch is process-wide, so every test resets it first.
[NotInParallel]
public sealed class HostedApplicationTests {
    private static InfiniFrameApplication BuildHosted(ScriptedHostProvider provider, params string[] windowIds) {
        InfiniFrameHostSelection.ResetForTests();
        InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder().UseHostProvider(provider);
        foreach (string id in windowIds) builder.WithWindow(id, HostedHarness.ConfigureDefaults);
        return builder.Build();
    }

    [Test]
    [Timeout(60_000)]
    public async Task Run_CreatesWindowsThroughTheProvider_AndReturnsAfterTheLastWindowCloses(CancellationToken ct) {
        var provider = new ScriptedHostProvider();
        InfiniFrameApplication application = BuildHosted(provider, "main");
        var created = new List<IInfiniFrameWindow>();
        application.WindowCreated += created.Add;

        await Assert.That(provider.CreateHostCalls).IsEqualTo(1);
        await Assert.That(application.ApplicationHandle).IsEqualTo(IntPtr.Zero);
        await Assert.That(InfiniFrameHostSelection.Current).IsEqualTo(InfiniFrameHostSelection.Kind.Hosted);

        Task run = Task.Run(application.Run, ct);
        await Waits.UntilAsync(() => provider.HasHost && provider.Host.Windows.Count == 1, "window requested from the provider");
        IInfiniFrameWindow window = application.GetWindow("main");
        await window.Features.Lifecycle.WaitForReadyAsync(ct);
        await Assert.That(provider.Host.Services).IsNotNull();

        await window.Features.Lifecycle.CloseAsync(ct);
        await Waits.WithinAsync(run, "Run() returns after the last window closed", 15_000);

        ScriptedHost host = provider.Host;
        await Assert.That(created.Count).IsEqualTo(1);
        await Assert.That(host.StartCalls).IsEqualTo(1);
        await Assert.That(host.ShutdownCalls).IsEqualTo(1);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
        await Assert.That(host.Windows[0].DisposeCalls).IsEqualTo(1);
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Disposed);
        await Assert.That(application.Windows.Count).IsEqualTo(0);
    }

    [Test]
    [Timeout(60_000)]
    public async Task PartialStartupFailure_ClosesCreatedWindows_DisposesTheHostOnce_AndRethrows(CancellationToken ct) {
        var provider = new ScriptedHostProvider { FailCreateWindow = index => index == 1 ? new NotSupportedException("second window refused") : null };
        InfiniFrameApplication application = BuildHosted(provider, "first", "second");

        NotSupportedException exception = await Expect.ThrowsAsync<NotSupportedException>(() => Task.Run(application.Run, ct));
        await Assert.That(exception.Message).Contains("second window refused");
        ScriptedHost host = provider.Host;
        await Assert.That(host.Windows.Count).IsEqualTo(1);
        await Assert.That(host.Windows[0].RequestCloseCalls).IsEqualTo(1);
        await Assert.That(host.Windows[0].DisposeCalls).IsEqualTo(1);
        await Assert.That(host.ShutdownCalls).IsEqualTo(1);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(60_000)]
    public async Task HostFailure_EndsTheRun_FaultsOpenWindowsOnce_AndIsRethrown(CancellationToken ct) {
        var provider = new ScriptedHostProvider();
        InfiniFrameHostSelection.ResetForTests();
        int closedCallbacks = 0;
        InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .UseHostProvider(provider)
            .WithWindow("main", builder => {
                HostedHarness.ConfigureDefaults(builder);
                builder.RegisterWindowClosedHandler(_ => Interlocked.Increment(ref closedCallbacks));
            })
            .Build();

        Task run = Task.Run(application.Run, ct);
        await Waits.UntilAsync(() => provider.HasHost && provider.Host.Windows.Count == 1, "window requested");
        await application.GetWindow("main").Features.Lifecycle.WaitForReadyAsync(ct);
        provider.Host.FailHost(new IOException("companion process lost"));

        InfiniFrameHostOperationException exception = await Expect.ThrowsAsync<InfiniFrameHostOperationException>(() => run);
        await Assert.That(exception.InnerException).IsTypeOf<IOException>();
        await Assert.That(closedCallbacks).IsEqualTo(1);
        await Assert.That(provider.Host.DisposeCalls).IsEqualTo(1);
        await Assert.That(provider.Host.Windows[0].DisposeCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(60_000)]
    public async Task Shutdown_EndsTheRun_AndRunAsyncHonoursCancellation(CancellationToken ct) {
        var provider = new ScriptedHostProvider();
        InfiniFrameApplication application = BuildHosted(provider, "a", "b");
        Task run = Task.Run(application.Run, ct);
        await Waits.UntilAsync(() => provider.HasHost && provider.Host.Windows.Count == 2, "windows requested");
        application.Shutdown();
        await Waits.WithinAsync(run, "Run() after Shutdown()", 15_000);
        await Assert.That(provider.Host.Windows.All(static window => window.DisposeCalls == 1)).IsTrue();

        var second = new ScriptedHostProvider();
        InfiniFrameApplication cancellable = BuildHosted(second, "main");
        using var cancellation = new CancellationTokenSource();
        Task runAsync = cancellable.RunAsync(cancellation.Token);
        await Waits.UntilAsync(() => second.HasHost && second.Host.Windows.Count == 1, "window requested");
        await cancellation.CancelAsync();
        await Waits.WithinAsync(runAsync, "RunAsync() after cancellation", 15_000);
        await Assert.That(second.Host.DisposeCalls).IsEqualTo(1);
    }

    [Test]
    public async Task SelectionConflicts_ThrowBeforeAnyNativeOrProviderWork(CancellationToken ct) {
        InfiniFrameHostSelection.ResetForTests();
        var provider = new ScriptedHostProvider();

        await Assert.That(() => InfiniFrameApplication.CreateBuilder().UseHostProvider(provider).UseHostProvider(provider))
            .Throws<InvalidOperationException>();
        await Assert.That(() => InfiniFrameApplication.CreateBuilder().WithWebView2RuntimePath("C:/runtime").UseHostProvider(provider))
            .Throws<InvalidOperationException>();
        await Assert.That(() => InfiniFrameApplication.CreateBuilder().UseHostProvider(provider).WithAppUserModelId("app.id"))
            .Throws<InvalidOperationException>();
        await Assert.That(provider.CreateHostCalls).IsEqualTo(0);

        // Default window after a hosted selection: rejected by the latch before native work starts.
        InfiniFrameApplication hosted = BuildHosted(provider, "main");
        InvalidOperationException defaultAfterHosted = await Expect.ThrowsAsync<InvalidOperationException>(() =>
            Task.Run(() => new InfiniFrameWindowBuilder().Build(), ct));
        await Assert.That(defaultAfterHosted.Message).Contains("external host provider");
        await hosted.DisposeAsync();

        // Hosted application after a default selection: rejected before the provider creates a host.
        InfiniFrameHostSelection.ResetForTests();
        InfiniFrameHostSelection.Select(InfiniFrameHostSelection.Kind.Default);
        var late = new ScriptedHostProvider();
        await Assert.That(() => InfiniFrameApplication.CreateBuilder().UseHostProvider(late).Build()).Throws<InvalidOperationException>();
        await Assert.That(late.CreateHostCalls).IsEqualTo(0);
        InfiniFrameHostSelection.ResetForTests();
    }

    [Test]
    public async Task BlazorWebView_WithAHostProvider_RejectsSilentlyDroppingQueueModes(CancellationToken ct) {
        InfiniFrameHostSelection.ResetForTests();
        var provider = new ScriptedHostProvider();
        NotSupportedException exception = await Expect.ThrowsAsync<NotSupportedException>(() => Task.Run(() => InfiniFrameApplication.CreateBuilder()
            .UseHostProvider(provider)
            .WithWindow("main", HostedHarness.ConfigureDefaults)
            .UseBlazorWebView(blazor => blazor.Configure(options => options.WebMessageQueueFullMode = BoundedChannelFullMode.DropOldest), "main")
            .Build(), ct));
        await Assert.That(exception.Message).Contains("DropOldest");
        await Assert.That(provider.Host.DisposeCalls).IsEqualTo(1);
        InfiniFrameHostSelection.ResetForTests();
    }
}
