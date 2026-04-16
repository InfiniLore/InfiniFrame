// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrameTests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InfiniFrameTests.BlazorWebView;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorAppBuilderTests {
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

    [Test]
    public async Task Build_WithExternalProvider_ShouldUseProvidedServiceProvider() {
        // Arrange
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();
        await using ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

        // Act
        InfiniFrameBlazorApp app = builder.Build(serviceProvider);

        // Assert
        await Assert.That(app.ServiceProvider).IsSameReferenceAs(serviceProvider);
        await app.DisposeAsync();
    }

    [Test]
    public async Task Build_WithoutProvider_ShouldCreateServiceProvider() {
        // Arrange
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();

        // Act
        InfiniFrameBlazorApp app = builder.Build();

        // Assert
        await Assert.That(app.ServiceProvider).IsNotNull();
        await app.DisposeAsync();
    }

    [Test]
    public async Task GlobalUnhandledExceptionHandler_IsRemovedOnDispose() {
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
    public async Task GlobalUnhandledExceptionHandler_RepeatedBuildDispose_DoesNotAccumulate() {
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
    public async Task GlobalUnhandledExceptionHandler_CanBeDisabled() {
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
    public async Task CreateDefault_RegistersUnhandledExceptionSourceByDefault() {
        // Arrange
        var builder = InfiniFrameBlazorAppBuilder.CreateDefault();
        await using ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

        // Act
        var source = serviceProvider.GetService<IInfiniFrameUnhandledExceptionSource>();

        // Assert
        await Assert.That(source).IsNotNull();
    }

    [Test]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task SetBrowserControlInitParameters_ThroughCreateDefault_ShouldWork(CancellationToken ct) {
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
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task SetBrowserControlInitParameters_ThroughAppBuilder_ShouldWork(CancellationToken ct) {
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
    [Timeout(TimeoutUtility.DefaultTimeout)]
    [SkipUtility.SkipOnMacOs("Given init parameters are not supported on macOS")]
    [SkipUtility.SkipOnLinux("Given init parameters are not supported on Linux")]
    public async Task SetBrowserControlInitParameters_ThroughCreateDefault_ShouldWorkOnWindow(CancellationToken ct) {
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
    [Timeout(TimeoutUtility.DefaultTimeout)]
    [SkipUtility.SkipOnMacOs("Given init parameters are not supported on macOS")]
    [SkipUtility.SkipOnLinux("Given init parameters are not supported on Linux")]
    public async Task SetBrowserControlInitParameters_ThroughAppBuilder_ShouldWorkOnWindow(CancellationToken ct) {
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
