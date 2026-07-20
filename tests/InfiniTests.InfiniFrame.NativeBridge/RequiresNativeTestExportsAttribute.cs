// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests.InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class RequiresNativeTestExportsAttribute()
    : SkipAttribute("This test requires a native bridge built with test exports enabled.") {
    public override Task<bool> ShouldSkip(TestRegisteredContext context) {
        #if InfiniFrameNativeTestExports
        return Task.FromResult(false);
        #else
        return Task.FromResult(true);
        #endif
    }
}
