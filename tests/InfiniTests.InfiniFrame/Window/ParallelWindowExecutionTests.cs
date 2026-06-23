// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;
using System.Collections.Concurrent;

namespace InfiniTests.InfiniFrame.Window;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ParallelWindowExecutionTests {
    [Test]
    public async Task ConcurrentWindowCreation_UsesIsolatedTemporaryProfiles(CancellationToken ct = default) {
        // Arrange
        const int windowCount = 4;
        var paths = new ConcurrentBag<string>();

        // Act
        await Task.WhenAll(Enumerable.Range(0, windowCount).Select(_ => Task.Run(() => {
            using InfiniFrameTestWindow testWindow = InfiniFrameTestWindow.Create(ct);
            paths.Add(testWindow.Window.Configuration.StartupParameters.TemporaryFilesPath!);
        }, ct)));

        // Assert
        await Assert.That(paths).Count().IsEqualTo(windowCount);
        await Assert.That(paths.Distinct(StringComparer.OrdinalIgnoreCase)).Count().IsEqualTo(windowCount);
    }

    [Test]
    [SkipOnMacOs]
    public async Task ConcurrentRemoteDebuggingSetup_DistinctPorts_Succeeds(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) {
            Skip.Test("This test is only run on Windows and Linux");
            return;
        }

        // Arrange
        var portList = new List<int>();
        await foreach (int port in PortUtils.GetOpenPorts(2, ct)) {
            portList.Add(port);
        }

        int[] ports = portList.ToArray();
        var observedPorts = new ConcurrentBag<int?>();

        // Act
        await Task.WhenAll(ports.Select(port => Task.Run(() => {
            using InfiniFrameTestWindow testWindow = InfiniFrameTestWindow.Create(builder => {
                #pragma warning disable CA1416
                builder.SetRemoteDebuggingPort(port);
                #pragma warning restore CA1416
            }, ct);

            observedPorts.Add(testWindow.Window.Features.Debugging.RemoteDebuggingPort);
        }, ct)));

        // Assert
        await Assert.That(observedPorts).Count().IsEqualTo(ports.Length);
        foreach (int port in ports) {
            await Assert.That(observedPorts).Contains(port);
        }
    }

    [Test]
    [SkipOnMacOs]
    public async Task ConcurrentRemoteDebuggingSetup_SamePort_FailsDeterministically(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) {
            Skip.Test("This test is only run on Windows and Linux");
            return;
        }

        // Arrange
        int port = await PortUtils.GetOpenPort(ct);
        using var release = new ManualResetEventSlim(false);
        var results = new ConcurrentBag<Exception?>();

        // Act
        Task[] tasks = Enumerable.Range(0, 2).Select(_ => Task.Run(() => {
            // ReSharper disable once AccessToDisposedClosure
            release.Wait(ct);

            try {
                using InfiniFrameTestWindow testWindow = InfiniFrameTestWindow.Create(builder => {
                    #pragma warning disable CA1416
                    builder.SetRemoteDebuggingPort(port);
                    #pragma warning restore CA1416
                }, ct);
                results.Add(null);
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                results.Add(ex);
            }
        }, ct)).ToArray();

        release.Set();
        await Task.WhenAll(tasks);

        // Assert
        await Assert.That(results).Count().IsEqualTo(2);
        await Assert.That(results.Count(static result => result is null)).IsEqualTo(1);
        Exception conflict = results.Single(static result => result is not null)!;
        await Assert.That(conflict.Message).Contains(port.ToString());
    }

    [Test]
    public async Task ConcurrentCleanup_GeneratedProfilePaths_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        Guid[] windowIds = Enumerable.Range(0, 16).Select(_ => Guid.NewGuid()).ToArray();
        string root = Path.Join(Path.GetTempPath(), "infiniframe-cleanup-tests", Guid.NewGuid().ToString("N"));

        foreach (Guid windowId in windowIds) {
            string path = Path.Join(root, windowId.ToString("N"));
            Directory.CreateDirectory(path);
            await File.WriteAllTextAsync(Path.Join(path, "marker.txt"), windowId.ToString("N"), ct);
            BrowserProfileUtility.RegisterAutoProfilePath(windowId, path);
        }

        try {
            // Act
            await Task.WhenAll(windowIds.Select(windowId => Task.Run(() => {
                BrowserProfileUtility.CleanupAutoProfilePath(windowId);
            }, ct)));

            // Assert
            foreach (Guid windowId in windowIds) {
                await Assert.That(Directory.Exists(Path.Join(root, windowId.ToString("N")))).IsFalse();
            }
        }
        finally {
            try {
                if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            }
        }
    }
}
