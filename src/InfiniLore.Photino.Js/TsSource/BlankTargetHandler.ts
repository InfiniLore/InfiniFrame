// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {HostMessageIds, sendMessageToHost} from "./MessagingToHost";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
function isExternalLink(url: string): boolean {
    try {
        const u = new URL(url, location.href);
        return u.hostname !== location.hostname;
    } catch {
        return false;
    }
}

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
        if (target === "_blank" || anchor.hasAttribute("data-external") || isExternalLink(anchor.href)) {
            e.preventDefault();
            sendMessageToHost(HostMessageIds.openExternalLink, anchor.href);
            return;
        }
    }
}