// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TryProbeRemoteDebuggingEndpointTests {
    private const string NotSupportedReason = "Remote debugging endpoint probing is not supported on this platform.";
    private const string DisabledReason = "Remote debugging is disabled.";

    public static async IAsyncEnumerable<Func<int>> GetPorts() {
        await foreach(int port in PortUtils.GetOpenPorts(1)) {
            yield return () => port;
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment_DefaultConfiguration(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        bool supportsRemoteEndpoint = OperatingSystem.IsWindows() || OperatingSystem.IsLinux();

        // Act
        #pragma warning disable CA1416
        bool foundValue = window.Features.Debugging.TryProbeEndpoint(out Uri? endpoint, out string? reason);
        #pragma warning restore CA1416

        // Assert
        await Assert.That(foundValue).IsFalse();
        await Assert.That(endpoint).IsNull();
        await Assert.That(reason).IsEqualTo(supportsRemoteEndpoint ? DisabledReason : NotSupportedReason);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment_DefaultConfiguration(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        bool supportsRemoteEndpoint = OperatingSystem.IsWindows() || OperatingSystem.IsLinux();

        // Act
        #pragma warning disable CA1416
        bool foundValue = window.TryProbeRemoteDebuggingEndpoint(out Uri? endpoint, out string? reason);
        #pragma warning restore CA1416

        // Assert
        await Assert.That(foundValue).IsFalse();
        await Assert.That(endpoint).IsNull();
        await Assert.That(reason).IsEqualTo(supportsRemoteEndpoint ? DisabledReason : NotSupportedReason);
    }

    [Test]
    [NotInParallelInfiniTests]
    [MethodDataSource(nameof(GetPorts))]
    public async Task AtWindowStage_ThroughBuilderAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) return;
            builder.Features.Debugging.SetRemoteDebuggingPort(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        bool supportsRemoteEndpoint = OperatingSystem.IsWindows() || OperatingSystem.IsLinux();

        // Act
        #pragma warning disable CA1416
        bool foundValue = window.TryProbeRemoteDebuggingEndpoint(out Uri? endpoint, out string? reason);
        #pragma warning restore CA1416

        // Assert
        if (!supportsRemoteEndpoint) {
            await Assert.That(foundValue).IsFalse();
            await Assert.That(endpoint).IsNull();
            await Assert.That(reason).IsEqualTo(NotSupportedReason);
            return;
        }

        await Assert.That(endpoint).IsNotNull();
        await Assert.That(endpoint!.Port).IsEqualTo(value);

        if (foundValue) {
            await Assert.That(reason).IsNull();
            return;
        }

        await Assert.That(reason).IsNotNull();
        await Assert.That(reason!).IsNotEqualTo(string.Empty);
    }
}
