// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class PortUtils {
    private static readonly object RecentlyReturnedPortsLock = new();
    private static readonly HashSet<int> RecentlyReturnedPorts = [];

    /// <summary>
    ///     Returns a currently available loopback port after releasing the temporary reservation.
    ///     Intended for deferred test-data factories, so discovery never holds a port while a test is running.
    /// </summary>
    public static int GetOpenPortValue() {
        // WebView2 can keep its debugging listener alive briefly after its owning window
        // has completed teardown. Never hand the same ephemeral port to another test in
        // this process; otherwise deferred MethodDataSource rows can collide with that
        // still-draining browser process, especially on Windows ARM64.
        for (int attempt = 0; attempt < 100; attempt++) {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            lock (RecentlyReturnedPortsLock) {
                if (RecentlyReturnedPorts.Add(port))
                    return port;
            }
        }

        throw new InvalidOperationException("Could not allocate a unique loopback port for the test process.");
    }

    public static async Task<int> GetOpenPort(CancellationToken cancellationToken = default) {
        await foreach (int port in GetOpenPorts(1, cancellationToken)) {
            return port;
        }

        return 0;
    }
    public static async IAsyncEnumerable<int> GetOpenPorts(
        int count = 3,
        [EnumeratorCancellation]
        CancellationToken cancellationToken = default
    ) {
        var listeners = new List<TcpListener>();

        try {
            for (int i = 0; i < count; i++) {
                cancellationToken.ThrowIfCancellationRequested();

                var listener = new TcpListener(IPAddress.Loopback, 0);
                listener.Start();

                listeners.Add(listener);

                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
                yield return port;

                await Task.Yield();
            }
        }
        finally {
            foreach (TcpListener listener in listeners) {
                listener.Stop();
            }
        }
    }
}