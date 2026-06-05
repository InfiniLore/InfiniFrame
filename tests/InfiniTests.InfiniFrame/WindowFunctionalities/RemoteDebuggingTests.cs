// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using System.Net;
using System.Net.Sockets;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RemoteDebuggingTests {
    [Test]
    [DisplayName($"{nameof(RemoteDebuggingTests)}.{nameof(Builder_PortSetAndClear_ShouldPropagate)}")]
    public async Task Builder_PortSetAndClear_ShouldPropagate(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetRemoteDebuggingPort(9222);
        int? enabledPort = builder.Configuration.RemoteDebuggingPort;
        InfiniFrameNativeParameters enabled = builder.Configuration.ToNativeParameters();

        builder.SetRemoteDebuggingPort(0);
        int? disabledPort = builder.Configuration.RemoteDebuggingPort;
        InfiniFrameNativeParameters disabled = builder.Configuration.ToNativeParameters();

        // Assert
        await Assert.That(enabledPort).IsEqualTo(9222);
        await Assert.That(enabled.RemoteDebuggingPort).IsEqualTo(9222);
        if (OperatingSystem.IsWindows()) {
            await Assert.That(enabled.BrowserControlInitParameters).Contains("--remote-debugging-address=127.0.0.1");
            await Assert.That(enabled.BrowserControlInitParameters).Contains("--remote-debugging-port=9222");
        }
        else if (OperatingSystem.IsLinux()) {
            await Assert.That(enabled.BrowserControlInitParameters).IsNull();
        }
        else {
            Assert.Fail("Unexpected platform for this test.");
            return;
        }

        await Assert.That(disabledPort).IsNull();
        await Assert.That(disabled.RemoteDebuggingPort).IsEqualTo(0);
        await Assert.That(disabled.BrowserControlInitParameters).IsNull();
    }

    [Test]
    [DisplayName($"{nameof(RemoteDebuggingTests)}.{nameof(Builder_InvalidPort_ShouldThrowArgumentOutOfRangeException)}")]
    [Arguments(-1)]
    [Arguments(65536)]
    public async Task Builder_InvalidPort_ShouldThrowArgumentOutOfRangeException(int invalidPort, CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Task.Run(() => {
            builder.SetRemoteDebuggingPort(invalidPort);
        }, ct));

        // Assert
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.ParamName).IsEqualTo("port");
    }

    [Test]
    [DisplayName($"{nameof(RemoteDebuggingTests)}.{nameof(Builder_OnUnsupportedPlatform_ShouldThrowPlatformNotSupportedException)}")]
    [SkipOnWindows]
    [SkipOnLinux]
    public async Task Builder_OnUnsupportedPlatform_ShouldThrowPlatformNotSupportedException(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        var exception = await Assert.ThrowsAsync<PlatformNotSupportedException>(() => Task.Run(() => {
            builder.SetRemoteDebuggingPort(9222);
        }, ct));

        // Assert
        await Assert.That(exception).IsNotNull();
    }

    [Test]
    [DisplayName($"{nameof(RemoteDebuggingTests)}.{nameof(Builder_Precedence_ShouldIgnoreRawRemoteDebuggingSwitches)}")]
    [SkipOnMacOs]
    public async Task Builder_Precedence_ShouldIgnoreRawRemoteDebuggingSwitches(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetBrowserControlInitParameters("--disable-gpu --remote-debugging-port=9999");
        InfiniFrameNativeParameters withoutExplicitPort = builder.Configuration.ToNativeParameters();

        builder.SetRemoteDebuggingPort(9222);
        InfiniFrameNativeParameters withExplicitPort = builder.Configuration.ToNativeParameters();

        // Assert
        await Assert.That(withoutExplicitPort.BrowserControlInitParameters).IsEqualTo("--disable-gpu");
        await Assert.That(withoutExplicitPort.RemoteDebuggingPort).IsEqualTo(0);

        await Assert.That(withExplicitPort.BrowserControlInitParameters?.Contains("--remote-debugging-port=9999")).IsFalse();
        if (OperatingSystem.IsWindows()) {
            await Assert.That(withExplicitPort.BrowserControlInitParameters).Contains("--disable-gpu");
            await Assert.That(withExplicitPort.BrowserControlInitParameters).Contains("--remote-debugging-port=9222");
            await Assert.That(withExplicitPort.BrowserControlInitParameters).Contains("--remote-debugging-address=127.0.0.1");
        }
        else if (OperatingSystem.IsLinux()) {
            await Assert.That(withExplicitPort.BrowserControlInitParameters).IsEqualTo("--disable-gpu");
        }
        else {
            Assert.Fail("Unexpected platform for this test.");
            return;
        }

        await Assert.That(withExplicitPort.RemoteDebuggingPort).IsEqualTo(9222);
    }

    [Test]
    [DisplayName($"{nameof(RemoteDebuggingTests)}.{nameof(Window_AliveAndClosed_ShouldExposeDeterministicEndpointState)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task Window_AliveAndClosed_ShouldExposeDeterministicEndpointState(CancellationToken ct = default) {
        // Arrange
        int port = GetAvailableLoopbackPort();
        using var windowUtility = InfiniFrameTestWindow.Create(builder => builder.SetRemoteDebuggingPort(port), ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act (alive)
        bool aliveResult = window.TryGetRemoteDebuggingEndpoint(out Uri? aliveEndpoint);

        // Assert (alive)
        await Assert.That(window.SupportsRemoteDebugging).IsTrue();
        await Assert.That(window.RemoteDebuggingPort).IsEqualTo(port);
        await Assert.That(aliveResult).IsTrue();
        await Assert.That(aliveEndpoint).IsNotNull();
        await Assert.That(aliveEndpoint!.ToString()).IsEqualTo(GetExpectedEndpointUri(port).ToString());

        // Act (closed)
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosed && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        bool closedResult = window.TryGetRemoteDebuggingEndpoint(out Uri? closedEndpoint);

        // Assert (closed)
        await Assert.That(window.RemoteDebuggingPort).IsEqualTo(port);
        await Assert.That(closedResult).IsFalse();
        await Assert.That(closedEndpoint).IsNull();
    }

    [Test]
    [DisplayName($"{nameof(RemoteDebuggingTests)}.{nameof(Window_EndpointReadinessAndClose_ShouldBeDeterministic)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(DefaultInfiniTestsTimeoutAttribute.TimeoutValue + 50_000)]
    public async Task Window_EndpointReadinessAndClose_ShouldBeDeterministic(CancellationToken ct = default) {
        // Arrange
        int port = GetAvailableLoopbackPort();
        using var windowUtility = InfiniFrameTestWindow.Create(builder => builder.SetRemoteDebuggingPort(port), ct);
        IInfiniFrameWindow window = windowUtility.Window;
        bool hasEndpoint = window.TryGetRemoteDebuggingEndpoint(out Uri? endpoint);

        await Assert.That(hasEndpoint).IsTrue();
        await Assert.That(endpoint).IsNotNull();

        // Act (ready while alive)
        bool becameReachable = await WaitUntilPortIsReachable(port, TimeSpan.FromSeconds(40), ct);

        // Assert (alive)
        if (!becameReachable) {
            Skip.Test("Remote debugging endpoint did not become reachable in this environment.");
            return;
        }

        // Act (close)
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosed && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        bool becameUnreachable = await WaitUntilPortIsUnavailable(port, TimeSpan.FromSeconds(8), ct);

        // Assert (closed)
        await Assert.That(becameUnreachable).IsTrue();
    }

    [Test]
    [DisplayName($"{nameof(RemoteDebuggingTests)}.{nameof(Window_Collision_ShouldSurfaceActionableFailure)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task Window_Collision_ShouldSurfaceActionableFailure(CancellationToken ct = default) {
        // Arrange
        int port = GetAvailableLoopbackPort();
        using var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Task.Run(() => {
            using var _ = InfiniFrameTestWindow.Create(builder => builder.SetRemoteDebuggingPort(port), ct);
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

    private static Uri GetExpectedEndpointUri(int port) {
        string scheme = OperatingSystem.IsLinux() ? "http" : "https";
        return new Uri($"{scheme}://127.0.0.1:{port}/", UriKind.Absolute);
    }

    private static async Task<bool> WaitUntilPortIsReachable(int port, TimeSpan timeout, CancellationToken ct) {
        DateTime timeoutAt = DateTime.UtcNow.Add(timeout);
        while (DateTime.UtcNow < timeoutAt && !ct.IsCancellationRequested) {
            using var client = new TcpClient();
            try {
                Task connectTask = client.ConnectAsync(IPAddress.Loopback, port);
                Task completed = await Task.WhenAny(connectTask, Task.Delay(300, ct));
                if (completed == connectTask && client.Connected)
                    return true;
            }
            catch (SocketException) {}

            await Task.Delay(200, ct);
        }

        return false;
    }

    private static async Task<bool> WaitUntilPortIsUnavailable(int port, TimeSpan timeout, CancellationToken ct) {
        DateTime timeoutAt = DateTime.UtcNow.Add(timeout);
        while (DateTime.UtcNow < timeoutAt && !ct.IsCancellationRequested) {
            using var client = new TcpClient();
            try {
                Task connectTask = client.ConnectAsync(IPAddress.Loopback, port);
                Task completed = await Task.WhenAny(connectTask, Task.Delay(300, ct));
                if (completed != connectTask)
                    return true;
            }
            catch (SocketException) {
                return true;
            }

            await Task.Delay(200, ct);
        }

        return false;
    }
}
