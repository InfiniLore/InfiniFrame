// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef __APPLE__
#include "Runtime/Platform/Mac/Window.Cocoa.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::SetTaskbarProgress(int state, uint64_t current, uint64_t total) {
    if (!m_impl || !m_impl->_window) return;

    NSDockTile* dockTile = [NSApp dockTile];
    if (!dockTile) return;

    if (state == 0) {
        // None state - clear the badge
        dockTile.badgeLabel = @"";
        [dockTile display];
        return;
    }

    NSString* badgeText = nil;
    if (total > 0) {
        double percentage = static_cast<double>(current) / static_cast<double>(total) * 100.0;
        badgeText = [NSString stringWithFormat:@"%.0f%%", percentage];
    }
    else if (state == 1) {
        // Indeterminate - show a generic indicator
        badgeText = @"...";
    }

    dockTile.badgeLabel = badgeText ?: @"";
    [dockTile display];
}

void InfiniFrameWindow::ClearTaskbarProgress() {
    if (!m_impl || !m_impl->_window) return;

    NSDockTile* dockTile = [NSApp dockTile];
    if (!dockTile) return;

    dockTile.badgeLabel = @"";
    [dockTile display];
}

void InfiniFrameWindow::SetTaskbarFlash(int mode, uint32_t count) {
    if (!m_impl || !m_impl->_window) return;

    switch (mode) {
        case 0: // Stop - no-op on macOS, attention is one-shot
            break;
        case 1: // All - request attention until app activates
        case 3: // TimerAll
            [NSApp requestUserAttention:NSCriticalRequest];
            break;
        case 2: // Timer - request attention for informational purposes
            [NSApp requestUserAttention:NSInformationalRequest];
            break;
        default:
            break;
    }
}

void InfiniFrameWindow::StopTaskbarFlash() {
    // macOS attention requests are one-shot and cannot be cancelled.
    // The request is automatically cleared when the app activates.
    // No-op here.
}

void InfiniFrameWindow::GetTaskbarProgressSupported(bool* supported) const {
    if (supported) *supported = true;
}

#endif
