// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.SingleFile;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Indicates whether the application is running in single-file pack mode.
/// </summary>
public static class InfiniFramePackMode {
    // ReSharper disable once UnassignedField.Global
    #pragma warning disable CA2211
    /// <summary>
    ///     Gets or sets whether single-file pack mode is active. Set to <c>true</c> by the pack tool at build time.
    /// </summary>
    public static bool IsActive;
    #pragma warning restore CA2211
}
