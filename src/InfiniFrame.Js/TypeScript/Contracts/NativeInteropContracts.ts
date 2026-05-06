// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameSetup {
    windowExternalBridgeInitialized: boolean;
    blazorModulesFetchPatchInitialized: boolean;
    blazorCustomElementsPatchInitialized: boolean;
    customElementsInitialized: boolean;
}

export type BlazorCallback = (message: string) => void;

// noinspection JSDeprecatedSymbols
export interface InfiniFrameExternal extends External {
    receiveMessage?: (callback: BlazorCallback) => void;
    receiveCallback?: (callback: BlazorCallback) => void;
    sendMessage?: (message: string) => void;
    postMessage?: (message: string) => void;
}

export interface BlazorCustomElementParameterDefinition {
    name?: string;
    type?: string;
}

export type BlazorCustomElementInitMap = Record<string, string[]>;

export interface BlazorCustomElementAttributeInfo {
    name: string;
    type: string;
}

export interface PendingBlazorCustomElementRegistration {
    defs: Record<string, BlazorCustomElementParameterDefinition[]>;
    initMap: BlazorCustomElementInitMap;
}

export interface BlazorComponent {
    setParameters?: (params: Record<string, unknown>) => Promise<void>;
    dispose?: () => void | Promise<void>;
}

declare global {
    interface Window {
        __infiniframeSetup: InfiniFrameSetup;
        __blazorCallbacks?: BlazorCallback[];
        __blazorDispatchHooked?: boolean;
        registerBlazorCustomElement?: (
            identifier: string,
            parameterDefinitions: BlazorCustomElementParameterDefinition[]
        ) => void;
        Blazor?: {
            rootComponents?: {
                add: (
                    element: HTMLElement,
                    identifier: string,
                    params: Record<string, unknown>
                ) => Promise<BlazorComponent>;
            };
            _internal?: {
                attachWebRendererInterop?: (...args: unknown[]) => unknown;
                __infiniframeAttachWebRendererInteropPatched?: boolean;
            };
        };
    }
}
