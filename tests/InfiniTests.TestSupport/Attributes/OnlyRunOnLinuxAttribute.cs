// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests.TestSupport.Attributes;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OnlyRunOnLinuxAttribute(string? message = null) : SkipAttribute(message ?? "This test is only supported on Linux environments") {
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
        => Task.FromResult(!OperatingSystem.IsLinux());
}
