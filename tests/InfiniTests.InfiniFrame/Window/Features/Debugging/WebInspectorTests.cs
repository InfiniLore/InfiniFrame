// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebInspectorTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    [OnlyRunOnMacOs]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        if (!OperatingSystem.IsMacOSVersionAtLeast(13,3)) {
            Skip.Test("This test is only run on macOS");
            return;
        }
        
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        
        // Act
        builder.Features.Debugging.EnableWebInspector(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Debugging.IsWebInspectorEnabled).IsEqualTo(value);
        await Assert.That(initParameters.WebInspectorEnabled).IsEqualTo(value);
    }
    
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    [SkipOnMacOs("This test verifies the non-macOS unsupported-platform behavior")]
    public async Task AtBuilderStage_DirectAssignment_UnhappyFlow(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        
        // Act & Assert
        Assert.Throws<PlatformNotSupportedException>(() => {
            #pragma warning disable CA1416
            builder.Features.Debugging.EnableWebInspector(value);
            #pragma warning restore CA1416
        });
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Debugging.IsWebInspectorEnabled).IsFalse();
        await Assert.That(initParameters.WebInspectorEnabled).IsFalse();
    }
    
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    [OnlyRunOnMacOs]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        if (!OperatingSystem.IsMacOSVersionAtLeast(13,3)) {
            Skip.Test("This test is only run on macOS");
            return;
        }
        
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        
        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.EnableWebInspector(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Debugging.IsWebInspectorEnabled).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.WebInspectorEnabled).IsEqualTo(value);
    }
    
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    [SkipOnMacOs("This test verifies the non-macOS unsupported-platform behavior")]
    public async Task AtBuilderStage_ExtensionAssignment_UnhappyFlow(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        
        // Act & Assert
        Assert.Throws<PlatformNotSupportedException>(() => {
            #pragma warning disable CA1416
            builder.EnableWebInspector(value);
            #pragma warning restore CA1416
        });
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Debugging.IsWebInspectorEnabled).IsFalse();
        await Assert.That(initParameters.WebInspectorEnabled).IsFalse();
    }
    
    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    [OnlyRunOnMacOs]
    public async Task AtWindowStage_ThroughBuilderAssignment(bool value, CancellationToken ct) {
        if (!OperatingSystem.IsMacOSVersionAtLeast(13,3)) {
            Skip.Test("This test is only run on macOS");
            return;
        }
        
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            if (!OperatingSystem.IsMacOSVersionAtLeast(13,3)) return;
            builder.Features.Debugging.EnableWebInspector(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Debugging.IsWebInspectorEnabled).IsEqualTo(value);
        await Assert.That(window.Features.Debugging.IsWebInspectorEnabled).IsEqualTo(value);
    }
}
