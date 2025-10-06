// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.InfiniLore.Photino.NET.TestUtilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class SkipUtilities {
    public static void SkipOnLinux(Func<bool> predicate) {
        if (!OperatingSystem.IsLinux()) return;
        
        Skip.When(predicate(), "This test is not supported on Linux environments with the current test setup");
    }
    
    public static void SkipOnLinux(bool? state = null) {
        if (!OperatingSystem.IsLinux()) return;
        
        if (state is null) {
            Skip.Test("This test is not supported on Linux environments");
            return;
        }
        
        Skip.When(state.Value, "This test is not supported on Linux environments with the current test setup");
    }
}
