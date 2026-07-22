// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface Point { x: number; y: number }
export interface Size { width: number; height: number }
export interface Rectangle extends Point, Size {}
export interface InfiniMonitor { monitorArea: Rectangle; workArea: Rectangle; scale: number }
export interface FilePickerFilter { name: string; extensions: string[] }

export type ResizeOrigin = "topLeft" | "top" | "topRight" | "right" | "bottomRight" | "bottom" | "bottomLeft" | "left";
export type DialogButtons = "ok" | "okCancel" | "yesNo" | "yesNoCancel" | "retryCancel" | "abortRetryIgnore";
export type DialogIcon = "info" | "warning" | "error" | "question";
export type DialogResult = "cancel" | "ok" | "yes" | "no" | "abort" | "retry" | "ignore";
export type WindowLifecycleState = "created" | "initializing" | "running" | "closingRequested" | "nativeClosed" | "disposed";
export type DebugEndpointStatus = "notSupported" | "disabled" | "unavailable" | "configured" | "reachable" | "unreachable" | "probeFailed";

export interface DebugCapabilities {
    supportsLocalDevTools: boolean;
    supportsRemoteDebuggingEndpoint: boolean;
    supportsWebInspectorAttach: boolean;
    supportsScriptErrorForwarding: boolean;
    supportsNavigationDiagnostics: boolean;
}

export interface DebugDiagnostics {
    platform: string;
    runtime: string;
    browserRuntime: string | null;
    capabilities: DebugCapabilities;
    devToolsEnabled: boolean;
    remoteDebuggingPort: number | null;
    webInspectorEnabled: boolean;
    endpointStatus: DebugEndpointStatus;
    endpoint: string | null;
    endpointReason: string | null;
    isWindowClosed: boolean;
    platformNotes: string | null;
}

export interface DebugEndpointResult { success: boolean; endpoint: string | null; reason: string | null }
