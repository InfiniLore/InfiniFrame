// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text;
using InfiniFrame;
using InfiniFrame.Hosting;
using InfiniFrame.Window.Hosting;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// Resources: status fidelity at the core boundary, and asset-error isolation.
[NotInParallel]
public sealed class HostedResourceTests {
    private static InfiniFrameHostResourceRequest Get(string url, string method = "GET") => new() { Method = method, Url = new Uri(url) };

    private static Task<(InfiniFrameHostedWindow Window, ScriptedWindowHostCore Host)> CreateWithHandlerAsync(
        HostedHarness harness, Func<IInfiniFrameWindow, string, (Stream? Data, string? ContentType)> handler) =>
        harness.CreateAttachedAsync(1, builder => builder.RegisterCustomSchemeHandler("probe", handler));

    [Test]
    [Timeout(30_000)]
    public async Task Success_PassesTheHandlerStreamThroughUnbuffered_AndTheProviderOwnsIt(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        var body = new TrackingStream("hello"u8.ToArray());
        string? seenUrl = null;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await CreateWithHandlerAsync(harness, (_, url) => {
            seenUrl = url;
            return (body, "text/plain; charset=utf-8");
        });

        InfiniFrameHostResourceResponse response = await host.Events.GetResourceAsync(Get("probe://localhost/a.txt?x=1"), ct);
        await Assert.That(response.StatusCode).IsEqualTo(200);
        await Assert.That(response.ContentType).IsEqualTo("text/plain; charset=utf-8");
        await Assert.That(response.ContentLength).IsEqualTo(5L);
        await Assert.That(ReferenceEquals(response.Body, body)).IsTrue();
        await Assert.That(body.Disposed).IsFalse();
        await Assert.That(seenUrl).IsEqualTo("probe://localhost/a.txt?x=1");
        await Assert.That(host.Request.CustomSchemes.Contains("probe")).IsTrue();

        await response.DisposeAsync();
        await response.DisposeAsync();
        await Assert.That(body.DisposeCount).IsEqualTo(1);
    }

    [Test]
    [Timeout(30_000)]
    public async Task Head_ReturnsMetadataWithoutABody_AndDisposesTheStream(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        var body = new TrackingStream(new byte[42]);
        (_, ScriptedWindowHostCore host) = await CreateWithHandlerAsync(harness, (_, _) => (body, null));

        InfiniFrameHostResourceResponse response = await host.Events.GetResourceAsync(Get("probe://localhost/blob", "HEAD"), ct);
        await Assert.That(response.StatusCode).IsEqualTo(200);
        await Assert.That(response.ContentType).IsEqualTo("application/octet-stream");
        await Assert.That(response.ContentLength).IsEqualTo(42L);
        await Assert.That(response.Body).IsNull();
        await Assert.That(body.Disposed).IsTrue();
    }

    [Test]
    [Timeout(30_000)]
    public async Task MissingAsset_UnknownScheme_AndUnsupportedMethod_MapToDistinctStatuses(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        int handlerCalls = 0;
        (_, ScriptedWindowHostCore host) = await CreateWithHandlerAsync(harness, (_, _) => {
            Interlocked.Increment(ref handlerCalls);
            return (null, null);
        });

        await Assert.That((await host.Events.GetResourceAsync(Get("probe://localhost/missing"), ct)).StatusCode).IsEqualTo(404);
        await Assert.That((await host.Events.GetResourceAsync(Get("other://localhost/x"), ct)).StatusCode).IsEqualTo(404);
        await Assert.That((await host.Events.GetResourceAsync(Get("probe://localhost/x", "POST"), ct)).StatusCode).IsEqualTo(405);
        await Assert.That(handlerCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(30_000)]
    public async Task HandlerFailureAndInvalidContentType_Are500_AndTheWindowStaysHealthy(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        var badType = new TrackingStream([1]);
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await CreateWithHandlerAsync(harness, (_, url) => url switch {
            "probe://localhost/throws" => throw new IOException("disk gone"),
            "probe://localhost/bad-type" => (badType, "text/plain\r\nX-Injected: 1"),
            _ => (new MemoryStream("ok"u8.ToArray()), "text/plain")
        });

        await Assert.That((await host.Events.GetResourceAsync(Get("probe://localhost/throws"), ct)).StatusCode).IsEqualTo(500);
        await Assert.That((await host.Events.GetResourceAsync(Get("probe://localhost/bad-type"), ct)).StatusCode).IsEqualTo(500);
        await Assert.That(badType.Disposed).IsTrue();
        await using InfiniFrameHostResourceResponse healthy = await host.Events.GetResourceAsync(Get("probe://localhost/fine"), ct);
        await Assert.That(healthy.StatusCode).IsEqualTo(200);
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
        await Assert.That(window.Fault).IsNull();
        await Assert.That(harness.Host.Completion.IsCompleted).IsFalse();
    }

    [Test]
    [Timeout(30_000)]
    public async Task Cancellation_ThrowsOnlyForTheCancelledToken_AndDisposesALateBody(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        using var handlerEntered = new ManualResetEventSlim();
        using var releaseHandler = new ManualResetEventSlim();
        var lateBody = new TrackingStream([1, 2, 3]);
        (_, ScriptedWindowHostCore host) = await CreateWithHandlerAsync(harness, (_, _) => {
            handlerEntered.Set();
            releaseHandler.Wait(TimeSpan.FromSeconds(10));
            return (lateBody, "application/octet-stream");
        });

        using (var cancelled = new CancellationTokenSource()) {
            await cancelled.CancelAsync();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await host.Events.GetResourceAsync(Get("probe://localhost/x"), cancelled.Token));
        }

        using var cancellation = new CancellationTokenSource();
        Task<InfiniFrameHostResourceResponse> pending = host.Events.GetResourceAsync(Get("probe://localhost/slow"), cancellation.Token).AsTask();
        handlerEntered.Wait(TimeSpan.FromSeconds(10), ct);
        await cancellation.CancelAsync();
        releaseHandler.Set();
        await Assert.ThrowsAsync<OperationCanceledException>(() => pending);
        await Waits.UntilAsync(() => lateBody.Disposed, "late body disposed by core");
    }

    [Test]
    [Timeout(30_000)]
    public async Task ReadFailure_IsLimitedToThatResponse(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await CreateWithHandlerAsync(harness, (_, url) =>
            url.EndsWith("broken", StringComparison.Ordinal) ? (new ThrowingStream(), "text/plain") : (new MemoryStream("fine"u8.ToArray()), "text/plain"));

        await using (InfiniFrameHostResourceResponse broken = await host.Events.GetResourceAsync(Get("probe://localhost/broken"), ct)) {
            await Assert.That(broken.StatusCode).IsEqualTo(200);
            await Assert.ThrowsAsync<IOException>(async () => await broken.Body!.ReadExactlyAsync(new byte[4], ct));
        }
        await using InfiniFrameHostResourceResponse next = await host.Events.GetResourceAsync(Get("probe://localhost/next"), ct);
        using var reader = new StreamReader(next.Body!, Encoding.UTF8);
        await Assert.That(await reader.ReadToEndAsync(ct)).IsEqualTo("fine");
        await Assert.That(window.Fault).IsNull();
    }

    [Test]
    [Timeout(30_000)]
    public async Task AfterTheWindowEnds_RequestsAre503(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await CreateWithHandlerAsync(harness, (_, _) => (new MemoryStream([1]), null));
        await host.CloseNativelyAsync();
        await window.WaitForTeardownAsync(ct);

        await Assert.That((await host.Events.GetResourceAsync(Get("probe://localhost/x"), ct)).StatusCode).IsEqualTo(503);
    }

    [Test]
    [Timeout(30_000)]
    public async Task StatusReportingHandlerOwner_KeepsItsOwnStatusAndBody(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        var source = new StatusSource();
        (_, ScriptedWindowHostCore host) = await CreateWithHandlerAsync(harness, source.Handle);

        await using InfiniFrameHostResourceResponse missing = await host.Events.GetResourceAsync(Get("probe://localhost/missing.css"), ct);
        await Assert.That(missing.StatusCode).IsEqualTo(404);
        await Assert.That(missing.ContentType).IsEqualTo("text/plain");
        await Assert.That(missing.ContentLength).IsEqualTo(9L);
        await Assert.That((await host.Events.GetResourceAsync(Get("probe://localhost/forbidden"), ct)).StatusCode).IsEqualTo(403);
        await using InfiniFrameHostResourceResponse found = await host.Events.GetResourceAsync(Get("probe://localhost/found"), ct);
        await Assert.That(found.StatusCode).IsEqualTo(200);
        await Assert.That(source.TupleCalls).IsEqualTo(0);
    }

    private sealed class StatusSource : IInfiniFrameStatusResourceSource {
        internal int TupleCalls;

        // Registered as a method group, so the delegate's target is this status-reporting owner.
        internal (Stream? Data, string? ContentType) Handle(IInfiniFrameWindow window, string url) {
            Interlocked.Increment(ref TupleCalls);
            return (null, null);
        }

        public (int StatusCode, Stream? Body, string? ContentType) GetResource(IInfiniFrameWindow? window, string? url) => url switch {
            "probe://localhost/missing.css" => (404, new MemoryStream("not found"u8.ToArray()), "text/plain"),
            "probe://localhost/forbidden" => (403, null, null),
            _ => (200, new MemoryStream([1, 2]), "application/octet-stream")
        };
    }

    private sealed class TrackingStream(byte[] content) : MemoryStream(content) {
        private int _disposeCount;
        internal int DisposeCount => Volatile.Read(ref _disposeCount);
        internal bool Disposed => DisposeCount > 0;

        protected override void Dispose(bool disposing) {
            if (disposing) Interlocked.Increment(ref _disposeCount);
            base.Dispose(disposing);
        }
    }

    private sealed class ThrowingStream : Stream {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() {}
        public override int Read(byte[] buffer, int offset, int count) => throw new IOException("read failed");
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
