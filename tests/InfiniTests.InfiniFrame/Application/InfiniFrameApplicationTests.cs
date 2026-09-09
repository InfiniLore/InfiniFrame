// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Application;

namespace InfiniTests.InfiniFrame.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationTests {
    [Test]
    public async Task Initialize_CreatesApplicationWithNoWindows(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    public async Task CreateBuilder_RegistersUnnamedWindowWithoutIntegrationId(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(static window => window.SetStartPageContent("<html><body>App</body></html>"))
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    public async Task RegisterWindow_DuplicateIdThrows(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();
        application.RegisterWindow("main", configure: static _ => {});

        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => application.RegisterWindow("main", configure: static _ => {}))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task LookupBeforeRunFailsClearly(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();
        application.RegisterWindow("main", configure: static _ => {});

        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => application.GetWindow("main"))
            .Throws<InvalidOperationException>();
        await Assert.That(application.TryGetWindow("main")).IsNull();
    }

    [Test]
    public async Task WebView2RuntimeConfigurationCanBeSetBeforeRun(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWebView2RuntimePath(Environment.SystemDirectory)
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    public async Task ProcessWideConfigurationCanBeSetBeforeRun(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWebView2RuntimePath(Environment.SystemDirectory)
            .WithNotificationRegistrationId("InfiniFrame.Tests")
            .WithAppUserModelId("InfiniFrame.Tests")
            .WithDefaultNotificationIcon(Environment.ProcessPath!)
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    public async Task RegistrationAfterRunFails(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();
        await application.RunAsync(ct);

        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => application.RegisterWindow(static _ => {}))
            .Throws<InvalidOperationException>();
    }

    [Test]
    [Timeout(60_000)]
    public async Task ConcurrentRunAsync_IsRejected(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow(static window => window.SetStartPageContent("<html><body>Run guard</body></html>"));
        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100 && application.Windows.Count == 0; attempt++)
                await Task.Delay(100, ct);

            await Assert.That(() => application.RunAsync(ct))
                .Throws<InvalidOperationException>()
                .WithMessageContaining("only be run once");
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }

    [Test]
    public async Task RunFromMtaThreadFailsBeforeWindowCreation(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();
        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(async () => await Task.Run(application.Run, ct))
            .Throws<InvalidOperationException>();
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    public async Task RunAsyncBuildsAndRunsMultipleWindowsUntilAllClose(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"))
            .WithWindow("settings", configure: static builder => builder.SetStartPageContent("<html><body>Settings</body></html>"));

        Console.WriteLine("[ApplicationMultiWindow] starting RunAsync");
        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100 && application.Windows.Count < 2; attempt++) {
                if (runTask.IsFaulted) await runTask;
                await Task.Delay(100, ct);
            }

            Console.WriteLine($"[ApplicationMultiWindow] windows={application.Windows.Count} completed={runTask.IsCompleted}");
            await Assert.That(application.Windows).Count().IsEqualTo(2);
            application.Shutdown();
            foreach (IInfiniFrameWindow window in application.Windows) window.Close();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), ct);
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }

}
