// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Blazor;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrameTests.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ServiceCollectionExtensionsTests {
    [Test]
    public async Task AddInfiniFrameJs_RegistersScopedService(CancellationToken ct = default) {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddInfiniFrameJs();

        // Assert
        ServiceDescriptor descriptor = services.Single(d => d.ServiceType == typeof(IInfiniFrameJs));
        await Assert.That(descriptor.Lifetime).IsEqualTo(ServiceLifetime.Scoped);
        await Assert.That(descriptor.ImplementationType).IsEqualTo(typeof(InfiniFrameJs));
    }
}
