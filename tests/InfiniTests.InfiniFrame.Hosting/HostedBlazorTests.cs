// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using InfiniFrame.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// Status fidelity with the real Blazor WebView manager, and the existing native Blazor integration reused unchanged
// through the facade (one renderer, document-bound delivery). Contract tests only: they play the page side of the IPC.
[NotInParallel]
public sealed class HostedBlazorTests {
    private const string AttachPage = "__bwv:[\"AttachPage\",\"app://localhost/\",\"app://localhost/\"]";

    [Test]
    [Timeout(60_000)]
    public async Task RealWebViewManager_KeepsResourceStatuses_AndRendersIntoTheAttachingDocument(CancellationToken ct) {
        InfiniFrameHostSelection.ResetForTests();
        var provider = new ScriptedHostProvider();
        InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .UseHostProvider(provider)
            .WithWindow("main", static _ => {})
            .UseBlazorWebView(static blazor => blazor.RootComponents.Add<HelloComponent>("#app"), "main")
            .Build();

        Task run = Task.Run(application.Run, ct);
        await Waits.UntilAsync(() => provider.HasHost && provider.Host.Windows.Count == 1, "window requested");
        ScriptedWindowHostCore host = provider.Host.Windows[0];
        IInfiniFrameWindow window = application.GetWindow("main");
        await window.Features.Lifecycle.WaitForReadyAsync(ct);
        AcknowledgeRenderBatches(host);
        await host.AttachAsync(1);

        // blazor.webview.js reaches applications as a Razor static web asset, which this plain test project does not
        // have; an application built with the Razor SDK serves it with status 200.
        await using (InfiniFrameHostResourceResponse missing = await Get(host, "app://localhost/missing.css", ct)) {
            await Assert.That(missing.StatusCode).IsEqualTo(404);
        }
        await using (InfiniFrameHostResourceResponse untrusted = await Get(host, "app://untrusted.example/x.js", ct)) {
            await Assert.That(untrusted.StatusCode).IsEqualTo(403);
        }
        // The default-engine tuple contract is unchanged: the same missing asset still arrives as content there.
        (Stream? data, _) = application.GetWindow("main").ServiceProvider!.GetService(typeof(IInfiniFrameWebViewManager)) is IInfiniFrameWebViewManager manager
            ? manager.HandleWebRequest(window, "app://localhost/missing.css")
            : default;
        await using (data) await Assert.That(data).IsNotNull();

        await host.PostAsync(1, AttachPage);
        await Waits.UntilAsync(() => host.Sent.Any(static m => m.Document == 1 && m.Message.Contains("\"RenderBatch\"", StringComparison.Ordinal)),
            "render batch for document 1");
        await Assert.That(host.Sent.All(static m => m.Document == 1)).IsTrue();

        await window.Features.Lifecycle.CloseAsync(ct);
        await Waits.WithinAsync(run, "Run() after close", 15_000);
        InfiniFrameHostSelection.ResetForTests();
    }

    [Test]
    [Timeout(60_000)]
    public async Task WorkStartedUnderAnEarlierPage_RendersIntoTheCurrentPage(CancellationToken ct) {
        // Blazor output must follow the page context that produces it, not the execution context of
        // whoever started the work. A singleton keeps page 1's execution context (as a timer would) and later drives the
        // component of page 2; that render belongs to document 2.
        InfiniFrameHostSelection.ResetForTests();
        var provider = new ScriptedHostProvider();
        var ticker = new PageTicker();
        InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder();
        builder.Services.AddSingleton(ticker);
        InfiniFrameApplication application = builder
            .UseHostProvider(provider)
            .WithWindow("main", static _ => {})
            .UseBlazorWebView(static blazor => blazor.RootComponents.Add<TickingComponent>("#app"), "main")
            .Build();

        Task run = Task.Run(application.Run, ct);
        await Waits.UntilAsync(() => provider.HasHost && provider.Host.Windows.Count == 1, "window requested");
        ScriptedWindowHostCore host = provider.Host.Windows[0];
        IInfiniFrameWindow window = application.GetWindow("main");
        await window.Features.Lifecycle.WaitForReadyAsync(ct);
        AcknowledgeRenderBatches(host);

        await host.AttachAsync(1);
        await host.PostAsync(1, AttachPage);
        await Waits.UntilAsync(() => RenderBatches(host, 1) > 0, "page 1 rendered into document 1");
        await Assert.That(ticker.IsStarted).IsTrue();

        // Reload: document 2 replaces document 1 and its page attaches.
        await host.AttachAsync(2);
        int document1Messages = host.Sent.Count(static m => m.Document == 1);
        await host.PostAsync(2, AttachPage);
        await Waits.UntilAsync(() => RenderBatches(host, 2) > 0, "page 2 rendered into document 2");
        int document2Batches = RenderBatches(host, 2);

        ticker.Tick();
        await Waits.UntilAsync(() => RenderBatches(host, 2) > document2Batches, "the tick re-rendered page 2 in document 2");
        await Assert.That(host.Sent.Count(static m => m.Document == 1)).IsEqualTo(document1Messages);
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);

        await window.Features.Lifecycle.CloseAsync(ct);
        await Waits.WithinAsync(run, "Run() after close", 15_000);
        InfiniFrameHostSelection.ResetForTests();
    }

    private static int RenderBatches(ScriptedWindowHostCore host, ulong document) =>
        host.Sent.Count(m => m.Document == document && m.Message.StartsWith("__bwv:[\"RenderBatch\"", StringComparison.Ordinal));

    /// <summary>Plays the page: acknowledges each render batch in the document that received it.</summary>
    private static void AcknowledgeRenderBatches(ScriptedWindowHostCore host) =>
        host.OnSent = sent => {
            if (!sent.Message.StartsWith("__bwv:[\"RenderBatch\"", StringComparison.Ordinal)) return;
            using var json = JsonDocument.Parse(sent.Message.AsMemory(6));
            long batch = json.RootElement[1].GetInt64();
            host.Host.Post(() => host.Events.MessageReceived(new InfiniFrameDocumentId(sent.Document), $"__bwv:[\"OnRenderCompleted\",{batch},null]"));
        };

    private static async Task<InfiniFrameHostResourceResponse> Get(ScriptedWindowHostCore host, string url, CancellationToken ct) =>
        await host.Events.GetResourceAsync(new InfiniFrameHostResourceRequest { Method = "GET", Url = new Uri(url) }, ct);

    public sealed class HelloComponent : ComponentBase {
        protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddMarkupContent(0, "<p>hosted</p>");
    }

    /// <summary>A singleton that keeps the execution context of the page that started it, as a timer does.</summary>
    public sealed class PageTicker {
        private ExecutionContext? _context;

        public event Action? Ticked;
        public bool IsStarted => Volatile.Read(ref _context) is not null;

        public void Start() => Interlocked.CompareExchange(ref _context, ExecutionContext.Capture(), null);

        public void Tick() {
            ExecutionContext context = Volatile.Read(ref _context) ?? throw new InvalidOperationException("The ticker was never started.");
            ExecutionContext.Run(context.CreateCopy(), static state => ((PageTicker)state!).Ticked?.Invoke(), this);
        }
    }

    public sealed class TickingComponent : ComponentBase, IDisposable {
        private int _ticks;

        [Inject] public PageTicker Ticker { get; set; } = default!;

        protected override void OnInitialized() {
            Ticker.Start();
            Ticker.Ticked += OnTicked;
        }

        private void OnTicked() => _ = InvokeAsync(() => {
            _ticks++;
            StateHasChanged();
        });

        protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddMarkupContent(0, $"<p>ticks {_ticks}</p>");

        public void Dispose() => Ticker.Ticked -= OnTicked;
    }
}
