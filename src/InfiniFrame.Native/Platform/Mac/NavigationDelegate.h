#ifdef __APPLE__
#pragma once
/**
 * @file NavigationDelegate.h
 * @brief WKNavigationDelegate that handles TLS certificate validation for the embedded WebView
 */
#include "Core/InfiniFrame.h"

/**
 * @brief Navigation delegate conforming to WKNavigationDelegate.
 *
 * Intercepts authentication challenges to optionally suppress TLS certificate
 * errors when InfiniFrameInitParams::IgnoreCertificateErrors is set
 */
@interface NavigationDelegate: NSObject<WKNavigationDelegate>{
    @public
    NSWindow * window;           /// The host NSWindow
    InfiniFrame * infiniFrame;   /// The InfiniFrameWindow instance this delegate belongs to
}
@end
#endif
