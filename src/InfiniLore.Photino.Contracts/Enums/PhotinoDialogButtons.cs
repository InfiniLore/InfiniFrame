namespace InfiniLore.Photino;

/// <summary>
///     Represents the types of buttons that can be displayed in a Photino-dialog.
/// </summary>
public enum PhotinoDialogButtons
{
    /// <summary>
    ///     Represents a dialog with an OK button.
    /// </summary>
    Ok,

    /// <summary>
    ///     Represents a dialog with OK and Cancel buttons.
    /// </summary>
    OkCancel,

    /// <summary>
    ///     Represents a dialog with Yes and No buttons.
    /// </summary>
    YesNo,

    /// <summary>
    ///     Represents a dialog with Yes, No, and Cancel buttons.
    /// </summary>
    YesNoCancel,

    /// <summary>
    ///     Represents a dialog with Retry and Cancel buttons.
    /// </summary>
    RetryCancel,

    /// <summary>
    ///     Represents a dialog with Abort, Retry, and Ignore buttons.
    /// </summary>
    AbortRetryIgnore
}
