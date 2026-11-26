// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using InfiniFrame.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     One hosted Blazor application with the real Blazor WebView manager on the scripted provider. The test plays each
///     browser document's page: it acknowledges render batches in the document that received them and runs the JS
///     calls it was told to handle, in the document they reached (as a real page would).
/// </summary>
internal sealed class BlazorDocumentScenario {
    internal const string AttachPage = "__bwv:[\"AttachPage\",\"app://localhost/\",\"app://localhost/\"]";
    private readonly ConcurrentDictionary<string, Func<ulong, long, string, string?>> _handlers = new(StringComparer.Ordinal);

    private BlazorDocumentScenario(Task run, ScriptedWindowHostCore host, IInfiniFrameWindow window) {
        Run = run;
        Host = host;
        Window = window;
        Pages = window.ServiceProvider!.GetRequiredService<PageRegistry>();
    }

    internal Task Run { get; }
    internal ScriptedWindowHostCore Host { get; }
    internal IInfiniFrameWindow Window { get; }
    internal PageRegistry Pages { get; }

    internal static async Task<BlazorDocumentScenario> StartAsync(
        CancellationToken ct,
        Action<InfiniFrameBlazorWebViewConfiguration>? blazor = null,
        Action<IServiceCollection>? services = null
    ) {
        InfiniFrameHostSelection.ResetForTests();
        var provider = new ScriptedHostProvider();
        InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder();
        builder.Services.AddSingleton<PageRegistry>();
        builder.Services.AddScoped<PageScopeProbe>();
        services?.Invoke(builder.Services);
        InfiniFrameApplication application = builder
            .UseHostProvider(provider)
            .WithWindow("main", static _ => {})
            .UseBlazorWebView(configuration => {
                configuration.RootComponents.Add<IsolationProbe>("#app");
                blazor?.Invoke(configuration);
            }, "main")
            .Build();

        Task run = Task.Run(application.Run, ct);
        await Waits.UntilAsync(() => provider.HasHost && provider.Host.Windows.Count == 1, "window requested");
        ScriptedWindowHostCore host = provider.Host.Windows[0];
        IInfiniFrameWindow window = application.GetWindow("main");
        await window.Features.Lifecycle.WaitForReadyAsync(ct);
        var scenario = new BlazorDocumentScenario(run, host, window);
        host.OnSent = scenario.PlayPage;
        return scenario;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Documents and pages
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>Attaches <paramref name="document" />, attaches its page and waits until the page rendered into it.</summary>
    internal async Task<IsolationProbe> LoadPageAsync(ulong document) {
        int pages = Pages.Count;
        await Host.AttachAsync(document);
        await Host.PostAsync(document, AttachPage);
        await Waits.UntilAsync(() => Pages.Count > pages && RenderBatches(document) > 0, $"a page rendered into document {document}", 10_000);
        return Pages.Latest;
    }

    /// <summary>Waits for Blazor's dispatcher and the provider loop, then for a short quiet period.</summary>
    internal async Task SettleAsync() {
        await Pages.Latest.OnDispatcher(static () => {});
        await Host.Host.OnLoopAsync(static () => {});
        await Task.Delay(200);
        await Host.Host.OnLoopAsync(static () => {});
    }

    internal async Task CloseAsync(CancellationToken ct) {
        await Window.Features.Lifecycle.CloseAsync(ct);
        await Waits.WithinAsync(Run, "Run() after close", 15_000);
        InfiniFrameHostSelection.ResetForTests();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // What the provider was asked to send
    // -----------------------------------------------------------------------------------------------------------------
    internal int Mark => Host.Sent.Count;
    internal SentMessage[] SentSince(int mark) => [.. Host.Sent.Skip(mark)];
    internal int RenderBatches(ulong document) => Host.Sent.Count(m => m.Document == document && IsType(m.Message, "RenderBatch"));
    internal int CountOf(ulong document, string type) => Host.Sent.Count(m => m.Document == document && IsType(m.Message, type));

    internal SentMessage[] JsCalls(string identifier) =>
        [.. Host.Sent.Where(m => IsType(m.Message, "BeginInvokeJS") && ParseJsCall(m.Message).Identifier == identifier)];

    internal static bool IsType(string message, string type) => message.StartsWith($"__bwv:[\"{type}\"", StringComparison.Ordinal);

    /// <summary>True when a sent RenderBatch carries <paramref name="text" /> (render batches hold their strings as UTF-8).</summary>
    internal static bool RenderBatchContains(SentMessage sent, string text) {
        if (!IsType(sent.Message, "RenderBatch")) return false;
        using JsonDocument json = JsonDocument.Parse(sent.Message.AsMemory(6));
        byte[] batch = Convert.FromBase64String(json.RootElement[2].GetString() ?? "");
        return batch.AsSpan().IndexOf(Encoding.UTF8.GetBytes(text)) >= 0;
    }

    internal static string TypeOf(string message) =>
        message.StartsWith("__bwv:[\"", StringComparison.Ordinal) ? message[8..message.IndexOf('"', 8)] : "(not Blazor)";

    internal static (long TaskId, string Identifier, string ArgsJson) ParseJsCall(string message) {
        using JsonDocument json = JsonDocument.Parse(message.AsMemory(6));
        JsonElement root = json.RootElement;
        return (root[1].GetInt64(), root[2].GetString() ?? "", root[3].ValueKind == JsonValueKind.String ? root[3].GetString()! : "");
    }

    internal static string Describe(SentMessage[] messages) {
        var text = new StringBuilder();
        foreach (SentMessage message in messages) {
            string type = TypeOf(message.Message);
            string detail = type == "RenderBatch" ? "" : $" {Shorten(message.Message)}";
            text.Append($"[document {message.Document}: {type}{detail}] ");
        }
        return text.Length == 0 ? "(none)" : text.ToString();

        static string Shorten(string message) => message.Length <= 140 ? message : message[..140] + "…";
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Playing the page
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     The page runs every call of <paramref name="identifier" /> it receives: <paramref name="handler" /> gets the
    ///     document, task id and argument JSON and returns the JSON result to answer with, or null to leave it unanswered.
    /// </summary>
    internal void Handle(string identifier, Func<ulong, long, string, string?> handler) => _handlers[identifier] = handler;

    /// <summary>The page answers every call of <paramref name="identifier" /> with the JSON result for its document.</summary>
    internal void Answer(string identifier, Func<ulong, string?> answer) => Handle(identifier, (document, _, _) => answer(document));

    internal Task ReplyAsync(ulong document, long taskId, string resultJson) => Host.PostAsync(document, EndInvokeJs(taskId, resultJson));

    internal Task InvokeDotNetAsync(ulong document, string callId, long dotNetObjectId, string method, string argsJson) =>
        Host.PostAsync(document, "__bwv:" + JsonSerializer.Serialize(new object?[] { "BeginInvokeDotNet", callId, null, method, dotNetObjectId, argsJson }));

    internal static string EndInvokeJs(long taskId, string resultJson) =>
        "__bwv:" + JsonSerializer.Serialize(new object[] { "EndInvokeJS", taskId, true, $"[{taskId},true,{resultJson}]" });

    private void PlayPage(SentMessage sent) {
        string? reply = null;
        if (IsType(sent.Message, "RenderBatch")) {
            using JsonDocument json = JsonDocument.Parse(sent.Message.AsMemory(6));
            reply = $"__bwv:[\"OnRenderCompleted\",{json.RootElement[1].GetInt64()},null]";
        }
        else if (IsType(sent.Message, "BeginInvokeJS")) {
            (long taskId, string identifier, string argsJson) = ParseJsCall(sent.Message);
            if (_handlers.TryGetValue(identifier, out Func<ulong, long, string, string?>? handler)
                && handler(sent.Document, taskId, argsJson) is {} result)
                reply = EndInvokeJs(taskId, result);
        }
        if (reply is null) return;
        ulong document = sent.Document;
        Host.Host.Post(() => Host.Events.MessageReceived(new InfiniFrameDocumentId(document), reply));
    }

    internal static string Outcome(Task? task) => task switch {
        null => "not started",
        { IsCompletedSuccessfully: true } => task is Task<string> text ? $"completed with \"{text.Result}\"" : "completed",
        { IsCanceled: true } => "cancelled by its caller's token",
        { IsFaulted: true } => $"failed with {task.Exception!.GetBaseException().GetType().Name}",
        _ => "still pending"
    };
}

/// <summary>Application singleton: the pages in creation order, and an application-lifetime producer.</summary>
public sealed class PageRegistry {
    private readonly object _gate = new();
    private readonly List<IsolationProbe> _pages = [];
    private Action? _ticked;
    private int _subscribers;

    /// <summary>The page number whose initialization throws (0: none).</summary>
    public int FailPage { get; set; }

    /// <summary>Whether <see cref="DisposingJsProbe" /> catches <see cref="JSDisconnectedException" /> in its disposal.</summary>
    public bool DisposeJsCatches { get; set; }

    private int _disposeJsCaught;
    public int DisposeJsCaught => Volatile.Read(ref _disposeJsCaught);
    public void CountDisposeJsCaught() => Interlocked.Increment(ref _disposeJsCaught);

    public event Action? Ticked {
        add {
            lock (_gate) {
                _ticked += value;
                _subscribers++;
            }
        }
        remove {
            lock (_gate) {
                _ticked -= value;
                _subscribers--;
            }
        }
    }

    public int Subscribers {
        get {
            lock (_gate) return _subscribers;
        }
    }

    public int Count {
        get {
            lock (_gate) return _pages.Count;
        }
    }

    public IsolationProbe Latest {
        get {
            lock (_gate) return _pages[^1];
        }
    }

    public IsolationProbe[] All {
        get {
            lock (_gate) return [.. _pages];
        }
    }

    public IsolationProbe this[int number] {
        get {
            lock (_gate) return _pages[number - 1];
        }
    }

    public int Register(IsolationProbe page) {
        lock (_gate) {
            _pages.Add(page);
            return _pages.Count;
        }
    }

    private readonly List<PageScopeProbe> _secondRoots = [];

    /// <summary>Records a second root component created in the page that owns <paramref name="scope" />.</summary>
    public void RegisterSecondRoot(PageScopeProbe scope) {
        lock (_gate) _secondRoots.Add(scope);
    }

    /// <summary>How many second root components were created in the page that owns <paramref name="scope" />.</summary>
    public int SecondRoots(PageScopeProbe scope) {
        lock (_gate) return _secondRoots.Count(s => ReferenceEquals(s, scope));
    }

    public void Tick() {
        Action? handlers;
        lock (_gate) handlers = _ticked;
        handlers?.Invoke();
    }
}

/// <summary>A per-page scoped service: one per page scope, disposed with that scope.</summary>
public sealed class PageScopeProbe : IDisposable {
    private int _disposeCalls;
    public int DisposeCalls => Volatile.Read(ref _disposeCalls);
    public void Dispose() => Interlocked.Increment(ref _disposeCalls);
}

/// <summary>The root component. Every page (one per AttachPage) creates one, with its own scoped services.</summary>
public sealed class IsolationProbe : ComponentBase, IDisposable {
    private string _text = "initial";
    private int _ticks;
    private int _disposeCalls;
    private bool _subscribed;

    [Inject] public IJSRuntime JS { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public PageScopeProbe Scope { get; set; } = default!;
    [Inject] public PageRegistry Registry { get; set; } = default!;

    public int Number { get; private set; }
    public int DisposeCalls => Volatile.Read(ref _disposeCalls);
    public string Text => Volatile.Read(ref _text);
    public int Ticks => Volatile.Read(ref _ticks);

    protected override void OnInitialized() {
        Number = Registry.Register(this);
        if (Registry.FailPage == Number) throw new InvalidOperationException($"page {Number} failed to initialize");
        Registry.Ticked += OnTicked;
        _subscribed = true;
    }

    private void OnTicked() => _ = InvokeAsync(() => {
        _ticks++;
        StateHasChanged();
    });

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddMarkupContent(0, $"<p>page {Number}: {_text} ({_ticks} ticks)</p>");

    /// <summary>Runs on Blazor's dispatcher through this component's own render handle.</summary>
    public Task OnDispatcher(Action action) => InvokeAsync(action);

    public void Rerender(string text) {
        Volatile.Write(ref _text, text);
        StateHasChanged();
    }

    /// <summary>Called from the page (JS to .NET), as a UI event handler would be.</summary>
    [JSInvokable]
    public string Clicked(string label) {
        Volatile.Write(ref _text, label);
        _ = InvokeAsync(StateHasChanged);
        return $"page-{Number}:{label}";
    }

    public void Dispose() {
        if (_subscribed) Registry.Ticked -= OnTicked;
        Interlocked.Increment(ref _disposeCalls);
    }
}

/// <summary>
///     A second root component whose disposal makes a JS call, as a JS-module component's <c>DisposeAsync</c> does. It
///     catches <see cref="JSDisconnectedException" /> only when the registry says so.
/// </summary>
public sealed class DisposingJsProbe : ComponentBase, IAsyncDisposable {
    [Inject] public IJSRuntime JS { get; set; } = default!;
    [Inject] public PageRegistry Registry { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddMarkupContent(0, "<p>disposing probe</p>");

    public async ValueTask DisposeAsync() {
        try {
            await JS.InvokeVoidAsync("probe.dispose-js", "dispose-time-call");
        }
        catch (JSDisconnectedException) when (Registry.DisposeJsCatches) {
            Registry.CountDisposeJsCaught();
        }
    }
}

/// <summary>A small in-memory static file set (index.html and app.css), so resource requests can succeed.</summary>
internal sealed class InMemoryFiles : IFileProvider {
    private readonly Dictionary<string, byte[]> _files = new(StringComparer.Ordinal) {
        ["index.html"] = "<!DOCTYPE html><html><body><div id=\"app\"></div></body></html>"u8.ToArray(),
        ["app.css"] = "body { margin: 0; }"u8.ToArray()
    };

    public IFileInfo GetFileInfo(string subpath) =>
        _files.TryGetValue(subpath.TrimStart('/'), out byte[]? content) ? new File(subpath.TrimStart('/'), content) : new NotFoundFileInfo(subpath);

    public IDirectoryContents GetDirectoryContents(string subpath) => NotFoundDirectoryContents.Singleton;
    public IChangeToken Watch(string filter) => NullChangeToken.Singleton;

    private sealed class File(string name, byte[] content) : IFileInfo {
        public bool Exists => true;
        public long Length => content.Length;
        public string? PhysicalPath => null;
        public string Name => name;
        public DateTimeOffset LastModified { get; } = DateTimeOffset.UnixEpoch;
        public bool IsDirectory => false;
        public Stream CreateReadStream() => new MemoryStream(content, writable: false);
    }
}

/// <summary>A task scheduler that holds the tasks queued to it until released (to hold Blazor's own work queue).</summary>
internal sealed class HeldTaskScheduler : TaskScheduler {
    private readonly List<Task> _held = [];
    private bool _holding;

    internal void Hold() {
        lock (_held) _holding = true;
    }

    internal void Release() {
        Task[] released;
        lock (_held) {
            _holding = false;
            released = [.. _held];
            _held.Clear();
        }
        foreach (Task task in released) Run(task);
    }

    protected override void QueueTask(Task task) {
        lock (_held) {
            if (_holding) {
                _held.Add(task);
                return;
            }
        }
        Run(task);
    }

    private void Run(Task task) =>
        ThreadPool.UnsafeQueueUserWorkItem(static state => state.Scheduler.TryExecuteTask(state.Task), (Scheduler: this, Task: task), preferLocal: false);

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;

    protected override IEnumerable<Task> GetScheduledTasks() {
        lock (_held) return [.. _held];
    }
}
