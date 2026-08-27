// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging.Handlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebMessageDispatcherTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Browser Dispatcher Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task BrowserDispatcher_Get_IsContextMenuEnabled_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, Mock<IBrowserInfiniFrameWindowFeature> mock) = CreateBrowserWindow();
        mock.IsContextMenuEnabled.Returns(true);

        // Act
        string json = WindowFeatureWebMessageRouter.Get(window, "browser", "isContextMenuEnabled", null);

        // Assert
        await Assert.That(json).IsEqualTo("true");
    }

    [Test]
    public async Task BrowserDispatcher_Get_UserAgent_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, Mock<IBrowserInfiniFrameWindowFeature> mock) = CreateBrowserWindow();
        mock.UserAgent.Returns("TestAgent/1.0");

        // Act
        string json = WindowFeatureWebMessageRouter.Get(window, "browser", "userAgent", null);

        // Assert
        await Assert.That(json).IsEqualTo("\"TestAgent/1.0\"");
    }

    [Test]
    public async Task BrowserDispatcher_Get_UnsupportedCommand_Throws(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, _) = CreateBrowserWindow();

        // Act & Assert
        await Assert.That(() => WindowFeatureWebMessageRouter.Get(window, "browser", "unsupported", null))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task BrowserDispatcher_Post_EnableContextMenu_CallsFeature(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, Mock<IBrowserInfiniFrameWindowFeature> mock) = CreateBrowserWindow();
        JsonElement args = JsonDocument.Parse("""{"enabled": false}""").RootElement;

        // Act
        WindowFeatureWebMessageRouter.Post(window, "browser", "enableContextMenu", args);

        // Assert
        mock.EnableContextMenu(false);
    }

    [Test]
    public async Task BrowserDispatcher_Post_SetUserAgent_CallsFeature(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, Mock<IBrowserInfiniFrameWindowFeature> mock) = CreateBrowserWindow();
        JsonElement args = JsonDocument.Parse("""{"userAgent": "CustomAgent"}""").RootElement;

        // Act
        WindowFeatureWebMessageRouter.Post(window, "browser", "setUserAgent", args);

        // Assert
        mock.SetUserAgent("CustomAgent");
    }

    [Test]
    public async Task BrowserDispatcher_Post_SetUserAgent_NullUserAgent_CallsFeatureWithNull(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, Mock<IBrowserInfiniFrameWindowFeature> mock) = CreateBrowserWindow();
        JsonElement args = JsonDocument.Parse("""{"userAgent": null}""").RootElement;

        // Act
        WindowFeatureWebMessageRouter.Post(window, "browser", "setUserAgent", args);

        // Assert
        mock.SetUserAgent(Any<string?>()).WasCalled(Times.Once);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // State Dispatcher Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task StateDispatcher_Get_IsFullScreen_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, Mock<IStateInfiniFrameWindowFeature> mock) = CreateStateWindow();
        mock.IsFullScreen.Returns(true);

        // Act
        string json = WindowFeatureWebMessageRouter.Get(window, "state", "isFullScreen", null);

        // Assert
        await Assert.That(json).IsEqualTo("true");
    }

    [Test]
    public async Task StateDispatcher_Get_IsMaximized_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, Mock<IStateInfiniFrameWindowFeature> mock) = CreateStateWindow();
        mock.IsMaximized.Returns(false);

        // Act
        string json = WindowFeatureWebMessageRouter.Get(window, "state", "isMaximized", null);

        // Assert
        await Assert.That(json).IsEqualTo("false");
    }

    [Test]
    public async Task StateDispatcher_Post_SetFullScreen_CallsFeature(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, Mock<IStateInfiniFrameWindowFeature> mock) = CreateStateWindow();
        JsonElement args = JsonDocument.Parse("""{"fullScreen": true}""").RootElement;

        // Act
        WindowFeatureWebMessageRouter.Post(window, "state", "setFullScreen", args);

        // Assert
        mock.SetFullScreen(true);
    }

    [Test]
    public async Task StateDispatcher_Post_ToggleMaximized_CallsFeature(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, Mock<IStateInfiniFrameWindowFeature> mock) = CreateStateWindow();

        // Act
        WindowFeatureWebMessageRouter.Post(window, "state", "toggleMaximized", null);

        // Assert
        mock.ToggleMaximized();
    }

    [Test]
    public async Task StateDispatcher_Post_SetFocused_CallsFeature(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, Mock<IStateInfiniFrameWindowFeature> mock) = CreateStateWindow();

        // Act
        WindowFeatureWebMessageRouter.Post(window, "state", "setFocused", null);

        // Assert
        mock.SetFocused();
    }

    [Test]
    public async Task StateDispatcher_Get_UnsupportedCommand_Throws(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, _) = CreateStateWindow();

        // Act & Assert
        await Assert.That(() => WindowFeatureWebMessageRouter.Get(window, "state", "unsupported", null))
            .Throws<InvalidOperationException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Position Dispatcher Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task PositionDispatcher_Get_Location_ReturnsPoint(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IPositionInfiniFrameWindowFeature> position = MockFactory.CreatePositionMock();
        window.Features.Returns(features.Object);
        features.Position.Returns(position.Object);
        position.Location.Returns(new System.Drawing.Point(100, 200));

        // Act
        string json = WindowFeatureWebMessageRouter.Get(window.Object, "position", "location", null);

        // Assert
        await Assert.That(json).IsEqualTo("{\"x\":100,\"y\":200}");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Size Dispatcher Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task SizeDispatcher_Get_Size_ReturnsDimensions(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<ISizeInfiniFrameWindowFeature> size = MockFactory.CreateSizeMock();
        window.Features.Returns(features.Object);
        features.Size.Returns(size.Object);
        size.Size.Returns(new System.Drawing.Size(800, 600));

        // Act
        string json = WindowFeatureWebMessageRouter.Get(window.Object, "size", "size", null);

        // Assert
        await Assert.That(json).IsEqualTo("{\"width\":800,\"height\":600}");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Monitors Dispatcher Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task MonitorsDispatcher_Get_MainMonitor_ReturnsMonitor(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IMonitorsInfiniFrameWindowFeature> monitors = MockFactory.CreateMonitorsMock();
        window.Features.Returns(features.Object);
        features.Monitors.Returns(monitors.Object);
        monitors.GetMainMonitor().Returns(new InfiniMonitor(
            new System.Drawing.Rectangle(0, 0, 1920, 1080),
            new System.Drawing.Rectangle(0, 0, 1920, 1040),
            1.5));

        // Act
        string json = WindowFeatureWebMessageRouter.Get(window.Object, "monitors", "mainMonitor", null);

        // Assert
        await Assert.That(json).Contains("\"scale\":1.5");
        await Assert.That(json).Contains("\"width\":1920");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Lifecycle Dispatcher Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task LifecycleDispatcher_Get_State_ReturnsLifecycleState(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<ILifecycleInfiniFrameWindowFeature> lifecycle = MockFactory.CreateLifecycleMock();
        window.Features.Returns(features.Object);
        features.Lifecycle.Returns(lifecycle.Object);
        lifecycle.State.Returns(InfiniFrameWindowLifecycleState.Ready);

        // Act
        string json = WindowFeatureWebMessageRouter.Get(window.Object, "lifecycle", "state", null);

        // Assert
        await Assert.That(json).IsEqualTo("\"ready\"");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Decorations Dispatcher Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task DecorationsDispatcher_Get_Title_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IDecorationsInfiniFrameWindowFeature> decorations = MockFactory.CreateDecorationsMock();
        window.Features.Returns(features.Object);
        features.Decorations.Returns(decorations.Object);
        decorations.Title.Returns("My Window");

        // Act
        string json = WindowFeatureWebMessageRouter.Get(window.Object, "decorations", "title", null);

        // Assert
        await Assert.That(json).IsEqualTo("\"My Window\"");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // JavaScript Dispatcher Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task JavaScriptDispatcher_Get_UnsupportedCommand_Throws(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, _) = CreateBrowserWindow();

        // Act & Assert
        await Assert.That(() => WindowFeatureWebMessageRouter.Get(window, "javaScript", "unsupported", null))
            .Throws<InvalidOperationException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Debugging Dispatcher Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task DebuggingDispatcher_Get_UnsupportedCommand_Throws(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, _) = CreateBrowserWindow();

        // Act & Assert
        await Assert.That(() => WindowFeatureWebMessageRouter.Get(window, "debugging", "unsupported", null))
            .Throws<InvalidOperationException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // WebMessaging Dispatcher Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task WebMessagingDispatcher_UnsupportedCommand_Throws(CancellationToken ct = default) {
        // Arrange
        (IInfiniFrameWindow window, _) = CreateBrowserWindow();

        // Act & Assert
        await Assert.That(() => WindowFeatureWebMessageRouter.Get(window, "webMessaging", "unsupported", null))
            .Throws<InvalidOperationException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static (IInfiniFrameWindow Window, Mock<IBrowserInfiniFrameWindowFeature> Mock) CreateBrowserWindow() {
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IBrowserInfiniFrameWindowFeature> browser = MockFactory.CreateBrowserMock();
        window.Features.Returns(features.Object);
        features.Browser.Returns(browser.Object);
        return (window.Object, browser);
    }

    private static (IInfiniFrameWindow Window, Mock<IStateInfiniFrameWindowFeature> Mock) CreateStateWindow() {
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IStateInfiniFrameWindowFeature> state = MockFactory.CreateStateMock();
        window.Features.Returns(features.Object);
        features.State.Returns(state.Object);
        return (window.Object, state);
    }
}
