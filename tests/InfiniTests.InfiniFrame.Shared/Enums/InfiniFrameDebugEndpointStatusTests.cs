// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameDebugEndpointStatusTests {

    [Test]
    [Arguments(InfiniFrameDebugEndpointStatus.NotSupported)]
    [Arguments(InfiniFrameDebugEndpointStatus.Disabled)]
    [Arguments(InfiniFrameDebugEndpointStatus.Unavailable)]
    [Arguments(InfiniFrameDebugEndpointStatus.Configured)]
    [Arguments(InfiniFrameDebugEndpointStatus.Reachable)]
    [Arguments(InfiniFrameDebugEndpointStatus.Unreachable)]
    [Arguments(InfiniFrameDebugEndpointStatus.ProbeFailed)]
    public async Task Value_CanBeAssigned(InfiniFrameDebugEndpointStatus value, CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(value).IsEqualTo(value);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        // Arrange
        InfiniFrameDebugEndpointStatus[] values = Enum.GetValues<InfiniFrameDebugEndpointStatus>();

        // Act
        int count = values.Length;

        // Assert
        await Assert.That(count).IsEqualTo(7);
    }
}
