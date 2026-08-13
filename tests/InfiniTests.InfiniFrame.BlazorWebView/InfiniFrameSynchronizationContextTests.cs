// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameSynchronizationContextTests {
    [Test]
    public async Task InvokeAsync_WindowAlreadyClosed_ExecutesCallbackInline(CancellationToken ct = default) {
        // Arrange
        var windowMock = MockFactory.CreateWindowMock();
        var featuresMock = MockFactory.CreateFeaturesMock();
        var invokeMock = MockFactory.CreateInvokeMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.Invoke.Returns(invokeMock.Object);
        invokeMock.Invoke(Any<Action>()).Returns(InfiniFrameDispatchResult.WindowClosed);

        await using ServiceProvider provider = new ServiceCollection()
            .AddSingleton<IInfiniFrameWindow>(windowMock.Object)
            .BuildServiceProvider();
        var context = new InfiniFrameSynchronizationContext(provider);
        bool invoked = false;

        // Act
        await context.InvokeAsync(() => invoked = true).WaitAsync(ct);

        // Assert
        await Assert.That(invoked).IsTrue();
        invokeMock.Invoke(Any<Action>()).WasCalled(Times.Once);
    }
}
