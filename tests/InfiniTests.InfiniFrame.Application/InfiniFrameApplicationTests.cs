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
public sealed partial class InfiniFrameApplicationTests {
    [Test]
    [NotInParallelInfiniTests]
    [OnlyRunOnWindows]
    public async Task WebView2RuntimeConfigurationCanBeSetBeforeRun(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder();
        string runtimePath = Environment.SystemDirectory;

        // Act
        await using InfiniFrameApplication application = builder
            .WithWebView2RuntimePath(runtimePath)
            .Build();

        // Assert
        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
    [OnlyRunOnWindows]
    public async Task ProcessWideConfigurationCanBeSetBeforeRun(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder();

        // Act
        builder.WithWebView2RuntimePath(Environment.SystemDirectory);
        builder.WithNotificationRegistrationId("InfiniFrame.Tests");
        builder.WithAppUserModelId("InfiniFrame.Tests");
        builder.WithDefaultNotificationIcon(Environment.ProcessPath!);

        InfiniFrameApplication application = builder.Build();

        // Assert
        await Assert.That(application.Windows).IsEmpty();
    }

    [Test]
    [NotInParallelInfiniTests]
    [OnlyRunOnWindows]
    public async Task RunFromStaThreadFailsBeforeWindowCreation(CancellationToken ct = default) {
        // Arrange
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();

        // Act

        // Assert
        await Assert.That(async () => {
                // ReSharper disable once AccessToDisposedClosure
                await Task.Run(application.Run, ct);
            })
            .Throws<InvalidOperationException>();
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    public async Task RunAsyncBuildsAndRunsMultipleWindowsUntilAllClose(CancellationToken ct = default) {
        // Arrange
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"))
            .WithWindow("settings", configure: static builder => builder.SetStartPageContent("<html><body>Settings</body></html>"));


        // Act & Assert
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
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"));

        Task runTask = application.RunAsync(ct);
        application.Shutdown();
        application.Shutdown();

        await runTask.WaitAsync(TimeSpan.FromSeconds(30), ct);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnWindows]
    public void Run_WithNoWindows_CompletesDeterministically() {
        using InfiniFrameApplication application = InfiniFrameApplication.Initialize();
        application.Run();
    }

    [Test]
    [NotInParallelInfiniTests]
    [OnlyRunOnMacOs]
    public async Task MacOsApplication_CanBeCreatedAndDisposedSequentially(CancellationToken ct = default) {
        await using (InfiniFrameApplication first = InfiniFrameApplication.Initialize()) {
            await first.RunAsync(ct);
        }

        await using (InfiniFrameApplication second = InfiniFrameApplication.Initialize()) {
            await second.RunAsync(ct);
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    [OnlyRunOnMacOs]
    public async Task RunAsyncCancellation_StopsRunLoop(CancellationToken ct = default) {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
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
            catch (OperationCanceledException) {}
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    [OnlyRunOnMacOs]
    public async Task RunAsyncShutdownDuringStartup_DrainsRunAsync(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"));

        Task runTask = application.RunAsync(ct);
        application.Shutdown();
        application.Shutdown();

        await runTask.WaitAsync(TimeSpan.FromSeconds(30), ct);
    }

    [Test]
    [NotInParallelInfiniTests]
    [OnlyRunOnMacOs]
    public async Task RunAsyncWithNoWindows_CompletesWithoutBlockingMainThread(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();
        await application.RunAsync(ct);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    [OnlyRunOnMacOs]
    public async Task RunAsyncShutdownAfterWindows_CleansUpNativeTeardown(CancellationToken ct = default) {
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
            catch (OperationCanceledException) {}
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    [Timeout(60_000)]
    [OnlyRunOnMacOs]
    public async Task RunAsyncTwiceConsecutively_PreservesLifecycleGuarantees(CancellationToken ct = default) {
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
                catch (OperationCanceledException) {}
            }
        }
    }

}
