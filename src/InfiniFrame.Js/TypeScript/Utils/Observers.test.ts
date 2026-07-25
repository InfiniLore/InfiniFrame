// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {afterEach, describe, expect, it, vi} from "vitest";
import {getTitleObserver, getTitleObserverTarget} from "./Observers";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
type TestWindow = Window & {
    infiniframe?: {
        window: {
            features: {
                decorations: {
                    setTitle: (title: string | null) => void;
                };
            };
        };
    };
};

describe("Observers", () => {
    const testWindow = window as TestWindow;
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

        globalThis.MutationObserver = FakeMutationObserver as unknown as typeof MutationObserver;
        testWindow.infiniframe = {
            // @ts-ignore
            window: {
                features: {
                    decorations: {
                        setTitle
                    }
                }
            }
        };

        const title = document.createElement("title");
        title.textContent = "Initial";
        document.head.appendChild(title);

        getTitleObserver();
        callback(
            [
                {type: "attributes"} as MutationRecord,
                {type: "childList"} as MutationRecord
            ],
            {} as MutationObserver
        );

        expect(setTitle).toHaveBeenCalledTimes(1);
        expect(setTitle).toHaveBeenCalledWith("Initial");
    });
});
