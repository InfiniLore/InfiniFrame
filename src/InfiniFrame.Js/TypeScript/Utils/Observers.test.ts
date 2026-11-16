// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {afterEach, describe, expect, it, vi} from "vitest";
import {getTestWindow} from "../_testHelpers";
import {DecorationsInfiniFrameWindowFeature} from "../Window/Features/DecorationsInfiniFrameWindowFeature";
import {InfiniFrameWindow} from "../Window/InfiniFrameWindow";
import {getTitleObserver, getTitleObserverTarget} from "./Observers";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("Observers", () => {
    const testWindow = getTestWindow();
    const setTitle = vi.fn();
    const OriginalMutationObserver = globalThis.MutationObserver;

    afterEach(() => {
        setTitle.mockReset();
        globalThis.MutationObserver = OriginalMutationObserver;
        document.head.innerHTML = "";
    });

    it("getTitleObserverTarget returns the title element when present", () => {
        const title = document.createElement("title");
        title.textContent = "My Title";
        document.head.appendChild(title);

        expect(getTitleObserverTarget()).toBe(title);
    });

    it("getTitleObserver updates the window title feature only for childList mutations", () => {
        let callback: MutationCallback = () => undefined;

        class FakeMutationObserver {
            public observe = vi.fn();
            public disconnect = vi.fn();
            public takeRecords = vi.fn(() => []);

            constructor(cb: MutationCallback) {
                callback = cb;
            }
        }

        globalThis.MutationObserver =
            FakeMutationObserver as unknown as typeof MutationObserver;
        const decorations = new DecorationsInfiniFrameWindowFeature();
        decorations.setTitle = setTitle;
        const windowApi = new InfiniFrameWindow();
        windowApi.features.decorations = decorations;
        testWindow.infiniframe = {
            window: windowApi,
        };

        const title = document.createElement("title");
        title.textContent = "Initial";
        document.head.appendChild(title);

        getTitleObserver();
        callback(
            [
                {type: "attributes"} as MutationRecord,
                {type: "childList"} as MutationRecord,
            ],
            {} as MutationObserver,
        );

        expect(setTitle).toHaveBeenCalledTimes(1);
        expect(setTitle).toHaveBeenCalledWith("Initial");
    });
});
