#if INFINIFRAME_MACOS_TEST_HOST
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once CheckNamespace
namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class MacOsTestingPlatformEntryPoint {
    #pragma warning disable TUnit0034
    public static async Task<int> Main(string[] args) {
        if (!OperatingSystem.IsMacOS()) return await MacOsTestingPlatform.RunTestingPlatformAsync(args);
        return await MacOsTestingPlatform.RunMacOsTestingPlatformAsync(args);
    }
    #pragma warning restore TUnit0034
}
#endif
