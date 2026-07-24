// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;

namespace InfiniTests.InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     An in-process server used by lifecycle tests that do not need a network transport.
/// </summary>
internal sealed class NoopServer : IServer {
    private int _disposeCount;
    private int _startCount;
    private int _stopCount;

    public IFeatureCollection Features { get; } = new FeatureCollection();
    public int StartCount => _startCount;
    public int StopCount => _stopCount;
    public int DisposeCount => _disposeCount;

    public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        where TContext : notnull
    {
        Interlocked.Increment(ref _startCount);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        Interlocked.Increment(ref _stopCount);
        return Task.CompletedTask;
    }

    public void Dispose() => Interlocked.Increment(ref _disposeCount);
}
