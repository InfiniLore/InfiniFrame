// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowConfigurationTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task StartupParameters_Default_ShouldBeNull(CancellationToken ct = default) {
        // Arrange

        // Act
        var config = new InfiniFrameWindowConfiguration();

        // Assert
        await Assert.That(config.StartupParameters).IsNull();
    }

    [Test]
    public async Task ParentWindow_Default_ShouldBeNull(CancellationToken ct = default) {
        // Arrange

        // Act
        var config = new InfiniFrameWindowConfiguration();

        // Assert
        await Assert.That(config.ParentWindow).IsNull();
    }

    [Test]
    public async Task AssignNativeParameters_ShouldSetStartupParameters(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowConfiguration();
        var parameters = new InfiniFrameNativeParameters();

        // Act
        config.AssignNativeParameters(parameters);

        // Assert
        await Assert.That(config.StartupParameters).IsSameReferenceAs(parameters);
    }

    [Test]
    public async Task ChildWindows_ShouldBeEmptyByDefault(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowConfiguration();

        // Act
        IReadOnlyList<IInfiniFrameWindow> children = ((IInfiniFrameWindowConfiguration)config).ChildWindows;

        // Assert
        await Assert.That(children.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ParentWindow_Settable(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowConfiguration();
        var mock = MockFactory.CreateWindowMock();

        // Act
        config.ParentWindow = mock.Object;

        // Assert
        await Assert.That(config.ParentWindow).IsSameReferenceAs(mock.Object);
    }
}
