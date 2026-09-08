#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// Describes the geometry and scale of a connected display monitor.
struct Monitor {
    /// Rectangle describing a monitor area (position and size in screen pixels).
    struct MonitorRect {
        /// Horizontal position of the top-left corner in screen pixels.
        int x;
        /// Vertical position of the top-left corner in screen pixels.
        int y;
        /// Width of the rectangle in pixels.
        int width;
        /// Height of the rectangle in pixels.
        int height;
    };

    /// Full physical bounds of the monitor.
    MonitorRect monitor;
    /// Usable work area excluding taskbars and docks.
    MonitorRect work;

    /// Display scaling factor (e.g. 1.0 for 100%, 2.0 for 200%).
    double scale;
};
