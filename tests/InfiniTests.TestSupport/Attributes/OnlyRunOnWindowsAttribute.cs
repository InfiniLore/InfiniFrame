// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests.TestSupport.Attributes;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OnlyRunOnWindowsAttribute(string? message = null) : SkipAttribute(message ?? "This test is only supported on Windows environments") {
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
        => Task.FromResult(!OperatingSystem.IsWindows());
}
