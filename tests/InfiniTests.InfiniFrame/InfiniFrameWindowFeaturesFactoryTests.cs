// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeaturesFactoryTests {

    [Test]
    public async Task Constructor_WithServiceProvider_Succeeds(CancellationToken ct = default) {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfiniFrame();
        ServiceProvider provider = services.BuildServiceProvider();

        // Act
        var factory = new InfiniFrameWindowFeaturesFactory(provider);

        // Assert
        await Assert.That(factory).IsNotNull();
    }
}
