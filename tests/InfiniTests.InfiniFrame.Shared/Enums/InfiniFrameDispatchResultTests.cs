// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameDispatchResultTests {

    [Test]
    public async Task Completed_IsFirstValue(CancellationToken ct = default) {
        var value = InfiniFrameDispatchResult.Completed;
        await Assert.That(value).IsEqualTo(InfiniFrameDispatchResult.Completed);
    }

    [Test]
    public async Task TimedOut_IsSecondValue(CancellationToken ct = default) {
        var value = InfiniFrameDispatchResult.TimedOut;
        await Assert.That(value).IsEqualTo(InfiniFrameDispatchResult.TimedOut);
    }

    [Test]
    public async Task Cancelled_IsThirdValue(CancellationToken ct = default) {
        var value = InfiniFrameDispatchResult.Cancelled;
        await Assert.That(value).IsEqualTo(InfiniFrameDispatchResult.Cancelled);
    }

    [Test]
    public async Task WindowClosed_IsFourthValue(CancellationToken ct = default) {
        var value = InfiniFrameDispatchResult.WindowClosed;
        await Assert.That(value).IsEqualTo(InfiniFrameDispatchResult.WindowClosed);
    }

    [Test]
    public async Task Failed_IsFifthValue(CancellationToken ct = default) {
        var value = InfiniFrameDispatchResult.Failed;
        await Assert.That(value).IsEqualTo(InfiniFrameDispatchResult.Failed);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        InfiniFrameDispatchResult[] values = Enum.GetValues<InfiniFrameDispatchResult>();
        int count = values.Length;
        await Assert.That(count).IsEqualTo(5);
    }
}
