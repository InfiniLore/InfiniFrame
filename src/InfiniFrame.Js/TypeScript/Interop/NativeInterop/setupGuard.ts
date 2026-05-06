// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameSetup} from "../../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export function getSetupGuard(): InfiniFrameSetup {
    window.__infiniframeSetup = window.__infiniframeSetup ?? {
        windowExternalBridgeInitialized: false,
        blazorModulesFetchPatchInitialized: false,
        blazorCustomElementsPatchInitialized: false,
        customElementsInitialized: false,
    };
    return window.__infiniframeSetup;
}
