// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SkipOnMacOsAttribute(string? message = null) : SkipAttribute(message ?? "This test is not supported on Mac OS environments") {
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
        => Task.FromResult(OperatingSystem.IsMacOS());
}