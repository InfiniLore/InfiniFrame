// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Drawing;
using InfiniFrame.Hosting;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// Deterministic scripted provider used by the in-repository contract tests (no real browser). Every provider-side
// action runs on the provider's own "native loop" thread, separate from InfiniFrame's dispatcher, so the tests also
// show that neither side needs the other's thread.

internal readonly record struct SentMessage(ulong Document, string Message, int ThreadId);

internal sealed class ScriptedHostProvider(InfiniFrameHostCapabilities capabilities = ScriptedHostProvider.AllCapabilities) : IInfiniFrameHostProvider {
    internal const InfiniFrameHostCapabilities AllCapabilities =
        InfiniFrameHostCapabilities.Bounds | InfiniFrameHostCapabilities.WindowState | InfiniFrameHostCapabilities.Focus
        | InfiniFrameHostCapabilities.Title | InfiniFrameHostCapabilities.Icon | InfiniFrameHostCapabilities.BoundsEvents
        | InfiniFrameHostCapabilities.StateEvents | InfiniFrameHostCapabilities.FocusEvents | InfiniFrameHostCapabilities.CloseCancellation;

    private ScriptedHost? _host;
    private int _createHostCalls;

    public string DisplayName => "scripted test host";
    public Version ContractVersion { get; init; } = new(InfiniFrameHostContract.MajorVersion, InfiniFrameHostContract.MinorVersion);
    public InfiniFrameHostCapabilities Capabilities { get; } = capabilities;
    public bool AutoCreated { get; init; } = true;
    public bool AutoClose { get; init; } = true;
    public bool ImplementCapabilityInterfaces { get; init; } = true;
    public Func<int, Exception?>? FailCreateWindow { get; init; }

    /// <summary>Awaited inside StartAsync to simulate asynchronous provider startup.</summary>
    public Func<CancellationToken, Task>? StartGate { get; init; }

    /// <summary>Awaited inside CreateWindowAsync before the window host exists (simulates slow native creation).</summary>
    public Func<int, Task>? CreateWindowGate { get; init; }

    /// <summary>Awaited inside CreateWindowAsync after Created() was reported and before the window host is returned.</summary>
    public Func<int, Task>? AfterCreatedGate { get; init; }

    internal int CreateHostCalls => Volatile.Read(ref _createHostCalls);
    internal ScriptedHost Host => _host ?? throw new InvalidOperationException("CreateHost has not been called.");
    internal bool HasHost => _host is not null;

    public IInfiniFrameHost CreateHost(InfiniFrameHostCreateContext context) {
        Interlocked.Increment(ref _createHostCalls);
        return _host = new ScriptedHost(this);
    }
}

internal sealed class ScriptedHost : IInfiniFrameHost {
    private readonly BlockingCollection<Action> _loop = new();
    private readonly Thread _thread;
    private readonly List<ScriptedWindowHostCore> _windows = [];
    private readonly TaskCompletionSource _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _createWindowCalls;
    private int _startCalls;
    private int _shutdownCalls;
    private int _disposeCalls;

    internal ScriptedHost(ScriptedHostProvider provider) {
        Provider = provider;
        _thread = new Thread(RunLoop) { IsBackground = true, Name = "scripted provider loop" };
        _thread.Start();
    }

    internal ScriptedHostProvider Provider { get; }
    internal int LoopThreadId => _thread.ManagedThreadId;
    internal int CreateWindowCalls => Volatile.Read(ref _createWindowCalls);
    internal int StartCalls => Volatile.Read(ref _startCalls);
    internal int ShutdownCalls => Volatile.Read(ref _shutdownCalls);
    internal int DisposeCalls => Volatile.Read(ref _disposeCalls);
    internal IServiceProvider? Services { get; private set; }

    internal IReadOnlyList<ScriptedWindowHostCore> Windows {
        get {
            lock (_windows) return [.. _windows];
        }
    }

    public Task Completion => _completion.Task;

    internal void FailHost(Exception exception) => _completion.TrySetException(exception);

    internal Task OnLoopAsync(Action action) {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _loop.Add(() => {
            try {
                action();
                completion.TrySetResult();
            }
            catch (Exception exception) {
                completion.TrySetException(exception);
            }
        });
        return completion.Task;
    }

    internal Task<T> OnLoopAsync<T>(Func<T> function) {
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        _loop.Add(() => {
            try { completion.TrySetResult(function()); }
            catch (Exception exception) { completion.TrySetException(exception); }
        });
        return completion.Task;
    }

    internal void Post(Action action) => _loop.Add(action);

    private void RunLoop() {
        foreach (Action action in _loop.GetConsumingEnumerable()) action();
    }

    public async ValueTask StartAsync(InfiniFrameHostStartContext context, CancellationToken cancellationToken) {
        Interlocked.Increment(ref _startCalls);
        Services = context.Services;
        if (Provider.StartGate is {} gate) await gate(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<IInfiniFrameWindowHost> CreateWindowAsync(
        InfiniFrameHostWindowRequest request,
        IInfiniFrameWindowHostEvents events,
        CancellationToken cancellationToken
    ) {
        int index;
        lock (_windows) index = _windows.Count;
        Interlocked.Increment(ref _createWindowCalls);
        if (Provider.CreateWindowGate is {} gate) await gate(index).ConfigureAwait(false);
        if (Provider.FailCreateWindow?.Invoke(index) is {} failure) throw failure;
        ScriptedWindowHostCore window = Provider.ImplementCapabilityInterfaces
            ? new ScriptedWindowHost(this, request, events)
            : new ScriptedWindowHostCore(this, request, events);
        lock (_windows) _windows.Add(window);
        // Report creation on the provider loop before returning: events may precede the CreateWindowAsync result.
        if (Provider.AutoCreated) await OnLoopAsync(events.Created).ConfigureAwait(false);
        if (Provider.AfterCreatedGate is {} afterCreated) await afterCreated(index).ConfigureAwait(false);
        return window;
    }

    public ValueTask ShutdownAsync(CancellationToken cancellationToken) {
        Interlocked.Increment(ref _shutdownCalls);
        _completion.TrySetResult();
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync() {
        if (Interlocked.Increment(ref _disposeCalls) == 1) {
            _loop.CompleteAdding();
            _thread.Join(TimeSpan.FromSeconds(10));
        }
        return ValueTask.CompletedTask;
    }
}

internal class ScriptedWindowHostCore(ScriptedHost host, InfiniFrameHostWindowRequest request, IInfiniFrameWindowHostEvents events) : IInfiniFrameWindowHost {
    private int _requestCloseCalls;
    private int _disposeCalls;
    private ulong _maxDocument;

    internal ScriptedHost Host => host;
    internal InfiniFrameHostWindowRequest Request => request;
    internal IInfiniFrameWindowHostEvents Events => events;
    internal ConcurrentQueue<SentMessage> Sent { get; } = new();
    internal ConcurrentQueue<Uri> Navigations { get; } = new();
    internal int RequestCloseCalls => Volatile.Read(ref _requestCloseCalls);
    internal int DisposeCalls => Volatile.Read(ref _disposeCalls);
    private bool? _autoClose;

    internal bool AutoClose {
        get => _autoClose ?? host.Provider.AutoClose;
        set => _autoClose = value;
    }
    internal bool AttachOnNavigate { get; set; } = true;
    internal ulong ProviderDocument { get; private set; }
    internal Uri? CurrentUrl { get; private set; }
    internal Action<SentMessage>? OnSent { get; set; }

    /// <summary>Replaces message delivery (for example, to keep a send in flight and observe its token).</summary>
    internal Func<InfiniFrameDocumentId, string, CancellationToken, Task>? SendOverride { get; set; }

    /// <summary>A provider that never accepts a close request (and ignores its token).</summary>
    internal bool HangRequestClose { get; set; }

    public ValueTask RequestCloseAsync(CancellationToken cancellationToken) {
        Interlocked.Increment(ref _requestCloseCalls);
        if (HangRequestClose) return new ValueTask(new TaskCompletionSource().Task);
        if (AutoClose) host.Post(events.Closed);
        return ValueTask.CompletedTask;
    }

    public async ValueTask<InfiniFrameHostNavigation> NavigateAsync(Uri url, CancellationToken cancellationToken) {
        Navigations.Enqueue(url);
        ulong document = await host.OnLoopAsync(() => ++_maxDocument).ConfigureAwait(false);
        if (AttachOnNavigate) host.Post(() => AttachOnLoop(document, url));
        return new InfiniFrameHostNavigation(new InfiniFrameDocumentId(document), false);
    }

    public ValueTask SendMessageAsync(InfiniFrameDocumentId document, string message, CancellationToken cancellationToken) =>
        SendOverride is {} sendOverride ? new ValueTask(sendOverride(document, message, cancellationToken)) : new(host.OnLoopAsync(() => {
            if (document.Value != ProviderDocument) throw new InfiniFrameHostDocumentRetiredException(document);
            var sent = new SentMessage(document.Value, message, Environment.CurrentManagedThreadId);
            Sent.Enqueue(sent);
            OnSent?.Invoke(sent);
        }));

    public ValueTask<Uri?> GetCurrentUrlAsync(CancellationToken cancellationToken) => new(host.OnLoopAsync(() => CurrentUrl));

    public ValueTask DisposeAsync() {
        Interlocked.Increment(ref _disposeCalls);
        return ValueTask.CompletedTask;
    }

    // Script helpers: each runs on the provider loop and completes after the event call returned.
    internal Task CreatedAsync() => host.OnLoopAsync(events.Created);
    internal Task AttachAsync(ulong document, string url = HostedHarness.Origin) => host.OnLoopAsync(() => AttachOnLoop(document, new Uri(url)));
    internal Task RetireAsync(ulong document) => host.OnLoopAsync(() => {
        if (ProviderDocument == document) ProviderDocument = 0;
        events.DocumentRetired(new InfiniFrameDocumentId(document));
    });
    internal Task PostAsync(ulong document, string message) => host.OnLoopAsync(() => events.MessageReceived(new InfiniFrameDocumentId(document), message));
    internal Task CloseNativelyAsync() => host.OnLoopAsync(events.Closed);
    internal Task FaultAsync(Exception exception) => host.OnLoopAsync(() => events.Faulted(exception));

    private void AttachOnLoop(ulong document, Uri url) {
        ProviderDocument = document;
        if (document > _maxDocument) _maxDocument = document;
        CurrentUrl = url;
        events.DocumentAttached(new InfiniFrameDocumentId(document), url);
    }
}

internal sealed class ScriptedWindowHost(ScriptedHost host, InfiniFrameHostWindowRequest request, IInfiniFrameWindowHostEvents events)
    : ScriptedWindowHostCore(host, request, events), IInfiniFrameWindowHostBounds, IInfiniFrameWindowHostState,
        IInfiniFrameWindowHostTitle, IInfiniFrameWindowHostIcon {
    internal Rectangle Bounds { get; private set; } = new(request.InitialLocation ?? new Point(10, 20), request.InitialSize ?? new Size(800, 600));
    internal InfiniFrameHostWindowState State { get; private set; } = request.InitialState;
    internal string Title { get; private set; } = request.Title;
    internal byte[]? Icon { get; private set; } = request.InitialIconPng.IsEmpty ? null : request.InitialIconPng.ToArray();
    internal int LastOperationThreadId { get; private set; }

    public ValueTask<Rectangle> GetBoundsAsync(CancellationToken cancellationToken) => new(Host.OnLoopAsync(() => Bounds));

    public ValueTask SetBoundsAsync(Rectangle bounds, CancellationToken cancellationToken) =>
        new(Host.OnLoopAsync(() => {
            Bounds = bounds;
            LastOperationThreadId = Environment.CurrentManagedThreadId;
            if (Host.Provider.Capabilities.HasFlag(InfiniFrameHostCapabilities.BoundsEvents)) Events.BoundsChanged(bounds);
        }));

    public ValueTask<InfiniFrameHostWindowState> GetStateAsync(CancellationToken cancellationToken) => new(Host.OnLoopAsync(() => State));

    public ValueTask SetStateAsync(InfiniFrameHostWindowStateChange change, CancellationToken cancellationToken) =>
        new(Host.OnLoopAsync(() => {
            State = change switch {
                InfiniFrameHostWindowStateChange.Minimize => (State | InfiniFrameHostWindowState.Minimized) & ~InfiniFrameHostWindowState.Maximized,
                InfiniFrameHostWindowStateChange.Maximize => (State | InfiniFrameHostWindowState.Maximized) & ~InfiniFrameHostWindowState.Minimized,
                InfiniFrameHostWindowStateChange.Restore => State & ~(InfiniFrameHostWindowState.Minimized | InfiniFrameHostWindowState.Maximized),
                InfiniFrameHostWindowStateChange.EnterFullScreen => State | InfiniFrameHostWindowState.FullScreen,
                InfiniFrameHostWindowStateChange.ExitFullScreen => State & ~InfiniFrameHostWindowState.FullScreen,
                InfiniFrameHostWindowStateChange.Focus => State | InfiniFrameHostWindowState.Focused,
                _ => throw new InfiniFrameHostOperationException($"Unsupported change {change}.")
            };
            LastOperationThreadId = Environment.CurrentManagedThreadId;
            Events.StateChanged(State);
        }));

    public ValueTask<string> GetTitleAsync(CancellationToken cancellationToken) => new(Host.OnLoopAsync(() => Title));

    /// <summary>The provider's title call awaits without ConfigureAwait(false), so it resumes on any captured context.</summary>
    internal bool CapturesContext { get; set; }

    public ValueTask SetTitleAsync(string title, CancellationToken cancellationToken) =>
        CapturesContext ? SetTitleCapturingContextAsync(title) : new(Host.OnLoopAsync(() => {
            Title = title;
        }));

    private async ValueTask SetTitleCapturingContextAsync(string title) {
        await Task.Yield();
        await Host.OnLoopAsync(() => {
            Title = title;
        });
    }

    public ValueTask SetIconAsync(ReadOnlyMemory<byte> png, CancellationToken cancellationToken) {
        byte[] copy = png.ToArray();
        return new ValueTask(Host.OnLoopAsync(() => {
            Icon = copy;
        }));
    }
}
