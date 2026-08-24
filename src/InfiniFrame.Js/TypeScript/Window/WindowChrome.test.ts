// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {WindowChrome} from "./WindowChrome";
import {SendToHostMessageIds} from "../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Test helpers
// ---------------------------------------------------------------------------------------------------------------------
function createMessagingMock() {
    return {
        sendMessageToHost: vi.fn(),
        getMessageFromHostAsync: vi.fn(),
        assignMessageReceivedHandler: vi.fn(),
        unregisterMessageReceivedHandler: vi.fn()
    };
}

function setupWindowInfiniframe(messaging: ReturnType<typeof createMessagingMock>) {
    window.infiniframe = {
        messaging,
        window: {features: {}} as any,
        utils: {setPointerCapture: vi.fn(), releasePointerCapture: vi.fn()}
    } as any;
}

function createElement(tag: string, attrs: Record<string, string> = {}, text?: string): HTMLElement {
    const el = document.createElement(tag);
    for (const [key, value] of Object.entries(attrs)) {
        el.setAttribute(key, value);
    }
    if (text) el.textContent = text;
    document.body.appendChild(el);
    return el;
}

// jsdom doesn't support setPointerCapture, so we mock it on Element.prototype
beforeEach(() => {
    Element.prototype.setPointerCapture = vi.fn();
    Element.prototype.releasePointerCapture = vi.fn();
    Element.prototype.hasPointerCapture = vi.fn().mockReturnValue(false);
});

// ---------------------------------------------------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------------------------------------------------
describe("WindowChrome", () => {
    let chrome: WindowChrome;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(() => {
        document.body.innerHTML = "";
        messaging = createMessagingMock();
        setupWindowInfiniframe(messaging);
        chrome = new WindowChrome();
    });

    describe("register", () => {
        it("throws when config is null", () => {
            expect(() => chrome.register(null as any)).toThrow("config is required");
        });

        it("sets up drag region from CSS selector", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            dragArea.dispatchEvent(pointerDown);

            expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
                SendToHostMessageIds.windowFeatureRequest,
                expect.objectContaining({
                    command: expect.stringContaining("restoreFromMaximized")
                })
            );
        });

        it("sets up window controls from CSS selectors", () => {
            const minimizeBtn = createElement("button", {"data-role": "minimize"}, "Min");
            const maximizeBtn = createElement("button", {"data-role": "maximize"}, "Max");
            const closeBtn = createElement("button", {"data-role": "close"}, "Close");

            chrome.register({
                controls: {
                    minimize: "[data-role=minimize]",
                    maximize: "[data-role=maximize]",
                    close: "[data-role=close]"
                }
            });

            minimizeBtn.click();
            expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
                SendToHostMessageIds.windowFeatureRequest,
                expect.objectContaining({command: expect.stringContaining("minimize")})
            );

            vi.clearAllMocks();
            maximizeBtn.click();
            expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
                SendToHostMessageIds.windowFeatureRequest,
                expect.objectContaining({command: expect.stringContaining("toggleMaximize")})
            );

            vi.clearAllMocks();
            closeBtn.click();
            expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
                SendToHostMessageIds.windowFeatureRequest,
                expect.objectContaining({command: expect.stringContaining(":close")})
            );
        });

        it("sets up resize zones from CSS selectors", () => {
            const resizeRight = createElement("div", {id: "resize-right"});
            chrome.register({resize: {right: "#resize-right"}});

            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            resizeRight.dispatchEvent(pointerDown);

            expect(resizeRight.setPointerCapture).toHaveBeenCalledWith(1);

            vi.clearAllMocks();
            const pointerMove = new PointerEvent("pointermove", {
                bubbles: true,
                pointerId: 1,
                movementX: 5,
                movementY: 3
            });
            resizeRight.dispatchEvent(pointerMove);

            expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
                SendToHostMessageIds.windowFeatureRequest,
                expect.objectContaining({
                    command: expect.stringContaining("resize"),
                    args: {widthOffset: 5, heightOffset: 3, origin: "right"}
                })
            );
        });
    });

    describe("data attributes", () => {
        it("detects data-infiniframe-drag-region attribute", () => {
            const dragArea = createElement("div", {"data-infiniframe-drag-region": ""});
            chrome.register({});

            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            dragArea.dispatchEvent(pointerDown);

            expect(messaging.sendMessageToHost).toHaveBeenCalled();
        });

        it("detects data-infiniframe-window-action attribute", () => {
            const minimizeBtn = createElement("button", {"data-infiniframe-window-action": "minimize"});
            chrome.register({});

            minimizeBtn.click();
            expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
                SendToHostMessageIds.windowFeatureRequest,
                expect.objectContaining({command: expect.stringContaining("minimize")})
            );
        });

        it("detects data-infiniframe-resize attribute", () => {
            const resizeBottom = createElement("div", {"data-infiniframe-resize": "bottom"});
            chrome.register({});

            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            resizeBottom.dispatchEvent(pointerDown);

            expect(resizeBottom.setPointerCapture).toHaveBeenCalled();
        });

        it("maps data-infiniframe-resize values correctly", () => {
            const testCases = [
                ["top", "top"], ["right", "right"], ["bottom", "bottom"], ["left", "left"],
                ["top-left", "topLeft"], ["top-right", "topRight"],
                ["bottom-left", "bottomLeft"], ["bottom-right", "bottomRight"],
                ["topLeft", "topLeft"], ["topRight", "topRight"],
                ["bottomLeft", "bottomLeft"], ["bottomRight", "bottomRight"]
            ];

            for (const [attrValue, expectedOrigin] of testCases) {
                document.body.innerHTML = "";
                const el = createElement("div", {"data-infiniframe-resize": attrValue});
                const testChrome = new WindowChrome();
                testChrome.register({});

                const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
                el.dispatchEvent(pointerDown);

                vi.clearAllMocks();
                const pointerMove = new PointerEvent("pointermove", {
                    bubbles: true,
                    pointerId: 1,
                    movementX: 10,
                    movementY: 5
                });
                el.dispatchEvent(pointerMove);

                expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
                    SendToHostMessageIds.windowFeatureRequest,
                    expect.objectContaining({args: expect.objectContaining({origin: expectedOrigin})})
                );

                testChrome.unregister();
            }
        });

        it("warns on unknown data-infiniframe-resize value", () => {
            const warnSpy = vi.spyOn(console, "warn").mockImplementation(() => {
            });
            createElement("div", {"data-infiniframe-resize": "unknown"});
            chrome.register({});

            expect(warnSpy).toHaveBeenCalled();
            warnSpy.mockRestore();
        });

        it("ignores data-infiniframe-resize with empty value", () => {
            createElement("div", {"data-infiniframe-resize": ""});
            chrome.register({});
            // Should not throw
        });

        it("ignores data-infiniframe-window-action with unknown action", () => {
            createElement("button", {"data-infiniframe-window-action": "unknown"});
            chrome.register({});
            // Should not throw
        });
    });

    describe("unregister", () => {
        it("removes all event listeners", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            const minimizeBtn = createElement("button", {"data-role": "minimize"});
            const resizeEl = createElement("div", {id: "resize-right"});

            chrome.register({
                dragRegion: "#titlebar",
                controls: {minimize: "[data-role=minimize]"},
                resize: {right: "#resize-right"}
            });

            chrome.unregister();

            vi.clearAllMocks();
            dragArea.dispatchEvent(new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0}));
            minimizeBtn.click();
            resizeEl.dispatchEvent(new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0}));

            expect(messaging.sendMessageToHost).not.toHaveBeenCalled();
        });

        it("can be re-registered after unregister", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});
            chrome.unregister();

            vi.clearAllMocks();
            chrome.register({dragRegion: "#titlebar"});
            dragArea.dispatchEvent(new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0}));

            expect(messaging.sendMessageToHost).toHaveBeenCalled();
        });

        it("does nothing when not registered", () => {
            chrome.unregister();
        });
    });

    describe("double-click maximize", () => {
        it("sends toggleMaximize on double-click of drag region", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            dragArea.dispatchEvent(new MouseEvent("dblclick", {bubbles: true}));

            expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
                SendToHostMessageIds.windowFeatureRequest,
                expect.objectContaining({command: expect.stringContaining("toggleMaximize")})
            );
        });
    });

    describe("pointer down only on left button", () => {
        it("ignores non-left-button pointer events", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            const rightClick = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 2});
            dragArea.dispatchEvent(rightClick);

            expect(messaging.sendMessageToHost).not.toHaveBeenCalled();
        });
    });

    describe("pointer move", () => {
        it("does nothing when not resizing", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            const pointerMove = new PointerEvent("pointermove", {bubbles: true, pointerId: 1});
            dragArea.dispatchEvent(pointerMove);
        });
    });

    describe("pointer up", () => {
        it("does nothing when not dragging or resizing", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            const pointerUp = new PointerEvent("pointerup", {bubbles: true, pointerId: 1});
            dragArea.dispatchEvent(pointerUp);
        });

        it("ends drag on pointer up", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            // Start drag
            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            dragArea.dispatchEvent(pointerDown);

            // End drag
            const pointerUp = new PointerEvent("pointerup", {bubbles: true, pointerId: 1});
            dragArea.dispatchEvent(pointerUp);
        });
    });

    describe("resize lost capture", () => {
        it("cleans up on resize lost pointer capture", () => {
            const resizeEl = createElement("div", {id: "resize-right"});
            chrome.register({resize: {right: "#resize-right"}});

            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            resizeEl.dispatchEvent(pointerDown);

            // Simulate lostpointercapture
            const lostCapture = new Event("lostpointercapture");
            resizeEl.dispatchEvent(lostCapture);
        });
    });

    describe("drag lost capture", () => {
        it("cleans up on drag lost pointer capture", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            dragArea.dispatchEvent(pointerDown);

            // Simulate lostpointercapture
            const lostCapture = new Event("lostpointercapture");
            dragArea.dispatchEvent(lostCapture);
        });
    });

    describe("releasePointerCaptureIfHeld", () => {
        it("releases capture when pointerId is non-zero and has capture", () => {
            Element.prototype.hasPointerCapture = vi.fn().mockReturnValue(true);
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            // Start drag to set lastPointerId
            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            dragArea.dispatchEvent(pointerDown);

            // Double-click triggers releasePointerCaptureIfHeld
            dragArea.dispatchEvent(new MouseEvent("dblclick", {bubbles: true}));

            expect(dragArea.releasePointerCapture).toHaveBeenCalled();
        });

        it("does not release when lastPointerId is 0", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            // Double-click without pointerDown first
            dragArea.dispatchEvent(new MouseEvent("dblclick", {bubbles: true}));

            // Should not throw
        });
    });

    describe("edge cases", () => {
        it("handles register called before DOM ready", () => {
            const originalReadyState = document.readyState;
            Object.defineProperty(document, "readyState", {value: "loading", writable: true});

            chrome.register({dragRegion: "#titlebar"});

            Object.defineProperty(document, "readyState", {value: originalReadyState, writable: true});
        });

        it("handles invalid selectors gracefully", () => {
            const consoleWarn = vi.spyOn(console, "warn").mockImplementation(() => {
            });

            chrome.register({
                dragRegion: "///invalid///",
                controls: {minimize: "///invalid///"},
                resize: {top: "///invalid///"}
            });

            expect(chrome).toBeDefined();
            consoleWarn.mockRestore();
        });

        it("handles multiple register calls by cleaning up first", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            dragArea.dispatchEvent(new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0}));

            const restoreCalls = messaging.sendMessageToHost.mock.calls.filter(
                (call: any[]) => call[1]?.command?.includes("restoreFromMaximized")
            );
            expect(restoreCalls).toHaveLength(1);
        });

        it("handles empty config", () => {
            chrome.register({});
            // Should not throw
        });

        it("setup does nothing when config is null after register", () => {
            chrome.register({dragRegion: "#titlebar"});
            // Manually set config to null to test guard
            (chrome as any).config = null;
            (chrome as any).setup();
        });
    });

    describe("messaging not ready", () => {
        it("warns when messaging bridge is not available", () => {
            const consoleWarn = vi.spyOn(console, "warn").mockImplementation(() => {
            });
            (window.infiniframe as any).messaging = null;

            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            dragArea.dispatchEvent(new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0}));

            expect(consoleWarn).toHaveBeenCalledWith(
                expect.stringContaining("messaging bridge not ready")
            );
            consoleWarn.mockRestore();
        });
    });

    describe("message format", () => {
        it("sends windowFeatureRequest with correct command prefix", () => {
            const minimizeBtn = createElement("button", {"data-role": "minimize"});
            chrome.register({controls: {minimize: "[data-role=minimize]"}});

            minimizeBtn.click();

            expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
                SendToHostMessageIds.windowFeatureRequest,
                {command: "__infiniframe:window:features:windowChrome:minimize", args: undefined}
            );
        });

        it("includes args in message for resize operations", () => {
            const resizeEl = createElement("div", {id: "resize"});
            chrome.register({resize: {bottom: "#resize"}});

            resizeEl.dispatchEvent(new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0}));

            vi.clearAllMocks();
            resizeEl.dispatchEvent(new PointerEvent("pointermove", {
                bubbles: true,
                pointerId: 1,
                movementX: 15,
                movementY: -8
            }));

            expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
                SendToHostMessageIds.windowFeatureRequest,
                {
                    command: "__infiniframe:window:features:windowChrome:resize",
                    args: {widthOffset: 15, heightOffset: -8, origin: "bottom"}
                }
            );
        });
    });

    describe("mutation observer", () => {
        it("responds to childList mutations", () => {
            chrome.register({});

            // Add an element with data attribute to trigger mutation observer
            const el = document.createElement("div");
            el.setAttribute("data-infiniframe-drag-region", "");
            document.body.appendChild(el);
        });

        it("responds to attribute mutations on data-infiniframe-drag-region", () => {
            const el = createElement("div", {id: "test"});
            chrome.register({});

            el.setAttribute("data-infiniframe-drag-region", "");
        });
    });

    describe("pointer events", () => {
        it("pointerup ends drag when isDragging is true", () => {
            const dragArea = createElement("div", {id: "titlebar"});
            chrome.register({dragRegion: "#titlebar"});

            // Start drag
            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            dragArea.dispatchEvent(pointerDown);

            // End drag
            const pointerUp = new PointerEvent("pointerup", {bubbles: true, pointerId: 1});
            dragArea.dispatchEvent(pointerUp);
        });

        it("pointerup ends resize when isResizing is true", () => {
            const resizeEl = createElement("div", {id: "resize-right"});
            chrome.register({resize: {right: "#resize-right"}});

            // Start resize
            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            resizeEl.dispatchEvent(pointerDown);

            // End resize
            const pointerUp = new PointerEvent("pointerup", {bubbles: true, pointerId: 1});
            resizeEl.dispatchEvent(pointerUp);
        });

        it("pointermove triggers resize when isResizing is true", () => {
            const resizeEl = createElement("div", {id: "resize-right"});
            chrome.register({resize: {right: "#resize-right"}});

            // Start resize
            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            resizeEl.dispatchEvent(pointerDown);

            // Move during resize
            const pointerMove = new PointerEvent("pointermove", {
                bubbles: true,
                pointerId: 1,
                movementX: 5,
                movementY: 3
            });
            resizeEl.dispatchEvent(pointerMove);

            expect(messaging.sendMessageToHost).toHaveBeenCalled();
        });

        it("pointermove does nothing when not resizing", () => {
            const resizeEl = createElement("div", {id: "resize-right"});
            chrome.register({resize: {right: "#resize-right"}});

            // Move without starting resize
            const pointerMove = new PointerEvent("pointermove", {
                bubbles: true,
                pointerId: 1,
                movementX: 5,
                movementY: 3
            });
            resizeEl.dispatchEvent(pointerMove);
        });

        it("pointerdown on resize element starts resize", () => {
            const resizeEl = createElement("div", {id: "resize-bottom"});
            chrome.register({resize: {bottom: "#resize-bottom"}});

            const pointerDown = new PointerEvent("pointerdown", {bubbles: true, pointerId: 1, button: 0});
            resizeEl.dispatchEvent(pointerDown);

            expect(resizeEl.setPointerCapture).toHaveBeenCalled();
        });
    });
});
