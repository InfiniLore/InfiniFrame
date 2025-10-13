// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {SendToHostMessageIds} from "./Contracts/IHostMessaging";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
function isExternalLink(url: string): boolean {
    try {
        return new URL(url, location.href).hostname !== location.hostname;
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
        if (!(target === "_blank" || anchor.hasAttribute("data-external") || isExternalLink(anchor.href))) {
            el = el.parentElement;
            continue;
        }
        
        e.preventDefault();
        window.infiniWindow.HostMessaging.sendMessageToHost(SendToHostMessageIds.openExternalLink, anchor.href);
        return;
    }
}