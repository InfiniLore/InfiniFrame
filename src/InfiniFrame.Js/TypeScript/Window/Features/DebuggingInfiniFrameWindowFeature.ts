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
export class DebuggingInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor() {
        super("debugging");
    }

    isDevToolsEnabledAsync() {
        return this.get<boolean>("isDevToolsEnabled");
    }

    supportsWebInspectorAttachAsync() {
        return this.get<boolean>("supportsWebInspectorAttach");
    }

    isWebInspectorEnabledAsync() {
        return this.get<boolean>("isWebInspectorEnabled");
    }

    supportsRemoteDebuggingEndpointAsync() {
        return this.get<boolean>("supportsRemoteDebuggingEndpoint");
    }

    getRemoteDebuggingPortAsync() {
        return this.get<number | null>("remoteDebuggingPort");
    }

    getCapabilitiesAsync() {
        return this.get<DebugCapabilities>("capabilities");
    }

    getDiagnosticsAsync() {
        return this.get<DebugDiagnostics>("diagnostics");
    }

    tryGetRemoteDebuggingEndpointAsync() {
        return this.get<DebugEndpointResult>("remoteDebuggingEndpoint");
    }

    tryProbeEndpointAsync() {
        return this.get<DebugEndpointResult>("probeEndpoint");
    }

    enableDevTools(enabled: boolean) {
        return this.post("enableDevTools", {enabled});
    }
}
