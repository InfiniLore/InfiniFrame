// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters.Window;
using InfiniFrame.Window.Builder;

namespace InfiniTests.InfiniFrame.Window.Features.State;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MaximizedTests {
    // The maximize state is applied at map time and echoed back through the GTK4 toplevel
    // state notification once the compositor acknowledges it. Assert the echoed state with a
    // bounded wait instead of synchronously (Wayland acks can land a few frames later).
    private static async Task<bool> WaitForMaximizedStateAsync(
        IInfiniFrameWindow window,
        bool expected,
        CancellationToken ct
        ) {
        DateTime deadline = DateTime.UtcNow + TimeSpan.FromSeconds(5);
        bool observed = window.Features.State.IsMaximized;
        while (observed != expected && DateTime.UtcNow < deadline) {
            await Task.Delay(25, ct);
            observed = window.Features.State.IsMaximized;
        }
        return observed;
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.State.SetMaximized(value);
        InfiniFrameNativeWindowParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.State.StartMaximized).IsEqualTo(value);
        await Assert.That(initParameters.Maximized).IsEqualTo(value);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetMaximized(value);
        InfiniFrameNativeWindowParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.State.StartMaximized).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Maximized).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    [SkipOnLinux("Test unable to be verified on Linux due to lack of support for maximizing windows")]
    public async Task AtWindowStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.State.SetMaximized(value);

        // Assert
        bool observed = await WaitForMaximizedStateAsync(window, value, ct);
        await Assert.That(observed).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    [SkipOnLinux("Test unable to be verified on Linux due to lack of support for maximizing windows")]
    public async Task AtWindowStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetMaximized(value);

        // Assert
        bool observed = await WaitForMaximizedStateAsync(window, value, ct);
        await Assert.That(observed).IsEqualTo(value);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_ThroughBuilderAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.State.SetMaximized(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.State.StartMaximized).IsEqualTo(value);
        bool observed = await WaitForMaximizedStateAsync(window, value, ct);
        await Assert.That(observed).IsEqualTo(value);
    }
}
