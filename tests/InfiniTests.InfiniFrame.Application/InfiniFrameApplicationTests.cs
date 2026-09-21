// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Application;
using InfiniTests.Attributes;

namespace InfiniTests.InfiniFrame.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[NotInParallelInfiniTests]
public sealed class InfiniFrameApplicationTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task Initialize_CreatesApplicationWithNoWindows(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task CreateBuilder_RegistersUnnamedWindowWithoutIntegrationId(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWindow(static window => window.SetStartPageContent("<html><body>App</body></html>"))
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task RegisterWindow_DuplicateIdThrows(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();
        application.RegisterWindow("main", configure: static _ => {});

        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => application.RegisterWindow("main", configure: static _ => {}))
            .Throws<ArgumentException>();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task LookupBeforeRunFailsClearly(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();
        application.RegisterWindow("main", configure: static _ => {});

        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => application.GetWindow("main"))
            .Throws<InvalidOperationException>();
        await Assert.That(application.TryGetWindow("main")).IsNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task WebView2RuntimeConfigurationCanBeSetBeforeRun(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .WithWebView2RuntimePath(Environment.SystemDirectory)
            .Build();

        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
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
    [NotInParallelInfiniTests]
    public async Task RegistrationAfterRunFails(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();
        await application.RunAsync(ct);

        // ReSharper disable once AccessToDisposedClosure
        await Assert.That(() => application.RegisterWindow(static _ => {}))
            .Throws<InvalidOperationException>();
    }

    [Test]
    [NotInParallelInfiniTests]
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
         if ((!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS()) || Environment.Version.Major < 8) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"))
            .WithWindow("settings", configure: static builder => builder.SetStartPageContent("<html><body>Settings</body></html>"));

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100 && application.Windows.Count < 2; attempt++) {
                if (runTask.IsFaulted) await runTask;
                await Task.Delay(100, ct);
            }

            await Assert.That(application.Windows).Count().IsEqualTo(2);
            await Task.WhenAll(application.Windows.Select(window => Task.Run(window.Close, ct)));
            application.Shutdown();
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), ct);
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    public async Task ShutdownBeforeWebView2InitializationCompletesDrainsRunAsync(CancellationToken ct = default) {
         if ((!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS()) || Environment.Version.Major < 8) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"));

        Task runTask = application.RunAsync(ct);
        application.Shutdown();
        application.Shutdown();

        await runTask.WaitAsync(TimeSpan.FromSeconds(30), ct);
    }

    [Test]
    [NotInParallelInfiniTests]
    public void Run_WithNoWindows_CompletesDeterministically() {
        if (!OperatingSystem.IsMacOS() && !OperatingSystem.IsLinux()) return;

        using InfiniFrameApplication application = InfiniFrameApplication.Initialize();
        application.Run();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task MacOsApplication_CanBeCreatedAndDisposedSequentially(CancellationToken ct = default) {
        if (!OperatingSystem.IsMacOS()) return;

        await using (InfiniFrameApplication first = InfiniFrameApplication.Initialize())
            await first.RunAsync(ct);
        await using (InfiniFrameApplication second = InfiniFrameApplication.Initialize())
            await second.RunAsync(ct);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    public async Task RunAsyncCancellation_StopsRunLoop(CancellationToken ct = default) {
        if (!OperatingSystem.IsMacOS()) return;

        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"));

        Task runTask = application.RunAsync(cts.Token);
        try {
            for (int attempt = 0; attempt < 100 && application.Windows.Count < 1; attempt++) {
                if (runTask.IsFaulted) await runTask;
                await Task.Delay(100, ct);
            }

            cts.Cancel();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), ct);
        }
        finally {
            application.Shutdown();
            try { await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None); }
            catch (OperationCanceledException) { }
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    public async Task RunAsyncShutdownDuringStartup_DrainsRunAsync(CancellationToken ct = default) {
        if (!OperatingSystem.IsMacOS()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"));

        Task runTask = application.RunAsync(ct);
        application.Shutdown();
        application.Shutdown();

        await runTask.WaitAsync(TimeSpan.FromSeconds(30), ct);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task RunAsyncWithNoWindows_CompletesWithoutBlockingMainThread(CancellationToken ct = default) {
        if (!OperatingSystem.IsMacOS()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();
        await application.RunAsync(ct);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    public async Task RunAsyncShutdownAfterWindows_CleansUpNativeTeardown(CancellationToken ct = default) {
        if (!OperatingSystem.IsMacOS()) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"));

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100 && application.Windows.Count < 1; attempt++) {
                if (runTask.IsFaulted) await runTask;
                await Task.Delay(100, ct);
            }

            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), ct);
        }
        finally {
            application.Shutdown();
            try { await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None); }
            catch (OperationCanceledException) { }
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    public async Task RunAsyncTwiceConsecutively_PreservesLifecycleGuarantees(CancellationToken ct = default) {
        if (!OperatingSystem.IsMacOS()) return;

        for (int i = 0; i < 2; i++) {
            await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
                .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"));

            Task runTask = application.RunAsync(ct);
            try {
                for (int attempt = 0; attempt < 100 && application.Windows.Count < 1; attempt++) {
                    if (runTask.IsFaulted) await runTask;
                    await Task.Delay(100, ct);
                }

                application.Shutdown();
                await runTask.WaitAsync(TimeSpan.FromSeconds(30), ct);
            }
            finally {
                application.Shutdown();
                try { await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None); }
                catch (OperationCanceledException) { }
            }
        }
    }

}
