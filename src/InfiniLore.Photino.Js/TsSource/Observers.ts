// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {sendMessageToHost, HostMessageIds} from "./MessagingToHost";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export const TitleObserverTarget = document.querySelector('title') as HTMLElement;

export function getTitleObserver() : MutationObserver {
    return new MutationObserver((mutations, _) => {
        mutations.forEach((mutation) => {
            if (mutation.type === "childList") {
                sendMessageToHost(HostMessageIds.titleChange, document.title)
            }
        })
    })
}