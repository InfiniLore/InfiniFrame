// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Handles;

namespace InfiniTests.InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NativeHandleAccessTests {

    [Test]
    public async Task Feature_IsFirstValue(CancellationToken ct = default) {
        var value = NativeHandleAccess.Feature;
        await Assert.That(value).IsEqualTo(NativeHandleAccess.Feature);
    }

    [Test]
    public async Task Close_IsSecondValue(CancellationToken ct = default) {
        var value = NativeHandleAccess.Close;
        await Assert.That(value).IsEqualTo(NativeHandleAccess.Close);
    }

    [Test]
    public async Task WaitForExit_IsThirdValue(CancellationToken ct = default) {
        var value = NativeHandleAccess.WaitForExit;
        await Assert.That(value).IsEqualTo(NativeHandleAccess.WaitForExit);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        NativeHandleAccess[] values = Enum.GetValues<NativeHandleAccess>();
        int count = values.Length;
        await Assert.That(count).IsEqualTo(3);
    }
}
