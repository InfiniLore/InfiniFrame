/**
 * Handles clicks on links with target="_blank" by opening them in the default system browser via the native host.
 * @module Utils/BlankTargetHandler
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {SendToHostMessageIds} from "../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Determines whether a URL points to a different hostname than the current page.
 * @param url - The URL to test (may be relative or absolute).
 * @returns `true` if the URL's hostname differs from `location.hostname`.
 */
function isExternalLink(url: string): boolean {
    try {
        return new URL(url, location.href).hostname !== location.hostname;
    } catch {
        return false;
    }
}

/**
 * Click-event handler that intercepts anchor elements whose `target` is `_blank`,
 * carry a `data-external` attribute, or link to an external host.
 * Prevents default navigation and forwards the URL to the native host for
 * opening in the system browser.
 * @param e - The mouse event originating from the clicked element.
 */
export async function blankTargetHandler(e: MouseEvent) {
    let el = e.target as HTMLElement | null;

    while (el && el !== document.body) {
        if (el.tagName?.toLowerCase() !== "a") {
            el = el.parentElement;
            continue;
        }

        const anchor = el as HTMLAnchorElement;
        if (!anchor.href) {
            el = el.parentElement;
            continue;
        }

        const target = anchor.getAttribute("target");
        const shouldHandle = target === "_blank" || anchor.hasAttribute("data-external") || isExternalLink(anchor.href);

        if (!shouldHandle) {
            el = el.parentElement;
            continue;
        }

        e.preventDefault();
        window.infiniframe.messaging.sendMessageToHost(SendToHostMessageIds.openExternalLink, anchor.href);
        return;
    }
}
