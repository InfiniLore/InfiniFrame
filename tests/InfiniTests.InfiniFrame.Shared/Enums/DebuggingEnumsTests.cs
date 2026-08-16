// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DebuggingEnumsTests {

    [Test]
    public async Task InfiniFrameDebugEventKind_AllValuesDistinct(CancellationToken ct = default) {
        // Arrange
        InfiniFrameDebugEventKind[] values = (InfiniFrameDebugEventKind[])Enum.GetValues(typeof(InfiniFrameDebugEventKind));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    public async Task InfiniFrameDebugEndpointStatus_AllValuesDistinct(CancellationToken ct = default) {
        // Arrange
        InfiniFrameDebugEndpointStatus[] values = (InfiniFrameDebugEndpointStatus[])Enum.GetValues(typeof(InfiniFrameDebugEndpointStatus));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    public async Task InfiniFrameDebugEndpointStatus_HasExpectedValues(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(Enum.IsDefined(typeof(InfiniFrameDebugEndpointStatus), 0)).IsTrue();
        await Assert.That(Enum.IsDefined(typeof(InfiniFrameDebugEndpointStatus), 5)).IsTrue();
    }
}
