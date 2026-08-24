// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FeatureEnumsTests {

    [Test]
    [Arguments(typeof(NavigationStatus))]
    [Arguments(typeof(InfiniFrameDispatchResult))]
    [Arguments(typeof(InfiniFrameMenuItemType))]
    [Arguments(typeof(TaskbarProgressState))]
    [Arguments(typeof(TaskbarFlashMode))]
    [Arguments(typeof(InfiniFrameNotificationUrgency))]
    [Arguments(typeof(InfiniFrameNotificationResult))]
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
}
