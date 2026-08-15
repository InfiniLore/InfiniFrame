// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Blazor;
using InfiniTests.JsRuntimes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
namespace InfiniTests.InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameJsTests {
    [Test]
    public async Task SetPointerCaptureAsync_InvokesExpectedJsFunction(CancellationToken ct = default) {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        Mock<ILogger<InfiniFrameJs>> loggerMock = Mock.Of<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, loggerMock.Object);
        var element = new ElementReference("element-1");

        // Act
        await sut.SetPointerCaptureAsync(element, 42, ct);

        // Assert
        (string identifier, object?[] jsArguments, CancellationToken cancellationToken) = jsRuntime.Invocations.Single();
        await Assert.That(identifier).IsEqualTo("infiniframe.utils.setPointerCapture");
        await Assert.That(cancellationToken).IsEqualTo(ct);
        await Assert.That(jsArguments.Length).IsEqualTo(2);
        // ReSharper disable once RedundantCast
        await Assert.That(jsArguments[0]).IsEqualTo(element as object);
        await Assert.That((long)jsArguments[1]!).IsEqualTo(42L);
    }

    [Test]
    public async Task ReleasePointerCaptureAsync_InvokesExpectedJsFunction(CancellationToken ct = default) {
        // Arrange
        var jsRuntime = new RecordingJsRuntime();
        Mock<ILogger<InfiniFrameJs>> loggerMock = Mock.Of<ILogger<InfiniFrameJs>>();
        var sut = new InfiniFrameJs(jsRuntime, loggerMock.Object);
        var element = new ElementReference("element-2");

        // Act
        await sut.ReleasePointerCaptureAsync(element, 7, ct);

        // Assert
        (string identifier, object?[] jsArguments, CancellationToken cancellationToken) = jsRuntime.Invocations.Single();
        await Assert.That(identifier).IsEqualTo("infiniframe.utils.releasePointerCapture");
        await Assert.That(cancellationToken).IsEqualTo(ct);
        await Assert.That(jsArguments.Length).IsEqualTo(2);
        // ReSharper disable once RedundantCast
        await Assert.That(jsArguments[0]).IsEqualTo(element as object);
        await Assert.That((long)jsArguments[1]!).IsEqualTo(7L);
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
}
