// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameDispatchResultTests {

    [Test]
    [Arguments(InfiniFrameDispatchResult.Completed)]
    [Arguments(InfiniFrameDispatchResult.TimedOut)]
    [Arguments(InfiniFrameDispatchResult.Cancelled)]
    [Arguments(InfiniFrameDispatchResult.WindowClosed)]
    [Arguments(InfiniFrameDispatchResult.Failed)]
    public async Task Value_CanBeAssigned(InfiniFrameDispatchResult value, CancellationToken ct = default) {
        // Arrange & Act
        InfiniFrameDispatchResult assigned = value;

        // Assert
        await Assert.That(assigned).IsEqualTo(value);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        // Arrange
        InfiniFrameDispatchResult[] values = Enum.GetValues<InfiniFrameDispatchResult>();

        // Act
        int count = values.Length;

        // Assert
        await Assert.That(count).IsEqualTo(5);
    }
}
