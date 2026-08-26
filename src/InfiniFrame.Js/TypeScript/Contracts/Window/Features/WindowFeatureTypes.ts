/**
 * Shared type definitions for window features. Includes Point, Size, Rectangle, and feature-specific types.
 * @module WindowFeatureTypes
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * A 2D coordinate point.
 */
export interface Point {
    /** Horizontal coordinate. */
    x: number;
    /** Vertical coordinate. */
    y: number
}

/**
 * A 2D dimensions value.
 */
export interface Size {
    /** Horizontal dimension. */
    width: number;
    /** Vertical dimension. */
    height: number
}

/**
 * A rectangular region defined by position and size.
 * Extends both {@link Point} and {@link Size}.
 */
export interface Rectangle extends Point, Size {
}

/**
 * Information about a connected display monitor.
 */
export interface InfiniMonitor {
    /** Full physical area of the monitor including taskbar. */
    monitorArea: Rectangle;
    /** Usable work area excluding taskbar and dock. */
    workArea: Rectangle;
    /** Display scale factor (e.g. 1.0, 1.5, 2.0). */
    scale: number
}

/**
 * Definition of a file type filter for file picker dialogs.
 */
export interface FilePickerFilter {
    /** Display name for the filter (e.g. "Images"). */
    name: string;
    /** File extensions allowed by this filter (e.g. ["png", "jpg"]). */
    extensions: string[]
}

/**
 * Origin anchor point for resize operations.
 */
export type ResizeOrigin = "topLeft" | "top" | "topRight" | "right" | "bottomRight" | "bottom" | "bottomLeft" | "left";

/**
 * Predefined button combinations for message dialogs.
 */
export type DialogButtons = "ok" | "okCancel" | "yesNo" | "yesNoCancel" | "retryCancel" | "abortRetryIgnore";

/**
 * Standard icon types for message dialogs.
 */
export type DialogIcon = "info" | "warning" | "error" | "question";

/**
 * Possible return values from a message dialog.
 */
export type DialogResult = "cancel" | "ok" | "yes" | "no" | "abort" | "retry" | "ignore";

/**
 * Lifecycle states of an InfiniFrame window.
 */
export type WindowLifecycleState =
    /** Window has been created but not yet initialized. */
    "created"
    /** Window is performing initialization. */
    | "initializing"
    /** Window is running and interactive. */
    | "running"
    /** A close has been requested but the window is still visible. */
    | "closingRequested"
    /** The native window has been closed. */
    | "nativeClosed"
    /** The window instance has been disposed. */
    | "disposed";

/**
 * Status of a remote debugging endpoint.
 */
export type DebugEndpointStatus =
    /** Platform does not support remote debugging endpoints. */
    "notSupported"
    /** Remote debugging is disabled in configuration. */
    | "disabled"
    /** Endpoint is not available (e.g. port not open). */
    | "unavailable"
    /** Endpoint is configured but not yet verified. */
    | "configured"
    /** Endpoint is reachable and responding. */
    | "reachable"
    /** Endpoint is configured but not responding. */
    | "unreachable"
    /** Endpoint probe attempt failed. */
    | "probeFailed";

/**
 * Platform debugging capabilities.
 */
export interface DebugCapabilities {
    /** Whether local DevTools can be opened. */
    supportsLocalDevTools: boolean;
    /** Whether a remote debugging endpoint can be exposed. */
    supportsRemoteDebuggingEndpoint: boolean;
    /** Whether the Web Inspector can be attached. */
    supportsWebInspectorAttach: boolean;
    /** Whether script errors can be forwarded to the debugger. */
    supportsScriptErrorForwarding: boolean;
    /** Whether navigation diagnostics are available. */
    supportsNavigationDiagnostics: boolean;
}

/**
 * Comprehensive debugging diagnostic information for the current window.
 */
export interface DebugDiagnostics {
    /** Platform name (e.g. "Windows", "macOS", "Linux"). */
    platform: string;
    /** Runtime identifier (e.g. "WebView2", "WebKit"). */
    runtime: string;
    /** Browser engine version string, or null if unavailable. */
    browserRuntime: string | null;
    /** Platform debugging capabilities. */
    capabilities: DebugCapabilities;
    /** Whether DevTools are currently enabled. */
    devToolsEnabled: boolean;
    /** Remote debugging port number, or null if not exposed. */
    remoteDebuggingPort: number | null;
    /** Whether Web Inspector attachment is enabled. */
    webInspectorEnabled: boolean;
    /** Current status of the remote debugging endpoint. */
    endpointStatus: DebugEndpointStatus;
    /** Remote debugging endpoint URL, or null if not available. */
    endpoint: string | null;
    /** Reason the endpoint is in its current status, or null. */
    endpointReason: string | null;
    /** Whether the window has been closed. */
    isWindowClosed: boolean;
    /** Platform-specific notes or warnings, or null. */
    platformNotes: string | null;
}

/**
 * Result of a remote debugging endpoint probe operation.
 */
export interface DebugEndpointResult {
    /** Whether the probe was successful. */
    success: boolean;
    /** Endpoint URL if the probe succeeded, or null. */
    endpoint: string | null;
    /** Failure reason if the probe failed, or null. */
    reason: string | null
}
