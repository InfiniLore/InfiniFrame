// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.InfiniLore.Photino.NET.TestUtilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SkinOnLinuxAttribute() : SkipAttribute("This test is not supported on Linux environments") {
    public override Task<bool> ShouldSkip(TestRegisteredContext context) {
        return Task.FromResult(OperatingSystem.IsLinux());
    }
}
