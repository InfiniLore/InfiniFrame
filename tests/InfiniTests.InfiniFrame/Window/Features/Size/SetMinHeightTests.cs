// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetMinHeightTests {
    [Test]
    [Arguments(220)]
    [Arguments(360)]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Size.SetMinHeight(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MinHeight).IsEqualTo(value);
        await Assert.That(initParameters.MinHeight).IsEqualTo(value);
    }

    [Test]
    [Arguments(240)]
    [Arguments(380)]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetMinHeight(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MinHeight).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.MinHeight).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(260)]
    public async Task AtWindowStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalMinHeight = window.Features.Size.MinHeight;
        int targetMinHeight = value == originalMinHeight ? value + 20 : value;

        // Act
        window.Features.Size.SetMinHeight(targetMinHeight);

        // Assert
        int newMinHeight = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.MinHeight, originalMinHeight, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newMinHeight).IsEqualTo(targetMinHeight);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(280)]
    public async Task AtWindowStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalMinHeight = window.Features.Size.MinHeight;
        int targetMinHeight = value == originalMinHeight ? value + 20 : value;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetMinHeight(targetMinHeight);

        // Assert
        int newMinHeight = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.MinHeight, originalMinHeight, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newMinHeight).IsEqualTo(targetMinHeight);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
