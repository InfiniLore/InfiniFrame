// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {
    BlazorComponent,
    BlazorCustomElementAttributeInfo,
    BlazorCustomElementInitMap,
    BlazorCustomElementParameterDefinition,
    PendingBlazorCustomElementRegistration
} from "../../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
function toKebabCase(name: string): string {
    return String(name)
        .replace(/([a-z0-9])([A-Z])/g, "$1-$2")
        .replace(/_/g, "-")
        .toLowerCase();
}

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

function isNumericType(typeName: string): boolean {
    return ["number", "int", "float", "double", "decimal"].indexOf(typeName) >= 0;
}

const pendingAutoCustomElementRegistrations: PendingBlazorCustomElementRegistration[] = [];
let autoCustomElementRegistrationScheduled = false;

function scheduleAutoRegisterMissingInitializerCustomElements(
    defs: Record<string, BlazorCustomElementParameterDefinition[]>,
    initMap: BlazorCustomElementInitMap
): void {
    if (!defs) return;

    pendingAutoCustomElementRegistrations.push({ defs, initMap });

    if (autoCustomElementRegistrationScheduled) return;
    autoCustomElementRegistrationScheduled = true;

    window.setTimeout(() => {
        autoCustomElementRegistrationScheduled = false;
        flushAutoRegisterMissingInitializerCustomElements();
    }, 0);
}

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

export function initBlazorCustomElementsPatch(): void {
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

export function initCustomElements(): void {
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
            map[attr] = { name: def.name, type };
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
                if (c?.dispose) Promise.resolve(c.dispose()).catch(() => {});
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
