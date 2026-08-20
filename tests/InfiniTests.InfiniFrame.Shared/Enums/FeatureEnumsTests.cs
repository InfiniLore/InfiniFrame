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
    public async Task NavigationStatus_AllValuesDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (NavigationStatus[])Enum.GetValues(typeof(NavigationStatus));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    public async Task InfiniFrameDispatchResult_AllValuesDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (InfiniFrameDispatchResult[])Enum.GetValues(typeof(InfiniFrameDispatchResult));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    public async Task InfiniFrameMenuItemType_AllValuesDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (InfiniFrameMenuItemType[])Enum.GetValues(typeof(InfiniFrameMenuItemType));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    public async Task TaskbarProgressState_AllValuesDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (TaskbarProgressState[])Enum.GetValues(typeof(TaskbarProgressState));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    public async Task TaskbarFlashMode_AllValuesDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (TaskbarFlashMode[])Enum.GetValues(typeof(TaskbarFlashMode));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    public async Task InfiniFrameNotificationUrgency_AllValuesDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (InfiniFrameNotificationUrgency[])Enum.GetValues(typeof(InfiniFrameNotificationUrgency));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    public async Task InfiniFrameNotificationResult_AllValuesDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (InfiniFrameNotificationResult[])Enum.GetValues(typeof(InfiniFrameNotificationResult));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }
}
