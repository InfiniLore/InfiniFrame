// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.InfiniLore.Photino.NET.TestUtilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class SkipUtilities {
    #region Attributes
    public class OnLinuxAttribute() : SkipAttribute("This test is not supported on Linux environments") {
        public override Task<bool> ShouldSkip(TestRegisteredContext context)
            => Task.FromResult(OperatingSystem.IsLinux());
    }

    public class OnWindowsAttribute() : SkipAttribute("This test is not supported on Windows environments") {
        public override Task<bool> ShouldSkip(TestRegisteredContext context)
            => Task.FromResult(OperatingSystem.IsWindows());
    }
        
    public class OnMacOsAttribute() : SkipAttribute("This test is not supported on Mac OS environments") {
        public override Task<bool> ShouldSkip(TestRegisteredContext context)
            => Task.FromResult(OperatingSystem.IsMacOS());
    }
    #endregion
    
    #region Methods
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
    #endregion
}
