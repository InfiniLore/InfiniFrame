// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DecorationsInfiniFrameWindowBuilderFeatureTests {

    [Test]
    public async Task DefaultValues_AreCorrect(CancellationToken ct = default) {
        // Arrange & Act
        var feature = new DecorationsInfiniFrameWindowBuilderFeature();

        // Assert
        await Assert.That(feature.IsChromeless).IsFalse();
        await Assert.That(feature.IsTransparent).IsFalse();
        await Assert.That(feature.BackgroundColor).IsNull();
        await Assert.That(feature.Title).IsEqualTo("InfiniFrame");
        await Assert.That(feature.IconFilePath).IsNull();
#pragma warning disable CS0618 // Type or member is obsolete
        await Assert.That(feature.WindowsAppUserModelId).IsNull();
#pragma warning restore CS0618
        await Assert.That(feature.LimitLinuxWindowTitleLength).IsFalse();
    }

    [Test]
    public async Task SetChromeless_TogglesValue(CancellationToken ct = default) {
        // Arrange
        var feature = new DecorationsInfiniFrameWindowBuilderFeature();

        // Act
        feature.SetChromeless(true);

        // Assert
        await Assert.That(feature.IsChromeless).IsTrue();
    }

    [Test]
    public async Task SetTransparent_TogglesValue(CancellationToken ct = default) {
        // Arrange
        var feature = new DecorationsInfiniFrameWindowBuilderFeature();

        // Act
        feature.SetTransparent(true);

        // Assert
        await Assert.That(feature.IsTransparent).IsTrue();
    }

    [Test]
    public async Task SetBackgroundColor_SetsValue(CancellationToken ct = default) {
        // Arrange
        var feature = new DecorationsInfiniFrameWindowBuilderFeature();

        // Act
        feature.SetBackgroundColor("#FF0000");

        // Assert
        await Assert.That(feature.BackgroundColor).IsEqualTo("#FF0000");
    }

    [Test]
    public async Task SetTitle_SetsValue(CancellationToken ct = default) {
        // Arrange
        var feature = new DecorationsInfiniFrameWindowBuilderFeature();

        // Act
        feature.SetTitle("My Window");

        // Assert
        await Assert.That(feature.Title).IsEqualTo("My Window");
    }

    [Test]
    public async Task SetIconFile_SetsValue(CancellationToken ct = default) {
        // Arrange
        var feature = new DecorationsInfiniFrameWindowBuilderFeature();

        // Act
        feature.SetIconFile("/path/to/icon.png");

        // Assert
        await Assert.That(feature.IconFilePath).IsEqualTo("/path/to/icon.png");
    }

    [Test]
    public async Task SetWindowsAppUserModelId_SetsValue(CancellationToken ct = default) {
        // Arrange
        var feature = new DecorationsInfiniFrameWindowBuilderFeature();

        // Act
#pragma warning disable CS0618 // Type or member is obsolete
        feature.SetWindowsAppUserModelId("com.myapp");
#pragma warning restore CS0618

        // Assert
        await Assert.That(feature.WindowsAppUserModelId).IsEqualTo("com.myapp");
    }

    [Test]
    public async Task SetLimitLinuxWindowTitleLength_TogglesValue(CancellationToken ct = default) {
        // Arrange
        var feature = new DecorationsInfiniFrameWindowBuilderFeature();

        // Act
        feature.SetLimitLinuxWindowTitleLength(true);

        // Assert
        await Assert.That(feature.LimitLinuxWindowTitleLength).IsTrue();
    }

    [Test]
    public async Task ApplyToNativeParameters_SetsChromelessAndTransparent(CancellationToken ct = default) {
        // Arrange
        var feature = new DecorationsInfiniFrameWindowBuilderFeature();
        feature.SetChromeless(true);
        feature.SetTransparent(true);
        feature.SetTitle("Test Title");

        var parameters = new InfiniFrameNativeParameters();

        // Act
        feature.ApplyToNativeParameters(ref parameters);

        // Assert
        await Assert.That(parameters.Chromeless).IsTrue();
        await Assert.That(parameters.Transparent).IsTrue();
        await Assert.That(parameters.Title).IsEqualTo("Test Title");
    }

    [Test]
    public async Task ApplyToNativeParameters_SetsWindowsAppUserModelId(CancellationToken ct = default) {
        // Arrange
        var feature = new DecorationsInfiniFrameWindowBuilderFeature();
#pragma warning disable CS0618 // Type or member is obsolete
        feature.SetWindowsAppUserModelId("my.app.id");
#pragma warning restore CS0618

        var parameters = new InfiniFrameNativeParameters();

        // Act
        feature.ApplyToNativeParameters(ref parameters);

        // Assert — WindowsAppUserModelId is now an application-level setting, no longer set on window parameters.
    }
}
