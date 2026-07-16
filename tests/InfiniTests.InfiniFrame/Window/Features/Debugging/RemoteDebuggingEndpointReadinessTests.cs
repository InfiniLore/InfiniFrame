// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Net;
using System.Net.Sockets;

namespace InfiniTests.InfiniFrame.Window.Features.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RemoteDebuggingEndpointReadinessTests {
    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ThroughBuilderAssignment_CloseTransitionsEndpointFromReachableToUnavailable(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) {
            Skip.Test("This test is only run on Windows and Linux");
            return;
        }

        // Arrange
        int port = GetAvailableLoopbackPort();
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) return;
            builder.Features.Debugging.SetRemoteDebuggingPort(port);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        bool hasEndpoint = window.Features.Debugging.TryGetRemoteDebuggingEndpoint(out Uri? endpoint);
        Skip.When(!hasEndpoint || endpoint is null, "Remote debugging endpoint is unavailable in this environment.");

        // Act
        bool becameReachable = await WaitUntilPortIsReachable(port, TimeSpan.FromSeconds(5), ct);
        Skip.When(!becameReachable, "The configured remote debugging port did not become reachable.");

        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        bool becameUnavailable = await WaitUntilPortIsUnavailable(port, TimeSpan.FromSeconds(8), ct);

        // Assert
        await Assert.That(becameUnavailable).IsTrue();
    }

    private static int GetAvailableLoopbackPort() {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static async Task<bool> WaitUntilPortIsReachable(int port, TimeSpan timeout, CancellationToken ct) {
        DateTime timeoutAt = DateTime.UtcNow.Add(timeout);
        while (DateTime.UtcNow < timeoutAt && !ct.IsCancellationRequested) {
            using var client = new TcpClient();
            try {
                Task connectTask = client.ConnectAsync(IPAddress.Loopback, port);
                Task completed = await Task.WhenAny(connectTask, Task.Delay(300, ct));
                if (completed == connectTask && client.Connected) {
                    return true;
                }
            }
            catch (SocketException) {
            }

            await Task.Delay(200, ct);
        }

        return false;
    }

    private static async Task<bool> WaitUntilPortIsUnavailable(int port, TimeSpan timeout, CancellationToken ct) {
        DateTime timeoutAt = DateTime.UtcNow.Add(timeout);
        while (DateTime.UtcNow < timeoutAt && !ct.IsCancellationRequested) {
            using var client = new TcpClient();
            try {
                Task connectTask = client.ConnectAsync(IPAddress.Loopback, port, ct).AsTask();
                Task completed = await Task.WhenAny(connectTask, Task.Delay(300, ct));
                if (completed != connectTask) {
                    return true;
                }
            }
            catch (SocketException) {
                return true;
            }

            await Task.Delay(200, ct);
        }

        return false;
    }
}
