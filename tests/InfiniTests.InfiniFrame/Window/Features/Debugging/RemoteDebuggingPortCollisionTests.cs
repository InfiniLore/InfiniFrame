// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Net;
using System.Net.Sockets;

namespace InfiniTests.InfiniFrame.Window.Features.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RemoteDebuggingPortCollisionTests {
    [Test]
    [SkipOnMacOs]
    public async Task AtWindowStage_ThroughBuilderAssignment_PortCollision_ThrowsActionableError(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) {
            Skip.Test("This test is only run on Windows and Linux");
            return;
        }

        // Arrange
        int port = GetAvailableLoopbackPort();
        using var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Task.Run(() => {
            using var _ = InfiniFrameTestWindow.Create(builder => {
                if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) return;
                builder.Features.Debugging.SetRemoteDebuggingPort(port);
            }, ct);
        }, ct));

        // Assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.Message).Contains(port.ToString());
    }

    private static int GetAvailableLoopbackPort() {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}
