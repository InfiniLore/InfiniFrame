// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class SkipUtility {
    #region Reasons
    public const string LinuxMovement = "The current test environment does not properly support window moving";
    public const string MacOsMainThreadIssue = "API misuse: setting the main menu on a non-main thread. Main menu contents should only be modified from the main thread";
    #endregion

    #region Methods
    public static void SkipOnLinux(Func<bool> predicate) {
        if (!OperatingSystem.IsLinux()) return;

        Skip.When(predicate(), "This test is not supported on Linux environments with the current test setup");
    }

    public static void SkipOnLinux(bool? state = null) {
        if (!OperatingSystem.IsLinux()) return;

        Skip.When(state is null, "This test is not supported on Linux environments");
        Skip.When(state.Value, "This test is not supported on Linux environments with the current test setup");
    }

    public static void SkipOnWindows(Func<bool> predicate) {
        if (!OperatingSystem.IsWindows()) return;

        Skip.When(predicate(), "This test is not supported on Windows environments with the current test setup");
    }

    public static void SkipOnWindows(bool? state = null) {
        if (!OperatingSystem.IsWindows()) return;

        Skip.When(state is null, "This test is not supported on Windows environments");
        Skip.When(state.Value, "This test is not supported on Windows environments with the current test setup");
    }
    #endregion
}