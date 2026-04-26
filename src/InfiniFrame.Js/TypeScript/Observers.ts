// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {SendToHostMessageIds} from "./Contracts/IInfiniFrameHostMessaging";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export function getTitleObserverTarget(): HTMLTitleElement | null {
    return document.querySelector('title');
}

export function getTitleObserver(): MutationObserver {
    return new MutationObserver((mutations, _) => {
        mutations.forEach((mutation) => {
            if (mutation.type !== "childList") return;
            window.infiniFrame.HostMessaging.sendMessageToHost(SendToHostMessageIds.titleChange, document.title);
        })
    })
}
