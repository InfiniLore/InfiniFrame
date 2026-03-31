// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrameTests.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrameTests.BlazorWebView;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorAppBuilderTests {
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
    [SkipUtility.SkipOnMacOs]
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
