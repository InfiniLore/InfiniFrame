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
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task WebView2Mode_Default_RemainsIsolatedPerWindow(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) {
            Skip.Test("This test is only run on Windows");
            return;
        }

        // Arrange / Act
        using var first = InfiniFrameTestWindow.Create(ct);
        using var second = InfiniFrameTestWindow.Create(ct);

        // Assert
        await Assert.That(first.Window.Configuration.StartupParameters.WebView2WindowMode)
            .IsEqualTo((int)WebView2WindowMode.IsolatedPerWindow);
        await Assert.That<string?>(first.Window.Configuration.StartupParameters.TemporaryFilesPath)
            .IsNotEqualTo(second.Window.Configuration.StartupParameters.TemporaryFilesPath);
    }

    [Test]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task WebView2Mode_ExplicitIsolatedPerWindow_CreatesIsolatedProfilePaths(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) {
            Skip.Test("This test is only run on Windows");
            return;
        }

        // Arrange / Act
        using var first = InfiniFrameTestWindow.Create(
            builder => builder.UseWebView2Mode(WebView2WindowMode.IsolatedPerWindow),
            ct);
        using var second = InfiniFrameTestWindow.Create(
            builder => builder.UseWebView2Mode(WebView2WindowMode.IsolatedPerWindow),
            ct);

        // Assert
        await Assert.That(first.Window.Configuration.StartupParameters.WebView2WindowMode)
            .IsEqualTo((int)WebView2WindowMode.IsolatedPerWindow);
        await Assert.That<string?>(first.Window.Configuration.StartupParameters.TemporaryFilesPath)
            .IsNotEqualTo(second.Window.Configuration.StartupParameters.TemporaryFilesPath);
    }

    [Test]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task WebView2Mode_ManagedShared_ConcurrentWindowCreation_Succeeds(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) {
            Skip.Test("This test is only run on Windows");
            return;
        }

        // Arrange
        string profileRoot = Path.Combine(Path.GetTempPath(), "infiniframe-managed-shared-tests", Guid.NewGuid().ToString("N"));
        var paths = new ConcurrentBag<string>();

        try {
            // Act
            await Task.WhenAll(Enumerable.Range(0, 2).Select(_ => Task.Run(() => {
                using var testWindow = InfiniFrameTestWindow.Create(builder => {
                    builder.UseWebView2Mode(WebView2WindowMode.ManagedShared);
                    builder.UseWebView2SharedEnvironmentProfileRoot(profileRoot);
                }, ct);

                paths.Add(testWindow.Window.Configuration.StartupParameters.TemporaryFilesPath!);
            }, ct)));

            // Assert
            await Assert.That(paths).Count().IsEqualTo(2);
            await Assert.That(paths.Distinct(StringComparer.OrdinalIgnoreCase)).Count().IsEqualTo(1);
        }
        finally {
            TryDeleteDirectory(profileRoot);
        }
    }

    [Test]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task WebView2Mode_ManagedShared_IncompatibleConfig_FailsDeterministically(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) {
            Skip.Test("This test is only run on Windows");
            return;
        }

        // Arrange
        string profileRoot = Path.Combine(Path.GetTempPath(), "infiniframe-managed-shared-tests", Guid.NewGuid().ToString("N"));

        try {
            using var first = InfiniFrameTestWindow.Create(builder => {
                builder.UseWebView2Mode(WebView2WindowMode.ManagedShared);
                builder.UseWebView2SharedEnvironmentProfileRoot(profileRoot);
                builder.SetUserAgent("InfiniFrame Managed Shared A");
            }, ct);

            // Act
            Exception? exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Task.Run(() => {
                using var second = InfiniFrameTestWindow.Create(builder => {
                    builder.UseWebView2Mode(WebView2WindowMode.ManagedShared);
                    builder.UseWebView2SharedEnvironmentProfileRoot(profileRoot);
                    builder.SetUserAgent("InfiniFrame Managed Shared B");
                }, ct);
            }, ct));

            // Assert
            await Assert.That(exception).IsNotNull();
            await Assert.That(exception!.Message).Contains("incompatible active environment settings");
        }
        finally {
            TryDeleteDirectory(profileRoot);
        }
    }

    [Test]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task WebView2Mode_ManagedShared_RemoteDebuggingSamePortSharedEnvironment_Succeeds(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) {
            Skip.Test("This test is only run on Windows");
            return;
        }

        // Arrange
        int port = await PortUtils.GetOpenPort(ct);
        string profileRoot = Path.Combine(Path.GetTempPath(), "infiniframe-managed-shared-tests", Guid.NewGuid().ToString("N"));
        var observedPorts = new ConcurrentBag<int?>();

        try {
            // Act
            await Task.WhenAll(Enumerable.Range(0, 2).Select(_ => Task.Run(() => {
                using var testWindow = InfiniFrameTestWindow.Create(builder => {
                    builder.UseWebView2Mode(WebView2WindowMode.ManagedShared);
                    builder.UseWebView2SharedEnvironmentProfileRoot(profileRoot);
                    #pragma warning disable CA1416
                    builder.SetRemoteDebuggingPort(port);
                    #pragma warning restore CA1416
                }, ct);

                observedPorts.Add(testWindow.Window.Features.Debugging.RemoteDebuggingPort);
            }, ct)));

            // Assert
            await Assert.That(observedPorts).Count().IsEqualTo(2);
            foreach (int? observedPort in observedPorts) {
                await Assert.That(observedPort).IsEqualTo(port);
            }
        }
        finally {
            TryDeleteDirectory(profileRoot);
        }
    }

    [Test]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task WebView2Mode_ManagedShared_RemoteDebuggingDifferentPorts_FailsDeterministically(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) {
            Skip.Test("This test is only run on Windows");
            return;
        }

        // Arrange
        var portList = new List<int>();
        await foreach (int port in PortUtils.GetOpenPorts(2, ct)) {
            portList.Add(port);
        }

        string profileRoot = Path.Combine(Path.GetTempPath(), "infiniframe-managed-shared-tests", Guid.NewGuid().ToString("N"));

        try {
            using var first = InfiniFrameTestWindow.Create(builder => {
                builder.UseWebView2Mode(WebView2WindowMode.ManagedShared);
                builder.UseWebView2SharedEnvironmentProfileRoot(profileRoot);
                #pragma warning disable CA1416
                builder.SetRemoteDebuggingPort(portList[0]);
                #pragma warning restore CA1416
            }, ct);

            // Act
            Exception? exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Task.Run(() => {
                using var second = InfiniFrameTestWindow.Create(builder => {
                    builder.UseWebView2Mode(WebView2WindowMode.ManagedShared);
                    builder.UseWebView2SharedEnvironmentProfileRoot(profileRoot);
                    #pragma warning disable CA1416
                    builder.SetRemoteDebuggingPort(portList[1]);
                    #pragma warning restore CA1416
                }, ct);
            }, ct));

            // Assert
            await Assert.That(exception).IsNotNull();
            await Assert.That(exception!.Message).Contains("incompatible active environment settings");
        }
        finally {
            TryDeleteDirectory(profileRoot);
        }
    }

    [Test]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task WebView2Mode_ManagedShared_CleanupReleasesActiveGroup(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) {
            Skip.Test("This test is only run on Windows");
            return;
        }

        // Arrange
        string profileRoot = Path.Combine(Path.GetTempPath(), "infiniframe-managed-shared-tests", Guid.NewGuid().ToString("N"));

        try {
            using (InfiniFrameTestWindow unused = InfiniFrameTestWindow.Create(builder => {
                    builder.UseWebView2Mode(WebView2WindowMode.ManagedShared);
                    builder.UseWebView2SharedEnvironmentProfileRoot(profileRoot);
                    builder.SetUserAgent("InfiniFrame Managed Shared A");
                }, ct)) {
            }

            // Act
            using var second = InfiniFrameTestWindow.Create(builder => {
                builder.UseWebView2Mode(WebView2WindowMode.ManagedShared);
                builder.UseWebView2SharedEnvironmentProfileRoot(profileRoot);
                builder.SetUserAgent("InfiniFrame Managed Shared B");
            }, ct);

            // Assert
            await Assert.That(second.Window.Configuration.StartupParameters.WebView2WindowMode)
                .IsEqualTo((int)WebView2WindowMode.ManagedShared);
        }
        finally {
            TryDeleteDirectory(profileRoot);
        }
    }

    [Test]
    public async Task ConcurrentWindowCreation_UsesIsolatedTemporaryProfiles(CancellationToken ct = default) {
        // Arrange
        const int windowCount = 4;
        var paths = new ConcurrentBag<string>();

        // Act
        await Task.WhenAll(Enumerable.Range(0, windowCount).Select(_ => Task.Run(() => {
            using var testWindow = InfiniFrameTestWindow.Create(ct);
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
            using var testWindow = InfiniFrameTestWindow.Create(builder => {
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
                using var testWindow = InfiniFrameTestWindow.Create(builder => {
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
        string root = Path.Combine(Path.GetTempPath(), "infiniframe-cleanup-tests", Guid.NewGuid().ToString("N"));

        foreach (Guid windowId in windowIds) {
            string path = Path.Combine(root, windowId.ToString("N"));
            Directory.CreateDirectory(path);
            await File.WriteAllTextAsync(Path.Combine(path, "marker.txt"), windowId.ToString("N"), ct);
            BrowserProfileUtility.RegisterAutoProfilePath(windowId, path);
        }

        try {
            // Act
            await Task.WhenAll(windowIds.Select(windowId => Task.Run(() => {
                BrowserProfileUtility.CleanupAutoProfilePath(windowId);
            }, ct)));

            // Assert
            foreach (Guid windowId in windowIds) {
                await Assert.That(Directory.Exists(Path.Combine(root, windowId.ToString("N")))).IsFalse();
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

    private static void TryDeleteDirectory(string path) {
        try {
            if (Directory.Exists(path)) Directory.Delete(path, recursive: true);
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
        }
    }
}
