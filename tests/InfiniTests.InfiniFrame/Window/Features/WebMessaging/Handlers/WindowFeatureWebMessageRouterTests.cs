// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using InfiniFrame;
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge.Dialogs;
using System.Drawing;
using System.Text.Json;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging.Handlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFeatureWebMessageRouterTests {
    [Test]
    [SuppressMessage("ReSharper", "UseCollectionExpression")]
    public async Task RegisteredDispatchers_HaveUniqueNamesAndCoverEveryFeature() {
        string[] expected = [
            "browser", "debugging", "decorations", "filePickerDialogs", "invoke", "javaScript", "lifecycle", "monitors",
            "notifications", "pageNavigation", "position", "size", "state", "webMessaging"
        ];

        IReadOnlyList<string> actual = WindowFeatureWebMessageRouter.RegisteredFeatureNames;

        await Assert.That(actual.Count).IsEqualTo(actual.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        await Assert.That(actual.Order(StringComparer.Ordinal).ToArray())
            .IsEquivalentTo(expected.Order(StringComparer.Ordinal).ToArray());
    }

    [Test]
    public async Task StateGet_SerializesRectangleWithExactWebShape() {
        (IInfiniFrameWindow window, Mock<IStateInfiniFrameWindowFeature> stateMock) = CreateStateWindow();
        stateMock.CachedPreFullScreenBounds.Returns(new Rectangle(1, 2, 800, 600));

        string json = WindowFeatureWebMessageRouter.Get(window, "state", "cachedPreFullScreenBounds", null);

        await Assert.That(json).IsEqualTo("{\"x\":1,\"y\":2,\"width\":800,\"height\":600}");
        stateMock.CachedPreFullScreenBounds.WasCalled(Times.Once);
    }

    [Test]
    public async Task GeometryAndMonitorResults_UseExactContractShapes() {
        var window = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var position = MockFactory.CreatePositionMock();
        var size = MockFactory.CreateSizeMock();
        var monitors = MockFactory.CreateMonitorsMock();
        window.Features.Returns(features.Object);
        features.Position.Returns(position.Object);
        features.Size.Returns(size.Object);
        features.Monitors.Returns(monitors.Object);
        position.Location.Returns(new Point(10, 20));
        size.Size.Returns(new System.Drawing.Size(800, 600));
        monitors.GetMainMonitor().Returns(new InfiniMonitor(
            new Rectangle(0, 0, 1920, 1080), new Rectangle(0, 0, 1920, 1040), 1.25));

        string point = WindowFeatureWebMessageRouter.Get(window.Object, "position", "location", null);
        string dimensions = WindowFeatureWebMessageRouter.Get(window.Object, "size", "size", null);
        string monitor = WindowFeatureWebMessageRouter.Get(window.Object, "monitors", "mainMonitor", null);

        await Assert.That(point).IsEqualTo("{\"x\":10,\"y\":20}");
        await Assert.That(dimensions).IsEqualTo("{\"width\":800,\"height\":600}");
        await Assert.That(monitor).IsEqualTo("{\"monitorArea\":{\"x\":0,\"y\":0,\"width\":1920,\"height\":1080},\"workArea\":{\"x\":0,\"y\":0,\"width\":1920,\"height\":1040},\"scale\":1.25}");
    }

    [Test]
    public async Task LifecycleAndDebuggingResults_UseCamelCaseEnumsDtosAndNulls() {
        var window = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var lifecycle = MockFactory.CreateLifecycleMock();
        var debugging = MockFactory.CreateDebuggingMock();
        window.Features.Returns(features.Object);
        features.Lifecycle.Returns(lifecycle.Object);
        features.Debugging.Returns(debugging.Object);
        lifecycle.State.Returns(InfiniFrameWindowLifecycleState.ClosingRequested);
        debugging.GetDiagnostics().Returns(new InfiniFrameDebugDiagnostics {
            Platform = "windows", Runtime = "net10.0", BrowserRuntime = null,
            Capabilities = new InfiniFrameDebugCapabilities {
                SupportsLocalDevTools = true, SupportsRemoteDebuggingEndpoint = true, SupportsWebInspectorAttach = false,
                SupportsScriptErrorForwarding = true, SupportsNavigationDiagnostics = true
            },
            DevToolsEnabled = true, RemoteDebuggingPort = null, WebInspectorEnabled = false,
            EndpointStatus = InfiniFrameDebugEndpointStatus.Disabled, Endpoint = null, EndpointReason = null,
            IsWindowClosed = false, PlatformNotes = null
        });

        string state = WindowFeatureWebMessageRouter.Get(window.Object, "lifecycle", "state", null);
        string diagnostics = WindowFeatureWebMessageRouter.Get(window.Object, "debugging", "diagnostics", null);
        using JsonDocument document = JsonDocument.Parse(diagnostics);
        JsonElement root = document.RootElement;

        await Assert.That(state).IsEqualTo("\"closeRequested\"");
        await Assert.That(root.GetProperty("endpointStatus").GetString()).IsEqualTo("disabled");
        await Assert.That(root.GetProperty("browserRuntime").ValueKind).IsEqualTo(JsonValueKind.Null);
        await Assert.That(root.GetProperty("remoteDebuggingPort").ValueKind).IsEqualTo(JsonValueKind.Null);
        await Assert.That(root.GetProperty("capabilities").GetProperty("supportsLocalDevTools").GetBoolean()).IsTrue();
    }

    [Test]
    public async Task StatePost_SetsBothCachedBoundsFromRectangleArguments() {
        (IInfiniFrameWindow window, Mock<IStateInfiniFrameWindowFeature> state) = CreateStateWindow();
        var fullScreenBounds = new Rectangle(1, 2, 800, 600);
        var maximizedBounds = new Rectangle(3, 4, 1024, 768);

        WindowFeatureWebMessageRouter.Post(window, "state", "setCachedPreFullScreenBounds", Args("""{"bounds":{"x":1,"y":2,"width":800,"height":600}}"""));
        WindowFeatureWebMessageRouter.Post(window, "state", "setCachedPreMaximizedBounds", Args("""{"bounds":{"x":3,"y":4,"width":1024,"height":768}}"""));

        state.CachedPreFullScreenBounds.Setter.WasCalled(Times.Once);
        state.CachedPreMaximizedBounds.Setter.WasCalled(Times.Once);
        await Task.CompletedTask;
    }

    [Test]
    public async Task OptionalArguments_UseManagedDefaultsWhenMissingOrNull() {
        var windowMock = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var state = MockFactory.CreateStateMock();
        windowMock.Features.Returns(features.Object);
        features.State.Returns(state.Object);
        IInfiniFrameWindow window = windowMock.Object;

        WindowFeatureWebMessageRouter.Post(window, "state", "setMaximized", null);
        WindowFeatureWebMessageRouter.Post(window, "state", "setMinimized", Args("{}"));
        WindowFeatureWebMessageRouter.Post(window, "state", "setFullScreen", Args("""{"fullScreen":null}"""));
        WindowFeatureWebMessageRouter.Post(window, "state", "enableZoom", null);
        WindowFeatureWebMessageRouter.Post(window, "state", "setTopMost", null);

        await Assert.That(Mock.Invocations(state).Count(c => c.MemberName == "SetMaximized")).IsEqualTo(1);
        await Assert.That(Mock.Invocations(state).Count(c => c.MemberName == "SetMinimized")).IsEqualTo(1);
        await Assert.That(Mock.Invocations(state).Count(c => c.MemberName == "SetFullScreen")).IsEqualTo(1);
        await Assert.That(Mock.Invocations(state).Count(c => c.MemberName == "EnableZoom")).IsEqualTo(1);
        await Assert.That(Mock.Invocations(state).Count(c => c.MemberName == "SetTopMost")).IsEqualTo(1);
        await Task.CompletedTask;
    }

    [Test]
    public async Task ComplexArguments_ConvertFiltersAndEnumsExactly() {
        var window = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var filePickers = MockFactory.CreateFilePickerDialogsMock();
        var notifications = MockFactory.CreateNotificationsMock();
        var size = MockFactory.CreateSizeMock();
        window.Features.Returns(features.Object);
        features.FilePickerDialogs.Returns(filePickers.Object);
        features.Notifications.Returns(notifications.Object);
        features.Size.Returns(size.Object);

        var expectedFilters = new[] { ("Text", new[] { "txt", "md" }) };

        object? openResult = WindowFeatureWebMessageRouter.Get(window.Object, "filePickerDialogs", "showOpenFile", Args("""{"title":"Open","defaultPath":null,"multiSelect":true,"filters":[{"name":"Text","extensions":["txt","md"]}]}"""));
        object? showMessageResult = WindowFeatureWebMessageRouter.Get(window.Object, "notifications", "showMessage", Args("""{"title":"Question","text":null,"buttons":"yesNo","icon":"question"}"""));
        WindowFeatureWebMessageRouter.Post(window.Object, "size", "resize", Args("""{"widthOffset":10,"heightOffset":20,"origin":"bottomRight"}"""));

        await Assert.That(openResult).IsNotNull();
        await Assert.That(showMessageResult).IsNotNull();
    }

    [Test]
    public async Task InvalidEnum_HasDeterministicArgumentError() {
        var window = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var size = MockFactory.CreateSizeMock();
        features.Size.Returns(size.Object);
        window.Features.Returns(features.Object);

        var exception = Assert.Throws<ArgumentException>(() =>
            WindowFeatureWebMessageRouter.Post(window.Object, "size", "resize", Args("""{"widthOffset":1,"heightOffset":2,"origin":"diagonal"}""")));

        await Assert.That(exception.Message).IsEqualTo("Argument 'origin' is invalid. (Parameter 'origin')");
    }

    [Test]
    [Arguments(null, "Argument 'bounds' is required.")]
    [Arguments("{}", "Argument 'bounds' is required.")]
    [Arguments("null", "Argument 'bounds' is required.")]
    [Arguments("{\"bounds\":null}", "Argument 'bounds' cannot be null.")]
    [Arguments("{\"bounds\":42}", "Argument 'bounds' is invalid. (Parameter 'bounds')")]
    [Arguments("{\"bounds\":{\"x\":\"wrong\"}}", "Argument 'bounds' is invalid. (Parameter 'bounds')")]
    public async Task RequiredRectangleArgument_InvalidShape_HasDeterministicError(string? json, string expectedMessage) {
        (IInfiniFrameWindow window, Mock<IStateInfiniFrameWindowFeature> _) = CreateStateWindow();

        var exception = Assert.Throws<ArgumentException>(() =>
            WindowFeatureWebMessageRouter.Post(window, "state", "setCachedPreFullScreenBounds", json is null ? null : Args(json)));

        await Assert.That(exception.Message).IsEqualTo(expectedMessage);
    }

    [Test]
    public async Task RoutingPolicy_FeatureIsCaseInsensitiveButCommandAndArgumentsAreCaseSensitive() {
        var windowMock = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var state = MockFactory.CreateStateMock();
        windowMock.Features.Returns(features.Object);
        features.State.Returns(state.Object);
        IInfiniFrameWindow window = windowMock.Object;

        WindowFeatureWebMessageRouter.Post(window, "STATE", "setZoomFactor", Args("""{"zoom":125}"""));
        state.SetZoomFactor(125).WasCalled(Times.Once);

        var commandException = Assert.Throws<InvalidOperationException>(() =>
            WindowFeatureWebMessageRouter.Post(window, "state", "SetZoomFactor", Args("""{"zoom":125}""")));
        var argumentException = Assert.Throws<ArgumentException>(() =>
            WindowFeatureWebMessageRouter.Post(window, "state", "setZoomFactor", Args("""{"Zoom":125}""")));

        await Assert.That(commandException.Message).IsEqualTo("Window feature command 'state:SetZoomFactor' is not supported.");
        await Assert.That(argumentException.Message).IsEqualTo("Argument 'zoom' is required.");
    }

    [Test]
    public async Task UnsupportedFeature_HasDeterministicError() {
        var window = MockFactory.CreateWindowMock().Object;

        var exception = Assert.Throws<InvalidOperationException>(() =>
            WindowFeatureWebMessageRouter.Get(window, "unknown", "anything", null));

        await Assert.That(exception.Message).IsEqualTo("Window feature 'unknown' is not supported.");
    }

    private static (IInfiniFrameWindow Window, Mock<IStateInfiniFrameWindowFeature> State) CreateStateWindow() {
        var window = MockFactory.CreateWindowMock();
        var features = MockFactory.CreateFeaturesMock();
        var state = MockFactory.CreateStateMock();
        window.Features.Returns(features.Object);
        features.State.Returns(state.Object);
        return (window.Object, state);
    }

    private static JsonElement Args(string json) {
        using JsonDocument document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
