// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameDebugEndpointStatusTests {

    [Test]
    public async Task NotSupported_IsFirstValue(CancellationToken ct = default) {
        var value = InfiniFrameDebugEndpointStatus.NotSupported;
        await Assert.That(value).IsEqualTo(InfiniFrameDebugEndpointStatus.NotSupported);
    }

    [Test]
    public async Task Disabled_IsSecondValue(CancellationToken ct = default) {
        var value = InfiniFrameDebugEndpointStatus.Disabled;
        await Assert.That(value).IsEqualTo(InfiniFrameDebugEndpointStatus.Disabled);
    }

    [Test]
    public async Task Unavailable_IsThirdValue(CancellationToken ct = default) {
        var value = InfiniFrameDebugEndpointStatus.Unavailable;
        await Assert.That(value).IsEqualTo(InfiniFrameDebugEndpointStatus.Unavailable);
    }

    [Test]
    public async Task Configured_IsFourthValue(CancellationToken ct = default) {
        var value = InfiniFrameDebugEndpointStatus.Configured;
        await Assert.That(value).IsEqualTo(InfiniFrameDebugEndpointStatus.Configured);
    }

    [Test]
    public async Task Reachable_IsFifthValue(CancellationToken ct = default) {
        var value = InfiniFrameDebugEndpointStatus.Reachable;
        await Assert.That(value).IsEqualTo(InfiniFrameDebugEndpointStatus.Reachable);
    }

    [Test]
    public async Task Unreachable_IsSixthValue(CancellationToken ct = default) {
        var value = InfiniFrameDebugEndpointStatus.Unreachable;
        await Assert.That(value).IsEqualTo(InfiniFrameDebugEndpointStatus.Unreachable);
    }

    [Test]
    public async Task ProbeFailed_IsSeventhValue(CancellationToken ct = default) {
        var value = InfiniFrameDebugEndpointStatus.ProbeFailed;
        await Assert.That(value).IsEqualTo(InfiniFrameDebugEndpointStatus.ProbeFailed);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        InfiniFrameDebugEndpointStatus[] values = Enum.GetValues<InfiniFrameDebugEndpointStatus>();
        int count = values.Length;
        await Assert.That(count).IsEqualTo(7);
    }
}
