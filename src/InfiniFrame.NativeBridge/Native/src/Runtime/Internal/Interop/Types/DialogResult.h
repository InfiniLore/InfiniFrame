#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// Possible user responses from a native dialog.
enum class DialogResult {
    /// The user dismissed the dialog without choosing an action.
    Cancel = -1,
    /// The user clicked the OK button.
    Ok,
    /// The user clicked the Yes button.
    Yes,
    /// The user clicked the No button.
    No,
    /// The user clicked the Abort button.
    Abort,
    /// The user clicked the Retry button.
    Retry,
    /// The user clicked the Ignore button.
    Ignore,
};
