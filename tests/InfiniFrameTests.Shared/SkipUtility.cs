// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class SkipUtility {
    #region Reasons
    public const string LinuxMovement = "The current test environment does not properly support window moving";
    public const string MacOsMainThreadIssue = "API misuse: setting the main menu on a non-main thread. Main menu contents should only be modified from the main thread";
    #endregion

    #region Attributes
    public class SkipOnLinuxAttribute(string? message = null) : SkipAttribute(message ?? "This test is not supported on Linux environments") {
        public override Task<bool> ShouldSkip(TestRegisteredContext context)
            => Task.FromResult(OperatingSystem.IsLinux());
    }

    public class SkipOnWindowsAttribute(string? message = null) : SkipAttribute(message ?? "This test is not supported on Windows environments") {
        public override Task<bool> ShouldSkip(TestRegisteredContext context)
            => Task.FromResult(OperatingSystem.IsWindows());
    }

    public class SkipOnWindowsArmAttribute(string? message = null) : SkipAttribute(message ?? "This test is not supported on Windows environments") {
        public override Task<bool> ShouldSkip(TestRegisteredContext context)
            => Task.FromResult(OperatingSystem.IsWindows() && RuntimeInformation.ProcessArchitecture == Architecture.Arm64);
    }

    public class SkipOnMacOsAttribute(string? message = null) : SkipAttribute(message ?? "This test is not supported on Mac OS environments") {
        public override Task<bool> ShouldSkip(TestRegisteredContext context)
            => Task.FromResult(OperatingSystem.IsMacOS());
    }
    
    public class OnlyRunOnWindows(string? message = null) : SkipAttribute(message ?? "This test is only supported on Windows environments") {
        public override Task<bool> ShouldSkip(TestRegisteredContext context)
            => Task.FromResult(!OperatingSystem.IsWindows());
    }
    
    public class OnlyRunOnWindowsX64(string? message = null) : SkipAttribute(message ?? "This test is only supported on Windows environments") {
        public override Task<bool> ShouldSkip(TestRegisteredContext context)
            => Task.FromResult(!OperatingSystem.IsWindows() || RuntimeInformation.ProcessArchitecture != Architecture.X64);
    }
    
    public class OnlyRunOnMacOs(string? message = null) : SkipAttribute(message ?? "This test is only supported on MacOs environments") {
        public override Task<bool> ShouldSkip(TestRegisteredContext context)
            => Task.FromResult(!OperatingSystem.IsMacOS());
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

    public static void SkipOnWindows(Func<bool> predicate) {
        if (!OperatingSystem.IsWindows()) return;

        Skip.When(predicate(), "This test is not supported on Windows environments with the current test setup");
    }

    public static void SkipOnWindows(bool? state = null) {
        if (!OperatingSystem.IsWindows()) return;

        if (state is null) {
            Skip.Test("This test is not supported on Windows environments");
            return;
        }

        Skip.When(state.Value, "This test is not supported on Windows environments with the current test setup");
    }
    #endregion
}
