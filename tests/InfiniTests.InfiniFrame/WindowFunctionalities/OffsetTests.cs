// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Drawing;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OffsetTests {

    [Test]
    [DisplayName($"{nameof(OffsetTests)}.{nameof(Window)}")]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Arguments(0, 0)]
    [Arguments(100, 100)]
    [Arguments(-100, -100)]
    public async Task Window(int x, int y, CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Point initialLocation = window.Location;

        // Act
        window.Offset(x, y);

        // Assert
        Point location = window.Location;
        await Assert.That(location.X).IsEqualTo(initialLocation.X + x);
        await Assert.That(location.Y).IsEqualTo(initialLocation.Y + y);
    }

    [Test]
    [DisplayName($"{nameof(OffsetTests)}.{nameof(Window_AsPoint)}")]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Arguments(0, 0)]
    [Arguments(100, 100)]
    [Arguments(-100, -100)]
    public async Task Window_AsPoint(int x, int y, CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Point initialLocation = window.Location;

        // Act
        window.Offset(new Point(x, y));

        // Assert
        Point location = window.Location;
        await Assert.That(location.X).IsEqualTo(initialLocation.X + x);
        await Assert.That(location.Y).IsEqualTo(initialLocation.Y + y);
    }

    [Test]
    [DisplayName($"{nameof(OffsetTests)}.{nameof(Window_AsDouble)}")]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Arguments(0, 0)]
    [Arguments(100, 100)]
    [Arguments(-100, -100)]
    public async Task Window_AsDouble(double x, double y, CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Point initialLocation = window.Location;

        // Act
        window.Offset(x, y);

        // Assert
        Point location = window.Location;
        await Assert.That(location.X).IsEqualTo(initialLocation.X + (int)x);
        await Assert.That(location.Y).IsEqualTo(initialLocation.Y + (int)y);
    }
}
