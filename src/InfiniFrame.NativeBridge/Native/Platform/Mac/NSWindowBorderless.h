#ifdef __APPLE__
#pragma once
/**
 * @file NSWindowBorderless.h
 * @brief NSWindow subclass that accepts first-mouse events and supports transparent backgrounds
 *
 * Used when InfiniFrameInitParams::Transparent is set, allowing the WebView to render
 * over a fully transparent window background without the standard title bar and borders
 */
#include "Public/InfiniFrame.h"

/**
 * @brief Borderless, transparent NSWindow subclass.
 *
 * Overrides acceptsFirstMouse: to return YES so that the first click activates
 * the window and is also delivered to the web content simultaneously
 */
@ interface NSWindowBorderless :
    NSWindow
    {
    }
@ end
#endif
