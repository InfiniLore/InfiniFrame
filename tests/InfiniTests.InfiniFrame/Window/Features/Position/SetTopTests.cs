// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Position;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetTopTests {
    [Test]
    [Arguments(220)]
    [Arguments(420)]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Position.SetTop(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Position.Top).IsEqualTo(value);
        await Assert.That(builder.Features.Position.StartAtOsDefaultLocation).IsFalse();
        await Assert.That(initParameters.Top).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultLocation).IsFalse();
    }

    [Test]
    [Arguments(240)]
    [Arguments(440)]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetTop(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Position.Top).IsEqualTo(value);
        await Assert.That(builder.Features.Position.StartAtOsDefaultLocation).IsFalse();
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Top).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultLocation).IsFalse();
    }

    [Test]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        int originalTop = window.Features.Position.Top;
        int expectedTop = originalTop + 40;
        window.Features.Position.SetTop(expectedTop);

        // Assert
        int actualTop = await PollUtility.WaitForChangeAsync(
            () => window.Features.Position.Top,
            originalTop,
            TimeSpan.FromSeconds(5),
            ct
        );
        await Assert.That(actualTop).IsEqualTo(expectedTop);
    }

    [Test]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment_ReturnsSameWindow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        int originalTop = window.Features.Position.Top;
        int expectedTop = originalTop + 50;
        IInfiniFrameWindow returnedWindow = window.SetTop(expectedTop);

        // Assert
        int actualTop = await PollUtility.WaitForChangeAsync(
            () => window.Features.Position.Top,
            originalTop,
            TimeSpan.FromSeconds(5),
            ct
        );
        await Assert.That(actualTop).IsEqualTo(expectedTop);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
