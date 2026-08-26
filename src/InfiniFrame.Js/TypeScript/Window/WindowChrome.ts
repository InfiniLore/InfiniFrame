/**
 * Custom window chrome registration. Provides the JavaScript API for drag regions,
 * window control buttons, and resize handles.
 *
 * @module WindowChrome
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {SendToHostMessageIds} from "../Contracts";
// ---------------------------------------------------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------------------------------------------------
export interface WindowChromeControlsConfig {
    minimize?: string;
    maximize?: string;
    close?: string;
}

export interface WindowChromeResizeConfig {
    top?: string;
    right?: string;
    bottom?: string;
    left?: string;
    topLeft?: string;
    topRight?: string;
    bottomLeft?: string;
    bottomRight?: string;
}

export interface WindowChromeConfig {
    dragRegion?: string;
    controls?: WindowChromeControlsConfig;
    resize?: WindowChromeResizeConfig;
}

// ---------------------------------------------------------------------------------------------------------------------
// Data Attribute Constants
// ---------------------------------------------------------------------------------------------------------------------
const DATA_DRAG_REGION = "data-infiniframe-drag-region";
const DATA_WINDOW_ACTION = "data-infiniframe-window-action";
const DATA_RESIZE = "data-infiniframe-resize";

const RESIZE_ORIGIN_MAP: Record<string, string> = {
    "top": "top",
    "right": "right",
    "bottom": "bottom",
    "left": "left",
    "top-left": "topLeft",
    "top-right": "topRight",
    "bottom-left": "bottomLeft",
    "bottom-right": "bottomRight",
    "topLeft": "topLeft",
    "topRight": "topRight",
    "bottomLeft": "bottomLeft",
    "bottomRight": "bottomRight"
};

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/**
 * Manages custom window chrome by registering drag regions, window controls
 * (minimize/maximize/close), and resize zones.
 */
export class WindowChrome {
    private config: WindowChromeConfig | null = null;
    private isRegistered = false;

    private dragRegions: Element[] = [];
    private controlElements: Map<Element, string> = new Map();
    private resizeElements: Map<Element, string> = new Map();

    private isDragging = false;
    private isResizing = false;
    private lastPointerId = 0;
    private resizeOrigin: string | null = null;

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    private boundPointerDown: any;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    private boundPointerUp: any;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    private boundPointerMove: any;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    private boundDblClick: any;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    private boundClick: any;

    private mutationObserver: MutationObserver | null = null;

    /**
     * Creates a new {@link WindowChrome} instance and binds internal event handlers.
     */
    constructor() {
        this.boundPointerDown = this.onPointerDown.bind(this) as EventListener;
        this.boundPointerUp = this.onPointerUp.bind(this) as EventListener;
        this.boundPointerMove = this.onPointerMove.bind(this) as EventListener;
        this.boundDblClick = this.onDblClick.bind(this) as EventListener;
        this.boundClick = this.onClick.bind(this) as EventListener;
    }

    /**
     * Registers drag regions, window controls, and resize zones from the provided configuration.
     *
     * If already registered, the previous configuration is torn down first.
     *
     * @param config - The chrome configuration specifying CSS selectors for drag regions,
     * control buttons, and resize handles.
     * @throws {Error} If `config` is not provided.
     */
    public register(config: WindowChromeConfig): void {
        if (this.isRegistered) this.unregister();
        if (!config) throw new Error("WindowChrome.register: config is required.");

        this.config = config;
        this.isRegistered = true;

        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", () => this.setup());
        } else {
            this.setup();
        }
    }

    /**
     * Tears down all event listeners, disconnects the mutation observer, and clears the
     * current configuration.
     */
    public unregister(): void {
        if (!this.isRegistered) return;

        this.teardown();
        this.config = null;
        this.isRegistered = false;
    }

    private setup(): void {
        if (!this.config || !this.isRegistered) return;

        this.setupDragRegions(this.config.dragRegion);
        this.setupWindowControls(this.config.controls);
        this.setupResizeZones(this.config.resize);
        this.setupDataAttributes();
        this.setupMutationObserver();
    }

    private teardown(): void {
        this.removeDragRegionListeners();
        this.removeControlListeners();
        this.removeResizeListeners();

        if (this.mutationObserver) {
            this.mutationObserver.disconnect();
            this.mutationObserver = null;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Drag Regions
    // -----------------------------------------------------------------------------------------------------------------
    private setupDragRegions(selector?: string): void {
        if (!selector) return;

        try {
            this.dragRegions = [...document.querySelectorAll(selector)];
        } catch {
            console.warn(`WindowChrome: invalid drag region selector "${selector}".`);
            return;
        }

        for (const el of this.dragRegions) {
            el.addEventListener("pointerdown", this.boundPointerDown);
            el.addEventListener("dblclick", this.boundDblClick);
        }
    }

    private removeDragRegionListeners(): void {
        for (const el of this.dragRegions) {
            el.removeEventListener("pointerdown", this.boundPointerDown);
            el.removeEventListener("dblclick", this.boundDblClick);
        }
        this.dragRegions = [];
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Window Controls
    // -----------------------------------------------------------------------------------------------------------------
    private setupWindowControls(controls?: WindowChromeControlsConfig): void {
        if (!controls) return;

        const entries: [string, string][] = [
            controls.minimize ? [controls.minimize, "minimize"] : [],
            controls.maximize ? [controls.maximize, "maximize"] : [],
            controls.close ? [controls.close, "close"] : []
        ].filter((e): e is [string, string] => e.length === 2);

        for (const [sel, action] of entries) {
            try {
                for (const el of document.querySelectorAll(sel)) {
                    el.addEventListener("click", this.boundClick);
                    this.controlElements.set(el, action);
                }
            } catch {
                console.warn(`WindowChrome: invalid selector "${sel}" for ${action} control.`);
            }
        }
    }

    private removeControlListeners(): void {
        for (const [el] of this.controlElements) {
            el.removeEventListener("click", this.boundClick);
        }
        this.controlElements = new Map();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Resize Zones
    // -----------------------------------------------------------------------------------------------------------------
    private setupResizeZones(resize?: WindowChromeResizeConfig): void {
        if (!resize) return;

        const entries: [string, string][] = [
            resize.top ? [resize.top, "top"] : [],
            resize.right ? [resize.right, "right"] : [],
            resize.bottom ? [resize.bottom, "bottom"] : [],
            resize.left ? [resize.left, "left"] : [],
            resize.topLeft ? [resize.topLeft, "topLeft"] : [],
            resize.topRight ? [resize.topRight, "topRight"] : [],
            resize.bottomLeft ? [resize.bottomLeft, "bottomLeft"] : [],
            resize.bottomRight ? [resize.bottomRight, "bottomRight"] : []
        ].filter((e): e is [string, string] => e.length === 2);

        for (const [sel, origin] of entries) {
            try {
                for (const el of document.querySelectorAll(sel)) {
                    el.addEventListener("pointerdown", this.boundPointerDown);
                    this.resizeElements.set(el, origin);
                }
            } catch {
                console.warn(`WindowChrome: invalid selector "${sel}" for ${origin} resize zone.`);
            }
        }
    }

    private removeResizeListeners(): void {
        for (const [el] of this.resizeElements) {
            el.removeEventListener("pointerdown", this.boundPointerDown);
            el.removeEventListener("pointerup", this.boundPointerUp);
            el.removeEventListener("pointermove", this.boundPointerMove);
        }
        this.resizeElements = new Map();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Data Attributes
    // -----------------------------------------------------------------------------------------------------------------
    private setupDataAttributes(): void {
        this.setupDataAttributeDragRegions();
        this.setupDataAttributeControls();
        this.setupDataAttributeResizeZones();
    }

    private setupDataAttributeDragRegions(): void {
        for (const el of document.querySelectorAll(`[${DATA_DRAG_REGION}]`)) {
            el.addEventListener("pointerdown", this.boundPointerDown);
            el.addEventListener("dblclick", this.boundDblClick);
            this.dragRegions.push(el);
        }
    }

    private setupDataAttributeControls(): void {
        for (const el of document.querySelectorAll(`[${DATA_WINDOW_ACTION}]`)) {
            const action = el.getAttribute(DATA_WINDOW_ACTION);
            if (action === "minimize" || action === "maximize" || action === "close") {
                el.addEventListener("click", this.boundClick);
                this.controlElements.set(el, action);
            }
        }
    }

    private setupDataAttributeResizeZones(): void {
        for (const el of document.querySelectorAll(`[${DATA_RESIZE}]`)) {
            const value = el.getAttribute(DATA_RESIZE);
            if (!value) continue;

            const origin = RESIZE_ORIGIN_MAP[value];
            if (!origin) {
                console.warn(`WindowChrome: unknown data-infiniframe-resize value "${value}".`);
                continue;
            }

            el.addEventListener("pointerdown", this.boundPointerDown);
            this.resizeElements.set(el, origin);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Mutation Observer
    // -----------------------------------------------------------------------------------------------------------------
    private setupMutationObserver(): void {
        this.mutationObserver = new MutationObserver((mutations) => {
            let needsUpdate = false;
            for (const mutation of mutations) {
                if (mutation.type === "childList") {
                    needsUpdate = true;
                    break;
                }
                if (mutation.type === "attributes" &&
                    mutation.attributeName &&
                    (mutation.attributeName === DATA_DRAG_REGION ||
                        mutation.attributeName === DATA_WINDOW_ACTION ||
                        mutation.attributeName === DATA_RESIZE)) {
                    needsUpdate = true;
                    break;
                }
            }

            if (needsUpdate) {
                this.teardown();
                this.setup();
            }
        });

        this.mutationObserver.observe(document.body, {
            childList: true,
            subtree: true,
            attributes: true,
            attributeFilter: [DATA_DRAG_REGION, DATA_WINDOW_ACTION, DATA_RESIZE]
        });
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Pointer Event Handlers
    // -----------------------------------------------------------------------------------------------------------------
    private onPointerDown(e: PointerEvent): void {
        if (e.button !== 0) return;

        const target = e.currentTarget as Element;

        if (this.resizeElements.has(target)) {
            this.startResize(e, target);
            return;
        }

        if (this.dragRegions.includes(target)) {
            this.startDrag(e, target);
            return;
        }
    }

    private onPointerUp(e: PointerEvent): void {
        if (this.isDragging) {
            this.endDrag();
        } else if (this.isResizing) {
            this.endResize(e);
        }
    }

    private onPointerMove(e: PointerEvent): void {
        if (this.isResizing) {
            this.handleResizeMove(e);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Drag
    // -----------------------------------------------------------------------------------------------------------------
    private startDrag(e: PointerEvent, target: Element): void {
        if (e.detail >= 2) return;

        this.isDragging = true;
        this.lastPointerId = e.pointerId;

        this.postToHost("restoreFromMaximized", {screenX: e.screenX, screenY: e.screenY});
        target.setPointerCapture(e.pointerId);
        target.addEventListener("lostpointercapture", () => this.onDragLostCapture(), {once: true});
    }

    private onDragLostCapture(): void {
        if (!this.isDragging) return;
        this.isDragging = false;
        this.lastPointerId = 0;
    }

    private endDrag(): void {
        this.isDragging = false;
        this.lastPointerId = 0;
    }

    private endResize(_e: PointerEvent): void {
        this.isResizing = false;
        this.lastPointerId = 0;
        this.resizeOrigin = null;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Double-Click (Maximize Toggle)
    // -----------------------------------------------------------------------------------------------------------------
    private onDblClick(e: MouseEvent): void {
        const target = e.currentTarget as Element;
        this.postToHost("toggleMaximize");
        this.releasePointerCaptureIfHeld(target);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Window Controls Click
    // -----------------------------------------------------------------------------------------------------------------
    private onClick(e: MouseEvent): void {
        const target = e.currentTarget as Element;
        const action = this.controlElements.get(target);
        if (!action) return;

        switch (action) {
            case "minimize":
                this.postToHost("minimize");
                break;
            case "maximize":
                this.postToHost("toggleMaximize");
                break;
            case "close":
                this.postToHost("close");
                break;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Resize
    // -----------------------------------------------------------------------------------------------------------------
    private startResize(e: PointerEvent, target: Element): void {
        this.isResizing = true;
        this.lastPointerId = e.pointerId;
        this.resizeOrigin = this.resizeElements.get(target) ?? null;
        target.setPointerCapture(e.pointerId);
        target.addEventListener("pointermove", this.boundPointerMove);
        target.addEventListener("lostpointercapture", () => this.onResizeLostCapture(target), {once: true});
    }

    private onResizeLostCapture(target: Element): void {
        if (!this.isResizing) return;
        this.isResizing = false;
        this.lastPointerId = 0;
        this.resizeOrigin = null;
        target.removeEventListener("pointermove", this.boundPointerMove);
    }

    private handleResizeMove(e: PointerEvent): void {
        if (!this.resizeOrigin) return;

        this.postToHost("resize", {
            widthOffset: e.movementX,
            heightOffset: e.movementY,
            origin: this.resizeOrigin
        });
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------------------------------------------------
    private releasePointerCaptureIfHeld(target: Element): void {
        if (this.lastPointerId === 0) return;
        try {
            if (target.hasPointerCapture(this.lastPointerId)) {
                target.releasePointerCapture(this.lastPointerId);
            }
        } catch { /* Element may no longer exist */
        }
        this.lastPointerId = 0;
    }

    private postToHost(command: string, args?: unknown): void {
        if (!isMessagingReady()) {
            console.warn("WindowChrome: messaging bridge not ready.");
            return;
        }
        window.infiniframe.messaging.sendMessageToHost(
            SendToHostMessageIds.windowFeatureRequest,
            {command: `__infiniframe:window:features:windowChrome:${command}`, args}
        );
    }
}

function isMessagingReady(): boolean {
    return !!(window.infiniframe?.messaging);
}

const windowChrome = new WindowChrome();
export default windowChrome;
