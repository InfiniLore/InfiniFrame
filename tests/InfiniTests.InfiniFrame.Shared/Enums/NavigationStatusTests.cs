// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NavigationStatusTests {

    [Test]
    public async Task Succeeded_IsFirstValue(CancellationToken ct = default) {
        var value = NavigationStatus.Succeeded;
        await Assert.That(value).IsEqualTo(NavigationStatus.Succeeded);
    }

    [Test]
    public async Task Failed_IsSecondValue(CancellationToken ct = default) {
        var value = NavigationStatus.Failed;
        await Assert.That(value).IsEqualTo(NavigationStatus.Failed);
    }

    [Test]
    public async Task Superseded_IsThirdValue(CancellationToken ct = default) {
        var value = NavigationStatus.Superseded;
        await Assert.That(value).IsEqualTo(NavigationStatus.Superseded);
    }

    [Test]
    public async Task WindowClosed_IsFourthValue(CancellationToken ct = default) {
        var value = NavigationStatus.WindowClosed;
        await Assert.That(value).IsEqualTo(NavigationStatus.WindowClosed);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        NavigationStatus[] values = Enum.GetValues<NavigationStatus>();
        int count = values.Length;
        await Assert.That(count).IsEqualTo(4);
    }
}
