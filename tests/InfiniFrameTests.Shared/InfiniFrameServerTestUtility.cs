// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.WebServer;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[MustDisposeResource]
public sealed class InfiniFrameServerTestUtility : IDisposable {
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
                windowBuilder?.Invoke(builder.Window);

                InfiniFrameWebApplication app = builder.Build();

                app.WebApp.UseDefaultFiles();
                app.WebApp.UseStaticFiles();

                #if !NET8_0
                app.WebApp.MapStaticAssets();
                #endif

                using var util = new InfiniFrameServerTestUtility(Thread.CurrentThread) {
                    Window = app.Window,
                    WebApplication = app.WebApp
                };

                app.WebApp.StartAsync(cancellationToken).GetAwaiter().GetResult();

                ready.SetResult(util);

                app.Window.WaitForClose();

                app.WebApp.StopAsync(cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex) when (IsNonFatalException(ex)) {
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
    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        try {
            Window.Close();
        }
        catch (ApplicationException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        try {
            WebApplication.StopAsync().GetAwaiter().GetResult();
        }
        catch (OperationCanceledException) {
            // ignored
        }
        catch (ObjectDisposedException) {
            // ignored
        }

        if (!_thread.Join(TimeSpan.FromSeconds(5)))
            _thread.Interrupt();
    }

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);
}
