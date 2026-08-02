// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.InstanceArbitration;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InstanceArbitrationModeTests {

    [Test]
    public async Task Disabled_IsDefault(CancellationToken ct) {
        InstanceArbitrationMode defaultValue = default;
        await Assert.That(defaultValue).IsEqualTo(InstanceArbitrationMode.Disabled);
    }

    [Test]
    public async Task Disabled_HasExpectedValue(CancellationToken ct) {
        int value = (int)InstanceArbitrationMode.Disabled;
        await Assert.That(value).IsEqualTo(0);
    }

    [Test]
    public async Task PrimaryOnly_HasExpectedValue(CancellationToken ct) {
        int value = (int)InstanceArbitrationMode.PrimaryOnly;
        await Assert.That(value).IsEqualTo(1);
    }

    [Test]
    public async Task PrimaryWithArgForwarding_HasExpectedValue(CancellationToken ct) {
        int value = (int)InstanceArbitrationMode.PrimaryWithArgForwarding;
        await Assert.That(value).IsEqualTo(2);
    }
}
