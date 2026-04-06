#ifdef __APPLE__
#pragma once
/**
 * @file WindowDelegate.h
 * @brief NSWindow delegate that forwards window lifecycle events to InfiniFrameWindow callbacks
 */
#include "Core/InfiniFrame.h"

/**
 * @brief Per-window delegate conforming to NSWindowDelegate.
 *
 * Listens for window close, resize, move, miniaturize, and focus events and
 * translates them into the corresponding InfiniFrame Invoke* calls
 */
@ interface WindowDelegate : NSObject<NSWindowDelegate>
{
    @public
    InfiniFrameWindow * infiniFrame; ///< The InfiniFrameWindow instance this delegate belongs to

}
@ end
#endif
