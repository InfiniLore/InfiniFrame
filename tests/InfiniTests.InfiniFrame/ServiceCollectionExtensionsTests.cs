// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.NativeBridge.Parameters.Window;
using InfiniFrame.Window;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ServiceCollectionExtensionsTests {

    [Test]
    public async Task AddInfiniFrame_RegistersAllServices(CancellationToken ct = default) {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddInfiniFrame();
        ServiceProvider provider = services.BuildServiceProvider();

        // Assert
        await Assert.That(provider.GetService<IInfiniFrameEventsStore>()).IsNotNull();
        await Assert.That(provider.GetService<IInfiniFrameWindowConfiguration>()).IsNotNull();
        await Assert.That(provider.GetService<IValidator<InfiniFrameNativeWindowParameters>>()).IsNotNull();
        await Assert.That(provider.GetService<InfiniFrameWindowFeaturesFactory>()).IsNotNull();
    }

    [Test]
    public async Task AddInfiniFrame_ReturnsServiceCollection(CancellationToken ct = default) {
        // Arrange
        var services = new ServiceCollection();

        // Act
        IServiceCollection result = services.AddInfiniFrame();

        // Assert
        await Assert.That(result).IsSameReferenceAs(services);
    }

    [Test]
    public async Task AddInfiniFrame_CanBeChained(CancellationToken ct = default) {
        // Arrange & Act
        IServiceCollection services = new ServiceCollection()
            .AddLogging()
            .AddInfiniFrame();

        // Assert
        await Assert.That(services).IsNotNull();
    }
}
