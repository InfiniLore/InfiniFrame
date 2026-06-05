// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

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
        bool isSupported = OperatingSystem.IsMacOS() && OperatingSystem.IsMacOSVersionAtLeast(13, 3);

        // Act
        if (isSupported) {
            builder.SetWebInspectorEnabled(true);
            InfiniFrameNativeParameters nativeParameters = builder.Configuration.ToNativeParameters();

            // Assert
            await Assert.That(builder.Configuration.WebInspectorEnabled).IsTrue();
            await Assert.That(nativeParameters.WebInspectorEnabled).IsTrue();
            return;
        }

        var exception = await Assert.ThrowsAsync<PlatformNotSupportedException>(() => Task.Run(() => {
            builder.SetWebInspectorEnabled(true);
        }, ct));

        // Assert
        await Assert.That(exception).IsNotNull();
    }

    [Test]
    [DisplayName($"{nameof(WebInspectorTests)}.{nameof(Builder_Disable_ShouldAlwaysPropagate)}")]
    public async Task Builder_Disable_ShouldAlwaysPropagate(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetWebInspectorEnabled(false);
        InfiniFrameNativeParameters nativeParameters = builder.Configuration.ToNativeParameters();

        // Assert
        await Assert.That(builder.Configuration.WebInspectorEnabled).IsFalse();
        await Assert.That(nativeParameters.WebInspectorEnabled).IsFalse();
    }
}
