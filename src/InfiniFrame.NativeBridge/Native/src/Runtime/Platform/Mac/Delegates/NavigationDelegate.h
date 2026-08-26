#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

/**
 * @file NavigationDelegate.h
 * @brief WKNavigationDelegate that handles TLS certificate validation for the embedded WebView
 */
#include "Runtime/Shared/Window/InfiniFrame.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * @brief Navigation delegate conforming to WKNavigationDelegate.
 *
 * Intercepts authentication challenges to optionally suppress TLS certificate
 * errors when InfiniFrameInitParams::IgnoreCertificateErrors is set
 */
@
interface NavigationDelegate:
    NSObject<WKNavigationDelegate>{
        @public
         NSWindow * window;           /// The host NSWindow
         InfiniFrameWindow * infiniFrame;   /// The InfiniFrameWindow instance this delegate belongs to

    }
@
end
