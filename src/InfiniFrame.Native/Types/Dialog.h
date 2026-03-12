#pragma once
/**
 * @file Dialog.h
 * @brief Dialog-related types and enums
 */

#ifndef INFINIFRAME_TYPES_DIALOG_H
#define INFINIFRAME_TYPES_DIALOG_H

namespace InfiniFrame {

// ============================================================================
// Dialog Result
// ============================================================================

enum class DialogResult
{
    Cancel = -1,
    Ok,
    Yes,
    No,
    Abort,
    Retry,
    Ignore,
};

// ============================================================================
// Dialog Buttons
// ============================================================================

enum class DialogButtons
{
    Ok,
    OkCancel,
    YesNo,
    YesNoCancel,
    RetryCancel,
    AbortRetryIgnore,
};

// ============================================================================
// Dialog Icon
// ============================================================================

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

struct Monitor {
    struct MonitorRect {
        int x, y;
        int width, height;
    } monitor, work;
    double scale;
};

} // namespace InfiniFrame

#endif // INFINIFRAME_TYPES_DIALOG_H
