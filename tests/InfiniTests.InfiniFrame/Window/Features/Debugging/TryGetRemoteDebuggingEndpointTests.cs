// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TryGetRemoteDebuggingEndpointTests {
    public static Func<int> GetPort() => PortUtils.GetOpenPortValue;

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment_DefaultConfiguration(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
#pragma warning disable CA1416
        bool foundValue = window.Features.Debugging.TryGetRemoteDebuggingEndpoint(out Uri? endpoint);
#pragma warning restore CA1416

        // Assert
        await Assert.That(foundValue).IsFalse();
        await Assert.That(endpoint).IsNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment_DefaultConfiguration(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
#pragma warning disable CA1416
        bool foundValue = window.TryGetRemoteDebuggingEndpoint(out Uri? endpoint);
#pragma warning restore CA1416

        // Assert
        await Assert.That(foundValue).IsFalse();
        await Assert.That(endpoint).IsNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [MethodDataSource(nameof(GetPort))]
    public async Task AtWindowStage_ThroughBuilderAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) return;

            builder.Features.Debugging.SetRemoteDebuggingPort(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;
        bool supportsRemoteEndpoint = OperatingSystem.IsWindows() || OperatingSystem.IsLinux();

        // Act
#pragma warning disable CA1416
        bool foundValue = window.TryGetRemoteDebuggingEndpoint(out Uri? endpoint);
#pragma warning restore CA1416

        // Assert
        await Assert.That(builder.Features.Debugging.RemoteDebuggingPort).IsEqualTo(supportsRemoteEndpoint ? value : 0);
        await Assert.That(window.Features.Debugging.RemoteDebuggingPort).IsEqualTo(supportsRemoteEndpoint ? value : null);
        await Assert.That(foundValue).IsEqualTo(supportsRemoteEndpoint);

        if (!supportsRemoteEndpoint) {
            await Assert.That(endpoint).IsNull();
            return;
        }

        Uri expectedEndpoint = new($"http://127.0.0.1:{value}", UriKind.Absolute);
        await Assert.That(endpoint).IsNotNull();
        await Assert.That(endpoint).IsEqualTo(expectedEndpoint);
    }

    [Test]
    [NotInParallelInfiniTests]
    [MethodDataSource(nameof(GetPort))]
    public async Task AtWindowStage_ThroughBuilderAssignment_WhenClosed_ReturnsFalseAndNullEndpoint(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) return;

            builder.Features.Debugging.SetRemoteDebuggingPort(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

#pragma warning disable CA1416
        bool foundValue = window.TryGetRemoteDebuggingEndpoint(out Uri? endpoint);
#pragma warning restore CA1416

        // Assert
        await Assert.That(foundValue).IsFalse();
        await Assert.That(endpoint).IsNull();
    }
}
