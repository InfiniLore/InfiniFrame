// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace InfiniTests.InfiniFrame.BlazorWebView;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorAppBuilderTests {
    private sealed class TestJsComponent : IComponent {
        public void Attach(RenderHandle renderHandle) { }

        public Task SetParametersAsync(ParameterView parameters) => Task.CompletedTask;
    }

    private sealed class DisposeProbe : IDisposable {
        public bool IsDisposed { get; private set; }

        public void Dispose() {
            IsDisposed = true;
        }
    }

    private sealed class RecordingUnhandledExceptionSource : IInfiniFrameUnhandledExceptionSource {
        private int _activeHandlers;
        private int _registrationCount;

        public int ActiveHandlers => Volatile.Read(ref _activeHandlers);
        public int RegistrationCount => Volatile.Read(ref _registrationCount);

        public IDisposable Register(UnhandledExceptionEventHandler handler) {
            _ = handler;
            Interlocked.Increment(ref _registrationCount);
            Interlocked.Increment(ref _activeHandlers);
            return new Subscription(this);
        }

        private sealed class Subscription : IDisposable {
            private RecordingUnhandledExceptionSource? _owner;

            public Subscription(RecordingUnhandledExceptionSource owner) {
                _owner = owner;
            }

            public void Dispose() {
                RecordingUnhandledExceptionSource? owner = Interlocked.Exchange(ref _owner, null);
                if (owner is null) return;
                Interlocked.Decrement(ref owner._activeHandlers);
            }
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Build_WithExternalProvider_ShouldUseProvidedServiceProvider(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();
        ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

        // Act
        InfiniFrameBlazorApp app = builder.Build(serviceProvider);

        // Assert
        await Assert.That(app.ServiceProvider).IsSameReferenceAs(serviceProvider);
        await app.DisposeAsync();
    }

    [Test]
    public async Task Build_WithoutProvider_ShouldCreateServiceProvider(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();

        // Act
        InfiniFrameBlazorApp app = builder.Build();

        // Assert
        await Assert.That(app.ServiceProvider).IsNotNull();
        await app.DisposeAsync();
    }

    [Test]
    public async Task CreateDefault_RootComponents_ImplementsIJsComponentConfiguration(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();

        // Act
        IJSComponentConfiguration configuration = builder.RootComponents;

        // Assert
        await Assert.That(configuration.JSComponents).IsSameReferenceAs(builder.RootComponents.JSComponents);
    }

    [Test]
    public async Task CreateDefault_RootComponents_RegisterForJavaScript_WritesToSharedStore(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();

        // Act
        builder.RootComponents.RegisterForJavaScript<TestJsComponent>("test-js-component");

        // Assert
        ServiceProvider provider = builder.Services.BuildServiceProvider();
        var store = provider.GetRequiredService<JSComponentConfigurationStore>();
        var config = provider.GetRequiredService<IInfiniFrameJsComponentConfiguration>();

        await Assert.That(store).IsSameReferenceAs(builder.RootComponents.JSComponents);
        await Assert.That(config.JSComponents).IsSameReferenceAs(builder.RootComponents.JSComponents);
    }

    [Test]
    public async Task GlobalUnhandledExceptionHandler_IsRemovedOnDispose(CancellationToken ct = default) {
        // Arrange
        var recordingSource = new RecordingUnhandledExceptionSource();
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();
        builder.Services.RemoveAll<IInfiniFrameUnhandledExceptionSource>();
        builder.Services.AddSingleton<IInfiniFrameUnhandledExceptionSource>(recordingSource);

        // Act
        InfiniFrameBlazorApp app = builder.Build();

        // Assert
        await Assert.That(recordingSource.ActiveHandlers).IsEqualTo(1);
        await app.DisposeAsync();
        await Assert.That(recordingSource.ActiveHandlers).IsEqualTo(0);
    }

    [Test]
    public async Task GlobalUnhandledExceptionHandler_RepeatedBuildDispose_DoesNotAccumulate(CancellationToken ct = default) {
        // Arrange
        var recordingSource = new RecordingUnhandledExceptionSource();
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();
        builder.Services.RemoveAll<IInfiniFrameUnhandledExceptionSource>();
        builder.Services.AddSingleton<IInfiniFrameUnhandledExceptionSource>(recordingSource);
        var activeBeforeDispose = new List<int>();
        var activeAfterDispose = new List<int>();

        // Act
        for (int i = 0; i < 3; i++) {
            InfiniFrameBlazorApp app = builder.Build();
            activeBeforeDispose.Add(recordingSource.ActiveHandlers);
            await app.DisposeAsync();
            activeAfterDispose.Add(recordingSource.ActiveHandlers);
        }

        // Assert
        await Assert.That(activeBeforeDispose.All(static count => count == 1)).IsTrue();
        await Assert.That(activeAfterDispose.All(static count => count == 0)).IsTrue();
        await Assert.That(recordingSource.RegistrationCount).IsEqualTo(3);
    }

    [Test]
    public async Task GlobalUnhandledExceptionHandler_CanBeDisabled(CancellationToken ct = default) {
        // Arrange
        var recordingSource = new RecordingUnhandledExceptionSource();
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();
        builder.Services.RemoveAll<IInfiniFrameUnhandledExceptionSource>();
        builder.Services.AddSingleton<IInfiniFrameUnhandledExceptionSource>(recordingSource);
        builder.Services.Configure<InfiniFrameBlazorAppConfiguration>(options => options.EnableGlobalUnhandledExceptionHandler = false);

        // Act
        InfiniFrameBlazorApp app = builder.Build();

        // Assert
        await Assert.That(recordingSource.RegistrationCount).IsEqualTo(0);
        await Assert.That(recordingSource.ActiveHandlers).IsEqualTo(0);
        await app.DisposeAsync();
    }

    [Test]
    public async Task CreateDefault_RegistersUnhandledExceptionSourceByDefault(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();
        ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

        // Act
        var source = serviceProvider.GetService<IInfiniFrameUnhandledExceptionSource>();

        // Assert
        await Assert.That(source).IsNotNull();
    }

    [Test]
    public async Task CreateDefault_ExceptionSourceRejectsNullHandler(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();
        ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
        var source = serviceProvider.GetRequiredService<IInfiniFrameUnhandledExceptionSource>();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() => {
            source.Register(null!);
        }));

        // Assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.ParamName).IsEqualTo("handler");
    }

    [Test]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    public async Task Run_WindowAlreadyClosed_DoesNotInvokeWindowAndDisposesServices(CancellationToken ct = default) {
        // Arrange
        var window = Substitute.For<IInfiniFrameWindow>();
        window.When(x => x.Invoke(Arg.Any<Action>()))
            .Do(_ => throw new InvalidOperationException("Invoke should not be used during Run() shutdown."));

        ServiceProvider services = new ServiceCollection()
            .AddSingleton(window)
            .AddSingleton<DisposeProbe>()
            .BuildServiceProvider();
        var disposeProbe = services.GetRequiredService<DisposeProbe>();

        var app = new InfiniFrameBlazorApp(
            provider: services,
            rootComponents: new InfiniFrameRootComponentList());

        // Act
        app.Run();

        // Assert
        window.Received(1).WaitForClose();
        window.DidNotReceive().Invoke(Arg.Any<Action>());
        await Assert.That(disposeProbe.IsDisposed).IsTrue();
    }

    [Test]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task SetBrowserControlInitParameters_ThroughCreateDefault_ShouldWork(CancellationToken ct = default) {
        // Arrange
        string[] args = Array.Empty<string>();
        const string initParameters = "--force-device-scale-factor=1";
        
        // Act
        var appbuilder = InfiniFrameBlazorAppBuilder.CreateDefault(args, builder => builder
            .SetTitle("Test")
            .SetBrowserControlInitParameters(initParameters)
            .SetLeft(0)
            .SetTop(0)
            .SetSize(100, 100)
            .SetResizable(false)
            .SetChromeless(true)
            .SetSmoothScrollingEnabled(false)
        );
        
        // Assert
        await Assert.That(appbuilder).IsNotNull();
        await Assert.That(appbuilder.WindowBuilder.Configuration.BrowserControlInitParameters).IsEqualTo(
            initParameters
        );
    }
    
    [Test]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task SetBrowserControlInitParameters_ThroughAppBuilder_ShouldWork(CancellationToken ct = default) {
        // Arrange
        string[] args = Array.Empty<string>();
        const string initParameters = "--force-device-scale-factor=1";
        
        // Act
        var appbuilder = InfiniFrameBlazorAppBuilder.CreateDefault(args);
        appbuilder.WindowBuilder
            .SetTitle("Test")
            .SetBrowserControlInitParameters(initParameters)
            .SetLeft(0)
            .SetTop(0)
            .SetSize(100, 100)
            .SetResizable(false)
            .SetChromeless(true)
            .SetSmoothScrollingEnabled(false);
        
        // Assert
        await Assert.That(appbuilder).IsNotNull();
        await Assert.That(appbuilder.WindowBuilder.Configuration.BrowserControlInitParameters).IsEqualTo(
            initParameters
        );
    }
    
    [Test]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [SkipUtility.SkipOnMacOs("Given init parameters are not supported on macOS")]
    [SkipUtility.SkipOnLinux("Given init parameters are not supported on Linux")]
    public async Task SetBrowserControlInitParameters_ThroughCreateDefault_ShouldWorkOnWindow(CancellationToken ct = default) {
        // Arrange
        string[] args = Array.Empty<string>();
        const string initParameters = "--force-device-scale-factor=1";
        
        // Act
        var appbuilder = InfiniFrameBlazorAppBuilder.CreateDefault(args, builder => builder
            .SetTitle("Test")
            .SetBrowserControlInitParameters(initParameters)
            .SetLeft(0)
            .SetTop(0)
            .SetSize(100, 100)
            .SetResizable(false)
            .SetChromeless(true)
            .SetSmoothScrollingEnabled(false)
        );

        InfiniFrameBlazorApp app = appbuilder.Build();
        var window = app.ServiceProvider.GetRequiredService<IInfiniFrameWindow>();
        
        // Assert
        await Assert.That(window).IsNotNull();
        await Assert.That(window.BrowserControlInitParameters).IsEqualTo(
            initParameters
        );
    }
    
    [Test]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [SkipUtility.SkipOnMacOs("Given init parameters are not supported on macOS")]
    [SkipUtility.SkipOnLinux("Given init parameters are not supported on Linux")]
    public async Task SetBrowserControlInitParameters_ThroughAppBuilder_ShouldWorkOnWindow(CancellationToken ct = default) {
        // Arrange
        string[] args = Array.Empty<string>();
        const string initParameters = "--force-device-scale-factor=1";
        
        // Act
        var appbuilder = InfiniFrameBlazorAppBuilder.CreateDefault(args);
        appbuilder.WindowBuilder
            .SetTitle("Test")
            .SetBrowserControlInitParameters(initParameters)
            .SetLeft(0)
            .SetTop(0)
            .SetSize(100, 100)
            .SetResizable(false)
            .SetChromeless(true)
            .SetSmoothScrollingEnabled(false);

        InfiniFrameBlazorApp app = appbuilder.Build();
        var window = app.ServiceProvider.GetRequiredService<IInfiniFrameWindow>();
        
        // Assert
        await Assert.That(window).IsNotNull();
        await Assert.That(window.BrowserControlInitParameters).IsEqualTo(
            initParameters
        );
    }
}
