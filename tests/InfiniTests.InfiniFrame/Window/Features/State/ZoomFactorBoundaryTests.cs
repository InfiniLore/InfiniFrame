// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.State;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ZoomFactorBoundaryTests {

    [Test]
    [Arguments(25)]
    [Arguments(500)]
    public async Task Builder_StoresValidZoomRange(int value, CancellationToken ct) {
        var builder = InfiniFrameWindowBuilder.Create();

        builder.Features.State.SetZoomFactor(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        await Assert.That(builder.Features.State.ZoomFactor).IsEqualTo(value);
        await Assert.That(initParameters.Zoom).IsEqualTo(value);
    }

    [Test]
    [Arguments(0)]
    [Arguments(999)]
    public async Task Builder_StoresOutOfRangeZoom(int value, CancellationToken ct) {
        var builder = InfiniFrameWindowBuilder.Create();

        builder.Features.State.SetZoomFactor(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        await Assert.That(builder.Features.State.ZoomFactor).IsEqualTo(value);
        await Assert.That(initParameters.Zoom).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task Window_OutOfRangeZoom_RevertsToDefault(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        int defaultZoom = window.Features.State.ZoomFactor;
        await Assert.That(defaultZoom).IsEqualTo(100);

        window.Features.State.SetZoomFactor(0);
        await Assert.That(window.Features.State.ZoomFactor).IsEqualTo(100);

        window.Features.State.SetZoomFactor(999);
        await Assert.That(window.Features.State.ZoomFactor).IsEqualTo(100);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(25)]
    [Arguments(500)]
    public async Task Window_ValidZoomRange_Persists(int value, CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        window.Features.State.SetZoomFactor(value);

        await Assert.That(window.Features.State.ZoomFactor).IsEqualTo(value);
    }
}
