// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {DebugCapabilities, DebugDiagnostics, DebugEndpointResult} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameWindowFeatureDebugging {
    isDevToolsEnabledAsync(): Promise<boolean>;
    supportsWebInspectorAttachAsync(): Promise<boolean>;
    isWebInspectorEnabledAsync(): Promise<boolean>;
    supportsRemoteDebuggingEndpointAsync(): Promise<boolean>;
    getRemoteDebuggingPortAsync(): Promise<number | null>;
    getCapabilitiesAsync(): Promise<DebugCapabilities>;
    getDiagnosticsAsync(): Promise<DebugDiagnostics>;
    tryGetRemoteDebuggingEndpointAsync(): Promise<DebugEndpointResult>;
    tryProbeEndpointAsync(): Promise<DebugEndpointResult>;
    enableDevTools(enabled: boolean): void;
}
