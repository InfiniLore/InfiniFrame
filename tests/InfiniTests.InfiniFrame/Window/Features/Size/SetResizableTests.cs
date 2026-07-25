// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetResizableTests {
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Size.SetResizable(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.IsResizable).IsEqualTo(value);
        await Assert.That(initParameters.Resizable).IsEqualTo(value);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetResizable(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.IsResizable).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Resizable).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        bool currentResizable = window.Features.Size.IsResizable;
        if (currentResizable == value) {
            window.Features.Size.SetResizable(!value);
            currentResizable = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.IsResizable, currentResizable, TimeSpan.FromSeconds(5), ct);
        }

        // Act
        window.Features.Size.SetResizable(value);

        // Assert
        bool newResizable = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.IsResizable, currentResizable, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newResizable).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        bool currentResizable = window.Features.Size.IsResizable;
        if (currentResizable == value) {
            window.Features.Size.SetResizable(!value);
            currentResizable = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.IsResizable, currentResizable, TimeSpan.FromSeconds(5), ct);
        }

        // Act
        IInfiniFrameWindow returnedWindow = window.SetResizable(value);

        // Assert
        bool newResizable = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.IsResizable, currentResizable, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newResizable).IsEqualTo(value);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
