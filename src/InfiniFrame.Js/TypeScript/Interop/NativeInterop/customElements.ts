/**
 * @file Blazor custom elements patch. Registers custom HTML elements that bridge to Blazor component parameters.
 */
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {
    BlazorComponent,
    BlazorCustomElementAttributeInfo,
    BlazorCustomElementInitMap,
    BlazorCustomElementParameterDefinition,
    InfiniFrameSetup,
    PendingBlazorCustomElementRegistration
} from "../../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Converts a camelCase or PascalCase identifier to kebab-case.
 *
 * @param name - The identifier string to convert.
 * @returns The kebab-case version of the input string.
 */
function toKebabCase(name: string): string {
    return String(name)
        .replace(/([a-z0-9])([A-Z])/g, "$1-$2")
        .replace(/_/g, "-")
        .toLowerCase();
}

/**
 * Converts a raw HTML attribute string value to the appropriate .NET parameter type.
 *
 * @param rawValue - The raw attribute value from the DOM, or null when the attribute is absent.
 * @param typeName - The .NET type name (e.g. "bool", "int", "number") that determines the conversion.
 * @returns The converted value appropriate for the given type.
 */
function toParameterValue(rawValue: string | null, typeName: string): unknown {
    if (typeName === "bool" || typeName === "boolean") {
        if (rawValue === null) return false;
        if (rawValue === "") return true;
        return String(rawValue).toLowerCase() !== "false";
    }

    if (isNumericType(typeName)) {
        const value = Number(rawValue);
        return Number.isNaN(value) ? rawValue : value;
    }

    return rawValue;
}

/**
 * Checks whether a .NET type name represents a numeric type.
 *
 * @param typeName - The .NET type name to check.
 * @returns `true` if the type is one of: "number", "int", "float", "double", or "decimal".
 */
function isNumericType(typeName: string): boolean {
    return ["number", "int", "float", "double", "decimal"].indexOf(typeName) >= 0;
}

const pendingAutoCustomElementRegistrations: PendingBlazorCustomElementRegistration[] = [];
let autoCustomElementRegistrationScheduled = false;

/**
 * Schedules auto-registration of custom elements that lack an initializer, deferring to the next microtask.
 *
 * @param defs - A map of custom element tag names to their parameter definitions.
 * @param initMap - A map of Blazor root component IDs to the tag names they have already initialized.
 * @returns {void}
 */
function scheduleAutoRegisterMissingInitializerCustomElements(
    defs: Record<string, BlazorCustomElementParameterDefinition[]>,
    initMap: BlazorCustomElementInitMap
): void {
    if (!defs) return;

    pendingAutoCustomElementRegistrations.push({defs, initMap});

    if (autoCustomElementRegistrationScheduled) return;
    autoCustomElementRegistrationScheduled = true;

    window.setTimeout(() => {
        autoCustomElementRegistrationScheduled = false;
        flushAutoRegisterMissingInitializerCustomElements();
    }, 0);
}

/**
 * Flushes the pending queue of auto-registration tasks, registering any custom elements not yet covered by an initializer.
 *
 * @returns {void}
 */
function flushAutoRegisterMissingInitializerCustomElements(): void {
    if (typeof window.registerBlazorCustomElement !== "function") return;

    while (pendingAutoCustomElementRegistrations.length > 0) {
        const item = pendingAutoCustomElementRegistrations.shift();
        if (!item) {
            return;
        }

        try {
            autoRegisterMissingInitializerCustomElements(item.defs, item.initMap);
        } catch (error) {
            console.error(error);
        }
    }
}

/**
 * Registers custom element tag names that do not already have an initializer in the provided init map.
 *
 * @param defs - A map of custom element tag names to their parameter definitions.
 * @param initMap - A map of Blazor root component IDs to the tag names they have already initialized.
 * @returns {void}
 */
function autoRegisterMissingInitializerCustomElements(
    defs: Record<string, BlazorCustomElementParameterDefinition[]>,
    initMap: BlazorCustomElementInitMap
): void {
    const initialized: Record<string, boolean> = {};

    const initMapEntries = initMap ?? {};
    for (const key of Object.keys(initMapEntries)) {
        const list = initMapEntries[key];
        if (!Array.isArray(list)) continue;
        for (const id of list) initialized[id] = true;
    }

    const definitions = defs ?? {};
    for (const id of Object.keys(definitions)) {
        if (initialized[id]) continue;
        window.registerBlazorCustomElement!(id, definitions[id]);
    }
}

/**
 * Patches `Blazor._internal.attachWebRendererInterop` to intercept custom element definitions after Blazor initializes.
 *
 * @returns `true` if the function was patched or had already been patched; `false` if Blazor internals are not available.
 */
function patchAttachWebRendererInteropIfAvailable(): boolean {
    const blazor = window.Blazor;

    if (
        !blazor?._internal ||
        typeof blazor._internal.attachWebRendererInterop !== "function"
    ) {
        return false;
    }

    if (blazor._internal.__infiniframeAttachWebRendererInteropPatched) return true;

    const original = blazor._internal.attachWebRendererInterop;

    blazor._internal.attachWebRendererInterop = function (...args: unknown[]) {
        const result = original.apply(this, args);
        scheduleAutoRegisterMissingInitializerCustomElements(
            args[2] as Record<string, BlazorCustomElementParameterDefinition[]>,
            args[3] as BlazorCustomElementInitMap
        );
        return result;
    };

    blazor._internal.__infiniframeAttachWebRendererInteropPatched = true;
    return true;
}

/**
 * Initializes the Blazor custom elements patch. If `Blazor._internal.attachWebRendererInterop` is not yet available,
 * a property setter is installed on `window.Blazor` to patch it once Blazor loads.
 *
 * @param setup - The setup guard that tracks which initializations have already been performed.
 * @returns {void}
 */
export function initBlazorCustomElementsPatch(setup: InfiniFrameSetup): void {
    if (setup.blazorCustomElementsPatchInitialized) return;
    setup.blazorCustomElementsPatchInitialized = true;

    if (!patchAttachWebRendererInteropIfAvailable()) {
        const descriptor = Object.getOwnPropertyDescriptor(window, "Blazor");

        if (!descriptor || descriptor.configurable) {
            let value = window.Blazor;

            Object.defineProperty(window, "Blazor", {
                configurable: true,
                enumerable: true,
                get: () => value,
                set: (v) => {
                    value = v;
                    patchAttachWebRendererInteropIfAvailable();
                },
            });

            if (value) {
                patchAttachWebRendererInteropIfAvailable();
            }
        }
    }
}

/**
 * Initializes the global `window.registerBlazorCustomElement` function. When called, it defines a custom HTML element
 * that hosts a Blazor root component, mapping HTML attributes to component parameters via `setParameters`.
 *
 * @param setup - The setup guard that tracks which initializations have already been performed.
 * @returns {void}
 */
export function initCustomElements(setup: InfiniFrameSetup): void {
    if (setup.customElementsInitialized) return;
    setup.customElementsInitialized = true;

    window.registerBlazorCustomElement = function (
        identifier: string,
        parameterDefinitions: BlazorCustomElementParameterDefinition[]
    ): void {
        if (!window.Blazor?.rootComponents?.add) return;
        if (!window.customElements?.define) return;
        if (window.customElements.get(identifier)) return;

        const defs = Array.isArray(parameterDefinitions) ? parameterDefinitions : [];
        const map: Record<string, BlazorCustomElementAttributeInfo> = {};

        for (const def of defs) {
            if (!def?.name) continue;
            const type = String(def.type ?? "").toLowerCase();
            if (type === "eventcallback") continue;

            const attr = toKebabCase(def.name);
            map[attr] = {name: def.name, type};
        }

        const observed = Object.keys(map);

        class Host extends HTMLElement {
            private _component: BlazorComponent | null = null;
            private _isDisconnected = false;

            static get observedAttributes(): string[] {
                return observed;
            }

            connectedCallback(): void {
                this._isDisconnected = false;

                window.Blazor!.rootComponents!.add(this, identifier, this._getParams())
                    .then((c) => {
                        this._component = c;
                        if (this._isDisconnected && c) {
                            this._component = null;
                            return c.dispose?.();
                        }
                    })
                    .catch(console.error);
            }

            disconnectedCallback(): void {
                this._isDisconnected = true;
                const c = this._component;
                this._component = null;
                if (c?.dispose) Promise.resolve(c.dispose()).catch(() => {
                });
            }

            attributeChangedCallback(
                name: string,
                oldValue: string | null,
                newValue: string | null
            ): void {
                if (oldValue === newValue) return;
                if (!this._component?.setParameters) return;

                const info = map[name];
                if (!info) return;

                const p: Record<string, unknown> = {};
                p[info.name] = toParameterValue(newValue, info.type);

                this._component.setParameters(p).catch(console.error);
            }

            private _getParams(): Record<string, unknown> {
                const p: Record<string, unknown> = {};
                for (const attr of observed) {
                    if (!this.hasAttribute(attr)) continue;
                    const info = map[attr];
                    p[info.name] = toParameterValue(this.getAttribute(attr), info.type);
                }
                return p;
            }
        }

        window.customElements.define(identifier, Host);
    };
}
