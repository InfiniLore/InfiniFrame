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
    /// <summary>
    ///     Returns a currently available loopback port after releasing the temporary reservation.
    ///     Intended for deferred test-data factories, so discovery never holds a port while a test is running.
    /// </summary>
    public static int GetOpenPortValue() {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
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
