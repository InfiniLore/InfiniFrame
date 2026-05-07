// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameSetup} from "../../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export function getSetupGuard(): InfiniFrameSetup {
    window.infiniframe = window.infiniframe ?? {} as Window["infiniframe"];
    window.infiniframe.setup = window.infiniframe.setup ?? {
        nativeInteropBridgeInitialized: false,
        windowExternalBridgeInitialized: false,
        blazorModulesFetchPatchInitialized: false,
        blazorCustomElementsPatchInitialized: false,
        customElementsInitialized: false,
    };
    return window.infiniframe.setup;
}
