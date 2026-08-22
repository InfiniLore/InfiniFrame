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
    // ParentWindow
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ParentWindow_Default_ShouldBeNull(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowConfiguration();

        // Assert
        await Assert.That(config.ParentWindow).IsNull();
    }

    [Test]
    public async Task ParentWindow_Settable(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowConfiguration();
        Mock<IInfiniFrameWindow> mock = MockFactory.CreateWindowMock();

        // Act
        config.ParentWindow = mock.Object;

        // Assert
        await Assert.That(config.ParentWindow).IsSameReferenceAs(mock.Object);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ChildWindowsInternal
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ChildWindowsInternal_Default_ShouldBeEmpty(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowConfiguration();

        // Assert
        await Assert.That(config.ChildWindowsInternal.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ChildWindowsInternal_Addable(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowConfiguration();
        Mock<IInfiniFrameWindow> mock = MockFactory.CreateWindowMock();

        // Act
        config.ChildWindowsInternal.Add(mock.Object);

        // Assert
        await Assert.That(config.ChildWindowsInternal.Count).IsEqualTo(1);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ChildWindowsLock
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ChildWindowsLock_IsNotNull(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowConfiguration();

        // Assert
        await Assert.That(config.ChildWindowsLock).IsNotNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // AssignNativeParameters
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AssignNativeParameters_SetsStartupParameters(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowConfiguration();
        var parameters = new InfiniFrameNativeParameters {
            Title = "Test Window",
            Width = 800,
            Height = 600
        };

        // Act
        config.AssignNativeParameters(parameters);

        // Assert
        await Assert.That(config.StartupParameters.Title).IsEqualTo("Test Window");
        await Assert.That(config.StartupParameters.Width).IsEqualTo(800);
        await Assert.That(config.StartupParameters.Height).IsEqualTo(600);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ChildWindows interface accessor
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ChildWindows_InterfaceAccessor_ReturnsInternalList(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowConfiguration();
        Mock<IInfiniFrameWindow> mock = MockFactory.CreateWindowMock();

        // Act
        config.ChildWindowsInternal.Add(mock.Object);

        // Assert
        IInfiniFrameWindowConfiguration ifaceConfig = config;
        await Assert.That(ifaceConfig.ChildWindows.Count).IsEqualTo(1);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // StartupParameters defaults
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task StartupParameters_Default_HasDefaultValues(CancellationToken ct = default) {
        // Arrange
        var config = new InfiniFrameWindowConfiguration();

        // Assert
        // StartupParameters is default struct - fields are zeroed
        await Assert.That(config.StartupParameters.Width).IsEqualTo(0);
        await Assert.That(config.StartupParameters.Height).IsEqualTo(0);
    }
}
