// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export type BlazorCallback = (message: string) => void;

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
