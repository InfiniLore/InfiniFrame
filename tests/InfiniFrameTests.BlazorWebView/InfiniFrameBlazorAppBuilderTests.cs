// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrameTests.Shared;

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
        var appbuilder = InfiniFrameBlazorAppBuilder.CreateDefault(args, builder => {
            builder.SetBrowserControlInitParameters(initParameters);
            
        });
        
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
        appbuilder.WindowBuilder.SetBrowserControlInitParameters(initParameters);
        
        // Assert
        await Assert.That(appbuilder).IsNotNull();
        await Assert.That(appbuilder.WindowBuilder.Configuration.BrowserControlInitParameters).IsEqualTo(
            initParameters
        );
    }
}
