// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.State;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ZoomDpiParityTests {

    [Test]
    [NotInParallelInfiniTests]
    public async Task ZoomRoundTrip_MultipleValues(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        int[] zoomValues = [50, 100, 150, 200];
        foreach (int zoom in zoomValues) {
            window.Features.State.SetZoomFactor(zoom);
            await Assert.That(window.Features.State.ZoomFactor).IsEqualTo(zoom);
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task ZoomThroughBuilder_PersistsThroughBuild(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.State.SetZoomFactor(150);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        await Assert.That(builder.Features.State.ZoomFactor).IsEqualTo(150);
        await Assert.That(window.Features.State.ZoomFactor).IsEqualTo(150);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task EnableZoomFalse_PreventsSetZoom(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.State.EnableZoom(false);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        await Assert.That(window.Features.State.IsZoomEnabled).IsFalse();

        window.Features.State.SetZoomFactor(150);

        await Assert.That(window.Features.State.ZoomFactor).IsEqualTo(100);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task Dpi_IsPositiveAndConsistent(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        int dpi1 = window.Features.Monitors.GetMainMonitorScreenDpi();
        int dpi2 = window.Features.Monitors.GetMainMonitorScreenDpi();

        await Assert.That(dpi1).IsGreaterThan(0);
        await Assert.That(dpi2).IsGreaterThan(0);
        await Assert.That(dpi1).IsEqualTo(dpi2);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task ZoomAndDpi_Independence(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        int dpiBefore = window.Features.Monitors.GetMainMonitorScreenDpi();
        window.Features.State.SetZoomFactor(200);
        int dpiAfter = window.Features.Monitors.GetMainMonitorScreenDpi();

        await Assert.That(dpiBefore).IsGreaterThan(0);
        await Assert.That(dpiAfter).IsEqualTo(dpiBefore);
    }
}
