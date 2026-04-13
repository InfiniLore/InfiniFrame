// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Js;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using NSubstitute;

namespace InfiniFrameTests.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameJsTests {
    [Test]
    [DisplayName($"{nameof(InfiniFrameJsTests)}.{nameof(SetPointerCaptureAsync_InvokesExpectedJsFunction)}")]
    public async Task SetPointerCaptureAsync_InvokesExpectedJsFunction() {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        var logger = Substitute.For<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, logger);
        var element = new ElementReference("element-1");

        // Act
        await sut.SetPointerCaptureAsync(element, 42);

        // Assert
        RecordingJsRuntime.Invocation invocation = jsRuntime.Invocations.Single();
        await Assert.That(invocation.Identifier).IsEqualTo("infiniFrame.setPointerCapture");
        await Assert.That(invocation.CancellationToken).IsEqualTo(CancellationToken.None);
        object?[] jsArguments = invocation.Arguments;
        await Assert.That(jsArguments.Length).IsEqualTo(2);
        await Assert.That(jsArguments[0]).IsEqualTo(element);
        await Assert.That(jsArguments[1]).IsEqualTo(42L);
    }

    [Test]
    [DisplayName($"{nameof(InfiniFrameJsTests)}.{nameof(ReleasePointerCaptureAsync_InvokesExpectedJsFunction)}")]
    public async Task ReleasePointerCaptureAsync_InvokesExpectedJsFunction() {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        var logger = Substitute.For<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, logger);
        var element = new ElementReference("element-2");

        // Act
        await sut.ReleasePointerCaptureAsync(element, 7);

        // Assert
        RecordingJsRuntime.Invocation invocation = jsRuntime.Invocations.Single();
        await Assert.That(invocation.Identifier).IsEqualTo("infiniFrame.releasePointerCapture");
        await Assert.That(invocation.CancellationToken).IsEqualTo(CancellationToken.None);
        object?[] jsArguments = invocation.Arguments;
        await Assert.That(jsArguments.Length).IsEqualTo(2);
        await Assert.That(jsArguments[0]).IsEqualTo(element);
        await Assert.That(jsArguments[1]).IsEqualTo(7L);
    }

    [Test]
    [DisplayName($"{nameof(InfiniFrameJsTests)}.{nameof(SetPointerCaptureAsync_SwallowsOperationCanceled_WhenCancellationRequested)}")]
    public async Task SetPointerCaptureAsync_SwallowsOperationCanceled_WhenCancellationRequested() {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        var logger = Substitute.For<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, logger);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // ReSharper disable once AccessToDisposedClosure
        jsRuntime.ExceptionFactory = _ => new OperationCanceledException(cts.Token);

        // Act / Assert
        await sut.SetPointerCaptureAsync(new ElementReference("element-3"), 1, cts.Token);
        logger.DidNotReceiveWithAnyArgs().Log(default, default, null!, null, null!);
        await Assert.That(jsRuntime.Invocations.Count).IsEqualTo(1);
    }

    private sealed class RecordingJsRuntime : IJSRuntime {
        public sealed record Invocation(string Identifier, CancellationToken CancellationToken, object?[] Arguments);

        public List<Invocation> Invocations { get; } = [];
        public Func<Invocation, Exception?>? ExceptionFactory { get; set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) {
            return InvokeAsync<TValue>(identifier, CancellationToken.None, args);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) {
            var invocation = new Invocation(identifier, cancellationToken, args ?? []);
            Invocations.Add(invocation);

            Exception? ex = ExceptionFactory?.Invoke(invocation);
            if (ex is not null)
                return ValueTask.FromException<TValue>(ex);

            return ValueTask.FromResult(default(TValue)!);
        }
    }
}
