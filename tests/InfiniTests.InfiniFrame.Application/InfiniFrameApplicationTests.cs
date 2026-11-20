// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using InfiniFrame;
using InfiniFrame.Application;
using InfiniTests.Native;

namespace InfiniTests.InfiniFrame.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[NotInParallelInfiniTests]
public sealed partial class InfiniFrameApplicationTests {
    /// <summary>
    ///     Waits for the deterministic drain signal of RunAsync: the run task completes
    ///     only after the native loop has exited, every window reached native teardown,
    ///     and application disposal finished. The wait completes exactly when the drain
    ///     completes - wall-clock bounds belong to the test timeout, not to the drain
    ///     wait itself. A fixed inner cut-off previously turned slow WebView2 startup on
    ///     loaded runners into spurious drain failures.
    /// </summary>
    private static Task WaitForDrainAsync(Task runTask, CancellationToken ct)
        => runTask.WaitAsync(ct);

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

        await using InfiniFrameApplication application = builder.Build();

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
    [Timeout(120_000)]
    public async Task ShutdownBeforeWebView2InitializationCompletesDrainsRunAsync(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"));

        Task runTask = application.RunAsync(ct);
        application.Shutdown();
        application.Shutdown();

        await WaitForDrainAsync(runTask, ct);

        // Drained deterministically: the native loop exited and every window reached
        // native teardown before application disposal completed.
        await Assert.That(application.Windows).IsEmpty();
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
    [Timeout(120_000)]
    [OnlyRunOnMacOs]
    public async Task RunAsyncShutdownDuringStartup_DrainsRunAsync(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", configure: static builder => builder.SetStartPageContent("<html><body>Main</body></html>"));

        Task runTask = application.RunAsync(ct);
        application.Shutdown();
        application.Shutdown();

        await WaitForDrainAsync(runTask, ct);

        // Drained deterministically: the native loop exited and every window reached
        // native teardown before application disposal completed.
        await Assert.That(application.Windows).IsEmpty();
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
    public async Task MacOsRunAsync_StartsAppKitAndShowsTheNativeWindow(CancellationToken ct = default) {
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", static builder => builder
                .SetTitle("InfiniFrame macOS lifecycle regression")
                .SetStartPageContent("<html><body>InfiniFrame loaded</body></html>"));

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 300; attempt++) {
                if (runTask.IsFaulted) await runTask;

                if (application.TryGetWindow("main") is not null
                    && MacOsNative.IsApplicationRegisteredWithAppKit()
                    && MacOsNative.GetVisibleApplicationWindowCount() > 0)
                    break;

                await Task.Delay(100, ct);
            }

            await Assert.That(application.TryGetWindow("main")).IsNotNull();
            await Assert.That(MacOsNative.IsApplicationRegisteredWithAppKit()).IsTrue();
            await Assert.That(MacOsNative.GetApplicationWindowCount()).IsGreaterThan(0);
            await Assert.That(MacOsNative.GetVisibleApplicationWindowCount()).IsGreaterThan(0);
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
    [OnlyRunOnMacOs]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public async Task MacOsRun_StartsAppKitAndShowsTheNativeWindow(CancellationToken ct = default) {
        using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", static builder => builder
                .SetTitle("InfiniFrame macOS synchronous lifecycle regression")
                .SetStartPageContent("<html><body>InfiniFrame loaded</body></html>"));

        var observed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _ = Task.Run(async () => {
            try {
                for (int attempt = 0; attempt < 300; attempt++) {
                    if (application.TryGetWindow("main") is not null
                        && MacOsNative.IsApplicationRegisteredWithAppKit()
                        && MacOsNative.GetVisibleApplicationWindowCount() > 0) {
                        observed.TrySetResult(true);
                        application.Shutdown();
                        return;
                    }

                    await Task.Delay(100, ct);
                }

                observed.TrySetResult(false);
                application.Shutdown();
            }
            catch (Exception exception) {
                observed.TrySetException(exception);
                application.Shutdown();
            }
        }, ct);

        application.Run();

        await observed.Task;
        await Assert.That(observed.Task.Result).IsTrue();
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
