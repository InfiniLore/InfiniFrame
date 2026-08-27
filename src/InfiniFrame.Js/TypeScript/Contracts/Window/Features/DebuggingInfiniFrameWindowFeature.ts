/**
 * Debugging feature contract. Defines the JS API for dev tools, remote debugging, web inspector,
 * and diagnostic queries on the InfiniFrame window.
 * @module DebuggingInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {DebugCapabilities, DebugDiagnostics, DebugEndpointResult} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Debugging feature API for the InfiniFrame window.
 * Provides methods to query and control debugging capabilities and remote endpoints.
 */
export interface DebuggingInfiniFrameWindowFeature {
    /**
     * Checks whether DevTools are enabled on this window.
     * @returns A promise resolving to true if DevTools can be opened.
     */
    isDevToolsEnabledAsync(): Promise<boolean>;

    /**
     * Checks whether the platform supports attaching the Web Inspector.
     * @returns A promise resolving to true if Web Inspector attach is supported.
     */
    supportsWebInspectorAttachAsync(): Promise<boolean>;

    /**
     * Checks whether Web Inspector attachment is currently enabled.
     * @returns A promise resolving to true if the Web Inspector is enabled.
     */
    isWebInspectorEnabledAsync(): Promise<boolean>;

    /**
     * Checks whether a remote debugging endpoint can be exposed.
     * @returns A promise resolving to true if remote debugging is supported.
     */
    supportsRemoteDebuggingEndpointAsync(): Promise<boolean>;

    /**
     * Gets the port number used for remote debugging.
     * @returns A promise resolving to the port number, or null if not exposed.
     */
    getRemoteDebuggingPortAsync(): Promise<number | null>;

    /**
     * Gets a summary of the platform's debugging capabilities.
     * @returns A promise resolving to the capabilities object.
     */
    getCapabilitiesAsync(): Promise<DebugCapabilities>;

    /**
     * Gets comprehensive debugging diagnostic information for the current window.
     * @returns A promise resolving to the diagnostics object.
     */
    getDiagnosticsAsync(): Promise<DebugDiagnostics>;

    /**
     * Attempts to retrieve the remote debugging endpoint URL.
     * @returns A promise resolving to the endpoint result.
     */
    tryGetRemoteDebuggingEndpointAsync(): Promise<DebugEndpointResult>;

    /**
     * Probes the remote debugging endpoint to verify it is reachable.
     * @returns A promise resolving to the probe result.
     */
    tryProbeEndpointAsync(): Promise<DebugEndpointResult>;

    /**
     * Enables or disables DevTools on this window.
     * @param enabled - true to enable DevTools, false to disable.
     */
    enableDevTools(enabled: boolean): void;
}
