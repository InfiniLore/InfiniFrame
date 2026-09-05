using InfiniFrame;

namespace InfiniTests.InfiniFrame.Application;

[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationTests {
    [Test]
    public async Task Initialize_CreatesApplicationWithNoWindows(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        using var application = InfiniFrameApplication.Initialize();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    public async Task RegisterWindow_DuplicateIdThrows(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        using var application = InfiniFrameApplication.Initialize();
        application.RegisterWindow("main", static _ => { });

        await Assert.That(() => application.RegisterWindow("main", static _ => { }))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task LookupBeforeRunFailsClearly(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        using var application = InfiniFrameApplication.Initialize();
        application.RegisterWindow("main", static _ => { });

        await Assert.That(() => application.GetWindow("main"))
            .Throws<InvalidOperationException>();
        await Assert.That(application.TryGetWindow("main")).IsNull();
    }

    [Test]
    public async Task WebView2RuntimeConfigurationCanBeSetBeforeRun(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        using var application = InfiniFrameApplication.Initialize()
            .WithWebView2RuntimePath(Environment.SystemDirectory);

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    public async Task ProcessWideConfigurationCanBeSetBeforeRun(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        using var application = InfiniFrameApplication.Initialize()
            .WithWebView2RuntimePath(Environment.SystemDirectory)
            .WithNotificationRegistrationId("InfiniFrame.Tests")
            .WithAppUserModelId("InfiniFrame.Tests")
            .WithDefaultNotificationIcon(Environment.ProcessPath!);

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    public async Task RegistrationAfterRunFails(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        using var application = InfiniFrameApplication.Initialize();
        application.Run();

        await Assert.That(() => application.RegisterWindow(static _ => { }))
            .Throws<InvalidOperationException>();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task RunAsyncBuildsAndRunsMultipleWindowsUntilAllClose(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using var application = InfiniFrameApplication.Initialize()
            .WithWindow("main", static builder => builder.SetStartPageContent("<html><body>Main</body></html>"))
            .WithWindow("settings", static builder => builder.SetStartPageContent("<html><body>Settings</body></html>"));

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100 && application.Windows.Count < 2; attempt++)
                await Task.Delay(100, ct);

            await Assert.That(application.Windows).Count().IsEqualTo(2);
            await Assert.That(application.GetWindow("main")).IsNotNull();
            await Assert.That(application.GetWindow("settings")).IsNotNull();

            foreach (IInfiniFrameWindow window in application.Windows)
                window.Close();

            await runTask.WaitAsync(TimeSpan.FromSeconds(30), ct);
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }

}
