// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeaturesTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AllFeatures_ShouldBeInitialized(CancellationToken ct = default) {
        // Arrange

        // Act
        var features = new InfiniFrameWindowBuilderFeatures();

        // Assert
        await Assert.That(features.Debugging).IsNotNull();
        await Assert.That(features.Browser).IsNotNull();
        await Assert.That(features.Decorations).IsNotNull();
        await Assert.That(features.Notifications).IsNotNull();
        await Assert.That(features.PageNavigation).IsNotNull();
        await Assert.That(features.Position).IsNotNull();
        await Assert.That(features.Size).IsNotNull();
        await Assert.That(features.State).IsNotNull();
        await Assert.That(features.InstanceArbitration).IsNotNull();
        await Assert.That(features.Menu).IsNotNull();
    }

    [Test]
    public async Task ApplyToNativeParameters_ShouldNotThrow(CancellationToken ct = default) {
        // Arrange
        var features = new InfiniFrameWindowBuilderFeatures();
        var parameters = new InfiniFrameNativeParameters();

        // Act
        features.ApplyToNativeParameters(ref parameters);

        // Assert
        await Assert.That(parameters).IsEquivalentTo(parameters);
    }

    [Test]
    public async Task Debugging_DefaultDevTools_ShouldBeEnabled(CancellationToken ct = default) {
        // Arrange
        var features = new InfiniFrameWindowBuilderFeatures();

        // Act & Assert
        await Assert.That(features.Debugging.IsDevToolsEnabled).IsTrue();
    }
}
