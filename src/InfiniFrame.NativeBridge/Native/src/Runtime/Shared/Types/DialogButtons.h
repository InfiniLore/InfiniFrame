#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// Button layout for a native message dialog.
enum class DialogButtons {
    /// Single OK button.
    Ok,
    /// OK and Cancel buttons.
    OkCancel,
    /// Yes and No buttons.
    YesNo,
    /// Yes, No, and Cancel buttons.
    YesNoCancel,
    /// Retry and Cancel buttons.
    RetryCancel,
    /// Abort, Retry, and Ignore buttons.
    AbortRetryIgnore,
};
