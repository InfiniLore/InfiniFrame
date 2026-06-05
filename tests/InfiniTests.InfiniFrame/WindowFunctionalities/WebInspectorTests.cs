// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using System.Runtime.Versioning;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebInspectorTests {
    [Test]
    [DisplayName($"{nameof(WebInspectorTests)}.{nameof(Builder_Enable_ShouldFollowPlatformSupport)}")]
    public async Task Builder_Enable_ShouldFollowPlatformSupport(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        if (OperatingSystem.IsMacOS() && OperatingSystem.IsMacOSVersionAtLeast(13, 3)) {
            builder.Debugging.SetWebInspectorEnabled();
            InfiniFrameNativeParameters nativeParameters = builder.Configuration.ToNativeParameters();

            // Assert
            await Assert.That(builder.Debugging.WebInspectorEnabled).IsTrue();
            await Assert.That(nativeParameters.WebInspectorEnabled).IsTrue();
            return;
        }

        var exception = await Assert.ThrowsAsync<PlatformNotSupportedException>(() => Task.Run(() => {
            #pragma warning disable CA1416
            builder.Debugging.SetWebInspectorEnabled();
            #pragma warning restore CA1416
        }, ct));

        // Assert
        await Assert.That(exception).IsNotNull();
    }

    [Test]
    [OnlyRunOnMacOs]
    [SupportedOSPlatform( "macos13.3")]
    [DisplayName($"{nameof(WebInspectorTests)}.{nameof(Builder_Disable_ShouldAlwaysPropagate)}")]
    public async Task Builder_Disable_ShouldAlwaysPropagate(CancellationToken ct = default) {

        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Debugging.SetWebInspectorEnabled(false);
        InfiniFrameNativeParameters nativeParameters = builder.Configuration.ToNativeParameters();

        // Assert
        await Assert.That(builder.Debugging.WebInspectorEnabled).IsFalse();
        await Assert.That(nativeParameters.WebInspectorEnabled).IsFalse();
    }
}
