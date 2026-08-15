// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderConfigurationTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ParentWindow_Default_ShouldBeNull(CancellationToken ct = default) {
        // Arrange

        // Act
        var config = new InfiniFrameWindowBuilderConfiguration();

        // Assert
        await Assert.That(config.ParentWindow).IsNull();
    }

    [Test]
    public async Task ChildWindows_ShouldBeEmptyByDefault(CancellationToken ct = default) {
        // Arrange

        // Act
        var config = new InfiniFrameWindowBuilderConfiguration();

        // Assert
        await Assert.That(config.ChildWindows.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ApplyToNativeParameters_ShouldNotThrow(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowBuilderConfiguration();
        var parameters = new InfiniFrameNativeParameters();

        // Act
        config.ApplyToNativeParameters(ref parameters);

        // Assert
        await Assert.That(parameters.Equals(parameters)).IsTrue();
    }

    [Test]
    public async Task ParentWindow_Settable(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowBuilderConfiguration();
        Mock<IInfiniFrameWindow> mock = MockFactory.CreateWindowMock();

        // Act
        config.ParentWindow = mock.Object;

        // Assert
        await Assert.That(config.ParentWindow).IsSameReferenceAs(mock.Object);
    }

    [Test]
    public async Task ChildWindows_Addable(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowBuilderConfiguration();
        Mock<IInfiniFrameWindow> mock = MockFactory.CreateWindowMock();

        // Act
        config.ChildWindows.Add(mock.Object);

        // Assert
        await Assert.That(config.ChildWindows.Count).IsEqualTo(1);
        await Assert.That(config.ChildWindows[0]).IsSameReferenceAs(mock.Object);
    }
}
