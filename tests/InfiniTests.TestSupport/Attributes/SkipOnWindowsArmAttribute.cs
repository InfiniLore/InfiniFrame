// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SkipOnWindowsArmAttribute(string? message = null) : SkipAttribute(message ?? "This test is not supported on Windows environments") {
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
        => Task.FromResult(OperatingSystem.IsWindows() && RuntimeInformation.ProcessArchitecture == Architecture.Arm64);
}
