// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Utilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class TitleStringHelper {
    internal const string DefaultTitle = "InfiniFrame";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static string Validate(string? title, bool limitLinuxLength) {
        if (string.IsNullOrWhiteSpace(title)) return DefaultTitle;
        string newTitle = title.Trim();

        if (limitLinuxLength && OperatingSystem.IsLinux() && newTitle.Length > 31)
            newTitle = newTitle[..31];

        return newTitle.Length > 0 ? newTitle : DefaultTitle;
    }
}
