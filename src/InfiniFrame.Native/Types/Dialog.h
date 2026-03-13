#pragma once
/**
 * @file Dialog.h
 * @brief Dialog-related types and enums
 */

#ifndef INFINIFRAME_TYPES_DIALOG_H
#define INFINIFRAME_TYPES_DIALOG_H

// ============================================================================
// Dialog Result
// ============================================================================

/** @brief Button pressed by the user to dismiss a message box */
enum class DialogResult
{
    Cancel = -1, /// Dialog was cancelled (Escape key or window close)
    Ok,          /// User pressed OK
    Yes,         /// User pressed Yes
    No,          /// User pressed No
    Abort,       /// User pressed Abort
    Retry,       /// User pressed Retry
    Ignore,      /// User pressed Ignore
};

// ============================================================================
// Dialog Buttons
// ============================================================================

/** @brief Button set to display in a message box */
enum class DialogButtons
{
    Ok,               /// Single OK button
    OkCancel,         /// OK and Cancel buttons
    YesNo,            /// Yes and No buttons
    YesNoCancel,      /// Yes, No, and Cancel buttons
    RetryCancel,      /// Retry and Cancel buttons
    AbortRetryIgnore, /// Abort, Retry, and Ignore buttons
};

// ============================================================================
// Dialog Icon
// ============================================================================

/** @brief Icon shown in a message box */
enum class DialogIcon
{
    Info,
    Warning,
    Error,
    Question,
};

// ============================================================================
// Monitor
// ============================================================================

/** @brief Describes the geometry of a single display */
struct Monitor
{
    /** @brief Pixel rectangle relative to the virtual desktop */
    struct MonitorRect
    {
        int x, y;          /// Top-left corner in virtual-desktop coordinates
        int width, height; /// Dimensions in physical pixels
    } monitor,             /// Full monitor bounds (including taskbar)
        work;              /// Work area bounds (excluding taskbar and docked toolbars)
    double scale;          /// DPI scale factor (1.0 = 100%, 1.5 = 150%)
};

#endif // INFINIFRAME_TYPES_DIALOG_H