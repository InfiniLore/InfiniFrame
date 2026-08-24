// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MonitorsExtensionMethodTests {

    [Test]
    public async Task GetMonitors_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IMonitorsInfiniFrameWindowFeature> monitors = MockFactory.CreateMonitorsMock();

        // Act
        IEnumerable<InfiniMonitor> result = monitors.Object.GetMonitors();

        // Assert
        await Assert.That(result).IsNotNull();
    }
}
