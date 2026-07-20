// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RemoteDebuggingPortTests {
    public static IEnumerable<Func<int>> GetPorts() {
        yield return PortUtils.GetOpenPortValue;
        yield return PortUtils.GetOpenPortValue;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    
    [Test]
    [MethodDataSource(nameof(GetPorts))]
    [SkipOnMacOs("Remote TCP debugging endpoints are not supported by WKWebView")]
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
    [SkipOnMacOs("Remote TCP debugging endpoints are not supported by WKWebView")]
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
    [SkipOnMacOs("Remote TCP debugging endpoints are not supported by WKWebView")]
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

    [Test]
    [Arguments(-1)]
    [Arguments(65536)]
    public async Task AtBuilderStage_DirectAssignment_InvalidPort_ThrowsArgumentOutOfRangeException(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        #pragma warning disable CA1416
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            Task.Run(() => {
                return builder.Features.Debugging.SetRemoteDebuggingPort(value);
            }, ct));
        #pragma warning restore CA1416

        // Assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.ParamName).IsEqualTo("port");
    }
}
