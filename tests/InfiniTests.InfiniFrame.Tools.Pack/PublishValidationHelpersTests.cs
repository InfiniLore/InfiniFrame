// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;

namespace InfiniTests.InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PublishValidationHelpersTests {

    // -----------------------------------------------------------------------------------------------------------------
    // ValidateRidConsistency
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("win-x64")]
    [Arguments("win-arm64")]
    [Arguments("linux-x64")]
    [Arguments("linux-arm64")]
    [Arguments("osx-x64")]
    [Arguments("osx-arm64")]
    public async Task ValidateRidConsistency_ValidRids_DoesNotThrow(string rid, CancellationToken ct) {
        await Assert.That(() => PublishValidationHelpers.ValidateRidConsistency(rid)).ThrowsNothing();
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task ValidateRidConsistency_Empty_ThrowsInvalidOperationException(string rid, CancellationToken ct) {
        await Assert.That(() => PublishValidationHelpers.ValidateRidConsistency(rid))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ValidateRidConsistency_NoDash_ThrowsInvalidOperationException(CancellationToken ct) {
        await Assert.That(() => PublishValidationHelpers.ValidateRidConsistency("winx64"))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ValidateRidConsistency_UnknownOs_ThrowsInvalidOperationException(CancellationToken ct) {
        await Assert.That(() => PublishValidationHelpers.ValidateRidConsistency("freebsd-x64"))
            .Throws<InvalidOperationException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ParseRid
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("win-x64", "windows", "x64")]
    [Arguments("win-arm64", "windows", "arm64")]
    [Arguments("linux-x64", "linux", "x64")]
    [Arguments("linux-arm64", "linux", "arm64")]
    [Arguments("osx-x64", "osx", "x64")]
    [Arguments("osx-arm64", "osx", "arm64")]
    public async Task ParseRid_ValidRids_ReturnsCorrectParts(string rid, string expectedPlatform, string expectedArch, CancellationToken ct) {
        var result = PublishValidationHelpers.ParseRid(rid);
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Value.Platform).IsEqualTo(expectedPlatform);
        await Assert.That(result.Value.Architecture).IsEqualTo(expectedArch);
    }

    [Test]
    public async Task ParseRid_InvalidRid_ReturnsNull(CancellationToken ct = default) {
        var result = PublishValidationHelpers.ParseRid("invalid");
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task ParseRid_UnknownPlatform_ReturnsNull(CancellationToken ct = default) {
        var result = PublishValidationHelpers.ParseRid("freebsd-x64");
        await Assert.That(result).IsNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ExpectedPeMachineForRid
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ExpectedPeMachineForRid_X64_ReturnsAmd64(CancellationToken ct = default) {
        ushort result = PublishValidationHelpers.ExpectedPeMachineForRid("win-x64");
        await Assert.That(result).IsEqualTo((ushort)0x8664);
    }

    [Test]
    public async Task ExpectedPeMachineForRid_Arm64_ReturnsArm64(CancellationToken ct = default) {
        ushort result = PublishValidationHelpers.ExpectedPeMachineForRid("win-arm64");
        await Assert.That(result).IsEqualTo((ushort)0xAA64);
    }

    [Test]
    public async Task ExpectedPeMachineForRid_UnsupportedArch_ThrowsInvalidOperationException(CancellationToken ct = default) {
        await Assert.That(() => PublishValidationHelpers.ExpectedPeMachineForRid("win-mips"))
            .Throws<InvalidOperationException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // DescribePeMachine
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task DescribePeMachine_Amd64_ReturnsX64(CancellationToken ct = default) {
        string result = PublishValidationHelpers.DescribePeMachine(0x8664);
        await Assert.That(result).Contains("x64");
    }

    [Test]
    public async Task DescribePeMachine_Arm64_ReturnsArm64(CancellationToken ct = default) {
        string result = PublishValidationHelpers.DescribePeMachine(0xAA64);
        await Assert.That(result).Contains("arm64");
    }

    [Test]
    public async Task DescribePeMachine_Unknown_ReturnsHexValue(CancellationToken ct = default) {
        string result = PublishValidationHelpers.DescribePeMachine(0x014C);
        await Assert.That(result).Contains("0x014C");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ReadPeMachineFromStream
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ReadPeMachineFromStream_TooShort_ThrowsInvalidOperationException(CancellationToken ct = default) {
        using var stream = new MemoryStream(new byte[10]);
        await Assert.That(() => PublishValidationHelpers.ReadPeMachineFromStream(stream))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ReadPeMachineFromStream_MissingMZ_ThrowsInvalidOperationException(CancellationToken ct = default) {
        byte[] data = new byte[64];
        data[0] = (byte)'N';
        data[1] = (byte)'E';
        using var stream = new MemoryStream(data);
        await Assert.That(() => PublishValidationHelpers.ReadPeMachineFromStream(stream))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ReadPeMachineFromStream_ValidX64PE_ReturnsAmd64(CancellationToken ct = default) {
        // Build a minimal valid PE with x64 machine
        byte[] data = new byte[256];
        data[0] = (byte)'M';
        data[1] = (byte)'Z';
        // PE header offset at 0x3C
        data[0x3C] = 0x40;
        // PE signature at offset 0x40
        data[0x40] = (byte)'P';
        data[0x41] = (byte)'E';
        // Machine at offset 0x44 (PE signature + 4)
        data[0x44] = 0x64;
        data[0x45] = 0x86; // 0x8664 = AMD64

        using var stream = new MemoryStream(data);
        ushort machine = PublishValidationHelpers.ReadPeMachineFromStream(stream);
        await Assert.That(machine).IsEqualTo((ushort)0x8664);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ValidateOutputPath
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ValidateOutputPath_UnderProjectBin_ReturnsTrue(CancellationToken ct = default) {
        string projectDir = Path.GetTempPath();
        string outputPath = Path.Join(projectDir, "bin", "test");
        bool result = PublishValidationHelpers.ValidateOutputPath(projectDir, outputPath, false);
        await Assert.That(result).IsTrue();
    }
}
