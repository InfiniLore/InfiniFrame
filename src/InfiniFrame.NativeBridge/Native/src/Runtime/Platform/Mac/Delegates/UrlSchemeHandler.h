#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Window/InfiniFrame.h"
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
        @private
    NSMutableSet* activeTasks;
    }
- (void)invalidate;
@ end
