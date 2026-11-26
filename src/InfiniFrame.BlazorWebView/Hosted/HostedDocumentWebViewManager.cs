// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame.BlazorWebView.Hosted;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     The framework manager of exactly one browser document of a hosted window (external host provider). Everything its
///     page contexts, JS runtimes and navigation managers send is bound to that document for the manager's whole life,
///     and only that document's messages reach it. It is retired and disposed once, when the document's lifetime ends;
///     afterwards its output is refused, never redirected to another document.
/// </summary>
internal sealed class HostedDocumentWebViewManager : WebViewManager {
    private readonly HostedDocumentManagers _owner;
    private readonly HashSet<string> _rootSelectors = new(StringComparer.Ordinal);// Blazor dispatcher only
    private readonly object _registrationGate = new();
    private CancellationTokenRegistration _lifetimeRegistration;
    private bool _registrationReleased;
    private int _retired;
    private long _refusedOutbound;

    internal HostedDocumentWebViewManager(
        HostedDocumentManagers owner,
        ulong document,
        IServiceProvider provider,
        Dispatcher dispatcher,
        Uri appBaseUri,
        IFileProvider fileProvider,
        JSComponentConfigurationStore jsComponents,
        string hostPage
    ) : base(provider, dispatcher, appBaseUri, fileProvider, jsComponents, hostPage) {
        _owner = owner;
        Document = document;
    }

    /// <summary>The browser document this manager belongs to (provider document id).</summary>
    internal ulong Document { get; }

    internal HostedDocumentManagers Owner => _owner;

    internal bool IsRetired => Volatile.Read(ref _retired) != 0;

    /// <summary>Closes the manager's gates; true only for the first caller, which then owns its disposal.</summary>
    internal bool TryMarkRetired() => Interlocked.Exchange(ref _retired, 1) == 0;

    /// <summary>Counts one refused outbound message and returns the running count for this manager.</summary>
    internal long CountRefusedOutbound() => Interlocked.Increment(ref _refusedOutbound);

    /// <summary>Keeps the registration on the document lifetime, or unregisters it at once if already released.</summary>
    internal void SetLifetimeRegistration(CancellationTokenRegistration registration) {
        lock (_registrationGate) {
            if (!_registrationReleased) {
                _lifetimeRegistration = registration;
                return;
            }
        }
        registration.Unregister();
    }

    /// <summary>Unregisters from the document lifetime (never waits for a running callback).</summary>
    internal void ReleaseLifetimeRegistration() {
        CancellationTokenRegistration registration;
        lock (_registrationGate) {
            if (_registrationReleased) return;
            _registrationReleased = true;
            registration = _lifetimeRegistration;
            _lifetimeRegistration = default;
        }
        registration.Unregister();
    }

    /// <summary>Adds a root component unless this manager already has one for the selector (Blazor dispatcher).</summary>
    internal Task AddRootComponentOnceAsync(Type componentType, string selector, ParameterView parameters) =>
        _rootSelectors.Add(selector) ? AddRootComponentAsync(componentType, selector, parameters) : Task.CompletedTask;

    /// <summary>Removes a root component if this manager has one for the selector (Blazor dispatcher).</summary>
    internal Task RemoveRootComponentIfAddedAsync(string selector) =>
        _rootSelectors.Remove(selector) ? RemoveRootComponentAsync(selector) : Task.CompletedTask;

    /// <summary>Delivers one message from this manager's own document (Blazor dispatcher).</summary>
    internal void Deliver(Uri origin, string message) => MessageReceived(origin, message);

    protected override void SendMessage(string message) => _owner.SendFromDocument(this, message);

    protected override void NavigateCore(Uri absoluteUri) => _owner.NavigateFromDocument(this, absoluteUri);
}
