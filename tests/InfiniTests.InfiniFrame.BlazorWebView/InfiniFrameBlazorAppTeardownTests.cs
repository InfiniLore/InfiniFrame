// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Versioning;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameBlazorAppTeardownTests {
    [Test]
    [OnlyRunOnWindowsX64]
    [NotInParallelInfiniTests]
    [SupportedOSPlatform("windows")]
    public async Task Run_WindowClosed_CompletesRendererDisposal(CancellationToken ct = default) {
        // Arrange
        var windowReady = new TaskCompletionSource<IInfiniFrameWindow>(TaskCreationOptions.RunContinuationsAsynchronously);
        var runCompleted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var thread = new Thread(() => {
            try {
                var appBuilder = InfiniFrameBlazorAppBuilder.CreateDefault();
                appBuilder.RootComponents.Add<TestComponent>("app");

                InfiniFrameBlazorApp app = appBuilder.Build();
                windowReady.TrySetResult(app.ServiceProvider.GetRequiredService<IInfiniFrameWindow>());
                app.Run();
                runCompleted.TrySetResult();
            }
            catch (Exception exception) {
                windowReady.TrySetException(exception);
                runCompleted.TrySetException(exception);
            }
        }) {
            IsBackground = true,
            Name = "InfiniFrame Blazor teardown integration test"
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        // Act
        IInfiniFrameWindow window = await windowReady.Task.WaitAsync(TimeSpan.FromSeconds(15), ct);
        await Task.Delay(TimeSpan.FromSeconds(1), ct);
        window.Close();

        // Assert
        await runCompleted.Task.WaitAsync(TimeSpan.FromSeconds(15), ct);
        await Assert.That(thread.Join(TimeSpan.FromSeconds(1))).IsTrue();
    }

    private sealed class TestComponent : IComponent {
        public void Attach(RenderHandle renderHandle) { }

        public Task SetParametersAsync(ParameterView parameters) => Task.CompletedTask;
    }
}