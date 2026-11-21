// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Blazor;
using InfiniTests.JsRuntimes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace InfiniTests.InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// A page whose document is gone (for example a page replaced in a hosted window) refuses JS interop with
// JSDisconnectedException. InfiniFrameJs treats that as "nothing to do", not as a failure, while a genuine JS error on a
// live page is still reported, and anything else still propagates.
public class InfiniFrameJsDisconnectTests {
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task OnADisconnectedPage_TheCallEndsQuietly_WithoutAnErrorLog(bool release, CancellationToken ct = default) {
        var jsRuntime = new RecordingJsRuntime { ExceptionFactory = _ => new JSDisconnectedException("the page's document retired") };
        var logger = new CapturingLogger();
        var sut = new InfiniFrameJs(jsRuntime, logger);

        await Invoke(sut, release, ct);

        await Assert.That(jsRuntime.Invocations.Count).IsEqualTo(1);
        await Assert.That(logger.Entries.Any(static e => e.Level >= LogLevel.Warning)).IsFalse();
        await Assert.That(logger.Entries.Count(static e => e.Level == LogLevel.Debug && e.Message.Contains("disconnected", StringComparison.Ordinal))).IsEqualTo(1);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task OnALivePage_AJsError_IsStillReportedAsAnError(bool release, CancellationToken ct = default) {
        var jsRuntime = new RecordingJsRuntime { ExceptionFactory = _ => new JSException("pointer capture failed in the page") };
        var logger = new CapturingLogger();
        var sut = new InfiniFrameJs(jsRuntime, logger);

        await Invoke(sut, release, ct);

        await Assert.That(logger.Entries.Count(static e => e.Level == LogLevel.Error && e.Exception is JSException)).IsEqualTo(1);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AnUnexpectedException_StillPropagates(bool release, CancellationToken ct = default) {
        var jsRuntime = new RecordingJsRuntime { ExceptionFactory = _ => new ArgumentException("unexpected") };
        var sut = new InfiniFrameJs(jsRuntime, new CapturingLogger());

        await Assert.ThrowsAsync<ArgumentException>(() => Invoke(sut, release, ct));
    }

    private static Task Invoke(InfiniFrameJs sut, bool release, CancellationToken ct) =>
        release
            ? sut.ReleasePointerCaptureAsync(new ElementReference("element-r"), 3, ct)
            : sut.SetPointerCaptureAsync(new ElementReference("element-s"), 3, ct);

    private sealed class CapturingLogger : ILogger<InfiniFrameJs> {
        private readonly List<(LogLevel Level, string Message, Exception? Exception)> _entries = [];

        public IReadOnlyList<(LogLevel Level, string Message, Exception? Exception)> Entries {
            get {
                lock (_entries) return [.. _entries];
            }
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
            lock (_entries) _entries.Add((logLevel, formatter(state, exception), exception));
        }
    }
}
