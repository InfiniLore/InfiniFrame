// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class AppDomainUnhandledExceptionSourceTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Register_NullHandler_ShouldThrowArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var source = new AppDomainUnhandledExceptionSource();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() => {
            source.Register(null!);
        }));

        // Assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.ParamName).IsEqualTo("handler");
    }

    [Test]
    public async Task Register_ValidHandler_ShouldReturnDisposable(CancellationToken ct = default) {
        // Arrange
        var source = new AppDomainUnhandledExceptionSource();
        UnhandledExceptionEventHandler handler = (_, _) => { };

        // Act
        IDisposable subscription = source.Register(handler);

        // Assert
        await Assert.That(subscription).IsNotNull();
        subscription.Dispose();
    }

    [Test]
    public async Task Register_Dispose_ShouldUnsubscribeHandler(CancellationToken ct = default) {
        // Arrange
        var source = new AppDomainUnhandledExceptionSource();
        bool handlerCalled = false;
        UnhandledExceptionEventHandler handler = (_, _) => handlerCalled = true;

        // Act
        IDisposable subscription = source.Register(handler);
        subscription.Dispose();

        // Assert
        await Assert.That(handlerCalled).IsFalse();
    }

    [Test]
    public async Task Register_MultipleDisposes_ShouldNotThrow(CancellationToken ct = default) {
        // Arrange
        var source = new AppDomainUnhandledExceptionSource();
        UnhandledExceptionEventHandler handler = (_, _) => { };
        IDisposable subscription = source.Register(handler);

        // Act
        subscription.Dispose();
        subscription.Dispose();

        // Assert
        await Assert.That(subscription).IsNotNull();
    }
}
