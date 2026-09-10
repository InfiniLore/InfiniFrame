using InfiniFrame;
using InfiniFrame.Application;
using InfiniTests.TestSupport.Attributes;

namespace InfiniTests.InfiniFrame.Application;

[NotInParallelInfiniTests]
public sealed class MultiWindowLifecycleTests {
    [Test]
    [Timeout(60_000)]
    public async Task FileUriWindow_LoadsAlongsidePlainWindow(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        string filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid():N}.html");
        await File.WriteAllTextAsync(filePath, "<html><body>File window</body></html>", ct);

        try {
            await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
                .WithWindow("file", builder => builder.SetStartPageUrl(new Uri(filePath).AbsoluteUri))
                .WithWindow("plain", builder => builder.SetStartPageContent("<html><body>Plain</body></html>"));

            Task runTask = application.RunAsync(ct);
            try {
                for (int attempt = 0; attempt < 100; attempt++) {
                    if (runTask.IsFaulted) await runTask;

                    Uri? fileUri = application.TryGetWindow("file")?.Features.PageNavigation.GetCurrentUri();
                    if (fileUri?.IsFile == true) break;

                    await Task.Delay(100, ct);
                }

                await Assert.That(application.Windows).Count().IsEqualTo(2);
                await Assert.That(application.GetWindow("file").Features.PageNavigation.GetCurrentUri()?.IsFile)
                    .IsTrue();
            }
            finally {
                application.Shutdown();
                await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
            }
        }
        finally {
            File.Delete(filePath);
        }
    }

    [Test]
    [Timeout(60_000)]
    public async Task ClosingOneWindow_DoesNotCloseItsSiblings(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", builder => builder.SetStartPageContent("<html><body>Main</body></html>"))
            .WithWindow("settings", builder => builder.SetStartPageContent("<html><body>Settings</body></html>"));

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100 && application.Windows.Count < 2; attempt++) {
                if (runTask.IsFaulted) await runTask;
                await Task.Delay(100, ct);
            }

            await Assert.That(application.Windows).Count().IsEqualTo(2);
            await application.GetWindow("main").CloseAsync();

            for (int attempt = 0; attempt < 100 && application.TryGetWindow("main") is not null; attempt++)
                await Task.Delay(50, ct);

            await Assert.That(application.TryGetWindow("main")).IsNull();
            await Assert.That(application.TryGetWindow("settings")).IsNotNull();
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }

    [Test]
    [Timeout(60_000)]
    public async Task WindowLifecycleEvents_AreRaisedOncePerWindow(CancellationToken ct = default) {
        if (!OperatingSystem.IsWindows() || Environment.Version.Major < 10) return;

        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWindow("main", builder => builder.SetStartPageContent("<html><body>Main</body></html>"))
            .WithWindow("secondary", builder => builder.SetStartPageContent("<html><body>Secondary</body></html>"));

        var lifecycleCounts = new int[2];
        application.WindowCreated += _ => Interlocked.Increment(ref lifecycleCounts[0]);
        application.WindowDestroyed += _ => Interlocked.Increment(ref lifecycleCounts[1]);

        Task runTask = application.RunAsync(ct);
        try {
            for (int attempt = 0; attempt < 100 && Volatile.Read(ref lifecycleCounts[0]) < 2; attempt++) {
                if (runTask.IsFaulted) await runTask;
                await Task.Delay(100, ct);
            }

            await Assert.That(lifecycleCounts[0]).IsEqualTo(2);
            application.CloseAll();

            for (int attempt = 0; attempt < 100 && Volatile.Read(ref lifecycleCounts[1]) < 2; attempt++)
                await Task.Delay(100, ct);

            await Assert.That(lifecycleCounts[1]).IsEqualTo(2);
        }
        finally {
            application.Shutdown();
            await runTask.WaitAsync(TimeSpan.FromSeconds(30), CancellationToken.None);
        }
    }
}
