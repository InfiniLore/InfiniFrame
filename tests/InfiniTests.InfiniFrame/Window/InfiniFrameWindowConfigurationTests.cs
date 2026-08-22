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

    [Test]
    public async Task ParentWindow_Default_ShouldBeNull(CancellationToken ct = default) {
        var config = new InfiniFrameWindowConfiguration();

        await Assert.That(config.ParentWindow).IsNull();
    }

    [Test]
    public async Task ParentWindow_Settable(CancellationToken ct = default) {
        var config = new InfiniFrameWindowConfiguration();
        Mock<IInfiniFrameWindow> mock = MockFactory.CreateWindowMock();

        config.ParentWindow = mock.Object;

        await Assert.That(config.ParentWindow).IsSameReferenceAs(mock.Object);
    }

    [Test]
    public async Task ChildWindowsInternal_Default_ShouldBeEmpty(CancellationToken ct = default) {
        var config = new InfiniFrameWindowConfiguration();

        await Assert.That(config.ChildWindowsInternal.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ChildWindowsInternal_Addable(CancellationToken ct = default) {
        var config = new InfiniFrameWindowConfiguration();
        Mock<IInfiniFrameWindow> mock = MockFactory.CreateWindowMock();

        config.ChildWindowsInternal.Add(mock.Object);

        await Assert.That(config.ChildWindowsInternal.Count).IsEqualTo(1);
    }

    [Test]
    public async Task ChildWindowsLock_IsNotNull(CancellationToken ct = default) {
        var config = new InfiniFrameWindowConfiguration();

        await Assert.That(config.ChildWindowsLock).IsNotNull();
    }

    [Test]
    public async Task AssignNativeParameters_SetsStartupParameters(CancellationToken ct = default) {
        var config = new InfiniFrameWindowConfiguration();
        var parameters = new InfiniFrameNativeParameters {
            Title = "Test Window",
            Width = 800,
            Height = 600
        };

        config.AssignNativeParameters(parameters);

        await Assert.That(config.StartupParameters.Title).IsEqualTo("Test Window");
        await Assert.That(config.StartupParameters.Width).IsEqualTo(800);
        await Assert.That(config.StartupParameters.Height).IsEqualTo(600);
    }

    [Test]
    public async Task ChildWindows_InterfaceAccessor_ReturnsInternalList(CancellationToken ct = default) {
        var config = new InfiniFrameWindowConfiguration();
        Mock<IInfiniFrameWindow> mock = MockFactory.CreateWindowMock();

        config.ChildWindowsInternal.Add(mock.Object);

        IInfiniFrameWindowConfiguration ifaceConfig = config;
        await Assert.That(ifaceConfig.ChildWindows.Count).IsEqualTo(1);
    }

    [Test]
    public async Task StartupParameters_Default_HasDefaultValues(CancellationToken ct = default) {
        var config = new InfiniFrameWindowConfiguration();

        // StartupParameters is default struct - fields are zeroed
        await Assert.That(config.StartupParameters.Width).IsEqualTo(0);
        await Assert.That(config.StartupParameters.Height).IsEqualTo(0);
    }
}
