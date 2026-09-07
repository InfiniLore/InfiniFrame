using InfiniFrame;

namespace InfiniTests.InfiniFrame.Application;

[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationTests {
    [Test]
    public async Task Initialize_CreatesApplicationWithNoWindows(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using var application = InfiniFrameApplication.Initialize();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    public async Task CreateBuilder_RegistersUnnamedWindowWithoutIntegrationId(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using var application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(static window => window.SetStartPageContent("<html><body>App</body></html>"))
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    public async Task RegisterWindow_DuplicateIdThrows(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using var application = InfiniFrameApplication.Initialize();
        application.RegisterWindow("main", static _ => { });

        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => application.RegisterWindow("main", static _ => { }))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task LookupBeforeRunFailsClearly(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using var application = InfiniFrameApplication.Initialize();
        application.RegisterWindow("main", static _ => { });

        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => application.GetWindow("main"))
            .Throws<InvalidOperationException>();
        await Assert.That(application.TryGetWindow("main")).IsNull();
    }

    [Test]
    public async Task WebView2RuntimeConfigurationCanBeSetBeforeRun(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using var application = InfiniFrameApplication.CreateBuilder()
            .WithWebView2RuntimePath(Environment.SystemDirectory)
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    public async Task ProcessWideConfigurationCanBeSetBeforeRun(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using var application = InfiniFrameApplication.CreateBuilder()
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

        await using var application = InfiniFrameApplication.Initialize();
        application.Run();

        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => application.RegisterWindow(static _ => { }))
            .Throws<InvalidOperationException>();
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    public async Task RunAsyncBuildsAndRunsMultipleWindowsUntilAllClose(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        await using var application = InfiniFrameApplication.Initialize()
            .WithWindow("main", static builder => builder.SetStartPageContent("<html><body>Main</body></html>"))
            .WithWindow("settings", static builder => builder.SetStartPageContent("<html><body>Settings</body></html>"));

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
