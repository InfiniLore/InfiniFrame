// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge.Dialogs;
using NSubstitute;
using System.Drawing;
using System.Text.Json;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging.Handlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFeatureWebMessageRouterTests {
    [Test]
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
        (IInfiniFrameWindow window, IStateInfiniFrameWindowFeature state) = CreateStateWindow();
        state.CachedPreFullScreenBounds.Returns(new Rectangle(1, 2, 800, 600));

        string json = WindowFeatureWebMessageRouter.Get(window, "state", "cachedPreFullScreenBounds", null);

        await Assert.That(json).IsEqualTo("{\"x\":1,\"y\":2,\"width\":800,\"height\":600}");
        _ = state.Received(1).CachedPreFullScreenBounds;
    }

    [Test]
    public async Task GeometryAndMonitorResults_UseExactContractShapes() {
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        var position = Substitute.For<IPositionInfiniFrameWindowFeature>();
        var size = Substitute.For<ISizeInfiniFrameWindowFeature>();
        var monitors = Substitute.For<IMonitorsInfiniFrameWindowFeature>();
        window.Features.Returns(features);
        features.Position.Returns(position);
        features.Size.Returns(size);
        features.Monitors.Returns(monitors);
        position.Location.Returns(new Point(10, 20));
        size.Size.Returns(new System.Drawing.Size(800, 600));
        monitors.GetMainMonitor().Returns(new InfiniMonitor(
            new Rectangle(0, 0, 1920, 1080), new Rectangle(0, 0, 1920, 1040), 1.25));

        string point = WindowFeatureWebMessageRouter.Get(window, "position", "location", null);
        string dimensions = WindowFeatureWebMessageRouter.Get(window, "size", "size", null);
        string monitor = WindowFeatureWebMessageRouter.Get(window, "monitors", "mainMonitor", null);

        await Assert.That(point).IsEqualTo("{\"x\":10,\"y\":20}");
        await Assert.That(dimensions).IsEqualTo("{\"width\":800,\"height\":600}");
        await Assert.That(monitor).IsEqualTo("{\"monitorArea\":{\"x\":0,\"y\":0,\"width\":1920,\"height\":1080},\"workArea\":{\"x\":0,\"y\":0,\"width\":1920,\"height\":1040},\"scale\":1.25}");
    }

    [Test]
    public async Task LifecycleAndDebuggingResults_UseCamelCaseEnumsDtosAndNulls() {
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        var lifecycle = Substitute.For<ILifecycleInfiniFrameWindowFeature>();
        var debugging = Substitute.For<IDebuggingInfiniFrameWindowFeature>();
        window.Features.Returns(features);
        features.Lifecycle.Returns(lifecycle);
        features.Debugging.Returns(debugging);
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

        string state = WindowFeatureWebMessageRouter.Get(window, "lifecycle", "state", null);
        string diagnostics = WindowFeatureWebMessageRouter.Get(window, "debugging", "diagnostics", null);
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
        (IInfiniFrameWindow window, IStateInfiniFrameWindowFeature state) = CreateStateWindow();
        var fullScreenBounds = new Rectangle(1, 2, 800, 600);
        var maximizedBounds = new Rectangle(3, 4, 1024, 768);

        WindowFeatureWebMessageRouter.Post(window, "state", "setCachedPreFullScreenBounds", Args("""{"bounds":{"x":1,"y":2,"width":800,"height":600}}"""));
        WindowFeatureWebMessageRouter.Post(window, "state", "setCachedPreMaximizedBounds", Args("""{"bounds":{"x":3,"y":4,"width":1024,"height":768}}"""));

        state.Received(1).CachedPreFullScreenBounds = fullScreenBounds;
        state.Received(1).CachedPreMaximizedBounds = maximizedBounds;
        await Task.CompletedTask;
    }

    [Test]
    public async Task OptionalArguments_UseManagedDefaultsWhenMissingOrNull() {
        (IInfiniFrameWindow window, IStateInfiniFrameWindowFeature state) = CreateStateWindow();

        WindowFeatureWebMessageRouter.Post(window, "state", "setMaximized", null);
        WindowFeatureWebMessageRouter.Post(window, "state", "setMinimized", Args("{}"));
        WindowFeatureWebMessageRouter.Post(window, "state", "setFullScreen", Args("""{"fullScreen":null}"""));
        WindowFeatureWebMessageRouter.Post(window, "state", "enableZoom", null);
        WindowFeatureWebMessageRouter.Post(window, "state", "setTopMost", null);

        state.Received(1).SetMaximized();
        state.Received(1).SetMinimized();
        state.Received(1).SetFullScreen();
        state.Received(1).EnableZoom();
        state.Received(1).SetTopMost();
        await Task.CompletedTask;
    }

    [Test]
    public async Task ComplexArguments_ConvertFiltersAndEnumsExactly() {
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        var filePickers = Substitute.For<IFilePickerDialogsInfiniFrameWindowFeature>();
        var notifications = Substitute.For<INotificationsInfiniFrameWindowFeature>();
        var size = Substitute.For<ISizeInfiniFrameWindowFeature>();
        window.Features.Returns(features);
        features.FilePickerDialogs.Returns(filePickers);
        features.Notifications.Returns(notifications);
        features.Size.Returns(size);

        WindowFeatureWebMessageRouter.Get(window, "filePickerDialogs", "showOpenFile", Args("""{"title":"Open","defaultPath":null,"multiSelect":true,"filters":[{"name":"Text","extensions":["txt","md"]}]}"""));
        WindowFeatureWebMessageRouter.Get(window, "notifications", "showMessage", Args("""{"title":"Question","text":null,"buttons":"yesNo","icon":"question"}"""));
        WindowFeatureWebMessageRouter.Post(window, "size", "resize", Args("""{"widthOffset":10,"heightOffset":20,"origin":"bottomRight"}"""));

        filePickers.Received(1).ShowOpenFile(
            "Open", null, true,
            Arg.Is<(string Name, string[] Extensions)[]?>(filters => filters != null
                && filters.Length == 1
                && filters[0].Name == "Text"
                && filters[0].Extensions.SequenceEqual(new[] { "txt", "md" })));
        notifications.Received(1).ShowMessage("Question", null, InfiniFrameDialogButtons.YesNo, InfiniFrameDialogIcon.Question);
        size.Received(1).Resize(10, 20, ResizeOrigin.BottomRight);
        await Task.CompletedTask;
    }

    [Test]
    public async Task InvalidEnum_HasDeterministicArgumentError() {
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        features.Size.Returns(Substitute.For<ISizeInfiniFrameWindowFeature>());
        window.Features.Returns(features);

        var exception = Assert.Throws<ArgumentException>(() =>
            WindowFeatureWebMessageRouter.Post(window, "size", "resize", Args("""{"widthOffset":1,"heightOffset":2,"origin":"diagonal"}""")));

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
        (IInfiniFrameWindow window, _) = CreateStateWindow();

        var exception = Assert.Throws<ArgumentException>(() =>
            WindowFeatureWebMessageRouter.Post(window, "state", "setCachedPreFullScreenBounds", json is null ? null : Args(json)));

        await Assert.That(exception.Message).IsEqualTo(expectedMessage);
    }

    [Test]
    public async Task RoutingPolicy_FeatureIsCaseInsensitiveButCommandAndArgumentsAreCaseSensitive() {
        (IInfiniFrameWindow window, IStateInfiniFrameWindowFeature state) = CreateStateWindow();

        WindowFeatureWebMessageRouter.Post(window, "STATE", "setZoomFactor", Args("""{"zoom":125}"""));
        state.Received(1).SetZoomFactor(125);

        var commandException = Assert.Throws<InvalidOperationException>(() =>
            WindowFeatureWebMessageRouter.Post(window, "state", "SetZoomFactor", Args("""{"zoom":125}""")));
        var argumentException = Assert.Throws<ArgumentException>(() =>
            WindowFeatureWebMessageRouter.Post(window, "state", "setZoomFactor", Args("""{"Zoom":125}""")));

        await Assert.That(commandException.Message).IsEqualTo("Window feature command 'state:SetZoomFactor' is not supported.");
        await Assert.That(argumentException.Message).IsEqualTo("Argument 'zoom' is required.");
    }

    [Test]
    public async Task UnsupportedFeature_HasDeterministicError() {
        var window = Substitute.For<IInfiniFrameWindow>();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            WindowFeatureWebMessageRouter.Get(window, "unknown", "anything", null));

        await Assert.That(exception.Message).IsEqualTo("Window feature 'unknown' is not supported.");
    }

    private static (IInfiniFrameWindow Window, IStateInfiniFrameWindowFeature State) CreateStateWindow() {
        var window = Substitute.For<IInfiniFrameWindow>();
        var features = Substitute.For<IInfiniFrameWindowFeatures>();
        var state = Substitute.For<IStateInfiniFrameWindowFeature>();
        window.Features.Returns(features);
        features.State.Returns(state);
        return (window, state);
    }

    private static JsonElement Args(string json) {
        using JsonDocument document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}