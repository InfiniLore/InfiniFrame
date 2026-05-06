export interface InfiniFrameSetup {
    messagingBridgeInitialized: boolean;
    WebviewReceiveAttached: boolean;
    windowExternalBridgeInitialized: boolean;
    blazorModulesFetchPatchInitialized: boolean;
    blazorCustomElementsPatchInitialized: boolean;
    customElementsInitialized: boolean;
}

declare global {
    interface Window {
        __infiniframeSetup: InfiniFrameSetup;
    }
}

export function getSetupGuard(): InfiniFrameSetup {
    window.__infiniframeSetup = window.__infiniframeSetup ?? {
        messagingBridgeInitialized: false,
        WebviewReceiveAttached: false,
        windowExternalBridgeInitialized: false,
        blazorModulesFetchPatchInitialized: false,
        blazorCustomElementsPatchInitialized: false,
        customElementsInitialized: false,
    };
    return window.__infiniframeSetup;
}
