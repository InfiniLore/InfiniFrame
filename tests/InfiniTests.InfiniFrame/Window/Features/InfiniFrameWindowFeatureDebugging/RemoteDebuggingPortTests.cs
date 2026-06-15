// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeatureDebugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RemoteDebuggingPortTests {
    
    public static async IAsyncEnumerable<Func<int>> GetPorts() {
        await foreach(int port in PortUtils.GetOpenPorts(2)) {
            yield return () => port;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    
    [Test]
    [MethodDataSource(nameof(GetPorts))]
    [SkipOnMacOs]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) {
            Skip.Test("This test is only run on Windows and Linux");
            return;
        }
        
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Debugging.SetRemoteDebuggingPort(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Debugging.RemoteDebuggingPort).IsEqualTo(value);
        await Assert.That(initParameters.RemoteDebuggingPort).IsEqualTo(value);
    }

    [Test]
    [MethodDataSource(nameof(GetPorts))]
    [SkipOnMacOs]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) {
            Skip.Test("This test is only run on Windows and Linux");
            return;
        }
        
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetRemoteDebuggingPort(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        
        await Assert.That(builder.Features.Debugging.RemoteDebuggingPort).IsEqualTo(value);
        await Assert.That(initParameters.RemoteDebuggingPort).IsEqualTo(value);
    }
    
    [Test]
    [NotInParallelInfiniTests]
    [MethodDataSource(nameof(GetPorts))]
    [SkipOnMacOs]
    public async Task AtWindowStage_ThroughBuilderAssignment(int value, CancellationToken ct) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) {
            Skip.Test("This test is only run on Windows and Linux");
            return;
        }
        
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) return;
            
            builder.Features.Debugging.SetRemoteDebuggingPort(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Debugging.RemoteDebuggingPort).IsEqualTo(value);
        await Assert.That(window.Features.Debugging.RemoteDebuggingPort).IsEqualTo(value);
    }
}
