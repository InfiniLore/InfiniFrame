/**
 * Global type augmentations. Extends the Window interface with the infiniframe namespace.
 * @module global
 */

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

    /**
     * Augmentation of the global Window interface to expose the InfiniFrame namespace
     * and host-specific runtime objects.
     */
    interface Window {
        /** Root InfiniFrame namespace providing access to all subsystems. */
        infiniframe: InfiniFrame;

        /** Internal dispatch function for routing incoming web messages. */
        __infiniframe_dispatch?: (message: string) => void;

        // Managed by the host: Webview or WebKit

        /** WebView2 host object injected by the Microsoft Edge WebView2 runtime. */
        chrome?: {
            webview?: {
                /**
                 * Posts a message to the native host via WebView2.
                 * @param message - Serialized message string.
                 */
                postMessage(message: string): void;

                /**
                 * Registers an event listener on the WebView2 message channel.
                 * @param type - Event type (must be "message").
                 * @param listener - Callback invoked with incoming message events.
                 */
                addEventListener(type: "message", listener: (event: { data: string }) => void): void;
            };
        };

        /** WebKit host object injected by the WKWebView runtime on macOS/iOS. */
        webkit?: {
            messageHandlers?: {
                infiniFrameInterop?: {
                    /**
                     * Posts a message to the native host via WKWebView.
                     * @param message - Serialized message string.
                     */
                    postMessage(message: string): void;
                };
            };
        };

        // Managed by the Blazor framework.

        /** Array of Blazor message callbacks registered by the framework. */
        __blazorCallbacks?: BlazorCallback[];

        /** Whether the Blazor dispatch hook has been applied. */
        __blazorDispatchHooked?: boolean;

        /** Blazor framework runtime object. */
        Blazor?: {
            rootComponents?: {
                /**
                 * Adds a root Blazor component to the specified element.
                 * @param element - The host HTML element.
                 * @param identifier - Component identifier.
                 * @param params - Initial parameters for the component.
                 * @returns A handle to the mounted component.
                 */
                add: (
                    element: HTMLElement,
                    identifier: string,
                    params: Record<string, unknown>
                ) => Promise<BlazorComponent>;
            };
            _internal?: {
                /** Attaches the web renderer interop layer. */
                attachWebRendererInterop?: (...args: unknown[]) => unknown;

                /** Whether the InfiniFrame patch to attachWebRendererInterop has been applied. */
                __infiniframeAttachWebRendererInteropPatched?: boolean;
            };
        };

        /**
         * Registers a Blazor custom element with the specified identifier and parameter definitions.
         * @param identifier - Custom element tag name or identifier.
         * @param parameterDefinitions - Array of parameter definitions for the element.
         */
        registerBlazorCustomElement?: (
            identifier: string,
            parameterDefinitions: BlazorCustomElementParameterDefinition[]
        ) => void;
    }
}
