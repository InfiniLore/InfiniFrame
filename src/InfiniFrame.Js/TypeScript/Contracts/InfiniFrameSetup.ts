// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameSetup {
    nativeInteropBridgeInitialized: boolean;
    windowExternalBridgeInitialized: boolean;
    blazorModulesFetchPatchInitialized: boolean;
    blazorCustomElementsPatchInitialized: boolean;
    customElementsInitialized: boolean;
}