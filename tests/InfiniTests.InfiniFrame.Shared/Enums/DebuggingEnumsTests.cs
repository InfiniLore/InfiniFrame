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
    [Arguments(typeof(InfiniFrameDebugEventKind))]
    [Arguments(typeof(InfiniFrameDebugEndpointStatus))]
    public async Task AllValues_AreDistinct(Type enumType, CancellationToken ct = default) {
        // Arrange
        Array values = Enum.GetValues(enumType);

        // Act
        int distinctCount = values.Cast<object>()
            .Select(Convert.ToInt32)
            .Distinct()
            .Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    [Arguments(0)]
    [Arguments(5)]
    public async Task InfiniFrameDebugEndpointStatus_HasExpectedValues(int value, CancellationToken ct = default) {
        // Arrange & Act
        bool isDefined = Enum.IsDefined(typeof(InfiniFrameDebugEndpointStatus), value);

        // Assert
        await Assert.That(isDefined).IsTrue();
    }
}
