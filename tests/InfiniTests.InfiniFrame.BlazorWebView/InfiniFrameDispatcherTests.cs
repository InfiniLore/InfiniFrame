// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
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
}
