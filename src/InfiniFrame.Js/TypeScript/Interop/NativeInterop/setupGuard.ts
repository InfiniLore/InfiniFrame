/**
 * @file Setup guard. Ensures the InfiniFrame native bridge has been initialized before use.
 */
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameSetup} from "../../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Returns the global setup guard object, creating it with default values if it does not already exist.
 *
 * @returns The {@link InfiniFrameSetup} object attached to `window.infiniframe.setup`.
 */
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
