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

    private static (InfiniFrameSynchronizationContext Context, Mock<IInvokeInfiniFrameWindowFeature> InvokeMock) CreateContextWithWindowClosedMock() {
        Mock<IInfiniFrameWindow> windowMock = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> featuresMock = MockFactory.CreateFeaturesMock();
        Mock<IInvokeInfiniFrameWindowFeature> invokeMock = MockFactory.CreateInvokeMock();
        windowMock.Features.Returns(featuresMock.Object);
        featuresMock.Invoke.Returns(invokeMock.Object);
        invokeMock.Invoke(Any<Action>()).Returns(InfiniFrameDispatchResult.WindowClosed);

        ServiceProvider provider = new ServiceCollection()
            .AddSingleton(windowMock.Object)
            .BuildServiceProvider();
        var context = new InfiniFrameSynchronizationContext(provider);
        return (context, invokeMock);
    }

    [Test]
    public async Task InvokeAsync_WindowAlreadyClosed_ExecutesCallbackInline(CancellationToken ct = default) {
        // Arrange
        var (context, invokeMock) = CreateContextWithWindowClosedMock();
        bool invoked = false;

        // Act
        await context.InvokeAsync(() => invoked = true).WaitAsync(ct);

        // Assert
        await Assert.That(invoked).IsTrue();
        invokeMock.Invoke(Any<Action>()).WasCalled(Times.Once);
    }

    [Test]
    public async Task InvokeAsync_WindowAlreadyClosed_FuncTResult_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        var (context, _) = CreateContextWithWindowClosedMock();

        // Act
        int result = await context.InvokeAsync(() => 42).WaitAsync(ct);

        // Assert
        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task CreateCopy_ReturnsNewInstance(CancellationToken ct = default) {
        // Arrange
        var (context, _) = CreateContextWithWindowClosedMock();

        // Act
        SynchronizationContext copy = context.CreateCopy();

        // Assert
        await Assert.That(copy).IsNotNull();
        await Assert.That(copy).IsNotSameReferenceAs(context);
    }

    [Test]
    public async Task CreateCopy_ReturnsInfiniFrameSynchronizationContext(CancellationToken ct = default) {
        // Arrange
        var (context, _) = CreateContextWithWindowClosedMock();

        // Act
        SynchronizationContext copy = context.CreateCopy();

        // Assert
        bool isCorrectType = copy.GetType().Name == "InfiniFrameSynchronizationContext";
        await Assert.That(isCorrectType).IsTrue();
    }

    [Test]
    public async Task InvokeAsync_WindowAlreadyClosed_FuncTask_ExecutesCallback(CancellationToken ct = default) {
        // Arrange
        var (context, _) = CreateContextWithWindowClosedMock();
        bool invoked = false;

        // Act
        await context.InvokeAsync(async () => {
            await Task.Yield();
            invoked = true;
        }).WaitAsync(ct);

        // Assert
        await Assert.That(invoked).IsTrue();
    }

    [Test]
    public async Task InvokeAsync_WindowAlreadyClosed_FuncTaskTResult_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        var (context, _) = CreateContextWithWindowClosedMock();

        // Act
        int result = await context.InvokeAsync(async () => {
            await Task.Yield();
            return 99;
        }).WaitAsync(ct);

        // Assert
        await Assert.That(result).IsEqualTo(99);
    }

    [Test]
    public async Task InvokeAsync_WindowAlreadyClosed_FuncTResult_Exception_Propagates(CancellationToken ct = default) {
        // Arrange
        var (context, _) = CreateContextWithWindowClosedMock();

        // Act & Assert
        InvalidOperationException? caught = null;
        try {
            Func<int> func = () => throw new InvalidOperationException("test error");
            await context.InvokeAsync(func).WaitAsync(ct);
        }
        catch (InvalidOperationException ex) {
            caught = ex;
        }
        await Assert.That(caught).IsNotNull();
    }
}
