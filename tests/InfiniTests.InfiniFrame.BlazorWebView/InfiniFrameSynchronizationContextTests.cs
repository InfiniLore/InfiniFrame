// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameSynchronizationContextTests {
    [Test]
    public async Task InvokeAsync_WindowAlreadyClosed_ExecutesCallbackInline(CancellationToken ct = default) {
        // Arrange
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        var invoke = Substitute.For<IInvokeInfiniFrameWindowFeature>();
        window.Features.Returns(features);
        features.Invoke.Returns(invoke);
        invoke.Invoke(Arg.Any<Action>()).Returns(InfiniFrameDispatchResult.WindowClosed);

        await using ServiceProvider provider = new ServiceCollection()
            .AddSingleton(window)
            .BuildServiceProvider();
        var context = new InfiniFrameSynchronizationContext(provider);
        bool invoked = false;

        // Act
        await context.InvokeAsync(() => invoked = true).WaitAsync(ct);

        // Assert
        await Assert.That(invoked).IsTrue();
        invoke.Received(1).Invoke(Arg.Any<Action>());
    }
}
