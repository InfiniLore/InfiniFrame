/**
 * Global InfiniFrame namespace contract. Defines the window.infiniframe shape.
 * @module InfiniFrame
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameHostMessaging} from "./InfiniFrameHostMessaging";
import type {InfiniFrameUtils} from "./InfiniFrameUtils";
import type {InfiniFrameWindow} from "./Window/InfiniFrameWindow";
import type {InfiniFrameHostBridge} from "./InfiniFrameHostBridge";
import type {InfiniFrameSetup} from "./InfiniFrameSetup";
import type {WindowChrome} from "../Window/WindowChrome";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Root shape of the `window.infiniframe` namespace.
 * Aggregates all subsystems exposed to JavaScript: host bridge, messaging, window API, utilities, and setup state.
 */
export interface InfiniFrame {
    /** Native host bridge for WebView2/WebKit communication. Available after host initialization. */
    host?: InfiniFrameHostBridge;
    /** Initialization configuration received from C# at startup. Available after setup completes. */
    setup?: InfiniFrameSetup;
    /** Messaging layer for sending and receiving messages with the native host. */
    messaging: InfiniFrameHostMessaging;
    /** Window API providing access to all window features (position, size, state, etc.). */
    window: InfiniFrameWindow;
    /** Utility functions for pointer capture and other DOM helpers. */
    utils: InfiniFrameUtils;
    /** Window chrome controller for custom title bar and frame styling. Available when chrome is enabled. */
    windowChrome?: WindowChrome;
}
