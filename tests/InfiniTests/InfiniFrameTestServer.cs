// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;
using InfiniFrame.WebServer;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[MustDisposeResource]
public sealed class InfiniFrameTestServer : IAsyncDisposable {

    private readonly Thread _thread;
    private int _disposed;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private InfiniFrameTestServer(Thread thread) {
        _thread = thread;
    }
    public required IInfiniFrameWindow Window { get; init; }
    public required WebApplication WebApplication { get; init; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async ValueTask DisposeAsync() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        try {
            await Window.CloseAsync();
        }
        catch (ApplicationException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        try {
            await WebApplication.StopAsync();
        }
        catch (OperationCanceledException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        bool stoppedInTime = _thread.Join(TimeSpan.FromSeconds(5));
        if (!stoppedInTime) {
            Console.WriteLine(
                $"[InfiniFrameServerTestUtility] Warning: server thread did not stop within 5s. " +
                $"ThreadId={_thread.ManagedThreadId}, State={_thread.ThreadState}. Interrupting thread.");
            _thread.Interrupt();
        }

    }

    public static InfiniFrameTestServer Create(
        Action<WebApplicationBuilder>? appBuilder = null,
        Action<IInfiniFrameWindowBuilder>? windowBuilder = null,
        CancellationToken cancellationToken = default
    ) {
        var ready = new TaskCompletionSource<(IInfiniFrameWindow Window, WebApplication WebApplication)>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() => {
            try {
                InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder();
                builder.WebApp.WebHost.UseStaticWebAssets();

                appBuilder?.Invoke(builder.WebApp);
                windowBuilder?.Invoke(builder.WindowBuilder);

                InfiniFrameWebApplication app = builder.Build();

                app.WebApp.UseDefaultFiles();
                app.WebApp.UseStaticFiles();

#if !NET8_0
                app.WebApp.MapStaticAssets();
#endif

                // Exercise the documented WebServer lifecycle in automation tests. Run() starts
                // Kestrel before creating the window, and the created callback runs on this STA
                // thread immediately before WaitForClose() enters the native message loop.
                app.UseAutoServerClose();
                builder.WindowBuilder.EventsStore.WindowCreated.Add(w => {
                    ready.SetResult((w, app.WebApp));
                });

                app.Run();
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                if (!ready.TrySetException(ex))
                    Console.WriteLine($"[InfiniFrameServerTestUtility] Server thread failed after startup: {ex}");
            }
        }) {
            IsBackground = true
        };

        if (OperatingSystem.IsWindows())
            thread.SetApartmentState(ApartmentState.STA);

        thread.Start();

        (IInfiniFrameWindow window, WebApplication webApplication) =
            ready.Task.WaitAsync(cancellationToken).GetAwaiter().GetResult();
        return new InfiniFrameTestServer(thread) {
            Window = window,
            WebApplication = webApplication
        };
    }
}