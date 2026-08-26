#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

/**
 * @file WindowDelegate.h
 * @brief NSWindow delegate that forwards window lifecycle events to InfiniFrameWindow callbacks
 */
#include "Runtime/Shared/Window/InfiniFrame.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * @brief Per-window delegate conforming to NSWindowDelegate.
 *
 * Listens for window close, resize, move, miniaturize, and focus events and
 * translates them into the corresponding InfiniFrame Invoke* calls.
 * Also handles file drag-and-drop when enabled.
 */
@
interface WindowDelegate :
    NSObject<NSWindowDelegate, NSDraggingDestination>
    {
        @public
         InfiniFrameWindow * infiniFrame; ///< The InfiniFrameWindow instance this delegate belongs to

    }

-
(NSDragOperation)draggingEntered:
    (id<NSDraggingInfo>)

sender;
-
(BOOL)performDragOperation:
    (id<NSDraggingInfo>)

sender;
@
end
