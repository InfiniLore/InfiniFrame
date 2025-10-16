namespace InfiniLore.InfiniFrame;
/// <summary>
///     Defines options for a dialog
/// </summary>
[Flags]
public enum PhotinoDialogOptions : byte {
    /// <summary>
    ///     Represents no options for the dialog.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Enables multi-selection in the dialog.
    /// </summary>
    MultiSelect = 0x1,

    /// <summary>
    ///     Forces an overwrite of existing files without prompting the user in the dialog.
    /// </summary>
    ForceOverwrite = 0x2,

    /// <summary>
    ///     Disables the capability of creating folders via the dialog.
    /// </summary>
    DisableCreateFolder = 0x4
}
