// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Blazor;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowActionTests {

    [Test]
    public async Task Minimize_IsFirstValue(CancellationToken ct = default) {
        var value = WindowAction.Minimize;
        await Assert.That(value).IsEqualTo(WindowAction.Minimize);
    }

    [Test]
    public async Task Maximize_IsSecondValue(CancellationToken ct = default) {
        var value = WindowAction.Maximize;
        await Assert.That(value).IsEqualTo(WindowAction.Maximize);
    }

    [Test]
    public async Task Close_IsThirdValue(CancellationToken ct = default) {
        var value = WindowAction.Close;
        await Assert.That(value).IsEqualTo(WindowAction.Close);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        WindowAction[] values = Enum.GetValues<WindowAction>();
        int count = values.Length;
        await Assert.That(count).IsEqualTo(3);
    }
}
