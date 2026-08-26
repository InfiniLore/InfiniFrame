// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides utility methods for validating and formatting window title strings.
/// </summary>
internal static class TitleStringUtility {
    /// <summary>
    ///     The default title used when no title is specified.
    /// </summary>
    public const string DefaultTitle = "InfiniFrame";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Validates and optionally truncates a title string for use in a window title bar.
    /// </summary>
    /// <param name="title">The title string to validate.</param>
    /// <param name="limitLinuxLength">If <c>true</c>, truncates the title to 31 characters on Linux.</param>
    /// <returns>The validated and possibly truncated title, or <c>null</c> if the input is null or whitespace.</returns>
    public static string? Validate(string? title, bool limitLinuxLength) {
        if (string.IsNullOrWhiteSpace(title)) return title;

        string newTitle = title.Trim();

        if (limitLinuxLength && OperatingSystem.IsLinux() && newTitle.Length > 31)
            newTitle = newTitle[..31];

        return newTitle.Length > 0 ? newTitle : DefaultTitle;
    }
}
