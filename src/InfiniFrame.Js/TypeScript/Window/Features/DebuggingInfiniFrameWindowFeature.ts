/**
 * Debug settings feature. Controls dev tools, remote debugging endpoints, and diagnostic queries.
 *
 * @module DebuggingInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {
    DebugCapabilities,
    DebugDiagnostics,
    DebugEndpointResult,
    DebuggingInfiniFrameWindowFeature as Contract
} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides access to debugging capabilities including dev tools, web inspector,
 * remote debugging endpoints, and diagnostic information.
 */
export class DebuggingInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new debugging feature instance.
     */
    constructor() {
        super("debugging");
    }

    /**
     * Checks whether browser developer tools are enabled.
     *
     * @returns A promise that resolves to `true` if dev tools are enabled.
     */
    isDevToolsEnabledAsync() {
        return this.get<boolean>("isDevToolsEnabled");
    }

    /**
     * Checks whether the host supports attaching the web inspector.
     *
     * @returns A promise that resolves to `true` if web inspector attach is supported.
     */
    supportsWebInspectorAttachAsync() {
        return this.get<boolean>("supportsWebInspectorAttach");
    }

    /**
     * Checks whether the web inspector is currently enabled.
     *
     * @returns A promise that resolves to `true` if the web inspector is enabled.
     */
    isWebInspectorEnabledAsync() {
        return this.get<boolean>("isWebInspectorEnabled");
    }

    /**
     * Checks whether the host supports a remote debugging endpoint.
     *
     * @returns A promise that resolves to `true` if remote debugging endpoint is supported.
     */
    supportsRemoteDebuggingEndpointAsync() {
        return this.get<boolean>("supportsRemoteDebuggingEndpoint");
    }

    /**
     * Retrieves the remote debugging port number.
     *
     * @returns A promise that resolves to the port number, or `null` if not available.
     */
    getRemoteDebuggingPortAsync() {
        return this.get<number | null>("remoteDebuggingPort");
    }

    /**
     * Retrieves the debugging capabilities of the host.
     *
     * @returns A promise that resolves to the {@link DebugCapabilities} object.
     */
    getCapabilitiesAsync() {
        return this.get<DebugCapabilities>("capabilities");
    }

    /**
     * Retrieves diagnostic information from the host.
     *
     * @returns A promise that resolves to the {@link DebugDiagnostics} object.
     */
    getDiagnosticsAsync() {
        return this.get<DebugDiagnostics>("diagnostics");
    }

    /**
     * Attempts to retrieve the remote debugging endpoint.
     *
     * @returns A promise that resolves to the {@link DebugEndpointResult} with endpoint details.
     */
    tryGetRemoteDebuggingEndpointAsync() {
        return this.get<DebugEndpointResult>("remoteDebuggingEndpoint");
    }

    /**
     * Probes the remote debugging endpoint for connectivity.
     *
     * @returns A promise that resolves to the {@link DebugEndpointResult} with probe results.
     */
    tryProbeEndpointAsync() {
        return this.get<DebugEndpointResult>("probeEndpoint");
    }

    /**
     * Enables or disables browser developer tools.
     *
     * @param enabled - Whether to enable dev tools.
     */
    enableDevTools(enabled: boolean) {
        return this.post("enableDevTools", {enabled});
    }
}
