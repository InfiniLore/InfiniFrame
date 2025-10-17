// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {SendToHostMessageIds} from "./Contracts/IHostMessaging";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export const TitleObserverTarget: HTMLTitleElement | null = document.querySelector('title');

export function getTitleObserver(): MutationObserver {
    return new MutationObserver((mutations, _) => {
        mutations.forEach((mutation) => {
            if (mutation.type !== "childList") return;
            window.infiniFrame.HostMessaging.sendMessageToHost(SendToHostMessageIds.titleChange, document.title);
        })
    })
}