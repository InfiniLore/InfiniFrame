// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Globalization;
using System.Text;
using System.Text.Json;
using InfiniFrame.Hosting;
using InfiniFrame.Js.Interop;
using InfiniFrame.Security;
using InfiniFrame.Utilities;
using InfiniFrame.Window.Features.WebMessaging;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Window.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <remarks>
///     Documents: ids are provider-assigned, non-zero and strictly increasing per window. Each document has a lifetime
///     token that is cancelled when it retires, so work bound to it (sends, acknowledgements, script evaluations,
///     queued inbound callbacks) is cancelled rather than redirected. Outbound work is bound when it is produced: to the
///     document whose inbound callback produced it (flowing with the execution context), otherwise to the most
///     recently attached document, otherwise (before any document) to the first document that attaches.
/// </remarks>
internal sealed partial class InfiniFrameHostedWindow {
    private static readonly TimeSpan NavigationTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan FirstDocumentTimeout = TimeSpan.FromSeconds(30);

    private readonly object _documentGate = new();
    private readonly AsyncLocal<ulong?> _boundDocument = new();
    private readonly SemaphoreSlim _sendGate = new(1, 1);
    private readonly SortedDictionary<ulong, TaskCompletionSource<NavigationStatus>> _navigationWaiters = new();
    private readonly TaskCompletionSource<ulong> _firstDocument = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private CancellationTokenSource _documentLifetime = new();
    private ulong _currentDocument;
    private ulong _lastAttachedDocument;
    private Uri? _currentDocumentUrl;
    private bool _documentsEnded;
    private long _nextNavigationId;

    public ulong CurrentDocument {
        get {
            lock (_documentGate) return _currentDocument;
        }
    }

    public ulong? IncomingMessageDocument => _boundDocument.Value;

    private bool TryGetDocumentLifetime(ulong document, out CancellationToken lifetime) {
        lock (_documentGate) {
            if (document != 0 && document == _currentDocument && !_documentsEnded) {
                lifetime = _documentLifetime.Token;
                return true;
            }
        }
        lifetime = CancellationToken.None;
        return false;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Provider notifications
    // -----------------------------------------------------------------------------------------------------------------
    private void OnHostDocumentAttached(InfiniFrameDocumentId document, Uri? url) {
        if (Volatile.Read(ref _terminal) != 0) {
            _logger.LogDebug("Ignored DocumentAttached({Document}) after the window ended. Window: {WindowId}", document.Value, Id);
            return;
        }
        if (url is null || !url.IsAbsoluteUri) {
            FaultWindow(new InvalidOperationException("DocumentAttached requires the absolute browser-reported document URL."));
            return;
        }

        CancellationTokenSource? previous = null;
        ulong lastAttached;
        var completed = new List<(ulong Document, TaskCompletionSource<NavigationStatus> Waiter)>();
        lock (_documentGate) {
            if (_documentsEnded) return;// The window ended concurrently; never bring a document back.
            lastAttached = _lastAttachedDocument;
            if (document.Value != 0 && document.Value > _lastAttachedDocument) {
                previous = _documentLifetime;
                _documentLifetime = new CancellationTokenSource();
                _currentDocument = document.Value;
                _lastAttachedDocument = document.Value;
                _currentDocumentUrl = url;
                foreach (KeyValuePair<ulong, TaskCompletionSource<NavigationStatus>> waiter in _navigationWaiters) {
                    if (waiter.Key > document.Value) break;
                    completed.Add((waiter.Key, waiter.Value));
                }
                foreach ((ulong waiting, _) in completed) _navigationWaiters.Remove(waiting);
            }
        }
        if (previous is null) {
            FaultWindow(new InvalidOperationException(string.Create(CultureInfo.InvariantCulture,
                $"Document ids must be non-zero and strictly increasing per window (got {document.Value} after {lastAttached}).")));
            return;
        }
        // Retire the previous document outside the lock and off this provider thread: cancellation continuations
        // (pending sends, the Blazor pump, application awaits) must not run inside the provider's DocumentAttached call.
        InfiniFrameProviderCall.CancelInBackground(previous, _logger);
        _firstDocument.TrySetResult(document.Value);
        foreach ((ulong waiting, TaskCompletionSource<NavigationStatus> waiter) in completed)
            waiter.TrySetResult(waiting == document.Value ? NavigationStatus.Succeeded : NavigationStatus.Superseded);
        _logger.LogDebug("Document {Document} attached. Window: {WindowId}", document.Value, Id);
    }

    private void OnHostDocumentRetired(InfiniFrameDocumentId document) {
        CancellationTokenSource retired;
        lock (_documentGate) {
            if (document.Value == 0 || document.Value != _currentDocument) {
                _logger.LogDebug("Ignored retirement of non-current document {Document}. Window: {WindowId}", document.Value, Id);
                return;
            }
            retired = _documentLifetime;
            _documentLifetime = new CancellationTokenSource();
            _currentDocument = 0;
            _currentDocumentUrl = null;
        }
        InfiniFrameProviderCall.CancelInBackground(retired, _logger);
        _logger.LogDebug("Document {Document} retired. Window: {WindowId}", document.Value, Id);
    }

    /// <summary>Ends every document on the terminal transition.</summary>
    private void RetireDocuments(bool windowClosing) {
        CancellationTokenSource lifetime;
        TaskCompletionSource<NavigationStatus>[] waiters;
        lock (_documentGate) {
            _documentsEnded = windowClosing;
            lifetime = _documentLifetime;
            _currentDocument = 0;
            _currentDocumentUrl = null;
            waiters = [.. _navigationWaiters.Values];
            _navigationWaiters.Clear();
        }
        InfiniFrameProviderCall.CancelInBackground(lifetime, _logger);
        if (_firstDocument.TrySetException(new ObjectDisposedException(nameof(InfiniFrameHostedWindow), "The window ended before a document attached.")))
            _ = _firstDocument.Task.Exception;
        foreach (TaskCompletionSource<NavigationStatus> waiter in waiters) waiter.TrySetResult(NavigationStatus.WindowClosed);
    }

    private void OnHostMessageReceived(InfiniFrameDocumentId document, string? message) {
        if (message is null) {
            _logger.LogWarning("Ignored a null message from host provider {Provider}. Window: {WindowId}", _runtime.DisplayName, Id);
            return;
        }
        if (Volatile.Read(ref _terminal) != 0) {
            _logger.LogDebug("Ignored a message after the window ended. Window: {WindowId}", Id);
            return;
        }
        Uri? url;
        lock (_documentGate) {
            if (document.Value == 0 || document.Value != _currentDocument) {
                _logger.LogWarning(
                    "Rejected a message from document {Document}, which is not the current document {Current}. Window: {WindowId}",
                    document.Value, _currentDocument, Id);
                return;
            }
            url = _currentDocumentUrl;
        }
        // The origin is derived from the browser-reported document URL, never from page-supplied data.
        string origin = url!.AbsoluteUri;
        ulong source = document.Value;
        // Never throw into the provider's thread: a message too large to count exhausts the budget instead.
        int bytes = message.Length > int.MaxValue / sizeof(char) ? int.MaxValue : message.Length * sizeof(char);
        if (!_runtime.Dispatcher.TryPost(() => DeliverMessage(source, message, origin), bytes))
            FaultWindow(new InvalidOperationException(
                "The hosted dispatcher budget is exhausted; the window cannot continue without dropping inbound messages."));
    }

    private void DeliverMessage(ulong document, string message, string origin) {
        if (!TryGetDocumentLifetime(document, out _)) {
            _logger.LogDebug("Cancelled a queued callback for retired document {Document}. Window: {WindowId}", document, Id);
            return;
        }
        ulong? previous = _boundDocument.Value;
        _boundDocument.Value = document;
        try {
            _events.OnWebMessageReceived(message, origin);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogError(exception, "A web message handler failed. Window: {WindowId}", Id);
        }
        finally {
            _boundDocument.Value = previous;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Outbound messaging
    // -----------------------------------------------------------------------------------------------------------------
    private async Task<ulong> BindDocumentAsync(CancellationToken cancellationToken) {
        ulong? bound = _boundDocument.Value;
        if (bound is not null) return bound.Value;
        lock (_documentGate) {
            if (_lastAttachedDocument != 0) return _lastAttachedDocument;
        }
        // Work produced before any document existed belongs to the first document, never to a later one.
        return await _firstDocument.Task.WaitAsync(FirstDocumentTimeout, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Fire-and-forget, as on default windows: never throws because a document retired or none attached yet, and never
    ///     waits for a document. It blocks only while the provider accepts the message (bounded), which keeps sequential
    ///     calls in order; providers complete sends without the core dispatcher, so this cannot deadlock it.
    /// </summary>
    public void SendWebMessage(string message) {
        ArgumentNullException.ThrowIfNull(message);
        if (IsClosedOrClosing()) return;
        ulong document = _boundDocument.Value ?? LastAttachedDocument;
        if (document == 0) {
            _logger.LogWarning("Dropped a message sent before any document attached (use SendWebMessageAsync to wait for one). Window: {WindowId}", Id);
            return;
        }
        try {
            InfiniFrameProviderCall.Start(() => SendWebMessageForDocumentAsync(message, document, CancellationToken.None)).AsTask()
                .WaitAsync(OperationTimeout).GetAwaiter().GetResult();
        }
        catch (InfiniFrameHostDocumentRetiredException exception) {
            _logger.LogDebug("Dropped a message for retired document {Document}. Window: {WindowId}", exception.Document.Value, Id);
        }
        catch (TimeoutException) {
            _logger.LogWarning("The host provider did not accept a message within {Timeout}; delivery is unconfirmed. Window: {WindowId}", OperationTimeout, Id);
        }
    }

    private ulong LastAttachedDocument {
        get {
            lock (_documentGate) return _lastAttachedDocument;
        }
    }

    public ValueTask SendWebMessageAsync(string message, CancellationToken ct = default) {
        ArgumentNullException.ThrowIfNull(message);
        if (ct.IsCancellationRequested) return ValueTask.FromCanceled(ct);
        if (IsClosedOrClosing()) return ValueTask.CompletedTask;
        return new ValueTask(SendBoundAsync(message, ct));
    }

    private async Task SendBoundAsync(string message, CancellationToken cancellationToken) {
        ulong document = await BindDocumentAsync(cancellationToken).ConfigureAwait(false);
        await SendWebMessageForDocumentAsync(message, document, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask SendWebMessageForDocumentAsync(string message, ulong document, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(message);
        cancellationToken.ThrowIfCancellationRequested();
        IInfiniFrameWindowHost? host = await _windowHost.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
        if (host is null || !TryGetDocumentLifetime(document, out CancellationToken lifetime))
            throw new InfiniFrameHostDocumentRetiredException(new InfiniFrameDocumentId(document));
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, lifetime);
        bool entered = false;
        try {
            await _sendGate.WaitAsync(linked.Token).ConfigureAwait(false);
            entered = true;
            if (!TryGetDocumentLifetime(document, out _))
                throw new InfiniFrameHostDocumentRetiredException(new InfiniFrameDocumentId(document));
            await InfiniFrameProviderCall.Start(() => host.SendMessageAsync(new InfiniFrameDocumentId(document), message, linked.Token))
                .ConfigureAwait(false);
            await InfiniFrameProviderCall.ToThreadPool();
        }
        catch (InfiniFrameHostDocumentRetiredException) {
            throw;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            throw;
        }
        catch (OperationCanceledException) when (lifetime.IsCancellationRequested) {
            throw new InfiniFrameHostDocumentRetiredException(new InfiniFrameDocumentId(document));
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            var failure = new InfiniFrameHostOperationException("The host provider failed to deliver a message to its document.", exception);
            FaultWindow(failure);
            throw failure;
        }
        finally {
            if (entered) _sendGate.Release();
        }
    }

    public async Task SendWebMessageWithAcknowledgementAsync(string message, CancellationToken ct = default) {
        ArgumentNullException.ThrowIfNull(message);
        ct.ThrowIfCancellationRequested();
        ObjectDisposedException.ThrowIf(IsClosedOrClosing(), this);
        ulong document = await BindDocumentAsync(ct).ConfigureAwait(false);
        if (!TryGetDocumentLifetime(document, out CancellationToken lifetime))
            throw new InfiniFrameHostDocumentRetiredException(new InfiniFrameDocumentId(document));
        ulong id = unchecked((ulong)Interlocked.Increment(ref _nextAcknowledgementId));
        var completion = NewSignal();
        if (!_acknowledgements.TryAdd(id, completion))
            throw new InvalidOperationException("Could not register the web-message acknowledgement.");
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, lifetime);
        try {
            string envelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.WebMessageAckRequest, CreateAcknowledgementPayload(id, message));
            await SendWebMessageForDocumentAsync(envelope, document, ct).ConfigureAwait(false);
            await completion.Task.WaitAsync(linked.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested && lifetime.IsCancellationRequested) {
            if (Volatile.Read(ref _terminal) != 0)
                throw new ObjectDisposedException(nameof(InfiniFrameHostedWindow), "The window closed before JavaScript acknowledged the message.");
            throw new InfiniFrameHostDocumentRetiredException(new InfiniFrameDocumentId(document));
        }
        finally {
            _acknowledgements.TryRemove(id, out _);
        }
    }

    private static string CreateAcknowledgementPayload(ulong id, string message) {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream)) {
            writer.WriteStartObject();
            writer.WriteString("OperationId", id.ToString(CultureInfo.InvariantCulture));
            writer.WriteString("Message", message);
            writer.WriteEndObject();
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    // -----------------------------------------------------------------------------------------------------------------
    // JavaScript (the default feature is composed unchanged, so its result contract is preserved)
    // -----------------------------------------------------------------------------------------------------------------
    public ValueTask<string?> ExecuteJavaScriptAsync(string script, CancellationToken ct = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(script);
        if (ct.IsCancellationRequested) return ValueTask.FromCanceled<string?>(ct);
        if (IsClosedOrClosing()) return ValueTask.FromException<string?>(new ObjectDisposedException(nameof(InfiniFrameHostedWindow)));
        return new ValueTask<string?>(ExecuteBoundAsync(script, ct));
    }

    private async Task<string?> ExecuteBoundAsync(string script, CancellationToken cancellationToken) {
        ulong document = await BindDocumentAsync(cancellationToken).ConfigureAwait(false);
        if (!TryGetDocumentLifetime(document, out CancellationToken lifetime))
            throw new InfiniFrameHostDocumentRetiredException(new InfiniFrameDocumentId(document));
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, lifetime);
        // Bind the evaluation request (sent by the composed feature through this window) to the same document.
        _boundDocument.Value = document;
        try {
            return await _scripts.ExecuteJavaScriptAsync(script, linked.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && lifetime.IsCancellationRequested) {
            if (Volatile.Read(ref _terminal) != 0)
                throw new ObjectDisposedException(nameof(InfiniFrameHostedWindow), "The window closed before JavaScript evaluation completed.");
            throw new InfiniFrameHostDocumentRetiredException(new InfiniFrameDocumentId(document));
        }
    }

    public async ValueTask<T?> ExecuteJavaScriptAsync<T>(string script, CancellationToken ct = default) {
        string? json = await ExecuteJavaScriptAsync(script, ct).ConfigureAwait(false);
        if (json is null) return default;
        return JsonSerializer.Deserialize(json, WindowFeatureWebMessageJsonContext.Default.GetTypeInfo(typeof(T))!) is T result
            ? result
            : default;
    }

    public void SendEvalToBrowser(string script, string? requestId = null) => _scripts.SendEvalToBrowser(script, requestId);

    // -----------------------------------------------------------------------------------------------------------------
    // Navigation
    // -----------------------------------------------------------------------------------------------------------------
    public void Load(Uri uri) => TryLoadUri(uri);
    public void Load(string path) => TryLoadPath(path);

    public bool TryLoadUri(Uri uri) {
        ArgumentNullException.ThrowIfNull(uri);
        if (!IsTrustedNavigation(uri)) {
            _logger.LogWarning("Uri {Uri} is not trusted", uri);
            return false;
        }
        if (IsClosedOrClosing()) return false;
        _ = ObserveNavigationAsync(NavigateCoreAsync(uri, CancellationToken.None));
        return true;
    }

    public bool TryLoadPath(string path) {
        ArgumentNullException.ThrowIfNull(path);
        return TryLoadUri(ResolveNavigationPath(path));
    }

    public Task<NavigationResult> LoadAsync(Uri uri, CancellationToken ct = default) {
        ArgumentNullException.ThrowIfNull(uri);
        return IsTrustedNavigation(uri)
            ? NavigateCoreAsync(uri, ct)
            : Task.FromResult(new NavigationResult(0, NavigationStatus.Failed, uri, FailureReason: "URI origin is not trusted."));
    }

    public Task<NavigationResult> LoadAsync(string path, CancellationToken ct = default) {
        ArgumentNullException.ThrowIfNull(path);
        return LoadAsync(ResolveNavigationPath(path), ct);
    }

    public void LoadRawString(string content) => throw Unsupported("Raw HTML navigation (serve content through a custom scheme)");
    public Task<NavigationResult> LoadRawStringAsync(string content, CancellationToken ct = default) =>
        throw Unsupported("Raw HTML navigation (serve content through a custom scheme)");

    public string? GetCurrentUrl() {
        if (IsClosedOrClosing()) return null;
        Task<IInfiniFrameWindowHost?> pending = _windowHost.Task;
        if (!pending.IsCompletedSuccessfully || pending.Result is not {} host) return null;
        return WaitForHost(host.GetCurrentUrlAsync, "GetCurrentUrl")?.AbsoluteUri;
    }

    public Uri? GetCurrentUri() =>
        GetCurrentUrl() is {} url && Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) ? uri : null;

    private Uri ResolveNavigationPath(string path) =>
        Uri.TryCreate(path, UriKind.Absolute, out Uri? absolute) ? absolute : new Uri(Request.StartUrl, path);

    private bool IsTrustedNavigation(Uri uri) {
        if (uri.IsFile) return true;
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForWindow(this);
        return policy.TrustAllOrigins || policy.IsTrustedOrigin(uri);
    }

    private async Task<NavigationResult> NavigateCoreAsync(Uri uri, CancellationToken cancellationToken) {
        ulong operation = unchecked((ulong)Interlocked.Increment(ref _nextNavigationId));
        if (IsClosedOrClosing()) return new NavigationResult(operation, NavigationStatus.WindowClosed, uri);
        IInfiniFrameWindowHost? host = await _windowHost.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
        if (host is null) return new NavigationResult(operation, NavigationStatus.WindowClosed, uri);

        InfiniFrameHostNavigation navigation;
        try {
            navigation = await InfiniFrameProviderCall.Start(() => host.NavigateAsync(uri, cancellationToken)).ConfigureAwait(false);
            await InfiniFrameProviderCall.ToThreadPool();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            throw;
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            return new NavigationResult(operation, NavigationStatus.Failed, uri, FailureReason: exception.Message);
        }
        if (navigation.Document.Value == 0)
            return new NavigationResult(operation, NavigationStatus.Failed, uri, FailureReason: "The provider reported no document.");
        if (navigation.SameDocument) {
            NavigationStatus same = navigation.Document.Value == CurrentDocument ? NavigationStatus.Succeeded : NavigationStatus.Superseded;
            return new NavigationResult(operation, same, uri);
        }

        NavigationStatus status = await WaitForDocumentAsync(navigation.Document.Value, cancellationToken).ConfigureAwait(false);
        return status == NavigationStatus.Failed
            ? new NavigationResult(operation, status, uri, FailureReason: $"Document {navigation.Document.Value} did not attach within {NavigationTimeout}.")
            : new NavigationResult(operation, status, uri);
    }

    private async Task<NavigationStatus> WaitForDocumentAsync(ulong document, CancellationToken cancellationToken) {
        TaskCompletionSource<NavigationStatus> waiter;
        lock (_documentGate) {
            if (_documentsEnded) return NavigationStatus.WindowClosed;
            if (_lastAttachedDocument >= document)
                return _currentDocument == document ? NavigationStatus.Succeeded : NavigationStatus.Superseded;
            if (!_navigationWaiters.TryGetValue(document, out TaskCompletionSource<NavigationStatus>? existing)) {
                existing = new TaskCompletionSource<NavigationStatus>(TaskCreationOptions.RunContinuationsAsynchronously);
                _navigationWaiters.Add(document, existing);
            }
            waiter = existing;
        }
        try {
            return await waiter.Task.WaitAsync(NavigationTimeout, cancellationToken).ConfigureAwait(false);
        }
        catch (TimeoutException) {
            lock (_documentGate) {
                if (_navigationWaiters.TryGetValue(document, out TaskCompletionSource<NavigationStatus>? current) && ReferenceEquals(current, waiter))
                    _navigationWaiters.Remove(document);
            }
            return NavigationStatus.Failed;
        }
    }

    private async Task ObserveNavigationAsync(Task<NavigationResult> navigation) {
        try {
            NavigationResult result = await navigation.ConfigureAwait(false);
            if (result.Status != NavigationStatus.Succeeded)
                _logger.LogWarning("Navigation to {Uri} ended with {Status}: {Reason}", result.Uri, result.Status, result.FailureReason);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogWarning(exception, "Navigation failed. Window: {WindowId}", Id);
        }
    }
}
