/**
 * Setup contract. Defines the initialization configuration passed from C# to JavaScript at startup.
 * @module InfiniFrameSetup
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Initialization state flags indicating which JavaScript subsystems have been set up.
 * Received from C# at startup to track bootstrap progress.
 */
export interface InfiniFrameSetup {
    /** Whether the native interop bridge (WebView2/WebKit postMessage) has been initialized. */
    nativeInteropBridgeInitialized: boolean;
    /** Whether the window.external bridge for Blazor compatibility has been initialized. */
    windowExternalBridgeInitialized: boolean;
    /** Whether the Blazor module fetch patch has been applied. */
    blazorModulesFetchPatchInitialized: boolean;
    /** Whether the Blazor custom elements patch has been applied. */
    blazorCustomElementsPatchInitialized: boolean;
    /** Whether custom element definitions have been registered. */
    customElementsInitialized: boolean;
}
