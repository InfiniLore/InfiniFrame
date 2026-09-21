// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniTests;
using Microsoft.Playwright;

namespace InfiniAutomationTests.Tests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public abstract class ZoomDpiParityAutomationTests : InfiniFramePlaywrightTestBase {

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task Zoom_SetFromNative_ReadFromJS(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        RuntimeContext.Window.Features.State.SetZoomFactor(150);
        int nativeZoom = await WaitForStateChangeAsync(
            100,
            stateProvider: () => RuntimeContext.Window.Features.State.ZoomFactor
        );

        int jsZoom = await EvaluateWhenPageReadyAsync<int>(
            page,
            "async () => await window.infiniframe.window.features.state.getZoomFactorAsync()"
        );

        await Assert.That(nativeZoom).IsEqualTo(150);
        await Assert.That(jsZoom).IsEqualTo(150);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task Zoom_SetFromJS_ReadFromNative(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        await EvaluateWhenPageReadyAsync(
            page,
            "async () => await window.infiniframe.window.features.state.setZoomFactorAsync(200)"
        );

        int nativeZoom = await WaitForStateChangeAsync(
            100,
            stateProvider: () => RuntimeContext.Window.Features.State.ZoomFactor
        );

        await Assert.That(nativeZoom).IsEqualTo(200);
    }

    [Test]
    [NotInParallelInfiniAutomationTests]
    public async Task Dpi_NativeAndJS_Match(CancellationToken ct = default) {
        IPage page = await GetRootPageAsync();

        int nativeDpi = RuntimeContext.Window.Features.Monitors.GetMainMonitorScreenDpi();
        double jsDpi = await EvaluateWhenPageReadyAsync<double>(
            page,
            "async () => await window.infiniframe.window.features.monitors.getMainMonitorScreenDpiAsync()"
        );

        await Assert.That(nativeDpi).IsGreaterThan(0);
        await Assert.That(jsDpi).IsEqualTo(nativeDpi);
    }
}
