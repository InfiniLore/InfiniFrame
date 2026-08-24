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
public class InfiniFrameDispatcherTests {

    [Test]
    public async Task CheckAccess_WhenNotOnContext_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        ServiceProvider provider = services.BuildServiceProvider();
        var context = new InfiniFrameSynchronizationContext(provider);
        var dispatcher = new InfiniFrameDispatcher(context);

        // Act
        bool result = dispatcher.CheckAccess();

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Constructor_InitializesSuccessfully(CancellationToken ct = default) {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        ServiceProvider provider = services.BuildServiceProvider();
        var context = new InfiniFrameSynchronizationContext(provider);

        // Act
        var dispatcher = new InfiniFrameDispatcher(context);

        // Assert
        await Assert.That(dispatcher).IsNotNull();
    }

    [Test]
    public async Task InvokeAsync_Action_WindowClosed_ExecutesCallback(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<IInvokeInfiniFrameWindowFeature> invokeMock = MockFactory.CreateInvokeMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.Invoke.Returns(invokeMock.Object);
        invokeMock.Invoke(Any<Action>()).Returns(InfiniFrameDispatchResult.WindowClosed);

        await using ServiceProvider provider = new ServiceCollection()
            .AddSingleton(windowMock.Object)
            .BuildServiceProvider();
        var context = new InfiniFrameSynchronizationContext(provider);
        var dispatcher = new InfiniFrameDispatcher(context);
        bool invoked = false;

        // Act
        await dispatcher.InvokeAsync(() => invoked = true);

        // Assert
        await Assert.That(invoked).IsTrue();
    }

    [Test]
    public async Task InvokeAsync_FuncTask_WindowClosed_ExecutesCallback(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<IInvokeInfiniFrameWindowFeature> invokeMock = MockFactory.CreateInvokeMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.Invoke.Returns(invokeMock.Object);
        invokeMock.Invoke(Any<Action>()).Returns(InfiniFrameDispatchResult.WindowClosed);

        await using ServiceProvider provider = new ServiceCollection()
            .AddSingleton(windowMock.Object)
            .BuildServiceProvider();
        var context = new InfiniFrameSynchronizationContext(provider);
        var dispatcher = new InfiniFrameDispatcher(context);
        bool invoked = false;

        // Act
        await dispatcher.InvokeAsync(async () => {
            await Task.Yield();
            invoked = true;
        });

        // Assert
        await Assert.That(invoked).IsTrue();
    }

    [Test]
    public async Task InvokeAsync_FuncTResult_WindowClosed_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<IInvokeInfiniFrameWindowFeature> invokeMock = MockFactory.CreateInvokeMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.Invoke.Returns(invokeMock.Object);
        invokeMock.Invoke(Any<Action>()).Returns(InfiniFrameDispatchResult.WindowClosed);

        await using ServiceProvider provider = new ServiceCollection()
            .AddSingleton(windowMock.Object)
            .BuildServiceProvider();
        var context = new InfiniFrameSynchronizationContext(provider);
        var dispatcher = new InfiniFrameDispatcher(context);

        // Act
        int result = await dispatcher.InvokeAsync(() => 42);

        // Assert
        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task InvokeAsync_FuncTaskTResult_WindowClosed_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<IInvokeInfiniFrameWindowFeature> invokeMock = MockFactory.CreateInvokeMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.Invoke.Returns(invokeMock.Object);
        invokeMock.Invoke(Any<Action>()).Returns(InfiniFrameDispatchResult.WindowClosed);

        await using ServiceProvider provider = new ServiceCollection()
            .AddSingleton(windowMock.Object)
            .BuildServiceProvider();
        var context = new InfiniFrameSynchronizationContext(provider);
        var dispatcher = new InfiniFrameDispatcher(context);

        // Act
        int result = await dispatcher.InvokeAsync(async () => {
            await Task.Yield();
            return 99;
        });

        // Assert
        await Assert.That(result).IsEqualTo(99);
    }
}
