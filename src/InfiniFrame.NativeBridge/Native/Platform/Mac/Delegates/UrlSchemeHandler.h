#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

/**
 * @file UrlSchemeHandler.h
 * @brief WKURLSchemeHandler that intercepts custom-scheme requests and serves responses from the .NET layer
 */
#include "Public/InfiniFrame.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * @brief URL scheme handler conforming to WKURLSchemeHandler.
 *
 * Intercepts navigation and resource requests for custom URI schemes registered via
 * InfiniFrameWindow::AddCustomSchemeName and delegates response generation to
 * the WebResourceRequestedCallback provided at window initialisation
 */
@ interface UrlSchemeHandler :
    NSObject<WKURLSchemeHandler>{
        @public
    WebResourceRequestedCallback requestHandler; /// Callback that produces the response body and MIME type

    }
@ end
