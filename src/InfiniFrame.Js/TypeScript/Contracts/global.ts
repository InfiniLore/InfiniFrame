// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrame} from "./InfiniFrame";
import type {BlazorCallback, BlazorComponent, BlazorCustomElementParameterDefinition} from "./BlazorInterop";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export {}
declare global {
    // noinspection JSUnusedGlobalSymbols
    interface Window {
        infiniframe: InfiniFrame;
        __infiniframe_dispatch?: (message: string) => void;

        // Managed by the host: Webview or WebKit
        chrome?: {
            webview?: {
                postMessage(message: string): void;
                addEventListener(type: "message", listener: (event: { data: string }) => void): void;
            };
        };
        webkit?: {
            messageHandlers?: {
                infiniFrameInterop?: {
                    postMessage(message: string): void;
                };
            };
        };

        // Managed by the Blazor framework.
        __blazorCallbacks?: BlazorCallback[];
        __blazorDispatchHooked?: boolean;
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
        registerBlazorCustomElement?: (
            identifier: string,
            parameterDefinitions: BlazorCustomElementParameterDefinition[]
        ) => void;
    }
}
