// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SkipOnWindowsAttribute(string? message = null) : SkipAttribute(message ?? "This test is not supported on Windows environments") {
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
        => Task.FromResult(OperatingSystem.IsWindows());
}
