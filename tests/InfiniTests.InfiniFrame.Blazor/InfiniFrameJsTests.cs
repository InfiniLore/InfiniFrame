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
public class InfiniFrameJsTests {

    [Test]
    [Arguments(42L)]
    [Arguments(0L)]
    [Arguments(long.MaxValue)]
    public async Task SetPointerCaptureAsync_InvokesExpectedJsFunction(long pointerId, CancellationToken ct = default) {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        Mock<ILogger<InfiniFrameJs>> loggerMock = Mock.Of<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, loggerMock.Object);
        var element = new ElementReference("element-1");

        // Act
        await sut.SetPointerCaptureAsync(element, pointerId, ct);

        // Assert
        (string identifier, object?[] jsArguments, CancellationToken cancellationToken) = jsRuntime.Invocations.Single();
        await Assert.That(identifier).IsEqualTo("infiniframe.utils.setPointerCapture");
        await Assert.That(cancellationToken).IsEqualTo(ct);
        await Assert.That(jsArguments.Length).IsEqualTo(2);
        await Assert.That(jsArguments[0]).IsEqualTo(element);
        await Assert.That((long)jsArguments[1]!).IsEqualTo(pointerId);
    }

    [Test]
    [Arguments(7L)]
    [Arguments(0L)]
    [Arguments(12345L)]
    public async Task ReleasePointerCaptureAsync_InvokesExpectedJsFunction(long pointerId, CancellationToken ct = default) {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        Mock<ILogger<InfiniFrameJs>> loggerMock = Mock.Of<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, loggerMock.Object);
        var element = new ElementReference("element-2");

        // Act
        await sut.ReleasePointerCaptureAsync(element, pointerId, ct);

        // Assert
        (string identifier, object?[] jsArguments, CancellationToken cancellationToken) = jsRuntime.Invocations.Single();
        await Assert.That(identifier).IsEqualTo("infiniframe.utils.releasePointerCapture");
        await Assert.That(cancellationToken).IsEqualTo(ct);
        await Assert.That(jsArguments.Length).IsEqualTo(2);
        await Assert.That(jsArguments[0]).IsEqualTo(element);
        await Assert.That((long)jsArguments[1]!).IsEqualTo(pointerId);
    }

    [Test]
    public async Task SetPointerCaptureAsync_SwallowsOperationCanceled_WhenCancellationRequested(CancellationToken ct = default) {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        Mock<ILogger<InfiniFrameJs>> loggerMock = Mock.Of<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, loggerMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // ReSharper disable once AccessToDisposedClosure
        jsRuntime.ExceptionFactory = _ => new OperationCanceledException(cts.Token);

        // Act / Assert
        await sut.SetPointerCaptureAsync(new ElementReference("element-3"), 1, cts.Token);
        await Assert.That(jsRuntime.Invocations.Count).IsEqualTo(1);
    }

    [Test]
    public async Task ReleasePointerCaptureAsync_SwallowsOperationCanceled_WhenCancellationRequested(CancellationToken ct = default) {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        Mock<ILogger<InfiniFrameJs>> loggerMock = Mock.Of<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, loggerMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // ReSharper disable once AccessToDisposedClosure
        jsRuntime.ExceptionFactory = _ => new OperationCanceledException(cts.Token);

        // Act / Assert
        await sut.ReleasePointerCaptureAsync(new ElementReference("element-4"), 1, cts.Token);
        await Assert.That(jsRuntime.Invocations.Count).IsEqualTo(1);
    }

    [Test]
    public async Task SetPointerCaptureAsync_SwallowsJSException(CancellationToken ct = default) {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        Mock<ILogger<InfiniFrameJs>> loggerMock = Mock.Of<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, loggerMock.Object);
        jsRuntime.ExceptionFactory = _ => new JSException("test error");

        // Act
        await sut.SetPointerCaptureAsync(new ElementReference("element-5"), 1, ct);

        // Assert
        await Assert.That(jsRuntime.Invocations.Count).IsEqualTo(1);
    }

    [Test]
    public async Task SetPointerCaptureAsync_SwallowsInvalidOperationException(CancellationToken ct = default) {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        Mock<ILogger<InfiniFrameJs>> loggerMock = Mock.Of<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, loggerMock.Object);
        jsRuntime.ExceptionFactory = _ => new InvalidOperationException("test error");

        // Act
        await sut.SetPointerCaptureAsync(new ElementReference("element-6"), 1, ct);

        // Assert
        await Assert.That(jsRuntime.Invocations.Count).IsEqualTo(1);
    }

    [Test]
    public async Task ReleasePointerCaptureAsync_SwallowsJSException(CancellationToken ct = default) {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        Mock<ILogger<InfiniFrameJs>> loggerMock = Mock.Of<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, loggerMock.Object);
        jsRuntime.ExceptionFactory = _ => new JSException("test error");

        // Act
        await sut.ReleasePointerCaptureAsync(new ElementReference("element-7"), 1, ct);

        // Assert
        await Assert.That(jsRuntime.Invocations.Count).IsEqualTo(1);
    }

    [Test]
    public async Task ReleasePointerCaptureAsync_SwallowsInvalidOperationException(CancellationToken ct = default) {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        Mock<ILogger<InfiniFrameJs>> loggerMock = Mock.Of<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, loggerMock.Object);
        jsRuntime.ExceptionFactory = _ => new InvalidOperationException("test error");

        // Act
        await sut.ReleasePointerCaptureAsync(new ElementReference("element-8"), 1, ct);

        // Assert
        await Assert.That(jsRuntime.Invocations.Count).IsEqualTo(1);
    }
}
