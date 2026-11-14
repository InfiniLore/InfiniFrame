/**
 * Blazor-specific interop type definitions for component registration and JS runtime integration.
 * @module BlazorInterop
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Callback signature for Blazor message handlers.
 * Receives a serialized message string from the native host.
 */
export type BlazorCallback = (message: string) => void;

/**
 * Definition of a single parameter on a Blazor custom element.
 */
export interface BlazorCustomElementParameterDefinition {
    /** Parameter name. */
    name?: string;
    /** Parameter type identifier (e.g. "string", "int", "bool"). */
    type?: string;
}

/**
 * Mapping of custom element identifiers to their required initialization parameter names.
 */
export type BlazorCustomElementInitMap = Record<string, string[]>;

/**
 * Runtime attribute information for a registered Blazor custom element.
 */
export interface BlazorCustomElementAttributeInfo {
    /** Attribute name. */
    name: string;
    /** Attribute type identifier. */
    type: string;
}

/**
 * Pending registration data for a Blazor custom element that has been declared but not yet mounted.
 */
export interface PendingBlazorCustomElementRegistration {
    /** Parameter definitions keyed by element identifier. */
    defs: Record<string, BlazorCustomElementParameterDefinition[]>;
    /** Initialization parameter names required by each element. */
    initMap: BlazorCustomElementInitMap;
}

/**
 * Handle to a mounted Blazor component instance.
 */
export interface BlazorComponent {
    /** Updates the component's parameters. Returns a promise that resolves when the update is applied. */
    setParameters?: (params: Record<string, unknown>) => Promise<void>;
    /** Disposes the component instance, releasing associated resources. */
    dispose?: () => void | Promise<void>;
}
