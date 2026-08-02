// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SkipOnLinuxAttribute(string? message = null) : SkipAttribute(message ?? "This test is not supported on Linux environments") {
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
        => Task.FromResult(OperatingSystem.IsLinux());
}