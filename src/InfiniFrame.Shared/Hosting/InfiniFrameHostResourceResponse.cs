// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>
///     The result of <see cref="IInfiniFrameWindowHostEvents.GetResourceAsync" />. InfiniFrame does not buffer the body:
///     <see cref="Body" /> is the handler's stream. After the call returns the provider owns the response and must dispose
///     it exactly once, after copying or streaming the body or when abandoning the request. Disposal is idempotent.
/// </summary>
public sealed class InfiniFrameHostResourceResponse : IAsyncDisposable, IDisposable {
    private int _disposed;

    /// <summary>The status to return to the browser for this request only.</summary>
    public required int StatusCode { get; init; }

    /// <summary>The validated content type, or <see langword="null" /> when there is no body.</summary>
    public string? ContentType { get; init; }

    /// <summary>The body length when it is known in advance (seekable streams), otherwise <see langword="null" />.</summary>
    public long? ContentLength { get; init; }

    /// <summary>The body, or <see langword="null" /> for error statuses and <c>HEAD</c>.</summary>
    public Stream? Body { get; init; }

    /// <inheritdoc />
    public ValueTask DisposeAsync() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0 || Body is null) return ValueTask.CompletedTask;
        return Body.DisposeAsync();
    }

    /// <inheritdoc />
    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        Body?.Dispose();
    }
}
