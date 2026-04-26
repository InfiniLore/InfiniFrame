// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Js;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrameTests.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ServiceCollectionExtensionsTests {
    [Test]
    [DisplayName($"{nameof(ServiceCollectionExtensionsTests)}.{nameof(AddInfiniFrameJs_RegistersScopedService)}")]
    public async Task AddInfiniFrameJs_RegistersScopedService() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddInfiniFrameJs();

        // Assert
        ServiceDescriptor descriptor = services.Single(d => d.ServiceType == typeof(IInfiniFrameJs));
        await Assert.That(descriptor.Lifetime).IsEqualTo(ServiceLifetime.Scoped);
        await Assert.That(descriptor.ImplementationType).IsEqualTo(typeof(InfiniFrameJs));

        ServiceDescriptor getMessageDescriptor = services.Single(d => d.ServiceType == typeof(IInfiniFrameGetMessageService));
        await Assert.That(getMessageDescriptor.Lifetime).IsEqualTo(ServiceLifetime.Singleton);
        await Assert.That(getMessageDescriptor.ImplementationType).IsEqualTo(typeof(InfiniFrameGetMessageService));
    }
}
