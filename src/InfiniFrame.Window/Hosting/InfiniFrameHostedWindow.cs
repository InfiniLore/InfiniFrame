// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using InfiniFrame.Hosting;
using InfiniFrame.Js.Interop;
using InfiniFrame.NativeBridge.Handles;
using InfiniFrame.NativeBridge.Parameters.Window;
using InfiniFrame.Security;
using InfiniFrame.Window.Builder;
using InfiniFrame.Window.Events;
using InfiniFrame.Window.Features.InstanceArbitration;
using InfiniFrame.Window.Features.JavaScript;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Window.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Core-owned <see cref="IInfiniFrameWindow" /> facade for a window supplied by an external host provider. It owns
///     the window's events, configuration, trust policy, lifecycle state machine, managed dispatch, document identity,
///     Blazor-facing delivery and resource dispatch. The provider only sees <see cref="IInfiniFrameWindowHost" /> and the
///     <see cref="IInfiniFrameWindowHostEvents" /> sink, so it never implements or mutates internal lifecycle members.
/// </summary>
internal sealed partial class InfiniFrameHostedWindow :
    IInfiniFrameWindow, IInfiniFrameWindowFeatures, ILifecycleInfiniFrameWindowFeature, IInvokeInfiniFrameWindowFeature,
    IWebMessagingInfiniFrameWindowFeature, IPageNavigationInfiniFrameWindowFeature, IJavaScriptInfiniFrameWindowFeature,
    IPositionInfiniFrameWindowFeature, ISizeInfiniFrameWindowFeature, IStateInfiniFrameWindowFeature,
    IDecorationsInfiniFrameWindowFeature, IInfiniFrameDocumentWindow, IDisposable, IAsyncDisposable {
    private static readonly Lazy<IntPtr> LazyMainProgramHandle = new(NativeLibrary.GetMainProgramHandle);
    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(30);

    private readonly InfiniFrameHostedRuntime _runtime;
    private readonly ILogger<InfiniFrameHostedWindow> _logger;
    private readonly InfiniFrameEvents _events;
    private readonly InfiniFrameWindowConfiguration _configuration;
    private readonly JavaScriptInfiniFrameWindowFeature _scripts;
    private readonly HostEventSink _sink;
    private readonly ConcurrentDictionary<ulong, TaskCompletionSource> _acknowledgements = new();
    private long _nextAcknowledgementId;
    private string? _iconPath;

    private InfiniFrameHostedWindow(
        InfiniFrameHostedRuntime runtime,
        InfiniFrameWindowBuilder builder,
        IServiceProvider services,
        InfiniFrameHostWindowRequest request,
        InfiniFrameNativeWindowParameters parameters,
        Guid id
    ) {
        _runtime = runtime;
        Id = id;
        Request = request;
        ServiceProvider = services;
        _logger = runtime.LoggerFactory.CreateLogger<InfiniFrameHostedWindow>();
        _sink = new HostEventSink(this);
        _iconPath = request.InitialIconPng.IsEmpty ? null : Path.GetFullPath(parameters.WindowIconFile!);

        _events = new InfiniFrameEvents(new InfiniFrameEventsStore(), runtime.LoggerFactory.CreateLogger<InfiniFrameEvents>());
        _events.PopulateFromBuilderEventStore(builder.EventsStore);
        _events.AssignToWindow(this);
        _events.AssignDefaultEventCallbacks();

        _configuration = new InfiniFrameWindowConfiguration();
        _configuration.AssignNativeParameters(parameters);
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(this, InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder));

        // The default window registers these through its web-messaging and JavaScript features.
        EventsStore.WebMessagePostData.Add(JsHandlerNames.WebMessageAckResponse, (_, payload) => {
            if (ulong.TryParse(payload, out ulong ackId) && _acknowledgements.TryRemove(ackId, out TaskCompletionSource? completion))
                completion.TrySetResult();
        });
        _scripts = new JavaScriptInfiniFrameWindowFeature(this, runtime.LoggerFactory.CreateLogger<JavaScriptInfiniFrameWindowFeature>());
    }

    /// <summary>
    ///     Validates and maps <paramref name="builder" />, then creates the facade. No provider call is made here: the
    ///     native window is requested later by <see cref="StartAsync" />, after the application has registered the window.
    /// </summary>
    internal static InfiniFrameHostedWindow Create(InfiniFrameHostedRuntime runtime, InfiniFrameWindowBuilder builder, IServiceProvider services) {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(services);
        var id = Guid.NewGuid();
        InfiniFrameHostWindowRequest request = InfiniFrameHostedWindowRequestFactory.Create(
            builder, id, runtime.Capabilities, runtime.DisplayName, out InfiniFrameNativeWindowParameters parameters);

        IInstanceArbitrationInfiniFrameWindowBuilderFeature arbitration = builder.Features.InstanceArbitration;
        if (arbitration.Mode != InstanceArbitrationMode.Disabled && !InstanceArbitration.TryAcquirePrimaryInstance(arbitration.MutexName))
            throw new InstanceAlreadyRunningException();

        return new InfiniFrameHostedWindow(runtime, builder, services, request, parameters, id);
    }

    /// <summary>The public request sent to the provider (for diagnostics and tests).</summary>
    internal InfiniFrameHostWindowRequest Request { get; }

    /// <summary>The event sink handed to the provider.</summary>
    internal IInfiniFrameWindowHostEvents HostEvents => _sink;

    // -----------------------------------------------------------------------------------------------------------------
    // IInfiniFrameWindow
    // -----------------------------------------------------------------------------------------------------------------
    public Guid Id { get; }
    public IServiceProvider ServiceProvider { get; }
    public IInfiniFrameEvents Events => _events;
    public IInfiniFrameEventsStore EventsStore => _events.EventsStore;
    public IInfiniFrameWindowConfiguration Configuration => _configuration;
    public IInfiniFrameWindowFeatures Features => this;
    public IDebuggingInfiniFrameWindowFeature Debugging => throw Unsupported("Debugging");
    public IntPtr MainProgramHandle => LazyMainProgramHandle.Value;
    public IntPtr WindowHandle => throw Unsupported("In-process native window handles");
    public int ManagedThreadId => _runtime.Dispatcher.ThreadId;

    public NativeHandleLease AcquireNativeHandle(NativeHandleAccess access = NativeHandleAccess.Feature) =>
        throw Unsupported("Native handle leases");

    // -----------------------------------------------------------------------------------------------------------------
    // IInfiniFrameWindowFeatures
    // -----------------------------------------------------------------------------------------------------------------
    IDebuggingInfiniFrameWindowFeature IInfiniFrameWindowFeatures.Debugging => throw Unsupported("Debugging");
    ILifecycleInfiniFrameWindowFeature IInfiniFrameWindowFeatures.Lifecycle => this;
    IInvokeInfiniFrameWindowFeature IInfiniFrameWindowFeatures.Invoke => this;
    IWebMessagingInfiniFrameWindowFeature IInfiniFrameWindowFeatures.WebMessaging => this;
    INotificationsInfiniFrameWindowFeature IInfiniFrameWindowFeatures.Notifications => throw Unsupported("Notifications and message dialogs");
    IFilePickerDialogsInfiniFrameWindowFeature IInfiniFrameWindowFeatures.FilePickerDialogs => throw Unsupported("File picker dialogs");
    IMonitorsInfiniFrameWindowFeature IInfiniFrameWindowFeatures.Monitors => throw Unsupported("Monitor enumeration");
    IPageNavigationInfiniFrameWindowFeature IInfiniFrameWindowFeatures.PageNavigation => this;
    IPositionInfiniFrameWindowFeature IInfiniFrameWindowFeatures.Position => this;
    ISizeInfiniFrameWindowFeature IInfiniFrameWindowFeatures.Size => this;
    IDecorationsInfiniFrameWindowFeature IInfiniFrameWindowFeatures.Decorations => this;
    IStateInfiniFrameWindowFeature IInfiniFrameWindowFeatures.State => this;
    IBrowserInfiniFrameWindowFeature IInfiniFrameWindowFeatures.Browser => throw Unsupported("Runtime browser settings");
    IDragDropInfiniFrameWindowFeature IInfiniFrameWindowFeatures.DragDrop => throw Unsupported("Drag and drop");
    ITaskbarInfiniFrameWindowFeature IInfiniFrameWindowFeatures.Taskbar => throw Unsupported("Taskbar integration");
    IMenuInfiniFrameWindowFeature IInfiniFrameWindowFeatures.Menu => throw Unsupported("Native menus");
    IJavaScriptInfiniFrameWindowFeature IInfiniFrameWindowFeatures.JavaScript => this;

    // -----------------------------------------------------------------------------------------------------------------
    // Invoke: application callbacks always run on the core dispatcher, never on a provider thread.
    // -----------------------------------------------------------------------------------------------------------------
    public InfiniFrameDispatchResult Invoke(Action callback) {
        ArgumentNullException.ThrowIfNull(callback);
        if (IsClosedOrClosing()) return InfiniFrameDispatchResult.WindowClosed;
        InfiniFrameDispatchResult result = _runtime.Dispatcher.Invoke(callback, CanExecuteApplicationCallback);
        return result == InfiniFrameDispatchResult.Failed && IsClosedOrClosing() ? InfiniFrameDispatchResult.WindowClosed : result;
    }

    public async ValueTask<InfiniFrameDispatchResult> DispatchAsync(Action callback, TimeSpan? timeout = null, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(callback);
        if (IsClosedOrClosing()) return InfiniFrameDispatchResult.WindowClosed;
        InfiniFrameDispatchResult result = await _runtime.Dispatcher.DispatchAsync(callback, timeout, cancellationToken, CanExecuteApplicationCallback).ConfigureAwait(false);
        return result == InfiniFrameDispatchResult.Failed && IsClosedOrClosing() ? InfiniFrameDispatchResult.WindowClosed : result;
    }

    private bool CanExecuteApplicationCallback() => Volatile.Read(ref _terminal) == 0 && !IsClosedOrClosing();

    // -----------------------------------------------------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------------------------------------------------
    private NotSupportedException Unsupported(string feature) =>
        new($"{feature} is not available for windows supplied by the external host provider '{_runtime.DisplayName}'. " +
            "No default-engine fallback is performed.");

    private void RequireCapability(InfiniFrameHostCapabilities capability, string operation) {
        if (!_runtime.Capabilities.HasFlag(capability))
            throw new NotSupportedException(
                $"{operation} needs the host capability {capability}, which the provider '{_runtime.DisplayName}' does not declare.");
    }

    private static TaskCompletionSource NewSignal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>
    ///     Blocks on a provider operation for a synchronous facade member. Providers must complete these tasks without the
    ///     core dispatcher, so this is safe on the dispatcher thread; the operation starts with no ambient synchronization
    ///     context (for example Blazor's), and the wait is bounded so a stuck provider cannot hang the caller.
    /// </summary>
    private T WaitForHost<T>(Func<CancellationToken, ValueTask<T>> operation, string name) {
        using var timeout = new CancellationTokenSource(OperationTimeout);
        try {
            return InfiniFrameProviderCall.Start(() => operation(timeout.Token)).AsTask().WaitAsync(OperationTimeout).GetAwaiter().GetResult();
        }
        catch (Exception exception) when (exception is TimeoutException || exception is OperationCanceledException && timeout.IsCancellationRequested) {
            throw new InfiniFrameHostOperationException($"The host provider did not complete '{name}' within {OperationTimeout}.", exception);
        }
    }

    private void WaitForHost(Func<CancellationToken, ValueTask> operation, string name) =>
        WaitForHost(async token => {
            await operation(token).ConfigureAwait(false);
            return true;
        }, name);

    private IInfiniFrameWindowHost RequireLiveHost() {
        ObjectDisposedException.ThrowIf(LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed, this);
        Task<IInfiniFrameWindowHost?> host = _windowHost.Task;
        if (!host.IsCompletedSuccessfully || host.Result is null)
            throw new InvalidOperationException("The hosted window has not been created by its provider yet.");
        return host.Result;
    }

    private TCapability RequireHostCapability<TCapability>(InfiniFrameHostCapabilities capability, string operation) where TCapability : class {
        RequireCapability(capability, operation);
        return RequireLiveHost() as TCapability
            ?? throw new InvalidOperationException($"The provider declared {capability} but its window host does not implement {typeof(TCapability).Name}.");
    }
}
