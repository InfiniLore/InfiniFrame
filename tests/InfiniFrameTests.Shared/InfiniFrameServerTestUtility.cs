// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;
using InfiniFrame.WebServer;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[MustDisposeResource]
public sealed class InfiniFrameServerTestUtility : IAsyncDisposable {
    public required IInfiniFrameWindow Window { get; init; }
    public required WebApplication WebApplication { get; init; }

    private readonly Thread _thread;
    private int _disposed;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private InfiniFrameServerTestUtility(Thread thread) {
        _thread = thread;
    }

    public static InfiniFrameServerTestUtility Create(
        Action<WebApplicationBuilder>? appBuilder = null,
        Action<IInfiniFrameWindowBuilder>? windowBuilder = null,
        CancellationToken cancellationToken = default
    ) {
        var ready = new TaskCompletionSource<InfiniFrameServerTestUtility>(
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

                app.WebApp.StartAsync(cancellationToken).GetAwaiter().GetResult();
                IInfiniFrameWindow window = app.Window;
                
                var util = new InfiniFrameServerTestUtility(Thread.CurrentThread) {
                    Window = window,
                    WebApplication = app.WebApp
                };

                ready.SetResult(util);

                window.WaitForClose();

                app.WebApp.StopAsync(cancellationToken).GetAwaiter().GetResult();
                util.DisposeAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                ready.TrySetException(ex);
            }
        }) {
            IsBackground = true
        };

        if (OperatingSystem.IsWindows())
            thread.SetApartmentState(ApartmentState.STA);

        thread.Start();

        return ready.Task.WaitAsync(cancellationToken).GetAwaiter().GetResult();
    }

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
}
