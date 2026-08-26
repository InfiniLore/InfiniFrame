/**
 * MutationObserver helpers for watching DOM changes.
 * @module Utils/Observers
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Returns the first `<title>` element in the document, or `null` if none exists.
 * @returns The `<title>` element, or `null`.
 */
export function getTitleObserverTarget(): HTMLTitleElement | null {
    return document.querySelector('title');
}

/**
 * Creates a MutationObserver that monitors the `<title>` element for child-list
 * changes and synchronises `document.title` to the host via the decorations feature.
 * @returns A new MutationObserver instance bound to title-change logic.
 */
export function getTitleObserver(): MutationObserver {
    return new MutationObserver((mutations, _) => {
        mutations.forEach((mutation) => {
            if (mutation.type !== "childList") return;
            window.infiniframe.window.features.decorations.setTitle(document.title);
        })
    })
}
