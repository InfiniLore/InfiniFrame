// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {sendMessageToHost, HostMessageIds} from "./MessagingToHost";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export const TitleObserverTarget : HTMLTitleElement | null = document.querySelector('title');

export function getTitleObserver() : MutationObserver {
    return new MutationObserver((mutations, _) => {
        mutations.forEach((mutation) => {
            if (mutation.type !== "childList") return;
            sendMessageToHost(HostMessageIds.titleChange, document.title)
        })
    })
}