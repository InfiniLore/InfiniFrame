// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame;
using System.Drawing;

namespace Tests.InfiniFrame.WindowFunctionalities;
using Tests.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OffsetTests {

    [Test, SkipUtility.SkipOnMacOs, SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement), NotInParallel(ParallelControl.InfiniFrame), Arguments(0, 0), Arguments(100, 100), Arguments(-100, -100)]
    public async Task Window(int x, int y) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;
        Point initialLocation = window.Location;

        // Act
        window.Offset(x, y);

        // Assert
        Point location = window.Location;
        await Assert.That(location.X).IsEqualTo(initialLocation.X + x);
        await Assert.That(location.Y).IsEqualTo(initialLocation.Y + y);
    }

    [Test, SkipUtility.SkipOnMacOs, SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement), NotInParallel(ParallelControl.InfiniFrame), Arguments(0, 0), Arguments(100, 100), Arguments(-100, -100)]
    public async Task Window_AsPoint(int x, int y) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;
        Point initialLocation = window.Location;

        // Act
        window.Offset(new Point(x, y));

        // Assert
        Point location = window.Location;
        await Assert.That(location.X).IsEqualTo(initialLocation.X + x);
        await Assert.That(location.Y).IsEqualTo(initialLocation.Y + y);
    }

    [Test, SkipUtility.SkipOnMacOs, SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement), NotInParallel(ParallelControl.InfiniFrame), Arguments(0, 0), Arguments(100, 100), Arguments(-100, -100)]
    public async Task Window_AsDouble(double x, double y) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
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
